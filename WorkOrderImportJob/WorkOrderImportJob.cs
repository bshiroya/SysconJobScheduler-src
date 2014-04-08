using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

using Sage.SMB.API;
using SysconCommon.Common;
using SysconCommon.Common.Environment;
using SysconCommon.Parsing;

using Syscon.ScheduledJob;
using System.Data;

namespace Syscon.ScheduledJob.WorkOrderImportJob
{
    /// <summary>
    /// Implementation of the work order import job.
    /// </summary>
    [Export(typeof(IScheduledJob))]    
    public class WorkOrderImportJob : IScheduledJob
    {
        #region Member variables
        private WorkOrderImportJobConfig    _jobConfig = null;
        private WorkOrderImportJobConfigUI  _configUI   = null;
        private DateTime                    _scheduledTime;

        // Create instance of sage API
        private IMBXML _iXML = null;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public WorkOrderImportJob()
        {
            _jobConfig = new WorkOrderImportJobConfig(this);
            _configUI = new WorkOrderImportJobConfigUI(this);

            _iXML = new IMBXML();
        }

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public void ExceuteJob()
        {
            this.Log("Started execution of work order import job.");

            _jobConfig.LoadConfig();
            SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(_jobConfig.SMBDir);

            string      currentFilePath = _jobConfig.WorkOrderQueueDirectory;
            string[]    files           = System.IO.Directory.GetFiles(currentFilePath, "*.csv", System.IO.SearchOption.TopDirectoryOnly);

            foreach (string fileName in files)
            {
                //Read all .csv files one by one, create the xml and send to Sage 100 using the Sage APIs
                string fileNameWithoutPath = fileName.Substring(fileName.LastIndexOf('\\') + 1);

                //Log the filename in the log file
                this.Log("Processing file - {0}", fileNameWithoutPath);

                try
                {
                    DataTable dt = CSV.ParseFile(fileName);

                    foreach (DataRow dr in dt.Rows)
                    {
                        string iXMLdoc = CreateWorkOrderXml(dr);
                        string iXMLOut;

                        // Submit XML request and get response. Provide the password that matches the user ID specified in the XML document
                        iXMLOut = _iXML.submitXML(iXMLdoc, _jobConfig.Password);

                        //Process the response Xml

                    }
                    //If file is successfully processed then move it into a child archive folder so that it is not processed again
                    string archivePath = Path.Combine(Path.GetDirectoryName(fileName), "Processed Files");
                    if (!Directory.Exists(archivePath))
                    {
                        Directory.CreateDirectory(archivePath);
                    }
                    File.Move(fileName, archivePath + fileNameWithoutPath);
                    Log("The file - {0} has been imported successfully.", DateTime.Now.ToString(), fileNameWithoutPath);
                }
                catch (Exception ex)
                { 

                }
            }

            //Log end of execution, time etc.
            this.Log("Finished execution of work order import job. Start time: {0}", DateTime.Now.ToString());
        }
                

        /// <summary>
        /// Set the configuration settings for the job.
        /// </summary>
        public void SetJobConfiguration()
        {
            if (_configUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.ScheduledTime = _jobConfig.ScheduledTime;
            }
        }

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IScheduledJobConfig JobConfig
        {
            get { return _jobConfig; }
        }

        /// <summary>
        /// The time scheduled to run this job.
        /// </summary>
        public DateTime ScheduledTime
        {
            get { return _scheduledTime; }
            set { _scheduledTime = value; }
        }

        /// <summary>
        /// The unique Guid of this job for identification.
        /// </summary>
        public Guid JobId
        {
            get { return GetAssemblyGuid(); }
        }

        /// <summary>
        /// Job Desctription
        /// </summary>
        public string JobDesc
        {
            get { return "Job for importing work orders"; }
        }

        /// <summary>
        /// The job status.
        /// </summary>
        public JobStatus JobStatus
        {
            get;
            set;
        }

        /// <summary>
        /// The config file path for this job.
        /// </summary>
        public string ConfigFilePath
        {
            get { return Assembly.GetExecutingAssembly().GetName().Name + ".xml"; }
        }

        /// <summary>
        /// The log file path for this job.
        /// </summary>
        public string LogFilePath
        {
            get { return _jobConfig.LogFilePath; }
        }

        /// <summary>
        /// Gets or sets whether this job is enqueued to the scheduler of not.
        /// </summary>
        public bool Enqueued
        {
            get;
            set;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Guid GetAssemblyGuid()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            var attr = (GuidAttribute)asm.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            return new Guid(attr.Value);
        }        

        /// <summary>
        /// Log a message to LogFile, the format is the same as string.Format
        /// </summary>
        /// <param name="msgFormat"></param>
        /// <param name="arguments"></param>
        private void Log(string msgFormat, params object[] arguments)
        {
            try
            {
                File.AppendAllText(LogFilePath, string.Format(DateTime.Now.ToString() + " - " + msgFormat + "\r\n", arguments));
            }
            catch
            {
                Env.Log("Error writing work order import job log file");
            }
        }

        private string CreateWorkOrderXml(DataRow dr)
        {
            string outXml = string.Empty;

            //TODO: Read the WorkOrderAddRq.xml and fill the relevant data

            return outXml;
        }

        #endregion

    }
}
