﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;

namespace Syscon.ScheduledJob
{
    /// <summary>
    /// Base class for schedule job config
    /// </summary>
    public abstract class ScheduledJobConfig: IScheduledJobConfig
    {
        private XmlSerializer _xmlSerializer;
        protected string _scheduledTime;

        /// <summary>
        /// Default Ctor
        /// </summary>
        public ScheduledJobConfig()
        {
        }

        #region IScheduledJobConfig Members

        /// <summary>
        /// SMB Directory
        /// </summary>
        [XmlElement(ElementName = "SMBDir")]
        public virtual string SMBDir
        {
            get;
            set;
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
        /// The time scheduled to run this job.
        /// </summary>
        [XmlElement(ElementName="ScheduledTime")]
        public string ScheduledTime
        {
            get
            {
                return string.IsNullOrEmpty(_scheduledTime) ? "Not Set" : _scheduledTime;
            }
            set
            {
                _scheduledTime = value;
            }
        }

        /// <summary>
        /// Get the current executing assembly name.
        /// </summary>
        [XmlIgnore()]
        public string AssemblyName
        {
            get
            {
                return Assembly.GetAssembly(this.GetType()).GetName().Name;
            }
        }

        /// <summary>
        /// Get the current executing assembly path.
        /// </summary>
        [XmlIgnore()]
        public string AssemblyPath
        {
            get
            {
                string location = Assembly.GetAssembly(this.GetType()).Location;
                return location.Substring(0, location.LastIndexOf('\\'));
            }
        }

        /// <summary>
        /// Log file path
        /// </summary>
        [XmlElement("LogFile")]
        public string LogFilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Save the config.
        /// </summary>
        public virtual void SaveConfig()
        {
            try
            {
                _xmlSerializer = new XmlSerializer(this.GetType());

                using (StreamWriter writer = new StreamWriter(string.Format(@"{0}\{1}.xml", AssemblyPath, AssemblyName), false))
                {
                    _xmlSerializer.Serialize(writer, this);
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
        /// Load the config.
        /// </summary>
        public virtual void LoadConfig()
        {
            try
            {
                _xmlSerializer = new XmlSerializer(this.GetType());
                string configFile = string.Format(@"{0}\{1}.xml", AssemblyPath, AssemblyName);

                if (File.Exists(configFile))
                {
                    using (FileStream stream = new FileInfo(configFile).OpenRead())
                    {
                        var dsObj = _xmlSerializer.Deserialize(stream);
                        IScheduledJobConfig config = dsObj as IScheduledJobConfig;
                        SMBDir = config.SMBDir;
                        ScheduledTime = config.ScheduledTime;
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

        #endregion
               
    }
}
