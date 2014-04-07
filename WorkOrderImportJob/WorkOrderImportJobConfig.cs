using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Syscon.ScheduledJob;
using System.Xml.Serialization;
using System.Reflection;


namespace Syscon.ScheduledJob.WorkOrderImportJob
{
    /// <summary>
    /// Job configuration settings for the Log job
    /// </summary>
    public class WorkOrderImportJobConfig : ScheduledJobConfig
    {
        private WorkOrderImportJob _workOrderImportJob = null;

        /// <summary>
        /// Default Ctor
        /// </summary>
        public WorkOrderImportJobConfig()
            :base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public WorkOrderImportJobConfig(IScheduledJob scheduledJob)
            :base(scheduledJob)
        {
            _workOrderImportJob = scheduledJob as WorkOrderImportJob;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("UserId")]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Password")]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Work Order Import File Directory 
        /// </summary>
        [XmlElement("WorkOrderQueueDir")]
        public string WorkOrderQueueDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("LogFile")]
        public string LogFilePath
        {
            get;
            set;
        }
    }
}
