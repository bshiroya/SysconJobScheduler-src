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
        /// Ctor
        /// </summary>
        public WorkOrderImportJobConfig(IScheduledJob scheduledJob)
            :base(scheduledJob)
        {
            _workOrderImportJob = scheduledJob as WorkOrderImportJob;
        }

        /// <summary>
        /// Load the config.
        /// </summary>
        public override void LoadConfig()
        {
            XmlSerializer _xmlSerializer = new XmlSerializer(this.GetType());

            try
            {
                string configFile = string.Format(@"{0}\{1}.xml", AssemblyPath, AssemblyName);

                if (File.Exists(configFile))
                {
                    using (FileStream stream = new FileInfo(configFile).OpenRead())
                    {
                        var dsObj = _xmlSerializer.Deserialize(stream);
                        WorkOrderImportJobConfig config = dsObj as WorkOrderImportJobConfig;
                        SMBDir                  = config.SMBDir;
                        ScheduledTime           = config.ScheduledTime;
                        LogFilePath             = config.LogFilePath;
                        WorkOrderQueueDirectory = config.WorkOrderQueueDirectory;
                        UserId                  = config.UserId;
                        Password                = config.Password;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log exception
            }
            finally
            {
                _xmlSerializer = null;
            }
        }

        /// <summary>
        /// User Id
        /// </summary>
        [XmlElement("UserId")]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// User Password
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
    }
}
