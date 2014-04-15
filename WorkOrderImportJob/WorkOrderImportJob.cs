using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Sage.SMB.API;
using Syscon.ScheduledJob;

using SysconCommon;
using SysconCommon.Common;
using SysconCommon.Common.Environment;
using SysconCommon.Parsing;
using System.Xml.Linq;

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
        COMMethods _methods = null;
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public WorkOrderImportJob()
        {
            _jobConfig = new WorkOrderImportJobConfig(this);
            _configUI = new WorkOrderImportJobConfigUI(this);

            _iXML = new IMBXML();
            _methods = new COMMethods();
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

            // Initialize Sage API
            if (!this.InitializeSageApi(_iXML, _jobConfig.SMBDir.Substring(0, 1)))
            {
                this.Log("Failed to initialize Sage API, exiting job. Check the error code for more details.");
                return;
            }

            using (var con = SysconCommon.Common.Environment.Connections.GetOLEDBConnection())
            {
                var hashed_password = _methods.smartEncrypt(_jobConfig.Password, false);

                //Login to the SMB Dir
                var login_result = con.GetScalar<int>("select count(*) from usrlst where upper(usrnme) == '{0}' and usrpsw == '{1}'", _jobConfig.UserId.ToUpper(), hashed_password);
                if (login_result == 0)
                {
                    this.Log("Login failure - Invalid user name or password. Exiting job...");
                    return;
                }

                string currentFilePath = _jobConfig.WorkOrderQueueDirectory;
                string[] files = System.IO.Directory.GetFiles(currentFilePath, "*.csv", System.IO.SearchOption.TopDirectoryOnly);

                foreach (string fileName in files)
                {
                    //Read all .csv files one by one, create the xml and send to Sage 100 using the Sage APIs
                    string fileNameWithoutPath = fileName.Substring(fileName.LastIndexOf('\\') + 1);

                    //Log the filename in the log file
                    this.Log("Processing file - {0}", fileNameWithoutPath);

                    try
                    {
                        DataTable dt = SysconCommon.Algebras.DataTables.DatatableExtensions.DataTableFromCSV("WorkOrders", fileName, true);

                        foreach (DataRow dr in dt.Rows)
                        {
                            //i.	Determine if the work order already exists based on the OrderNumber 

                            var result = con.GetScalar<int>("select count(*) from srvinv where ordnum='{0}'", dr["OrderNumber"]);
                            if (result == 0)
                            {
                                //Order number does not exits, create it.
                                string orderNum     = (string)dr["OrderNumber"];
                                string clientRef    = (string)dr["ClientRef"];
                                string desc         = (string)dr["Desc"];

                                //1. If the following fields do not exist in the csv file then skip this entry.
                                if (string.IsNullOrEmpty(orderNum) || string.IsNullOrEmpty(clientRef) || string.IsNullOrEmpty(desc))
                                {
                                    this.Log("Invalid record in csv file - {0}. Skipping this entry, OrderNumber = {1}, ClientRef = {2}, Desc = {3}.",
                                                        fileNameWithoutPath, orderNum, clientRef, desc);
                                    continue;
                                }

                                string iXMLdoc = CreateWorkOrderXml(dr);

                                // Submit XML request and get response. Provide the password that matches the user ID specified in the XML document
                                string iXMLOut = _iXML.submitXML(iXMLdoc, _jobConfig.Password);

                                //Process the response Xml
                                //If - statusCode="0" statusMessage="Ok" - Then SUCCESS
                                if (!(iXMLOut.Contains("statusCode=\"0\"") && iXMLOut.Contains("statusMessage=\"Ok\"")))
                                {
                                    this.Log("Failed to add record for Order Number - {0}", dr["OrderNumber"]);
                                    this.Log("Response Xml from Sage - \n {0}", iXMLOut);
                                }
                                else
                                {
                                    this.Log("Record sucessfully added for new Order Number - {0}", dr["OrderNumber"]);
                                    this.Log("Response Xml from Sage - \n {0}", iXMLOut);
                                }
                            }
                            else
                            {
                                int recNum = con.GetScalar<int>("select recnum from srvinv where ordnum='{0}'", dr["OrderNumber"]);

                                //Order number exists, Update it. For updation recnum is required.
                                string iXMLdoc = CreateWorkOrderUpdateXml(dr, recNum);

                                // Submit XML request and get response. Provide the password that matches the user ID specified in the XML document
                                string iXMLOut = _iXML.submitXML(iXMLdoc, _jobConfig.Password);

                                if (iXMLOut.Contains("statusCode=\"0\"") && iXMLOut.Contains("statusMessage=\"Ok\""))
                                {
                                    this.Log("Record sucessfully updated. Invoice Number - {0}, RecNum - {1}", dr["OrderNumber"], recNum);
                                    this.Log("Response Xml from Sage - \n {0}", iXMLOut);
                                }
                            }
                        }

                        this.Log("The file - {0} has been processed successfully.", fileNameWithoutPath);

                        //If file is successfully processed then move it into a child archive folder so that it is not processed again
                        this.MoveFileToProcessedFolder(fileName);
                    }
                    catch (Exception ex)
                    {
                        this.Log("Exception in work order import job while processing file - {0}.\nException Message - {1} \nStack trace - {2}",
                                            fileNameWithoutPath, ex.Message, ex.StackTrace);
                    }
                    finally
                    {
                        _iXML.DisableRequests();
                        _iXML.DeIntializeAPI();
                    }
                }
            }

            //Log end of execution, time etc.
            this.Log("Finished execution of work order import job.");
            this.Log("-----------------------------------------------------------------------------------------\n");
        }

        /// <summary>
        /// This method initializes Sage APIs
        /// </summary>
        /// <param name="iXML"></param>
        /// <returns></returns>
        private bool InitializeSageApi(IMBXML iXML, string dataDrive)
        {
            int retVal;

            // intialize API and error handling
            retVal = iXML.IntializeAPI();
            if (retVal != 0)
            {
                this.Log("SMB API failed to initialize. Error code - {0}", retVal);
                return false;
            }

            // Set data drive and error handling
            retVal = iXML.SetDataDrive(dataDrive);
            if (retVal != 0)
            {
                this.Log("Failed to set data drive. Error code - {0}", retVal);
                return false;
            }

            retVal = iXML.EnableRequests();
            if (retVal != 0)
            {
                this.Log("Failed to enable request to Sage APIs. Error code - {0}", retVal);
                return false;
            }

            return true;
        }                

        /// <summary>
        /// Set the configuration settings for the job.
        /// </summary>
        public void SetJobConfiguration()
        {
            if (_configUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _jobConfig.LoadConfig();
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

        /// <summary>
        /// Create add work order xml.
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        private string CreateWorkOrderXml(DataRow dr)
        {
            XNamespace nSpace = "http://sagemasterbuilder.com/api";
            XElement root = new XElement(nSpace + "MBXML",
                                    new XAttribute(XNamespace.Xmlns + "api", nSpace));

            XElement element = new XElement("MBXMLSessionRq",
                                                new XElement("Company", "Sample Company"),
                                                new XElement("User", _jobConfig.UserId));

            root.Add(element);

            element = new XElement("MBXMLMsgsRq",
                                        new XAttribute("messageSetID", Guid.NewGuid().ToString()),
                                        new XAttribute("onError", "continueOnError"));
            root.Add(element);

            XElement workOrderElement = new XElement("WorkOrderAddRq",
                                                 new XAttribute("requestID", 1));
            element.Add(workOrderElement);

            XElement dcElement = new XElement("OrderNumber", dr["OrderNumber"]);
            workOrderElement.Add(dcElement);

            dcElement = new XElement("ClientRef",
                                        new XElement("ObjectID", dr["ClientRef"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("WorkOrderDate", FormatDate((string)dr["WorkOrderDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("Desc", dr["Desc"]);
            workOrderElement.Add(dcElement);

            dcElement = new XElement("Addr1", dr["Addr1"]);
            workOrderElement.Add(dcElement);

            dcElement = new XElement("Addr2", dr["Addr2"]);
            workOrderElement.Add(dcElement);

            dcElement = new XElement("CallReceivedDate", FormatDate((string)dr["CallReceivedDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("ScheduledDate", FormatDate((string)dr["ScheduledDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("CompletedDate", FormatDate((string)dr["CompletedDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("ServiceInvoiceTypeRef",
                                        new XElement("ObjectID", dr["ServiceInvoiceTypeRef"]));
            workOrderElement.Add(dcElement);

            workOrderElement.Add(new XElement("WorkOrderStatus", 7));
            workOrderElement.Add(new XElement("PriorityRef",
                                        new XElement("ObjectID", 3)));

            //EntryDate - DateTime.Now          
            //workOrderElement.Add(new XElement("EntryDate", DateTime.Now.ToShortDateString()));

            return root.ToString();
        }

        private string CreateWorkOrderUpdateXml(DataRow dr, int recNum)
        {
            string outXml = string.Empty;

            // create the root element, with the 'PanelControls' namespace
            XNamespace nSpace = "http://sagemasterbuilder.com/api";
            XElement root = new XElement(nSpace + "MBXML",
                                    new XAttribute(XNamespace.Xmlns + "api", nSpace));

            XElement element = new XElement("MBXMLSessionRq",
                                                new XElement("Company", "Sample Company"),
                                                new XElement("User", _jobConfig.UserId));
            root.Add(element);

            element = new XElement("MBXMLMsgsRq",
                                        new XAttribute("messageSetID", Guid.NewGuid().ToString()),
                                        new XAttribute("onError", "continueOnError"));
            root.Add(element);

            XElement workOrderElement = new XElement("WorkOrderModRq",
                                                 new XAttribute("requestID", 1));
            element.Add(workOrderElement);

            //For update the ObjectRef value should be the srvinv.RecNum
            XElement dcElement = new XElement("ObjectRef",
                                        new XElement("ObjectID", recNum));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("InvoiceNumber", dr["OrderNumber"]);
            workOrderElement.Add(dcElement);

            dcElement = new XElement("WorkOrderDate", FormatDate((string)dr["WorkOrderDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("Desc", dr["Desc"] + "By AMIT");
            workOrderElement.Add(dcElement);

            dcElement = new XElement("Addr1", dr["Addr1"]);
            workOrderElement.Add(dcElement);

            dcElement = new XElement("Addr2", dr["Addr2"]);
            workOrderElement.Add(dcElement);

            dcElement = new XElement("CallReceivedDate", FormatDate((string)dr["CallReceivedDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("ScheduledDate", FormatDate((string)dr["ScheduledDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("CompletedDate", FormatDate((string)dr["CompletedDate"]));
            workOrderElement.Add(dcElement);

            dcElement = new XElement("ServiceInvoiceTypeRef",
                                        new XElement("ObjectID", dr["ServiceInvoiceTypeRef"]));
            workOrderElement.Add(dcElement);

            workOrderElement.Add(new XElement("WorkOrderStatus", 7));
            workOrderElement.Add(new XElement("PriorityRef",
                                        new XElement("ObjectID", 3)));
            //workOrderElement.Add(new XElement("EntryDate", FormatDate(DateTime.Now.ToShortDateString())));

            return root.ToString();
        }

        /// <summary>
        /// Format the date to Sage acceptable format.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static string FormatDate(string date)
        {
            string formattedDate = string.Empty;
            DateTime dt;

            if (DateTime.TryParse(date, out dt))
            {
                formattedDate = string.Format("{0}-{1}-{2}", dt.Year.ToString(), dt.Month.ToString().PadLeft(2, '0'), dt.Day.ToString().PadLeft(2, '0'));
            }
            return formattedDate;
        }

        /// <summary>
        /// Move the processed file to processed folder.
        /// </summary>
        /// <param name="fileName"></param>
        private void MoveFileToProcessedFolder(string fileName)
        {
            string fileNameWithoutPath = Path.GetFileName(fileName);

            string archivePath = Path.Combine(Path.GetDirectoryName(fileName), "Processed Files\\");
            if (!Directory.Exists(archivePath))
            {
                Directory.CreateDirectory(archivePath);
            }
            File.Move(fileName, archivePath + fileNameWithoutPath);
            this.Log("The file - {0} moved to processed folder after successful import.", fileNameWithoutPath);
        }

        #endregion

    }
}
