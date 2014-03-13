using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Syscon.JobSchedulerUI
{
	public class TimeSetControl : UserControl
	{
		private Label HoursSeperator;
		private Label MinutesSeperator;
		private DateTime dtTime;
		private NumericUpDown ctlHours;
		private NumericUpDown ctlMinutes;
        private NumericUpDown ctlSeconds;

		private Label HoursLetter;
		private Label SecondsLetter;
		private Label MinutesLetter;
		private bool bRePaint;
		private bool bShowHours = true;
		private bool bShowMinutes = true;
		private bool bShowSeconds = true;
		private bool bShowHoursLetter = true;
		private bool bShowMinutesLetter = true;
		private bool bShowSecondsLetter = true;
		private bool bTest = true;
		private Container components = null;


        public TimeSetControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.ctlHours = new NumericUpDown();
            this.ctlMinutes = new NumericUpDown();
            this.HoursSeperator = new Label();
            this.ctlSeconds = new NumericUpDown();
            this.MinutesSeperator = new Label();
            this.HoursLetter = new Label();
            this.SecondsLetter = new Label();
            this.MinutesLetter = new Label();
            this.ctlHours.BeginInit();
            this.ctlMinutes.BeginInit();
            this.ctlSeconds.BeginInit();
            base.SuspendLayout();
            
            //Hours Control
            this.ctlHours.Location = new Point(8, 8);
            int[] array = new int[4];
            array[0] = 23;
            this.ctlHours.Maximum = new decimal(array);
            this.ctlHours.Name = "ctlHours";
            this.ctlHours.Size = new Size(40, 20);
            this.ctlHours.TabIndex = 0;

            //Minutes Control
            this.ctlMinutes.Location = new Point(72, 8);            
            int[] array2 = new int[4];
            array2[0] = 59;
            this.ctlMinutes.Maximum = new decimal(array2);
            this.ctlMinutes.Name = "ctlMinutes";
            this.ctlMinutes.Size = new Size(40, 20);
            this.ctlMinutes.TabIndex = 2;
            this.HoursSeperator.AutoSize = true;
            this.HoursSeperator.Location = new Point(64, 8);
            this.HoursSeperator.Name = "HoursSeperator";
            this.HoursSeperator.Size = new Size(7, 13);
            this.HoursSeperator.TabIndex = 2;
            this.HoursSeperator.Text = ":";

            //Seconds Control
            this.ctlSeconds.Location = new Point(136, 8);
            int[] array3 = new int[4];
            array3[0] = 59;
            this.ctlSeconds.Maximum = new decimal(array3);
            this.ctlSeconds.Name = "ctlSeconds";
            this.ctlSeconds.Size = new Size(40, 20);
            this.ctlSeconds.TabIndex = 3;
            this.MinutesSeperator.AutoSize = true;
            this.MinutesSeperator.Location = new Point(128, 8);
            this.MinutesSeperator.Name = "MinutesSeperator";
            this.MinutesSeperator.Size = new Size(7, 13);
            this.MinutesSeperator.TabIndex = 4;
            this.MinutesSeperator.Text = ":";
            this.HoursLetter.AutoSize = true;
            this.HoursLetter.Location = new Point(48, 8);
            this.HoursLetter.Name = "HoursLetter";
            this.HoursLetter.Size = new Size(12, 13);
            this.HoursLetter.TabIndex = 5;
            this.HoursLetter.Text = "H";
            this.SecondsLetter.AutoSize = true;
            this.SecondsLetter.Location = new Point(176, 8);
            this.SecondsLetter.Name = "SecondsLetter";
            this.SecondsLetter.Size = new Size(12, 13);
            this.SecondsLetter.TabIndex = 7;
            this.SecondsLetter.Text = "S";
            this.MinutesLetter.AutoSize = true;
            this.MinutesLetter.Location = new Point(112, 8);
            this.MinutesLetter.Name = "MinutesLetter";
            this.MinutesLetter.Size = new Size(14, 13);
            this.MinutesLetter.TabIndex = 9;
            this.MinutesLetter.Text = "M";
            base.Controls.AddRange(new Control[]
			{
				this.MinutesLetter,
				this.SecondsLetter,
				this.HoursLetter,
				this.MinutesSeperator,
				this.ctlSeconds,
				this.HoursSeperator,
				this.ctlMinutes,
				this.ctlHours
			});
            base.Name = "TimeSetControl";
            base.Size = new Size(192, 40);
            base.Load += new EventHandler(this.TimeSetControl_Load);
            base.Paint += new PaintEventHandler(this.TimeSetControl_Paint);
            this.ctlHours.EndInit();
            this.ctlMinutes.EndInit();
            this.ctlSeconds.EndInit();
            base.ResumeLayout(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

		public decimal Hours
		{
			get
			{
                return this.ctlHours.Value;
			}
			set
			{
				this.ctlHours.Value = value;
			}
		}
		public decimal Minutes
		{
			get
			{
                return this.ctlMinutes.Value;
			}
			set
			{
                this.ctlMinutes.Value = value;
			}
		}
		public decimal Seconds
		{
			get
			{
                return this.ctlSeconds.Value;
			}
			set
			{
                this.ctlSeconds.Value = value;
			}
		}
		public bool ShowHours
		{
			get
			{
				return this.bShowHours;
			}
			set
			{
				this.bShowHours = value;
                this.ShowHoursLetter = value;
			}
		}
		public bool ShowMinutes
		{
			get
			{
				return this.bShowMinutes;
			}
			set
			{
				this.SetRePaint(true);
				this.bShowMinutes = value;
                this.ShowMinutesLetter = value;
				base.Invalidate();
			}
		}
		public bool ShowSeconds
		{
			get
			{
				return this.bShowSeconds;
			}
			set
			{
				this.SetRePaint(true);
				this.bShowSeconds = value;
                this.ShowSecondsLetter = value;
				base.Invalidate();
			}
		}
		public bool ShowHoursLetter
		{
			get
			{
				return this.bShowHoursLetter;
			}
			set
			{
				this.SetRePaint(true);
				this.bShowHoursLetter = value;
				base.Invalidate();
			}
		}
		public bool ShowMinutesLetter
		{
			get
			{
				return this.bShowMinutesLetter;
			}
			set
			{
				this.SetRePaint(true);
				this.bShowMinutesLetter = value;
				base.Invalidate();
			}
		}
		public bool ShowSecondsLetter
		{
			get
			{
				return this.bShowSecondsLetter;
			}
			set
			{
				this.SetRePaint(true);
				this.bShowSecondsLetter = value;
				base.Invalidate();
			}
		}

		private bool Test
		{
			get
			{
				return this.bTest;
			}
			set
			{
				this.bTest = value;
			}
		}

		public void SetRePaint(bool bRePaint)
		{
            this.bRePaint = bRePaint;
		}

		public bool GetRePaint()
		{
			return this.bRePaint;
		}

		public DateTime GetTime()
		{
			return this.dtTime;
		}

		public void SetTime(DateTime dateTime)
		{
			this.dtTime = dateTime;
		}

		private void TimeSetControl_Load(object sender, EventArgs e)
		{
			if (!this.ShowSeconds)
			{
				this.ctlSeconds.Hide();
				this.MinutesSeperator.Hide();
			}
			else
			{
				this.ctlSeconds.Show();
				this.MinutesSeperator.Show();
			}
			if (!this.ShowMinutes)
			{
				this.ctlMinutes.Hide();
			}
			else
			{
				this.ctlMinutes.Show();
			}
			if (!this.ShowHours)
			{
				this.ctlHours.Hide();
				this.HoursSeperator.Hide();
			}
			else
			{
				this.ctlHours.Show();
				this.HoursSeperator.Show();
			}
			if (!this.ShowHoursLetter)
			{
				this.HoursLetter.Hide();
				if (this.ShowHours)
				{
					this.HoursSeperator.Show();
				}
			}
			else
			{
				this.HoursLetter.Show();
				this.HoursSeperator.Hide();
			}
			if (!this.ShowMinutesLetter)
			{
				this.MinutesLetter.Hide();
				this.MinutesSeperator.Show();
			}
			else
			{
				this.MinutesLetter.Show();
				this.MinutesSeperator.Hide();
			}
			if (!this.ShowSecondsLetter)
			{
				this.SecondsLetter.Hide();
			}
			else
			{
				this.SecondsLetter.Show();
			}
		}

		private void TimeSetControl_Paint(object sender, PaintEventArgs e)
		{
			if (this.GetRePaint())
			{
				if (!this.ShowSeconds)
				{
					this.ctlSeconds.Hide();
					this.MinutesSeperator.Hide();
				}
				else
				{
					this.ctlSeconds.Show();
					this.MinutesSeperator.Show();
				}
				if (!this.ShowMinutes)
				{
					this.ctlMinutes.Hide();
				}
				else
				{
					this.ctlMinutes.Show();
				}
				if (!this.ShowHours)
				{
					this.ctlHours.Hide();
					this.HoursSeperator.Hide();
				}
				else
				{
					this.ctlHours.Show();
					this.HoursSeperator.Show();
				}
				if (!this.ShowHoursLetter)
				{
					this.HoursLetter.Hide();
					if (this.ShowHours)
					{
						this.HoursSeperator.Show();
					}
				}
				else
				{
					this.HoursLetter.Show();
					this.HoursSeperator.Hide();
				}
				if (!this.ShowMinutesLetter)
				{
					this.MinutesLetter.Hide();
					this.MinutesSeperator.Show();
				}
				else
				{
					this.MinutesLetter.Show();
					this.MinutesSeperator.Hide();
				}
				if (!this.ShowSecondsLetter)
				{
					this.SecondsLetter.Hide();
				}
				else
				{
					this.SecondsLetter.Show();
				}
				this.SetRePaint(false);
			}
		}
	}
}
