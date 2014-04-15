using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel.Composition;
using System.Data.OleDb;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

using SysconCommon.Common;
using SysconCommon.Common.Environment;
using SysconCommon.Algebras.DataTables;
using SysconCommon.DBManipulate;
using SysconCommon.GUI;
using SysconCommon.Foxpro;

using Syscon.ScheduledJob;
using System.IO;


namespace Syscon.ScheduledJob.SimpleLogJob
{
    /// <summary>
    /// Implementation for a simple log job.
    /// </summary>
    [Export(typeof(IScheduledJob))]    
    [DataContract]
    public class SimpleLogJob : IScheduledJob
    {
        IScheduledJobConfig _jobConfig = null;
        SimpleLogJobConfigUI configUI = null;

        DateTime _scheduledTime;

        /// <summary>
        /// Ctor
        /// </summary>
        public SimpleLogJob()
        {
            _jobConfig = new SimpleLogJobConfig(this);

            configUI = new SimpleLogJobConfigUI(this);

            //Load the config
            _jobConfig.LoadConfig();
        }

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public void ExceuteJob()
        {
            this.Log("Starting log job execution");
            _jobConfig.LoadConfig();

            SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(_jobConfig.SMBDir);

            using (var con = SysconCommon.Common.Environment.Connections.GetOLEDBConnection())
            {
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
        public void SetJobConfiguration()
        {
            if (configUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //TODO: The config saving and loading logic should be outside the config. 
                //Possibly put the logic inside this ConfigUI itself.
                //_jobConfig.SaveConfig();

                _jobConfig.LoadConfig();
            }
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IScheduledJobConfig JobConfig
        {
            get { return _jobConfig; }
        }

        /// <summary>
        /// The time scheduled to run this job.
        /// </summary>
        public DateTime ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }

        /// <summary>
        /// The unique Guid of this job for identification.
        /// </summary>
        public Guid JobId
        {
            get { return GetAssemblyGuid(); }
        }

        /// <summary>
        /// Job Desctription
        /// </summary>
        public string JobDesc
        {
            get { return "Simple Log Job"; }
        }

        /// <summary>
        /// The job status.
        /// </summary>
        [DataMember]
        public JobStatus JobStatus
        {
            get;
            set;
        }

        /// <summary>
        /// The config file path for this job.
        /// </summary>
        public string ConfigFilePath
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name + ".xml"; }
        }        
        
        /// <summary>
        /// The log file path for this job.
        /// </summary>
        public string LogFilePath
        {
            get 
            {
                return _jobConfig.LogFilePath;
            }
        }

        /// <summary>
        /// Gets or sets whether this job is enqueued to the scheduler of not.
        /// </summary>
        public bool Enqueued
        {
            get;
            set;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Guid GetAssemblyGuid()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var attr = (GuidAttribute)asm.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return new Guid(attr.Value);
        }

        /// <summary>
        /// Log a message to LogFile, the format is the same as string.Format
        /// </summary>
        /// <param name="msgFormat"></param>
        /// <param name="arguments"></param>
        private void Log(string msgFormat, params object[] arguments)
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //public XElement SaveAsXml()
        //{
        //    return new XElement("FilterSettings", new object[]
        //    {
        //        new XElement("ShowInternalAPI", true),
        //        new XElement("Language", "")
        //    });
        //}

        #endregion

    }
}
