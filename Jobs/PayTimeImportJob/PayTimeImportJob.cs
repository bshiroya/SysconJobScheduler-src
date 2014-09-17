using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

using SysconCommon;
using SysconCommon.Algebras.DataTables;
using SysconCommon.Common;
using SysconCommon.Common.Environment;
using SysconCommon.Parsing;
using SysconCommon.Foxpro;
using Syscon.ScheduledJob;

namespace Syscon.ScheduledJob.PayTimeImportJob
{
    /// <summary>
    /// Implementation of the work order import job.
    /// </summary>
    [Export(typeof(IScheduledJob))]    
    public class PayTimeImportJob : ScheduledJob
    {
        #region Member variables
        private PayTimeImportJobConfigUI  _configUI   = null;

        private COMMethods _methods = null;
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public PayTimeImportJob(): base()
        {
            _jobConfig = new PayTimeImportJobConfig();
            _configUI = new PayTimeImportJobConfigUI();

            _methods = new COMMethods();
        }

        #region IScheduledJob Members

        #region Methods

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public override void ExceuteJob()
        {
            this.Log("Started execution of work order export job.");

            _jobConfig.LoadConfig();

            SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(_jobConfig.SMBDir);

            using (var con = SysconCommon.Common.Environment.Connections.GetOLEDBConnection())
            {
                var hashed_password = _methods.smartEncrypt(_jobConfig.Password, false);

                //Login to the SMB Dir
                var login_result = con.GetScalar<int>("select count(*) from usrlst where upper(usrnme) == '{0}' and usrpsw == '{1}'", _jobConfig.UserId.ToUpper(), hashed_password);
                if (login_result == 0)
                {
                    this.Log("Login failure - Invalid user name or password. Exiting job...");
                    return;
                }

                string currentFilePath = (_jobConfig as PayTimeImportJobConfig).PayTimeImportCsvFile;
                if(string.IsNullOrEmpty(currentFilePath))
                {
                    this.Log("The file name is null or empty.");
                    return;
                }

                //string[] files = System.IO.Directory.GetFiles(currentFilePath, "*.csv", System.IO.SearchOption.TopDirectoryOnly);

                if(!File.Exists(currentFilePath))
                {
                    this.Log("The file - {0} does not exist.", currentFilePath);
                    return;
                }
                DataTable dtLogFile = null;

                //Logic for payroll time import program
                try
                {
                    //Read the CSV file and import the entries to Sage Database
                    string fileNameWithoutPath = currentFilePath.Substring(currentFilePath.LastIndexOf('\\') + 1);

                    //Log the filename in the log file
                    this.Log("Processing file - {0}", fileNameWithoutPath);

                    DataTable dt = this.DataTableFromCSV("TimeImportTable", currentFilePath, true);
                    dtLogFile = dt.Clone();
                    dtLogFile.Columns.Add("", typeof(string));
                    dtLogFile.Columns.Add("Reason for Rejected", typeof(string));

                    if (dt.Columns.Count != 6)
                    {
                        this.Log("The CSV file format is probably incorrect. Column count does not match. \nPlease re-check before selecting.");
                        return;
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime workDate;
                        DateTime.TryParse((string)dr["Date"], out workDate);

                        string empId        = (string)dr["EmployeeNumber"];
                        string workOrder    = (string)dr["WorkOrder"];
                        string costCode     = (string)dr["CostCode"];
                        string hours        = (string)dr["Hours"];
                        string payType      = (string)dr["PayType"];

                        int empIdCount = con.GetScalar<int>("select count(*) from employ where recnum={0}", empId);
                        int orderNumCount = con.GetScalar<int>("select count(*) from srvinv where ordnum='{0}'", workOrder);
                        int costCodeCount = con.GetScalar<int>("select count(*) from cstcde where recnum={0}", costCode);

                        if (empIdCount > 0 && orderNumCount > 0 && costCodeCount > 0)
                        {
                            //Add to database



                        }
                        else
                        {
                            this.Log("Invalid record in csv file. Skipping this entry");
                            continue;
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.Log("Exception in daily payroll time import job. Exception - {0}", ex.Message);
                }
                finally
                {
                    dtLogFile.SaveAsCSV("");
                }

                //Log end of execution, time etc.
                this.Log("Finished execution of daily payroll time import job.");
                this.Log("-----------------------------------------------------------------------------------------\n");
            }
            
        }

        /// <summary>
        /// Set the configuration settings for the job.
        /// </summary>
        public override void SetJobConfiguration()
        {
            if (_configUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _jobConfig.LoadConfig();
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Job Desctription
        /// </summary>
        public override string JobDesc
        {
            get { return "Job for daily payroll time import"; }
        }

        /// <summary>
        /// The log file path for this job.
        /// </summary>
        public override string LogFilePath
        {
            get { return _jobConfig.LogFilePath; }
        }

        #endregion

        #endregion
        
        #region Protected Methods

        /// <summary>
        /// Log a message to LogFile, the format is the same as string.Format
        /// </summary>
        /// <param name="msgFormat"></param>
        /// <param name="arguments"></param>
        protected override void Log(string msgFormat, params object[] arguments)
        {
            try
            {
                File.AppendAllText(LogFilePath, string.Format(DateTime.Now.ToString() + " - " + msgFormat + "\r\n", arguments));
            }
            catch
            {
                Env.Log("Error writing work order export job log file");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Copied from SysconCommon and modified to exclude blank rows and column names in .csv files.
        /// </summary>
        /// <param name="tblName"></param>
        /// <param name="csvFile"></param>
        /// <param name="includesHeaders"></param>
        /// <returns></returns>
        private DataTable DataTableFromCSV(string tblName, string csvFile, bool includesHeaders)
        {
            List<string> allLines = new List<string>();
            using (StreamReader sr = new StreamReader(csvFile))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line.Trim(',')))
                        allLines.Add(line);
                }
            }
            var lines = allLines.ToArray();
            var data = (from l in lines
                        select l.Split(',')).ToArray();

            if (data.Length == 0)
                return null;

            if (data[0].Length == 0)
                return null;

            foreach (var i in FunctionalOperators.Range(1, data.Length))
            {
                if (data[i].Length != data[0].Length)
                    throw new InvalidOperationException("CSV File has inconsistent amount of fields in lines");
            }

            var headers = includesHeaders ? data[0] : null;
            //Trim for empty header names and then trim the empty data rows
            headers = headers.TakeWhile(s => !string.IsNullOrEmpty(s.TrimEnd(','))).ToArray();

            var data2 = data;
            foreach (var i in FunctionalOperators.Range(0, data.Length))
            {
                data2[i] = data[i].Take(headers.Length).ToArray();
            }

            if (includesHeaders)
            {
                data2 = data2.Tail().ToArray();
            }

            var dt = data2.MultArrayToDataTable(tblName);

            if (headers != null)
            {
                foreach (var i in FunctionalOperators.Range(headers.Length))
                {
                    dt.Columns[i].ColumnName = headers[i];
                }
            }

            data = null;
            data2 = null;
            allLines.Clear();
            allLines = null;

            return dt;
        }
        #endregion
    }
}
