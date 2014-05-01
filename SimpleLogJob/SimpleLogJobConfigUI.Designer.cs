namespace Syscon.ScheduledJob.SimpleLogJob
{
    partial class SimpleLogJobConfigUI
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
            this.btnSMBDir = new System.Windows.Forms.Button();
            this.txtSMBDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnLogFile = new System.Windows.Forms.Button();
            this.txtLogFilePath = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.txtVerifyPwd = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.scheduleTimeLabel = new System.Windows.Forms.Label();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSMBDir
            // 
            this.btnSMBDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSMBDir.Location = new System.Drawing.Point(511, 22);
            this.btnSMBDir.Name = "btnSMBDir";
            this.btnSMBDir.Size = new System.Drawing.Size(90, 23);
            this.btnSMBDir.TabIndex = 23;
            this.btnSMBDir.Text = "&Browse";
            this.btnSMBDir.UseVisualStyleBackColor = true;
            this.btnSMBDir.Click += new System.EventHandler(this.btnSMBDir_Click);
            // 
            // txtSMBDir
            // 
            this.txtSMBDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSMBDir.Location = new System.Drawing.Point(1, 22);
            this.txtSMBDir.Name = "txtSMBDir";
            this.txtSMBDir.ReadOnly = true;
            this.txtSMBDir.Size = new System.Drawing.Size(504, 20);
            this.txtSMBDir.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 147);
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
            this.bottomPanel.Location = new System.Drawing.Point(0, 212);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(607, 35);
            this.bottomPanel.TabIndex = 28;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(526, 6);
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
            this.btnOk.Location = new System.Drawing.Point(445, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 28;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnLogFile
            // 
            this.btnLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogFile.Location = new System.Drawing.Point(511, 173);
            this.btnLogFile.Name = "btnLogFile";
            this.btnLogFile.Size = new System.Drawing.Size(90, 23);
            this.btnLogFile.TabIndex = 44;
            this.btnLogFile.Text = "Select &Log File";
            this.btnLogFile.UseVisualStyleBackColor = true;
            this.btnLogFile.Click += new System.EventHandler(this.btnLogFile_Click);
            // 
            // txtLogFilePath
            // 
            this.txtLogFilePath.Location = new System.Drawing.Point(132, 175);
            this.txtLogFilePath.Name = "txtLogFilePath";
            this.txtLogFilePath.ReadOnly = true;
            this.txtLogFilePath.Size = new System.Drawing.Size(370, 20);
            this.txtLogFilePath.TabIndex = 43;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 42;
            this.label5.Text = "Log File";
            // 
            // txtVerifyPwd
            // 
            this.txtVerifyPwd.Location = new System.Drawing.Point(132, 110);
            this.txtVerifyPwd.Name = "txtVerifyPwd";
            this.txtVerifyPwd.PasswordChar = '*';
            this.txtVerifyPwd.Size = new System.Drawing.Size(118, 20);
            this.txtVerifyPwd.TabIndex = 50;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(31, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 49;
            this.label4.Text = "Verify Password";
            // 
            // txtPwd
            // 
            this.txtPwd.Location = new System.Drawing.Point(132, 81);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.PasswordChar = '*';
            this.txtPwd.Size = new System.Drawing.Size(118, 20);
            this.txtPwd.TabIndex = 48;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(132, 53);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(118, 20);
            this.txtUserName.TabIndex = 47;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 45;
            this.label2.Text = "User Id";
            // 
            // scheduleTimeLabel
            // 
            this.scheduleTimeLabel.AutoSize = true;
            this.scheduleTimeLabel.Location = new System.Drawing.Point(129, 147);
            this.scheduleTimeLabel.Name = "scheduleTimeLabel";
            this.scheduleTimeLabel.Size = new System.Drawing.Size(43, 13);
            this.scheduleTimeLabel.TabIndex = 51;
            this.scheduleTimeLabel.Text = "Not Set";
            // 
            // SimpleLogJobConfigUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 247);
            this.Controls.Add(this.scheduleTimeLabel);
            this.Controls.Add(this.txtVerifyPwd);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPwd);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLogFile);
            this.Controls.Add(this.txtLogFilePath);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSMBDir);
            this.Controls.Add(this.txtSMBDir);
            this.MaximizeBox = false;
            this.Name = "SimpleLogJobConfigUI";
            this.Text = "Simple Log Job Config";
            this.Load += new System.EventHandler(this.SimpleLogJobConfigUI_Load);
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSMBDir;
        private System.Windows.Forms.TextBox txtSMBDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnLogFile;
        private System.Windows.Forms.TextBox txtLogFilePath;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox txtVerifyPwd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label scheduleTimeLabel;
    }
}