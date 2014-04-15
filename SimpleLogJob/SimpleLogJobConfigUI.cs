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

using Syscon.ScheduledJob;

namespace Syscon.ScheduledJob.SimpleLogJob
{
    public partial class SimpleLogJobConfigUI : Form
    {
        SimpleLogJobConfig _jobConfig = null;

        public SimpleLogJobConfigUI(IScheduledJob job )
        {
            InitializeComponent();
            _jobConfig = new SimpleLogJobConfig(job);
        }

        #region Event Handlers
        private void SimpleLogJobConfigUI_Load(object sender, EventArgs e)
        {
            //Load the config
            LoadConfig();

            //Bind to controls
            txtSMBDir.Text      = _jobConfig.SMBDir;
            txtLogFilePath.Text = _jobConfig.LogFilePath;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSMBDir.Text))
            {
                MessageBox.Show("Please select a Sage database directory.", "Select Sage directory", MessageBoxButtons.OK);
                return;
            }
            //txtLogFilePath.Text
            if (string.IsNullOrEmpty(txtLogFilePath.Text) && (!File.Exists(txtLogFilePath.Text)))
            {
                MessageBox.Show("Please set a valid log file path.", "Select Log File", MessageBoxButtons.OK);
                return;
            }

            _jobConfig.SMBDir = txtSMBDir.Text;
            _jobConfig.UserId = "";
            _jobConfig.Password = "";
            _jobConfig.ScheduledTime = DateTime.Parse(scheduleTimePicker.Text);
            _jobConfig.LogFilePath = txtLogFilePath.Text;

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


        XmlSerializer _xmlSerializer = null;
        /// <summary>
        /// 
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
        /// Load the settings from config file.
        /// </summary>
        public virtual void LoadConfig()
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
                        SimpleLogJobConfig tempConfig = dsObj as SimpleLogJobConfig;

                        _jobConfig.SMBDir = tempConfig.SMBDir;
                        _jobConfig.UserId = tempConfig.UserId;
                        _jobConfig.Password = tempConfig.Password;
                        _jobConfig.LogFilePath = tempConfig.LogFilePath;
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
                txtSMBDir.Text = folderBrowserDialog1.SelectedPath;
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

    }
}
