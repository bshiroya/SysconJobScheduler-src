using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32.TaskScheduler;

namespace Syscon.JobSchedulerUI
{
    /// <summary>
    /// UI for enabling the users to do scheduled time settings.
    /// </summary>
    public partial class ScheduledTaskSettingsDialog : Form
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public ScheduledTaskSettingsDialog()
        {
            InitializeComponent();            
        }

        private void ScheduledTaskSettingsDialog_Load(object sender, EventArgs e)
        {
            IList<Item> values = new List<Item>();
            values.Add(new Item("5 minutes", 5));
            values.Add(new Item("15 minutes", 15));
            values.Add(new Item("30 minutes", 30));
            values.Add(new Item("1 Hour", 60));
            values.Add(new Item("2 Hour", 120));
            values.Add(new Item("3 Hour", 180));
            values.Add(new Item("6 Hour", 360));
            values.Add(new Item("12 Hour", 720));

            cboRepeatTaskInterval.DisplayMember = "Text";
            cboRepeatTaskInterval.ValueMember = "Value";
            cboRepeatTaskInterval.DataSource = values;

            cboRepeatTaskInterval.SelectedIndex = 3;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            double interval = (double)(cboRepeatTaskInterval.SelectedValue as Item).Value;
            this.TimeInterval = interval;

            this.StartBoundary = dtpStartDate.Value;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void radDaily_CheckedChanged(object sender, EventArgs e)
        {
            if (radDaily.Checked)
            {
                panelRecurEvery.Visible = true;
            }
            else
            {
                panelRecurEvery.Visible = false;
            }
        }

        private void chkRepeatTaskEvery_CheckedChanged(object sender, EventArgs e)
        {
            cboRepeatTaskInterval.Enabled = chkRepeatTaskEvery.Checked;
        }


        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public double TimeInterval
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime StartBoundary
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime RepeatInterval
        {
            get;
            set;
        }

        #endregion
    }


    /// <summary>
    /// Content item for the combo box
    /// </summary>
    internal class Item
    {
        public string Name;// { get; set; }
        public int Value;// { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public Item(string name, int value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Generates the text shown in the combo box
            return Name;
        }
    }
}
