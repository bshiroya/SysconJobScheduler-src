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
        /// <summary>
        /// Default Ctor
        /// </summary>
        public ExportBillingUpdatesJobConfig()
            :base()
        {
        }

        #region IScheduledJobConfig Members

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
        /// Billing Update Export File Directory 
        /// </summary>
        [XmlElement("BillingUpdateQueueDir")]
        public string BillingUpdateQueueDirectory
        {
            get;
            set;
        }

        #endregion
    }
}
