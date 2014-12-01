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

        
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public PayTimeImportJob(): base()
        {
            _jobConfig = new PayTimeImportJobConfig();
            _configUI = new PayTimeImportJobConfigUI();            
        }

        #region IScheduledJob Members

        #region Methods

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public override void ExceuteJob()
        {
            _jobConfig.LoadConfig();

            this.Log("Started execution of daily payroll time import job.");

            SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(_jobConfig.SMBDir);

            using (var con = SysconCommon.Common.Environment.Connections.GetOLEDBConnection())
            {
                //Login to the SMB Dir
                var login_result = con.GetScalar<int>("select count(*) from usrlst where upper(usrnme) == '{0}' and usrpsw == '{1}'", _jobConfig.UserId.ToUpper(), _jobConfig.Password);
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

                if(!File.Exists(currentFilePath))
                {
                    this.Log("The file - {0} does not exist.", currentFilePath);
                    return;
                }
                DataTable dtLogFile = null;

                string folderPath = Path.GetDirectoryName(currentFilePath);
                string fileNameWithoutPath = Path.GetFileName(currentFilePath);

                //Logic for payroll time import program
                try
                {
                    //Read the CSV file and import the entries to Sage Database

                    //Log the filename in the log file
                    this.Log("Processing file - {0}", fileNameWithoutPath);

                    DataTable dt = this.DataTableFromCSV("TimeImport", currentFilePath, true);
                    dtLogFile = dt.Copy();
                    dtLogFile.Columns.Add("Accepted/Rejected", typeof(string));
                    dtLogFile.Columns.Add("Reason for Rejected", typeof(string));

                    if (dt.Columns.Count != 6)
                    {
                        this.Log("The CSV file format is probably incorrect. Column count does not match. \nPlease re-check before selecting.");
                        return;
                    }

                    int rowCount = 0;

                    foreach (DataRow dr in dt.Rows)
                    {
                        DateTime workDate;
                        DateTime.TryParse((string)dr["Date"], out workDate);

                        string empId        = (string)dr["EmployeeNumber"];
                        string workOrder    = (string)dr["WorkOrder"];
                        string cstCode      = (string)dr["CostCode"];
                        string hours        = (string)dr["Hours"];
                        string payTyp       = (string)dr["PayType"];

                        int empNum = 0;                        
                        Int32.TryParse(empId, out empNum);

                        decimal costCode = 0.0M;                        
                        Decimal.TryParse(cstCode, out costCode);

                        int empIdCount = con.GetScalar<int>("select count(*) from employ where recnum={0}", empNum);
                        int orderNumCount = con.GetScalar<int>("select count(*) from srvinv where ordnum='{0}'", workOrder);
                        int costCodeCount = con.GetScalar<int>("select count(*) from cstcde where recnum={0}", costCode);

                        if (empIdCount > 0 && orderNumCount > 0 && costCodeCount > 0)
                        {
                            try
                            {
                                //Getting a dummy DataRow with the required columns to fill the data
                                string selectSql = "SELECT paydte, empnum, wrkord, jobnum, loctax, crtfid, phsnum, cstcde, paytyp, paygrp, payrte, payhrs, cmpcde, usrdf1 FROM dlypyr WHERE paydte = DATE(1000,01,01)";
                                DataTable dtDlyPyr = con.GetDataTable("DlyPyr", selectSql);
                                dtDlyPyr.Rows.Clear();

                                decimal payHours = 0.0M;
                                Decimal.TryParse(hours, out payHours);
                                int payType = 0;
                                Int32.TryParse(payTyp, out payType);

                                string phaseNum = (workOrder.Length == 8) ? workOrder.Substring(workOrder.Length - 4, 4) : string.Empty;//workOrder.Substring(workOrder.Length - 3, 3);
                                //phaseNum = (phaseNum == "000") ? string.Empty : phaseNum;                                

                                decimal jobNumber = con.GetScalar<int>("SELECT jobnum from srvinv WHERE ordnum='{0}'", workOrder);
                                int localTax = con.GetScalar<int>("SELECT lcltax from actrec WHERE recnum= {0}", jobNumber);
                                int certified = con.GetScalar<int>("SELECT crtfid from actrec WHERE recnum= {0}", jobNumber);
                                int payGroup = con.GetScalar<int>("SELECT paygrp FROM employ WHERE recnum={0}", empNum);
                                int compCode = con.GetScalar<int>("SELECT wrkcmp  FROM employ WHERE recnum = {0}", empNum);
                                string usrDf1 = (fileNameWithoutPath.Length > 20) ? Path.GetFileNameWithoutExtension(fileNameWithoutPath).Substring(0, 20) : Path.GetFileNameWithoutExtension(fileNameWithoutPath);

                                //Verify phase number
                                int phaseNumber = 0;
                                Int32.TryParse(phaseNum, out phaseNumber);
                                int phase = con.GetScalar<int>("SELECT phsnum FROM jobphs where recnum = {0} AND phsnum={1}", jobNumber, phaseNumber);

                                DataRow drDlyPyr = dtDlyPyr.NewRow();
                                drDlyPyr["paydte"] = workDate;
                                drDlyPyr["empnum"] = empNum;
                                drDlyPyr["wrkord"] = workOrder;
                                drDlyPyr["jobnum"] = jobNumber;
                                drDlyPyr["loctax"] = localTax;
                                drDlyPyr["crtfid"] = (certified == 1) ? "Y" : "N";
                                drDlyPyr["phsnum"] = phase;
                                drDlyPyr["cstcde"] = costCode;
                                drDlyPyr["paytyp"] = payType;
                                drDlyPyr["paygrp"] = payGroup;
                                drDlyPyr["payrte"] = GetPayRate(payType, payGroup, empNum, con);
                                drDlyPyr["payhrs"] = payHours;
                                drDlyPyr["cmpcde"] = compCode;
                                drDlyPyr["usrdf1"] = usrDf1;

                                SetNullOff(con);
                                string insertSql = drDlyPyr.FoxproInsertString("dlypyr");
                                con.ExecuteNonQuery(insertSql);

                                DataRow dr1 = dtLogFile.Rows[rowCount];
                                dr1["Accepted/Rejected"] = "Accepted";
                                dr1["Reason for Rejected"] = string.Empty;
                                rowCount++;
                            }
                            catch (Exception ex)
                            {
                                DataRow dr1 = dtLogFile.Rows[rowCount];
                                dr1["Accepted/Rejected"] = "Rejected";
                                dr1["Reason for Rejected"] = string.Format("Error in adding record #{0} to DB - Employee Number - {1}; Work Order - {2}; Cost Code - {3}", rowCount + 1, empId, workOrder, cstCode);

                                this.Log("Invalid record # {0} in csv file. Skipping this entry\n Exception Message - {1}", rowCount + 1, ex.Message);
                                rowCount++;
                                continue;
                            }
                        }
                        else
                        {
                            DataRow dr1 = dtLogFile.Rows[rowCount];
                            dr1["Accepted/Rejected"] = "Rejected";
                            dr1["Reason for Rejected"] = string.Format("Matching record #{0} not found in DB. Employee Number - {1}; Work Order - {2}; Cost Code - {3}", rowCount + 1, empId, workOrder, cstCode);

                            this.Log("Invalid record # {0} in csv file. Skipping this entry", rowCount + 1);
                            rowCount++;
                            continue;
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.Log("Exception in processing file - {0}. Exception - {0}", fileNameWithoutPath, ex.Message);
                }
                finally
                {
                    string logFileName = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(fileNameWithoutPath) + "Log.csv");
                    dtLogFile.SaveAsCSV(logFileName);
                    SetNullOn(con);
                }

                //Log end of execution, time etc.
                this.Log("Finished execution of daily payroll time import job.");
                this.Log("-----------------------------------------------------------------------------------------\n");
            }            
        }

        private decimal GetPayRate(int payType, int payGroup, int empNum, System.Data.OleDb.OleDbConnection con)
        {
            decimal payRate = 0.0M;
            string strSql = string.Format("SELECT payrt{0} FROM paygrp where recnum ={1} ", 1, -11111);

            switch(payType)
            {
                case 0:
                    strSql = string.Format("SELECT payrt{0} FROM employ where recnum ={1} ", payType, empNum);
                    break;
                case 1:
                case 2:
                case 3:
                    strSql = string.Format("SELECT payrt{0} FROM paygrp where recnum ={1} ", payType, payGroup);
                    break;
                default:
                    break;
            }

            payRate = con.GetScalar<decimal>(strSql);

            return payRate;
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

        private void SetNullOn(System.Data.OleDb.OleDbConnection connection)
        {
            //Set this so that FoxPro doesn't try to insert null values in empty columns
            System.Data.OleDb.OleDbCommand dbCmdNull = connection.CreateCommand();
            dbCmdNull.CommandText = "SET NULL ON";
            dbCmdNull.ExecuteNonQuery();
        }

        private void SetNullOff(System.Data.OleDb.OleDbConnection connection)
        {
            //Set this so that FoxPro doesn't try to insert null values in empty columns
            System.Data.OleDb.OleDbCommand dbCmdNull = connection.CreateCommand();
            dbCmdNull.CommandText = "SET NULL OFF";
            dbCmdNull.ExecuteNonQuery();
        }
        #endregion
    }
}
