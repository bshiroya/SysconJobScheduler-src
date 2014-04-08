using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

using Syscon.ScheduledJob;

namespace Syscon.ScheduledJob.ExportBillingUpdatesJob
{
    /// <summary>
    /// Job configuration settings for the Log job
    /// </summary>
    public class ExportBillingUpdatesJobConfig : ScheduledJobConfig
    {
        private ExportBillingUpdatesJob _exportBillingUpdatesJob = null;        

        /// <summary>
        /// Default Ctor
        /// </summary>
        public ExportBillingUpdatesJobConfig()
            :base()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        public ExportBillingUpdatesJobConfig(IScheduledJob scheduledJob)
            :base(scheduledJob)
        {
            _exportBillingUpdatesJob = scheduledJob as ExportBillingUpdatesJob;
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
                        ExportBillingUpdatesJobConfig config = dsObj as ExportBillingUpdatesJobConfig;
                        SMBDir                      = config.SMBDir;
                        ScheduledTime               = config.ScheduledTime;
                        LogFilePath                 = config.LogFilePath;
                        BillingUpdateQueueDirectory = config.BillingUpdateQueueDirectory;
                        UserId                      = config.UserId;
                        Password                    = config.Password;
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
        /// User Id.
        /// </summary>
        [XmlElement("UserId")]
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// User password
        /// </summary>
        [XmlElement("Password")]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Billing Update Export File Directory 
        /// </summary>
        [XmlElement("BillingUpdateQueueDir")]
        public string BillingUpdateQueueDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("LogFilePath")]
        public string LogFilePath
        {
            get;
            set;
        }
    }
}
