using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using SysconCommon;
using SysconCommon.Common;
using SysconCommon.Foxpro;
using SysconCommon.Algebras.DataTables;
using SysconCommon.Common.Environment;


namespace Syscon.ScheduledJob.ExportBillingUpdatesJob
{

    /// <summary>
    /// Implementation of the billing status update job.
    /// </summary>
    [Export(typeof(IScheduledJob))] 
    public class ExportBillingUpdatesJob : ScheduledJob
    {
        #region Member variables
        private ExportBillingUpdatesJobConfigUI _configUI   = null;
        private COMMethods                      _methods    = null;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportBillingUpdatesJob()
        {
            _jobConfig = new ExportBillingUpdatesJobConfig();
            _configUI = new ExportBillingUpdatesJobConfigUI();

            _methods = new COMMethods();
        }

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public override void ExceuteJob()
        {
            _jobConfig.LoadConfig();

            this.Log("Started execution of export billing updates job.");
            
            SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(_jobConfig.SMBDir);

            string currentFilePath = (_jobConfig as ExportBillingUpdatesJobConfig).BillingUpdateQueueDirectory;
            using (var con = SysconCommon.Common.Environment.Connections.GetOLEDBConnection())
            {
                using (Env.TempDBFPointer
                    _exportlist = con.GetTempDBF())
                {
                    var hashed_password = _methods.smartEncrypt(_jobConfig.Password, false);

                    //Login to the SMB Dir
                    var login_result = con.GetScalar<int>("select count(*) from usrlst where upper(usrnme) == '{0}' and usrpsw == '{1}'", _jobConfig.UserId.ToUpper(), hashed_password);
                    if (login_result == 0)
                    {
                        this.Log("Login failure - Invalid user name or password. Exiting job...");
                        return;
                    }

                    //Get the list of invoices that have been completed (status = 8) 
                    //or posted as invoices (status = 1,2,3,4) and have not been sent 
                    //to the export file yet.
                    con.ExecuteNonQuery("SELECT s.recnum, s.ordnum, s.invnum, s.invttl, s.usrdf1 FROM srvinv s WHERE INLIST(s.status, 1,2,3,4,8) AND EMPTY(s.usrdf1) "
                                        + "INTO Table {0}", _exportlist);

                    DataTable dt = con.GetDataTable("srvinv", "SELECT * from {0}", _exportlist);

                    if (dt.Rows.Count > 0)
                    {
                        //Get unique file name
                        string csvFile = GetUniqueFileName(currentFilePath);

                        //Put the data to the .csv file
                        DataView view = new DataView(dt);
                        DataTable dtSpecificCols = view.ToTable(false, new string[] { "ordnum", "invnum", "invttl" });

                        dtSpecificCols.SaveAsCSV(csvFile);
                        this.Log("Exported billing updates to file : {0}", Path.GetFileName(csvFile));

                        //* Update the work orders in SRVINV 
                        con.ExecuteNonQuery("UPDATE srvinv SET usrdf1 = DTOC({0}) + \" I360 Updated\" "
                                                    + "from {1} _ExportList WHERE srvinv.recnum = _ExportList.recnum",
                                                        DateTime.Now.ToFoxproDate(), _exportlist);
                        this.Log("Updated work orders");
                    }
                    else
                    {
                        this.Log("No rows to process");
                    }
                }
            }            

            //Log end of execution, time etc.
            this.Log("Finished execution of export billing updates job.");

            this.Log("----------------------------------------------------------------------------------------\n\n");
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

        #region Properties

        /// <summary>
        /// Job Desctription
        /// </summary>
        public override string JobDesc
        {
            get { return "Job for exporting billing updates to csv file"; }
        }

        /// <summary>
        /// The log file path for this job.
        /// </summary>
        public override string LogFilePath
        {
            get { return _jobConfig.LogFilePath; }
        }

        #endregion

        #region Private Methods

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
                Env.Log("Error writing work order import job log file");
            }
        }

        /// <summary>
        /// This methods returns a unique csv file name in a given folder.
        /// </summary>
        /// <param name="queueDir"></param>
        /// <returns></returns>
        private string GetUniqueFileName(string queueDir)
        {
            string uniqueFileName = string.Empty;
            DateTime currDT = DateTime.Now;

            uniqueFileName = Path.Combine(queueDir, currDT.Year.ToString() + currDT.Month.ToString().PadLeft(2, '0') + currDT.Day.ToString().PadLeft(2, '0') + "-WorkOrderUpdate_");
            int count = 1;
            do
            {
                if (File.Exists(uniqueFileName + count.ToString().PadLeft(4, '0') + ".csv"))
                {
                    count++;
                }
                else
                {
                    uniqueFileName = uniqueFileName + count.ToString().PadLeft(4, '0') + ".csv";
                    break;
                }
            }
            while (count < 9999);

            return uniqueFileName;
        }
        #endregion

    }
}
