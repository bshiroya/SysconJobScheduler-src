using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Syscon.ScheduledJob;
using System.Xml.Serialization;
using System.Reflection;


namespace Syscon.ScheduledJob.PayTimeImportJob
{
    /// <summary>
    /// Job configuration settings for the Log job
    /// </summary>
    public class PayTimeImportJobConfig : ScheduledJobConfig
    {

        /// <summary>
        /// Default Ctor
        /// </summary>
        public PayTimeImportJobConfig()
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
                        PayTimeImportJobConfig config = dsObj as PayTimeImportJobConfig;
                        SMBDir                  = config.SMBDir;
                        ScheduledTime           = config.ScheduledTime;
                        LogFilePath             = config.LogFilePath;
                        PayTimeImportCsvFile    = config.PayTimeImportCsvFile;
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
        /// Payroll time import CSV file 
        /// </summary>
        [XmlElement("PayTimeImportCsvFile")]
        public string PayTimeImportCsvFile
        {
            get;
            set;
        }


        #endregion

    }
}
