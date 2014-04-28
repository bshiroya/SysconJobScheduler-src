using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Configuration;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using SysconCommon.Common;
using SysconCommon.Common.Environment;

using Syscon.ScheduledJob;
using System.Diagnostics;
using Microsoft.Win32.TaskScheduler;

namespace Syscon.JobSchedulerUI
{
    public partial class JobScheduler : Form
    {
        #region Member Variables

        private CompositionContainer _container;

        private SysconCommon.COMMethods mbapi = new SysconCommon.COMMethods();
        bool loaded = false;

        IList<IScheduledJob> _scheduledJobs = new List<IScheduledJob>();
        private IDictionary<string, bool> _scheduledJobAndStatus = new Dictionary<string, bool>();

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public JobScheduler()
        {
            InitializeComponent();

            LoadJobPlugIns();

            using (TaskService ts = new TaskService())
            {
                //Load which jobs are already scheduled in the service.
                foreach (IScheduledJob job in this.ScheduledJobs)
                {
                    string jobName = job.ToString().Substring(job.ToString().LastIndexOf('.') + 1);
                    string taskName = jobName + "-" + job.JobId.ToString();

                    // Retrieve the task, change the trigger and re-register it
                    Task t = ts.GetTask(taskName);
                    
                    _scheduledJobAndStatus.Add(job.JobId.ToString(), (t != null));
                }
            }
        }

