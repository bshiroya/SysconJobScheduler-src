using System;
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

namespace Syscon.ScheduledJob.SimpleLogJob
{
    public partial class SimpleLogJobConfigUI : Form
    {
        private SysconCommon.COMMethods mbapi = new SysconCommon.COMMethods();
        SimpleLogJobConfig _jobConfig = null;

        public SimpleLogJobConfigUI(IScheduledJob job )
        {
            InitializeComponent();
            _jobConfig = new SimpleLogJobConfig(job);

            //_jobConfig = jobConfig as SimpleLogJobConfig;
        }

        #region Event Handlers
        private void SimpleLogJobConfigUI_Load(object sender, EventArgs e)
        {
            //Load the config
            LoadConfig();

            //Bind to controls
            txtDataDir.Text = mbapi.smartGetSMBDir();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _jobConfig.SMBDir = txtDataDir.Text;
            _jobConfig.UserId = "";
            _jobConfig.Password = "";
            _jobConfig.ScheduledTime = DateTime.Parse(scheduleTimePicker.Text);

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
        /// 
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

    }
}
