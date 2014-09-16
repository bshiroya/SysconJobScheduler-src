using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

using SysconCommon;
using Syscon.ScheduledJob;

namespace Syscon.ScheduledJob.ExportBillingUpdatesJob
{
    /// <summary>
    /// Job settings UI for Export Billing Updates job
    /// </summary>
    public partial class ExportBillingUpdatesJobConfigUI : Form
    {
        #region Member variables
        private ExportBillingUpdatesJobConfig   _jobConfig = null;
        private XmlSerializer                   _xmlSerializer = null;
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="job"></param>
        public ExportBillingUpdatesJobConfigUI()
        {
            InitializeComponent();
            _jobConfig = new ExportBillingUpdatesJobConfig();

            LoadConfig();
        }

        #region Public Methods

        /// <summary>
        /// Save the job config
        /// </summary>
        public virtual void SaveConfig()
        {
            try
            {
                _xmlSerializer = new XmlSerializer(_jobConfig.GetType());

                using (StreamWriter writer = new StreamWriter(string.Format(@"{0}\{1}.xml", _jobConfig.AssemblyPath, _jobConfig.AssemblyName), false))
                {
                    _xmlSerializer.Serialize(writer, _jobConfig);
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
        /// Load the job configuration
        /// </summary>
        public void LoadConfig()
        {
            try
            {
                _xmlSerializer = new XmlSerializer(_jobConfig.GetType());
                string configFile = string.Format(@"{0}\{1}.xml", _jobConfig.AssemblyPath, _jobConfig.AssemblyName);

                if (File.Exists(configFile))
                {
                    using (FileStream stream = new FileInfo(configFile).OpenRead())
                    {
                        var dsObj = _xmlSerializer.Deserialize(stream);
                        ExportBillingUpdatesJobConfig tempConfig = dsObj as ExportBillingUpdatesJobConfig;

                        _jobConfig.SMBDir = tempConfig.SMBDir;
                        _jobConfig.UserId = tempConfig.UserId;
                        _jobConfig.Password = tempConfig.Password; //decrypt this
                        _jobConfig.LogFilePath = tempConfig.LogFilePath;
                        _jobConfig.BillingUpdateQueueDirectory = tempConfig.BillingUpdateQueueDirectory;
                        _jobConfig.ScheduledTime = tempConfig.ScheduledTime;
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

        #region Event Handlers
        private void WorkOrderImportJobConfigUI_Load(object sender, EventArgs e)
        {
            //Load the config
            LoadConfig();

            //Bind to controls
            txtSageDir.Text     = _jobConfig.SMBDir;
            txtLogFilePath.Text = _jobConfig.LogFilePath;
            txtUserName.Text    = _jobConfig.UserId;
            txtPwd.Text         = _jobConfig.Password;
            txtVerifyPwd.Text   = _jobConfig.Password;
            txtBillUpdatesExportDir.Text = _jobConfig.BillingUpdateQueueDirectory;

            //scheduleTimePicker.Text = _jobConfig.ScheduledTime.ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBillUpdatesExportDir.Text))
            {
                MessageBox.Show("Please select the work order import file (.csv) path.", "Select work order file path", MessageBoxButtons.OK);
                return;
            }
            if (string.IsNullOrEmpty(txtUserName.Text))
            {
                MessageBox.Show("Please give a valid username.", "Provide username", MessageBoxButtons.OK);
                return;
            }
            if (txtPwd.Text != txtVerifyPwd.Text)
            {
                MessageBox.Show("The two given password doesn't match.", "Password mismatch", MessageBoxButtons.OK);
                return;
            }

            _jobConfig.SMBDir = txtSageDir.Text;
            _jobConfig.UserId = txtUserName.Text;
            _jobConfig.Password = txtPwd.Text;// _methods.smartEncrypt(txtPwd.Text, false); //encrypt this before saving
            _jobConfig.LogFilePath = txtLogFilePath.Text;
            _jobConfig.BillingUpdateQueueDirectory = txtBillUpdatesExportDir.Text;
            _jobConfig.ScheduledTime = scheduleTimeLabel.Text;

            //Save the config
            SaveConfig();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnSMBDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSageDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnBrowseWoDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBillUpdatesExportDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnLogFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Set Log File";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.OpenFile() != null)
            {
                txtLogFilePath.Text = saveFileDialog1.FileName;
            }
        }
        #endregion
        
    }
}
