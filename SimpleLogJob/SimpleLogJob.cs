using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.OleDb;
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
using SysconCommon.DBManipulate;
using SysconCommon.Foxpro;
using SysconCommon.GUI;

using Syscon.ScheduledJob;

namespace Syscon.ScheduledJob.SimpleLogJob
{
    /// <summary>
    /// Implementation for a simple log job.
    /// </summary>
    [Export(typeof(IScheduledJob))]    
    public class SimpleLogJob : ScheduledJob
    {
        #region Member Variables
        private SimpleLogJobConfigUI _configUI      = null;
        private COMMethods           _methods       = null;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public SimpleLogJob() : base()
        {
            _jobConfig = new SimpleLogJobConfig();
            _configUI = new SimpleLogJobConfigUI();

            _methods = new COMMethods();

            //Load the config
            _jobConfig.LoadConfig();
        }

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public override void ExceuteJob()
        {
            this.Log("Starting log job execution");
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

                int count = con.GetScalar<int>("select count(*) from lgrtrn");

                //Log the result
                this.Log("Count of rows in lgrtrn table : {0}", count);
            }

            this.Log("Finished log job execution");
            this.Log("-----------------------------------------------------------------------------------------\n");
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
            get { return "Simple Log Job"; }
        }      

        /// <summary>
        /// The log file path for this job.
        /// </summary>
        public override string LogFilePath
        {
            get
            {
                return _jobConfig.LogFilePath;
            }
        }

        #endregion

        #region Private/Protected Methods

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
            catch(Exception ex)
            {
                Env.Log("Error writing to the simple log job file \n" + ex.Message);
            }
        }

        #endregion

    }
}
