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

namespace Syscon.ScheduledJob.WorkOrderExportJob
{
    /// <summary>
    /// Implementation of the work order import job.
    /// </summary>
    [Export(typeof(IScheduledJob))]    
    public class WorkOrderExportJob : ScheduledJob
    {
        #region Member variables
        private WorkOrderExportJobConfigUI  _configUI   = null;        
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public WorkOrderExportJob(): base()
        {
            _jobConfig = new WorkOrderExportJobConfig();
            _configUI = new WorkOrderExportJobConfigUI();
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
            //this.Log("Job Configuration loaded.");
            this.Log("Started execution of work order export job.");

            try
            {
                SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(_jobConfig.SMBDir);
                //this.Log("Successfully set the SMB directory to OLE DB connection. Connection string -\n {0}", SysconCommon.Common.Environment.Connections.OLEDBConnectionString);
            }
            catch (Exception ex)
            {
                this.Log("Exception in setting SMB direcotry to OLE DB Connection");
            }

            TextWriter txtWriter = null;
            System.Data.OleDb.OleDbConnection con = null;
            try
            {
                string currentFilePath = (_jobConfig as WorkOrderExportJobConfig).WorkOrderExportDirectory;
                string outFileName = Path.Combine(currentFilePath, "SageWorkOrderExport.csv");

                //this.Log("Trying to get the OLE DB data connection");

                con = SysconCommon.Common.Environment.Connections.GetOLEDBConnection();

                if (con != null)
                    this.Log("Got the database connection");
                
                this.Log("Validating login credentials");
                //Login to the SMB Dir
                var login_result = con.GetScalar<int>("select count(*) from usrlst where upper(usrnme) == '{0}' and usrpsw == '{1}'", _jobConfig.UserId.ToUpper(), _jobConfig.Password);
                if (login_result == 0)
                {
                    this.Log("Login failure - Invalid user name or password. Exiting job...");
                    return;
                }

                //this.Log("Setting lookback period");

                DateTime currDate = DateTime.Today;
                int lookBackPeriod = (int)(_jobConfig as WorkOrderExportJobConfig).LookBackPeriod;
                DateTime startDate = DateTime.Today.AddMonths(-lookBackPeriod);

                this.Log("Getting data from srvinv table");

                string strSql = string.Format("SELECT ordnum, dscrpt, status from srvinv WHERE orddte >= {0} AND orddte <= {1}",
                                                    startDate.ToFoxproDate(), currDate.ToFoxproDate());
                DataTable dt = con.GetDataTable("SrvInv", strSql);
                //this.Log("Creating the csv file. SQL string - {0}", strSql);
                txtWriter = File.CreateText(outFileName);

                //this.Log("Adding headers to the csv file");

                //Add headers
                txtWriter.WriteLine("Name, Number, Export, Status");

                this.Log("Putting data rows to the csv file. Row count - {0}", dt.Rows.Count);
                //put the data into SageWorkOrderExport.csv file in the WorkOrderExportDirectory
                foreach (DataRow dr in dt.Rows)
                {
                    string status = ((decimal)dr["status"]) == 7M ? "No" : "Yes";
                    string description = (string)dr["dscrpt"];
                    description = (description.Length > 50) ? description.Substring(0, 50) : description.PadRight(50);
                    string csvLine = string.Format("{0}, {1}, {2}, {3}", description, (string)dr["ordnum"], (string)dr["ordnum"], status);

                    txtWriter.WriteLine(csvLine);
                }

                //Log end of execution, time etc.
                this.Log("Finished execution of work order export job. All data synced to file : {0}", outFileName);
                this.Log("-----------------------------------------------------------------------------------------\n");
            }
            catch (Exception ex)
            {
                this.Log("Exception in work order export job. Exception - {0}", ex.Message);
            }
            finally
            {
                if (txtWriter != null)
                {
                    txtWriter.Close();
                    txtWriter = null;
                }

                if(con != null)
                {
                    con.Close();
                    con.Dispose();
                }
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
            get { return "Job for exporting work orders"; }
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

    }
}
