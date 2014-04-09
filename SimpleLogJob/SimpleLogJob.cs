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
        }

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public void ExceuteJob()
        {
            _jobConfig.LoadConfig();

            SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(_jobConfig.SMBDir);

            using (var con = SysconCommon.Common.Environment.Connections.GetOLEDBConnection())
            {
                using (var jobtyps = con.GetTempDBF())
                {
                    int count = con.GetScalar<int>("select count(*) from lgrtrn");

                    //Log the result
                    Env.Log("Count of rows in lgrtrn table : {0}", count);

                    //Log to the plug-in job specific log file
               }
            }
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
        [DataMember]
        public DateTime ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }

        /// <summary>
        /// The unique Guid of this job for identification.
        /// </summary>
        [DataMember]
        public Guid JobId
        {
            get { return GetAssemblyGuid(); }
        }

        /// <summary>
        /// Job Desctription
        /// </summary>
        [DataMember]
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
        [DataMember]
        public string ConfigFilePath
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name + ".xml"; }
        }        
        
        /// <summary>
        /// The log file path for this job.
        /// </summary>
        [DataMember]
        public string LogFilePath
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name + ".txt"; }
        }

        /// <summary>
        /// Gets or sets whether this job is enqueued to the scheduler of not.
        /// </summary>
        [DataMember]
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
