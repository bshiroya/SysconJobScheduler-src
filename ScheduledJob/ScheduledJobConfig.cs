using System;
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
    /// 
    /// </summary>
    public abstract class ScheduledJobConfig: IScheduledJobConfig
    {
        private XmlSerializer _xmlSerializer;
        protected IScheduledJob _scheduledJob = null;
        protected DateTime _scheduledTime;

        /// <summary>
        /// Default Ctor
        /// </summary>
        public ScheduledJobConfig()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public ScheduledJobConfig(IScheduledJob scheduledJob)
        {
            _scheduledJob = scheduledJob;
        }

        #region IScheduledJobConfig Members

        /// <summary>
        /// 
        /// </summary>
        public IScheduledJob ScheduledJob
        {
            get { return _scheduledJob; }
        }

        /// <summary>
        /// 
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
        [XmlElement(ElementName="ScheduledTime")]
        public DateTime ScheduledTime
        {
            get
            {
                return _scheduledTime;
            }
            set
            {
                _scheduledTime = value;
                if (_scheduledJob != null)
                    _scheduledJob.ScheduledTime = value;
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
        /// 
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
        /// 
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
