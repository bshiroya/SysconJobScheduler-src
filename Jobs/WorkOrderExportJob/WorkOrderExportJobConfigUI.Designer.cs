namespace Syscon.ScheduledJob.WorkOrderExportJob
{
    partial class WorkOrderImportJobConfigUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkOrderImportJobConfigUI));
            this.btnSMBDir = new System.Windows.Forms.Button();
            this.txtSageDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnBrowseWoDir = new System.Windows.Forms.Button();
            this.txtWOImportDir = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.txtVerifyPwd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLogFilePath = new System.Windows.Forms.TextBox();
            this.btnLogFile = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.scheduleTimeLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numUpDownLbPeriod = new System.Windows.Forms.NumericUpDown();
            this.bottomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownLbPeriod)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSMBDir
            // 
            this.btnSMBDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSMBDir.Location = new System.Drawing.Point(470, 22);
            this.btnSMBDir.Name = "btnSMBDir";
            this.btnSMBDir.Size = new System.Drawing.Size(102, 23);
            this.btnSMBDir.TabIndex = 23;
            this.btnSMBDir.Text = "&Browse Sage Dir";
            this.btnSMBDir.UseVisualStyleBackColor = true;
            this.btnSMBDir.Click += new System.EventHandler(this.btnSMBDir_Click);
            // 
            // txtSageDir
            // 
            this.txtSageDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSageDir.Location = new System.Drawing.Point(1, 22);
            this.txtSageDir.Name = "txtSageDir";
            this.txtSageDir.ReadOnly = true;
            this.txtSageDir.Size = new System.Drawing.Size(463, 20);
            this.txtSageDir.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 172);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 25;
            this.label1.Text = "Scheduled Time";
            // 
            // bottomPanel
            // 
            this.bottomPanel.Controls.Add(this.btnCancel);
            this.bottomPanel.Controls.Add(this.btnOk);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 267);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(578, 35);
            this.bottomPanel.TabIndex = 28;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(497, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 29;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(416, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 28;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnBrowseWoDir
            // 
            this.btnBrowseWoDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseWoDir.Location = new System.Drawing.Point(470, 54);
            this.btnBrowseWoDir.Name = "btnBrowseWoDir";
            this.btnBrowseWoDir.Size = new System.Drawing.Size(104, 23);
            this.btnBrowseWoDir.TabIndex = 32;
            this.btnBrowseWoDir.Text = "Browse &WO Dir";
            this.btnBrowseWoDir.UseVisualStyleBackColor = true;
            this.btnBrowseWoDir.Click += new System.EventHandler(this.btnBrowseWoDir_Click);
            // 
            // txtWOImportDir
            // 
            this.txtWOImportDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWOImportDir.Location = new System.Drawing.Point(3, 54);
            this.txtWOImportDir.Name = "txtWOImportDir";
            this.txtWOImportDir.ReadOnly = true;
            this.txtWOImportDir.Size = new System.Drawing.Size(461, 20);
            this.txtWOImportDir.TabIndex = 31;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "User Id";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Password";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(129, 85);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(118, 20);
            this.txtUserName.TabIndex = 35;
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(129, 113);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(118, 20);
            this.txtPwd.TabIndex = 36;
            // 
            // txtVerifyPwd
            // 
            this.txtVerifyPwd.Location = new System.Drawing.Point(129, 142);
            this.txtVerifyPwd.Name = "txtVerifyPwd";
            this.txtVerifyPwd.PasswordChar = '*';
            this.txtVerifyPwd.Size = new System.Drawing.Size(118, 20);
            this.txtVerifyPwd.TabIndex = 38;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Verify Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "Log File";
            // 
            // txtLogFilePath
            // 
            this.txtLogFilePath.Location = new System.Drawing.Point(129, 226);
            this.txtLogFilePath.Name = "txtLogFilePath";
            this.txtLogFilePath.ReadOnly = true;
            this.txtLogFilePath.Size = new System.Drawing.Size(335, 20);
            this.txtLogFilePath.TabIndex = 40;
            // 
            // btnLogFile
            // 
            this.btnLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogFile.Location = new System.Drawing.Point(470, 225);
            this.btnLogFile.Name = "btnLogFile";
            this.btnLogFile.Size = new System.Drawing.Size(104, 23);
            this.btnLogFile.TabIndex = 41;
            this.btnLogFile.Text = "Select &Log File";
            this.btnLogFile.UseVisualStyleBackColor = true;
            this.btnLogFile.Click += new System.EventHandler(this.btnLogFile_Click);
            // 
            // scheduleTimeLabel
            // 
            this.scheduleTimeLabel.AutoSize = true;
            this.scheduleTimeLabel.Location = new System.Drawing.Point(126, 172);
            this.scheduleTimeLabel.Name = "scheduleTimeLabel";
            this.scheduleTimeLabel.Size = new System.Drawing.Size(43, 13);
            this.scheduleTimeLabel.TabIndex = 42;
            this.scheduleTimeLabel.Text = "Not Set";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 196);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 13);
            this.label6.TabIndex = 43;
            this.label6.Text = "Look back period to sync";
            // 
            // numUpDownLbPeriod
            // 
            this.numUpDownLbPeriod.Location = new System.Drawing.Point(182, 196);
            this.numUpDownLbPeriod.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numUpDownLbPeriod.Name = "numUpDownLbPeriod";
            this.numUpDownLbPeriod.Size = new System.Drawing.Size(65, 20);
            this.numUpDownLbPeriod.TabIndex = 45;
            this.numUpDownLbPeriod.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // WorkOrderImportJobConfigUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 302);
            this.Controls.Add(this.numUpDownLbPeriod);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.scheduleTimeLabel);
            this.Controls.Add(this.btnLogFile);
            this.Controls.Add(this.txtLogFilePath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtVerifyPwd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowseWoDir);
            this.Controls.Add(this.txtWOImportDir);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSMBDir);
            this.Controls.Add(this.txtSageDir);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "WorkOrderImportJobConfigUI";
            this.Text = "Work order export job config settings";
            this.Load += new System.EventHandler(this.WorkOrderImportJobConfigUI_Load);
            this.bottomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDownLbPeriod)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSMBDir;
        private System.Windows.Forms.TextBox txtSageDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnBrowseWoDir;
        private System.Windows.Forms.TextBox txtWOImportDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.TextBox txtVerifyPwd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLogFilePath;
        private System.Windows.Forms.Button btnLogFile;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label scheduleTimeLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numUpDownLbPeriod;
    }
}