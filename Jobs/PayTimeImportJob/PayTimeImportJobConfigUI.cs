﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Syscon.ScheduledJob;
using System.Xml.Serialization;
using System.IO;
using SysconCommon;

namespace Syscon.ScheduledJob.PayTimeImportJob
{
    /// <summary>
    /// Config UI for the work order import job
    /// </summary>
    public partial class PayTimeImportJobConfigUI : Form
    {
        #region Member variables
        private PayTimeImportJobConfig    _jobConfig = null;
        private XmlSerializer             _xmlSerializer = null;
        private COMMethods               _methods = null;
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="job"></param>
        public PayTimeImportJobConfigUI()
        {
            InitializeComponent();
            _jobConfig = new PayTimeImportJobConfig();
            _methods = new COMMethods();
        }

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
            txtTimeImportFile.Text = _jobConfig.PayTimeImportCsvFile;
            scheduleTimeLabel.Text = _jobConfig.ScheduledTime;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTimeImportFile.Text))
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

            //encrypt password before saving
            var hashed_password = _methods.smartEncrypt(txtPwd.Text, false);
            _jobConfig.Password = hashed_password; 
            _jobConfig.LogFilePath = txtLogFilePath.Text;
            _jobConfig.ScheduledTime = scheduleTimeLabel.Text;
            _jobConfig.PayTimeImportCsvFile = txtTimeImportFile.Text;

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
        #endregion

        /// <summary>
        /// Save the config
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
        /// 
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
                        PayTimeImportJobConfig tempConfig = dsObj as PayTimeImportJobConfig;

                        _jobConfig.SMBDir = tempConfig.SMBDir;
                        _jobConfig.UserId = tempConfig.UserId;
                        _jobConfig.Password = tempConfig.Password; //decrypt this
                        _jobConfig.LogFilePath = tempConfig.LogFilePath;
                        _jobConfig.ScheduledTime = tempConfig.ScheduledTime;
                        _jobConfig.PayTimeImportCsvFile = tempConfig.PayTimeImportCsvFile;
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

        private void btnSMBDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSageDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }
       
        private void btnLogFile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Title = "Select Log File";
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFileDialog1.OpenFile() != null)
            {
                txtLogFilePath.Text = saveFileDialog1.FileName;
            }
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select Time Import CSV File";
            openFileDialog1.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK && openFileDialog1.OpenFile() != null)
            {
                txtTimeImportFile.Text = openFileDialog1.FileName;
            }
        }

    }
}
