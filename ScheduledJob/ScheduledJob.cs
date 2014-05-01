using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace Syscon.ScheduledJob
{
    /// <summary>
    /// Base class for Scheduled Job
    /// </summary>
    public abstract class ScheduledJob : IScheduledJob
    {
        #region Member Variables

        protected IScheduledJobConfig   _jobConfig = null;
        protected string                _scheduledTime = null;
        #endregion

        /// <summary>
        /// Default Ctor
        /// </summary>
        public ScheduledJob()
        {
        }

        #region IScheduledJob Members

        #region Properties

        /// <summary>
        /// Job Config
        /// </summary>
        public IScheduledJobConfig JobConfig
        {
            get { return _jobConfig; }
        }

        /// <summary>
        /// The unique Guid of this job for identification.
        /// </summary>
        public virtual Guid JobId
        {
            get { return GetAssemblyGuid(); }
        }

        /// <summary>
        /// Job Desctription
        /// </summary>
        public abstract string JobDesc
        {
            get;
        }

        /// <summary>
        /// The job status.
        /// </summary>
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
            get 
            { 
                return Assembly.GetAssembly(this.GetType()).GetName().Name + ".xml";
            }
        }

        /// <summary>
        /// The log file path for this job.
        /// </summary>
        public abstract string LogFilePath
        {
            get;            
        }

        /// <summary>
        /// Gets or sets whether this job is enqueued to the scheduler of not.
        /// </summary>
        public bool Enqueued
        {
            get;
            set;
        }

        /// <summary>
        /// The time scheduled to run this job.
        /// </summary>
        public string ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }

        #endregion  //Properties

        #region Methods

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public abstract void ExceuteJob();

        /// <summary>
        /// Set the configuration settings for the job.
        /// </summary>
        public abstract void SetJobConfiguration();

        #endregion  //Methods

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Guid GetAssemblyGuid()
        {
            Assembly asm = Assembly.GetAssembly(this.GetType());
            var attr = (GuidAttribute)asm.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return new Guid(attr.Value);
        }

        /// <summary>
        /// Log a message to LogFile, the format is the same as string.Format
        /// </summary>
        /// <param name="msgFormat"></param>
        /// <param name="arguments"></param>
        protected virtual void Log(string msgFormat, params object[] arguments)
        {
            try
            {
                File.AppendAllText(this.LogFilePath, string.Format(DateTime.Now.ToString() + " - " + msgFormat + "\r\n", arguments));
            }
            catch
            {
                //Env.Log("Error writing to the simple log job file \n" + ex.Message);
            }
        }

        #endregion
    }
}
