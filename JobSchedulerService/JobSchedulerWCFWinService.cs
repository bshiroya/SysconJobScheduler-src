using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Timers;

using SysconCommon.Common.Environment;

using Syscon.ScheduledJob;
using System.Configuration;

namespace Syscon.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class JobSchedulerWCFWindowsService : ServiceBase
    {

        public ServiceHost _serviceHost = null;
        private System.Timers.Timer _timer = null;

        private CompositionContainer _container = null;
        private ObservableCollection<IScheduledJob> _allJobs = null;
        private IDictionary<string, string> _scheduledJobAndTime = null;
        private const int TIMER_INTERVAL = 1;


        /// <summary>
        /// Ctor
        /// </summary>
        public JobSchedulerWCFWindowsService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        [Conditional("DEBUG_SERVICE")]
        private static void DebugMode()
        {
            Debugger.Break();
        }


        /// <summary>
        /// List of job plug-ins
        /// </summary>
        [ImportMany(typeof(IScheduledJob))]
        public ObservableCollection<IScheduledJob> ScheduledJobs
        {
            get { return _allJobs; }
            set { _allJobs = value; }
        }

        /// <summary>
        /// Method to load the MEF job plug-ins
        /// </summary>
        private void LoadJobPlugIns()
        {
            try
            {
                _allJobs.Clear();

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
                Env.Log("Exception in loading job plug-ins" + ex.Message);
            }
        }

        /// <summary>
        /// This method is called by Service Control Manager when the service is started.
        /// Add code here to start your service.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _allJobs = new ObservableCollection<IScheduledJob>();
            _scheduledJobAndTime = new Dictionary<string, string>();

            //if (_serviceHost != null)
            //{
            //    _serviceHost.Close();
            //}

            //// Create a ServiceHost for the CalculatorService type and 
            //// provide the base address.
            //_serviceHost = new ServiceHost(typeof(JobSchedulerService));

            //// Open the ServiceHostBase to create listeners and start 
            //// listening for messages.
            //_serviceHost.Open();

            Env.Log("Job scheduler service started");

            //Load the jobs
            LoadJobPlugIns();

            //Load scheduled jobs from application config
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);

            foreach (KeyValueConfigurationElement keyVal in config.AppSettings.Settings)
            {
                _scheduledJobAndTime.Add(keyVal.Key, keyVal.Value);
            }

            _timer = new Timer(TimeSpan.FromMinutes(TIMER_INTERVAL).TotalMilliseconds);
            //set the timer interval and start the service
            //Set autoreset to false. When autoreset is True there are reentrancy problems.
            _timer.AutoReset = false;
            _timer.Enabled = true;
            _timer.Elapsed += new ElapsedEventHandler(ServiceTimer_Tick);
        }

        /// <summary>
        /// This method is called by Service control manager when the service is stopped.
        /// Add code here to perform any tear-down necessary to stop your service.
        /// </summary>
        protected override void OnStop()
        {
            //Add code here to perform any tear-down necessary to stop your service.
            _timer.AutoReset = false;
            _timer.Enabled = false;

            _allJobs.Clear();
            _allJobs = null;

            _scheduledJobAndTime.Clear();
            _scheduledJobAndTime = null;

            //if (_serviceHost != null)
            //{
            //    _serviceHost.Close();
            //    _serviceHost = null;
            //}
            Env.Log("Job scheduler service stopped");
        }

        /// <summary>
        /// This method is called when the system is shutting down.
        /// Add code here to perform any tear-down necessary to stop your service .
        /// </summary>
        protected override void OnShutdown()
        {
            base.OnShutdown();

            _timer.Stop();
            _timer.Dispose();

            _allJobs.Clear();
            _allJobs = null;

            _scheduledJobAndTime.Clear();
            _scheduledJobAndTime = null;
        }

        /// <summary>
        /// This method is called when the service is paused.
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            this._timer.Stop();
        }

        /// <summary>
        /// This method is called when the service resumes after being paused.
        /// </summary>
        protected override void OnContinue()
        {
            base.OnContinue();

            this._timer.Interval = TimeSpan.FromMinutes(TIMER_INTERVAL).TotalMilliseconds;
            this._timer.Start();
        }

        /// <summary>
        /// When the timer is triggered check if there are any jobs to run
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServiceTimer_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            Env.Log("ScheduleService timer function called");

            ReloadJobs();

            string strHours;
            string strMins;
            DateTime dtNow = DateTime.Now;

            foreach (IScheduledJob job in _allJobs)
            {                
                // get the scheduled time of the job that the service is supposed 
                // to run and compare it to the current time.

                if (!_scheduledJobAndTime.ContainsKey(job.JobId.ToString()))
                    continue;

                DateTime scheduledTime;

                if (!DateTime.TryParse(_scheduledJobAndTime[job.JobId.ToString()], out scheduledTime))
                {
                    Env.Log("There is some problem with the date format for scheduled job in the service config file. Please verify");
                    continue;
                }

                strHours = scheduledTime.Hour.ToString(); //get the startig hours
                strMins = scheduledTime.Minute.ToString(); //get the startig minutes

                if (dtNow.Hour == Int32.Parse(strHours) && dtNow.Minute == Int32.Parse(strMins))
                {
                    //Execute the job
                    try
                    {
                        job.ExceuteJob();
                    }
                    catch (Exception ex)
                    {
                        Env.Log("Unable to start the scheduled job - Id - {0} \n Exception details - {1}\n{2}", 
                            job.JobId, ex.Message, ex.StackTrace);
                    }
                }
            }

            //Reset the timer
            this._timer.Interval = TimeSpan.FromMinutes(TIMER_INTERVAL).TotalMilliseconds;
            this._timer.Start();
        }


        private void ReloadJobs()
        {
            _scheduledJobAndTime.Clear();

            //Load the jobs
            LoadJobPlugIns();

            //Load scheduled jobs from application config
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            foreach (KeyValueConfigurationElement keyVal in config.AppSettings.Settings)
            {
                _scheduledJobAndTime.Add(keyVal.Key, keyVal.Value);
            }
        }
    }
}
