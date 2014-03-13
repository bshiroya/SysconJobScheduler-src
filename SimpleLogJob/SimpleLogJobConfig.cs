using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Syscon.ScheduledJob;
using System.Xml.Serialization;
using System.Reflection;


namespace Syscon.ScheduledJob.SimpleLogJob
{
    /// <summary>
    /// Job configuration settings for the Log job
    /// </summary>
    public class SimpleLogJobConfig : ScheduledJobConfig
    {
        private SimpleLogJob _simpleLogJob = null;


        /// <summary>
        /// Default Ctor
        /// </summary>
        public SimpleLogJobConfig()
            :base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public SimpleLogJobConfig(IScheduledJob scheduledJob)
            :base(scheduledJob)
        {
            _simpleLogJob = scheduledJob as SimpleLogJob;
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
        

        //public override void LoadConfig()
        //{
        //    //Load the config settings.
        //    base.LoadConfig();
            
        //}

        //public override void SaveConfig()
        //{
        //    //Serialize the job config
        //    XmlSerializer serializer = new XmlSerializer(typeof(SimpleLogJobConfig));
        //    TextWriter txtWriter = new StreamWriter("SimpleLogJobConfig.xml");
        //}
    }
}
