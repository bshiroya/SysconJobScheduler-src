namespace Syscon.JobSchedulerUI
{
    partial class ScheduledTaskSettingsDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cboRepeatTaskInterval = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.radDaily = new System.Windows.Forms.RadioButton();
            this.radOneTime = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.grpInner = new System.Windows.Forms.GroupBox();
            this.panelRecurEvery = new System.Windows.Forms.Panel();
            this.lblDays = new System.Windows.Forms.Label();
            this.nudRecurDaysInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpStartDate = new Microsoft.Win32.TaskScheduler.FullDateTimePicker();
            this.radioWeekly = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkRepeatTaskEvery = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.grpInner.SuspendLayout();
            this.panelRecurEvery.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecurDaysInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // cboRepeatTaskInterval
            // 
            this.cboRepeatTaskInterval.Enabled = false;
            this.cboRepeatTaskInterval.FormattingEnabled = true;
            this.cboRepeatTaskInterval.Location = new System.Drawing.Point(176, 200);
            this.cboRepeatTaskInterval.Name = "cboRepeatTaskInterval";
            this.cboRepeatTaskInterval.Size = new System.Drawing.Size(125, 21);
            this.cboRepeatTaskInterval.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(318, 232);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // radDaily
            // 
            this.radDaily.AutoSize = true;
            this.radDaily.Location = new System.Drawing.Point(20, 65);
            this.radDaily.Name = "radDaily";
            this.radDaily.Size = new System.Drawing.Size(48, 17);
            this.radDaily.TabIndex = 8;
            this.radDaily.TabStop = true;
            this.radDaily.Text = "Daily";
            this.radDaily.UseVisualStyleBackColor = true;
            this.radDaily.CheckedChanged += new System.EventHandler(this.radDaily_CheckedChanged);
            // 
            // radOneTime
            // 
            this.radOneTime.AutoSize = true;
            this.radOneTime.Checked = true;
            this.radOneTime.Location = new System.Drawing.Point(20, 36);
            this.radOneTime.Name = "radOneTime";
            this.radOneTime.Size = new System.Drawing.Size(67, 17);
            this.radOneTime.TabIndex = 7;
            this.radOneTime.TabStop = true;
            this.radOneTime.Text = "One time";
            this.radOneTime.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grpInner);
            this.groupBox1.Controls.Add(this.dtpStartDate);
            this.groupBox1.Controls.Add(this.radioWeekly);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.radOneTime);
            this.groupBox1.Controls.Add(this.radDaily);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(482, 179);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time settings";
            // 
            // grpInner
            // 
            this.grpInner.Controls.Add(this.panelRecurEvery);
            this.grpInner.Location = new System.Drawing.Point(147, 59);
            this.grpInner.Name = "grpInner";
            this.grpInner.Size = new System.Drawing.Size(319, 114);
            this.grpInner.TabIndex = 12;
            this.grpInner.TabStop = false;
            // 
            // panelRecurEvery
            // 
            this.panelRecurEvery.Controls.Add(this.lblDays);
            this.panelRecurEvery.Controls.Add(this.nudRecurDaysInterval);
            this.panelRecurEvery.Controls.Add(this.label2);
            this.panelRecurEvery.Location = new System.Drawing.Point(6, 19);
            this.panelRecurEvery.Name = "panelRecurEvery";
            this.panelRecurEvery.Size = new System.Drawing.Size(260, 35);
            this.panelRecurEvery.TabIndex = 0;
            this.panelRecurEvery.Visible = false;
            // 
            // lblDays
            // 
            this.lblDays.AutoSize = true;
            this.lblDays.Location = new System.Drawing.Point(187, 9);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(31, 13);
            this.lblDays.TabIndex = 2;
            this.lblDays.Text = "Days";
            // 
            // nudRecurDaysInterval
            // 
            this.nudRecurDaysInterval.Location = new System.Drawing.Point(106, 7);
            this.nudRecurDaysInterval.Maximum = new decimal(new int[] {
            31,
            0,
            0,
            0});
            this.nudRecurDaysInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRecurDaysInterval.Name = "nudRecurDaysInterval";
            this.nudRecurDaysInterval.ReadOnly = true;
            this.nudRecurDaysInterval.Size = new System.Drawing.Size(64, 20);
            this.nudRecurDaysInterval.TabIndex = 1;
            this.nudRecurDaysInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Recur every:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(216, 33);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(206, 20);
            this.dtpStartDate.TabIndex = 11;
            this.dtpStartDate.TimeFormat = Microsoft.Win32.TaskScheduler.FullDateTimePickerTimeFormat.ShortTime;
            this.dtpStartDate.UTCPrompt = "";
            // 
            // radioWeekly
            // 
            this.radioWeekly.AutoSize = true;
            this.radioWeekly.Enabled = false;
            this.radioWeekly.Location = new System.Drawing.Point(20, 96);
            this.radioWeekly.Name = "radioWeekly";
            this.radioWeekly.Size = new System.Drawing.Size(61, 17);
            this.radioWeekly.TabIndex = 9;
            this.radioWeekly.TabStop = true;
            this.radioWeekly.Text = "Weekly";
            this.radioWeekly.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(144, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Start Time";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(409, 232);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkRepeatTaskEvery
            // 
            this.chkRepeatTaskEvery.AutoSize = true;
            this.chkRepeatTaskEvery.Location = new System.Drawing.Point(32, 202);
            this.chkRepeatTaskEvery.Name = "chkRepeatTaskEvery";
            this.chkRepeatTaskEvery.Size = new System.Drawing.Size(111, 17);
            this.chkRepeatTaskEvery.TabIndex = 11;
            this.chkRepeatTaskEvery.Text = "Repeat Job Every";
            this.chkRepeatTaskEvery.UseVisualStyleBackColor = true;
            this.chkRepeatTaskEvery.CheckedChanged += new System.EventHandler(this.chkRepeatTaskEvery_CheckedChanged);
            // 
            // ScheduledTaskSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(505, 269);
            this.Controls.Add(this.chkRepeatTaskEvery);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cboRepeatTaskInterval);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScheduledTaskSettingsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Scheduled Job Settings Dialog";
            this.Load += new System.EventHandler(this.ScheduledTaskSettingsDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpInner.ResumeLayout(false);
            this.panelRecurEvery.ResumeLayout(false);
            this.panelRecurEvery.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudRecurDaysInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboRepeatTaskInterval;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.RadioButton radDaily;
        private System.Windows.Forms.RadioButton radOneTime;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkRepeatTaskEvery;
        private Microsoft.Win32.TaskScheduler.FullDateTimePicker dtpStartDate;
        private System.Windows.Forms.RadioButton radioWeekly;
        private System.Windows.Forms.GroupBox grpInner;
        private System.Windows.Forms.Panel panelRecurEvery;
        private System.Windows.Forms.NumericUpDown nudRecurDaysInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDays;
    }
}