using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Syscon.ScheduledJob;
using System.Xml.Serialization;
using System.Reflection;


namespace Syscon.ScheduledJob.WorkOrderExportJob
{
    /// <summary>
    /// Job configuration settings for the Log job
    /// </summary>
    public class WorkOrderExportJobConfig : ScheduledJobConfig
    {

        /// <summary>
        /// Default Ctor
        /// </summary>
        public WorkOrderExportJobConfig()
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
                        WorkOrderExportJobConfig config = dsObj as WorkOrderExportJobConfig;
                        SMBDir                  = config.SMBDir;
                        ScheduledTime           = config.ScheduledTime;
                        LogFilePath             = config.LogFilePath;
                        WorkOrderExportDirectory = config.WorkOrderExportDirectory;
                        UserId                  = config.UserId;
                        Password                = config.Password;
                        LookBackPeriod          = config.LookBackPeriod;
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
        /// Work Order Export Directory 
        /// </summary>
        [XmlElement("WorkOrderExportDir")]
        public string WorkOrderExportDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Look back period in months to sync
        /// </summary>
        [XmlElement("LookBackPeriod")]
        public decimal LookBackPeriod
        {
            get;
            set;
        }
        #endregion

    }
}
