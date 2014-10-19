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
using System.Xml.Linq;

using Sage.SMB.API;

using Syscon.ScheduledJob;
using SysconCommon;
using SysconCommon.Algebras.DataTables;
using SysconCommon.Common;
using SysconCommon.Common.Environment;
using SysconCommon.Parsing;

namespace Syscon.ScheduledJob.WorkOrderImportJob
{
    /// <summary>
    /// Implementation of the work order import job.
    /// </summary>
    [Export(typeof(IScheduledJob))]    
    public class WorkOrderImportJob : ScheduledJob
    {
        #region Member variables
        private WorkOrderImportJobConfigUI  _configUI   = null;

        // Create instance of sage API
        private IMBXML _iXML = null;
        private COMMethods _methods = null;
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public WorkOrderImportJob(): base()
        {
            _jobConfig = new WorkOrderImportJobConfig();
            _configUI = new WorkOrderImportJobConfigUI();

            _iXML = new IMBXML();
            _methods = new COMMethods();
        }

        #region IScheduledJob Members

        #region Methods

        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        public override void ExceuteJob()
        {
            _jobConfig.LoadConfig();

            this.Log("Started execution of work order import job.");

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

                string currentFilePath = (_jobConfig as WorkOrderImportJobConfig).WorkOrderQueueDirectory;
                string[] files = System.IO.Directory.GetFiles(currentFilePath, "*.csv", System.IO.SearchOption.TopDirectoryOnly);

                try
                {
                    foreach (string fileName in files)
                    {
                        //Read all .csv files one by one, create the xml and send to Sage 100 using the Sage APIs
                        string fileNameWithoutPath = fileName.Substring(fileName.LastIndexOf('\\') + 1);
                        bool partialSuccess = false;

                        //Log the filename in the log file
                        this.Log("Processing file - {0}", fileNameWithoutPath);

                        try
                        {
                            DataTable dt = this.DataTableFromCSV("WorkOrders", fileName, true);

                            foreach (DataRow dr in dt.Rows)
                            {
                                //i.	Determine if the work order already exists based on the OrderNumber 
                                var result = con.GetScalar<int>("select count(*) from srvinv where ordnum='{0}'", dr["OrderNumber"]);
                                if (result == 0)
                                {
                                    //Order number does not exits, create it.
                                    string orderNum = (string)dr["OrderNumber"];
                                    string clientRef = (string)dr["ClientRef"];
                                    string desc = (string)dr["Desc"];

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
                                        partialSuccess = true;
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

                            if (partialSuccess)
                            {
                                this.Log("The import of file - {0} to Sage was partially successful. Please check the log file for failed entries.", fileNameWithoutPath);
                                partialSuccess = false;
                            }
                            else
                            {
                                this.Log("The file - {0} has been processed successfully.", fileNameWithoutPath);
                            }

                            //If file is successfully processed then move it into a child archive folder so that it is not processed again
                            this.MoveFileToProcessedFolder(fileName);
                        }
                        catch (Exception ex)
                        {
                            this.Log("Exception in work order import job while processing file - {0}.\nException Message - {1} \nStack trace - {2}",
                                                fileNameWithoutPath, ex.Message, ex.StackTrace);
                        }                        
                    }
                }
                catch (Exception ex)
                {
                    this.Log("Exception in work order import job. Exception - {0}", ex.Message);
                }
                finally
                {
                    _iXML.DisableRequests();
                    _iXML.DeIntializeAPI();
                }
            }

            //Log end of execution, time etc.
            this.Log("Finished execution of work order import job.");
            this.Log("-----------------------------------------------------------------------------------------\n");
        }

        /// <summary>
        /// Set the configuration settings for the job.
        /// </summary>
        public override void SetJobConfiguration()
        {
            if (_configUI.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _jobConfig.LoadConfig();
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Job Desctription
        /// </summary>
        public override string JobDesc
        {
            get { return "Job for importing work orders"; }
        }

        /// <summary>
        /// The log file path for this job.
        /// </summary>
        public override string LogFilePath
        {
            get { return _jobConfig.LogFilePath; }
        }

        #endregion

        #endregion


        #region Protected Methods

        /// <summary>
        /// Log a message to LogFile, the format is the same as string.Format
        /// </summary>
        /// <param name="msgFormat"></param>
        /// <param name="arguments"></param>
        protected override void Log(string msgFormat, params object[] arguments)
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

        #endregion

        #region Private Methods

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
            this.Log("The file - {0} moved to processed folder after import.", fileNameWithoutPath);
        }

        /// <summary>
        /// Copied from SysconCommon and modified to exclude blank rows and column names in .csv files.
        /// </summary>
        /// <param name="tblName"></param>
        /// <param name="csvFile"></param>
        /// <param name="includesHeaders"></param>
        /// <returns></returns>
        private DataTable DataTableFromCSV(string tblName, string csvFile, bool includesHeaders)
        {
            List<string> allLines = new List<string>();
            using (StreamReader sr = new StreamReader(csvFile))
            {
                string line = null;
                while ((line = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line.Trim(',')))
                        allLines.Add(line);
                }
            }
            var lines = allLines.ToArray();
            var data = (from l in lines
                        select l.Split(',')).ToArray();

            if (data.Length == 0)
                return null;

            if (data[0].Length == 0)
                return null;

            foreach (var i in FunctionalOperators.Range(1, data.Length))
            {
                if (data[i].Length != data[0].Length)
                    throw new InvalidOperationException("CSV File has inconsistent amount of fields in lines");
            }

            var headers = includesHeaders ? data[0] : null;
            //Trim for empty header names and then trim the empty data rows
            headers = headers.TakeWhile(s => !string.IsNullOrEmpty(s.TrimEnd(','))).ToArray();

            var data2 = data;
            foreach (var i in FunctionalOperators.Range(0, data.Length))
            {
                data2[i] = data[i].Take(headers.Length).ToArray();
            }

            if (includesHeaders)
            {
                data2 = data2.Tail().ToArray();
            }

            var dt = data2.MultArrayToDataTable(tblName);

            if (headers != null)
            {
                foreach (var i in FunctionalOperators.Range(headers.Length))
                {
                    dt.Columns[i].ColumnName = headers[i];
                }
            }

            data = null;
            data2 = null;
            allLines.Clear();
            allLines = null;

            return dt;
        }

        #endregion

    }
}