        /// <summary>
        /// List of all available jobs.
        /// </summary>
        [ImportMany(typeof(IScheduledJob))]
        public IList<IScheduledJob> ScheduledJobs 
        {
            get { return _scheduledJobs; }
            set { _scheduledJobs = value; } 
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadJobPlugIns()
        {
            try
            {
                //Creating an instance of aggregate catalog. It aggregates other catalogs
                var aggregateCatalog = new AggregateCatalog();

                //Build the directory path where the parts will be available
                var directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PlugIns");                            

                //Load parts from the available DLLs in the specified path 
                //using the directory catalog
                var directoryCatalog = new DirectoryCatalog(directoryPath, "*.Plugin.dll");

                //Load parts from the current assembly if available
                var asmCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

                //Add to the aggregate catalog
                aggregateCatalog.Catalogs.Add(directoryCatalog);
                aggregateCatalog.Catalogs.Add(asmCatalog);

                //Crete the composition container
                _container = new CompositionContainer(aggregateCatalog);

                // Composable parts are created here i.e. 
                // the Import and Export components assembles here
                _container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                Env.Log("Exception in loading job plug-ins in JobSchedulerUI" + ex.Message);
            }
        }
        
        private void JobScheduler_Load(object sender, EventArgs e)
        {
            // resets it everytime it is run so that the user can't just change to a product they already have a license for
            Env.SetConfigVar("product_id", 178507);

            var product_id = Env.GetConfigVar("product_id", 0, false);
            var product_version = "1.3.0.0";
            bool require_login = false;

            if (!loaded)
            {
                require_login = true;
                loaded = true;
                this.Text += " (version " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
            }

            try
            {
                var license = SysconCommon.Protection.ProtectionInfo.GetLicense(product_id, product_version, 15751);
                if (license.IsTrial)
                {
                    if (!license.IsValid())
                    {
                        SetupInvalid();
                    }
                    else
                    {
                        var l = license as SysconCommon.Protection.TrialLicense;
                        SetupTrial(l.DaysLeft);
                    }
                }
                else
                {
                    SetupFull();
                }
            }
            catch
            {
                SetupInvalid();
            }

            txtDataDir.TextChanged += new EventHandler(txtDataDir_TextChanged);

            if (require_login)
            {
                mbapi.smartGetSMBDir();

                if (mbapi.RequireSMBLogin() == null)
                    this.Close();
            }

            txtDataDir.Text = mbapi.smartGetSMBDir();
        }

        private void txtDataDir_TextChanged(object sender, EventArgs e)
        {
            SysconCommon.Common.Environment.Connections.SetOLEDBFreeTableDirectory(txtDataDir.Text);
        }

        private void SetupTrial(int daysLeft)
        {
            var msg = string.Format("You have {0} days left to evaluate this software", daysLeft);
            this.demoLabel.Text = msg;
            jobsDataGridView.Enabled = true;
            LoadJobsDataGrid();
        }

        private void SetupInvalid()
        {
            jobsDataGridView.Enabled = false;
            this.demoLabel.Text = "Your License has expired or is invalid";
        }

        private void SetupFull()
        {
            this.demoLabel.Text = "";
            this.activateToolStripMenuItem.Visible = false;
            jobsDataGridView.Enabled = true;
            LoadJobsDataGrid();
        }

        private void LoadJobsDataGrid()
        {
            //Clear the data grid if loaded
            this.jobListBindingSrc.Clear();
            jobsDataGridView.Columns.Clear();

            this.jobsDataGridView.DataSource = this.jobListBindingSrc;

            DataGridViewColumn desCol = new DataGridViewTextBoxColumn();
            desCol.DataPropertyName = "Desc";
            desCol.Name = "Desc";
            desCol.ReadOnly = true;
            desCol.Width = 250;
            jobsDataGridView.Columns.Add(desCol);

            DataGridViewColumn statusCol = new DataGridViewTextBoxColumn();
            statusCol.DataPropertyName = "JobStatus";
            statusCol.ReadOnly = true;
            statusCol.Name = "JobStatus";
            jobsDataGridView.Columns.Add(statusCol);

            DataGridViewTextBoxColumn timeCol = new DataGridViewTextBoxColumn();
            timeCol.DataPropertyName = "ScheduledTime";
            timeCol.Name = "Time";
            timeCol.ReadOnly = true;
            timeCol.Width = 80;
            jobsDataGridView.Columns.Add(timeCol);

            DataGridViewButtonColumn confiCol = new DataGridViewButtonColumn();
            confiCol.Name = "Config";
            confiCol.Text = "Configure";
            confiCol.UseColumnTextForButtonValue = true;
            jobsDataGridView.Columns.Add(confiCol);

            DataGridViewCheckBoxColumn enueueCol = new DataGridViewCheckBoxColumn();
            enueueCol.DataPropertyName = "Enqueued";
            enueueCol.Name = "Enqueue";
            enueueCol.Width = 70;
            jobsDataGridView.Columns.Add(enueueCol);

            DataGridViewButtonColumn runJobCol = new DataGridViewButtonColumn();
            runJobCol.Name = "Run";
            runJobCol.Text = "Run Now";
            runJobCol.UseColumnTextForButtonValue = true;
            jobsDataGridView.Columns.Add(runJobCol);

            DataGridViewTextBoxColumn logFileCol = new DataGridViewTextBoxColumn();
            logFileCol.DataPropertyName = "LogFile";
            logFileCol.Name = "LogFile";
            logFileCol.ReadOnly = true;
            logFileCol.Width = 150;
            jobsDataGridView.Columns.Add(logFileCol);

            foreach (IScheduledJob job in _scheduledJobs)
            {
                if (_scheduledJobAndStatus.ContainsKey(job.JobId.ToString()))
                {
                    job.Enqueued = _scheduledJobAndStatus[job.JobId.ToString()];
                }
                
                job.JobConfig.LoadConfig();

                ScheduledJobModel jobModel = new ScheduledJobModel(job);
                jobListBindingSrc.Add(jobModel);
            }
        }        

        private void jobsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            ScheduledJobModel scheduledJobModel = (ScheduledJobModel)jobsDataGridView.Rows[e.RowIndex].DataBoundItem;

            if (e.ColumnIndex == jobsDataGridView.Columns["Config"].Index && e.RowIndex >= 0)
            {
                scheduledJobModel.Job.SetJobConfiguration();
            }
            
            if (e.ColumnIndex == jobsDataGridView.Columns["Enqueue"].Index && e.RowIndex >= 0)
            {
                DataGridViewCheckBoxColumn col = jobsDataGridView.Columns["Enqueue"] as DataGridViewCheckBoxColumn;
                DataGridViewCheckBoxCell cell = jobsDataGridView.Rows[e.RowIndex].Cells["Enqueue"] as DataGridViewCheckBoxCell;

                bool enqueued = !((bool)cell.EditingCellFormattedValue);

                //Add the entry to service app.config file.
                string exeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                //Add to the windows scheduler
                using (TaskService ts = new TaskService())
                {
                    string jobName = scheduledJobModel.Job.ToString().Substring(scheduledJobModel.Job.ToString().LastIndexOf('.') + 1);
                    string taskName = jobName + "-" + scheduledJobModel.Job.JobId.ToString();

                    if (enqueued)
                    {
                        //If enqueued then add to the scheduled task list
                        ScheduledTaskSettingsDialog schedulerTaskSettingsDlg = new ScheduledTaskSettingsDialog();
                        if (schedulerTaskSettingsDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            // Create a new task definition and assign properties
                            TaskDefinition td = ts.NewTask();
                            td.RegistrationInfo.Description = scheduledJobModel.Job.JobDesc;
                            td.Principal.LogonType = TaskLogonType.InteractiveToken;

                            // Add a trigger that will fire the task at this time every day
                            DailyTrigger dt = (DailyTrigger)td.Triggers.Add(new DailyTrigger { DaysInterval = 1, StartBoundary = schedulerTaskSettingsDlg.StartBoundary });
                            dt.Repetition.Interval = TimeSpan.FromMinutes(schedulerTaskSettingsDlg.TimeInterval);

                            // Add an action that will launch the job runner application for the given job at the scheduled time.
                            td.Actions.Add(new ExecAction("ScheduledJobRunner.exe", scheduledJobModel.Job.JobId.ToString(), exeLocation));

                            // Register the task in the root folder                        
                            ts.RootFolder.RegisterTaskDefinition(taskName, td);
                            scheduledJobModel.Enqueued = enqueued;
                            //cell.EditingCellFormattedValue = true;

                            MessageBox.Show("Job successfully added to the Windows scheduled tasks.");
                        }
                        else
                        {
                            scheduledJobModel.Enqueued = false;
                            //cell.EditingCellFormattedValue = false;
                        }
                    }
                    else
                    {
                        Task t = ts.GetTask(taskName);

                        // Remove the task.
                        ts.RootFolder.DeleteTask(taskName, false);
                        scheduledJobModel.Enqueued = false;
                        //cell.EditingCellFormattedValue = false;

                        if (t != null)
                            MessageBox.Show("Job successfully removed from the Windows scheduled tasks.");
                    }
                }
            }

            //Run now functionality
            if (e.ColumnIndex == jobsDataGridView.Columns["Run"].Index && e.RowIndex >= 0)
            {
                scheduledJobModel.Job.ExceuteJob();
            }

            if (e.ColumnIndex == jobsDataGridView.Columns["LogFile"].Index && e.RowIndex >= 0)
            {
                string filepath = scheduledJobModel.Job.LogFilePath;
                if (File.Exists(filepath))
                {
                    System.Diagnostics.Process.Start(filepath);
                }
            }

            jobsDataGridView.Refresh();
        }

        private void btnSMBDir_Click(object sender, EventArgs e)
        {
            mbapi.smartSelectSMBDirByGUI();
            var usr = mbapi.RequireSMBLogin();
            if (usr != null)
            {
                txtDataDir.Text = mbapi.smartGetSMBDir();
            }
        }

        #region Menu Items Event Handlers

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion       

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new About();
            frm.ShowDialog();
        }

        private void activateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var product_id = Env.GetConfigVar("product_id", 0, false);
            var product_version = Env.GetConfigVar("product_version", "0.0.0.0", false);

            var frm = new SysconCommon.Protection.ProtectionPlusOnlineActivationForm(product_id, product_version);
            frm.ShowDialog();
            this.OnLoad(null);
        }

        private void onlineHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://syscon-inc.com/product-support/CustomApplication/support.asp");
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var frm = new Settings();
            //frm.ShowDialog();
        }
            
    }
}
