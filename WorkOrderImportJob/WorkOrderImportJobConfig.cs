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

        /// <summary>
        /// Default Ctor
        /// </summary>
        public WorkOrderImportJobConfig()
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
        /// Work Order Import File Directory 
        /// </summary>
        [XmlElement("WorkOrderQueueDir")]
        public string WorkOrderQueueDirectory
        {
            get;
            set;
        }
        #endregion

    }
}
