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
            this.txtDataDir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.scheduleTimePicker = new System.Windows.Forms.DateTimePicker();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSMBDir
            // 
            this.btnSMBDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSMBDir.Location = new System.Drawing.Point(402, 22);
            this.btnSMBDir.Name = "btnSMBDir";
            this.btnSMBDir.Size = new System.Drawing.Size(75, 23);
            this.btnSMBDir.TabIndex = 23;
            this.btnSMBDir.Text = "&Browse";
            this.btnSMBDir.UseVisualStyleBackColor = true;
            // 
            // txtDataDir
            // 
            this.txtDataDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataDir.Location = new System.Drawing.Point(1, 22);
            this.txtDataDir.Name = "txtDataDir";
            this.txtDataDir.ReadOnly = true;
            this.txtDataDir.Size = new System.Drawing.Size(395, 20);
            this.txtDataDir.TabIndex = 22;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 60);
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
            this.bottomPanel.Location = new System.Drawing.Point(0, 157);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(483, 35);
            this.bottomPanel.TabIndex = 28;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(402, 6);
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
            this.btnOk.Location = new System.Drawing.Point(321, 6);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 28;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // scheduleTimePicker
            // 
            this.scheduleTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.scheduleTimePicker.Location = new System.Drawing.Point(135, 60);
            this.scheduleTimePicker.Name = "scheduleTimePicker";
            this.scheduleTimePicker.ShowCheckBox = true;
            this.scheduleTimePicker.ShowUpDown = true;
            this.scheduleTimePicker.Size = new System.Drawing.Size(118, 20);
            this.scheduleTimePicker.TabIndex = 30;
            // 
            // SimpleLogJobConfigUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 192);
            this.Controls.Add(this.scheduleTimePicker);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSMBDir);
            this.Controls.Add(this.txtDataDir);
            this.MaximizeBox = false;
            this.Name = "SimpleLogJobConfigUI";
            this.Text = "SimpleLogJobConfigUI";
            this.Load += new System.EventHandler(this.SimpleLogJobConfigUI_Load);
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSMBDir;
        private System.Windows.Forms.TextBox txtDataDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.DateTimePicker scheduleTimePicker;
    }
}