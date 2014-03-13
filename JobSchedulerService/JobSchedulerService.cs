using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections.ObjectModel;

using Syscon.ScheduledJob;
using System.Configuration;

namespace Syscon.Services
{

    /// <summary>
    /// 
    /// </summary>
    public class JobSchedulerService : IJobSchedulerService
    {
        IDictionary<Guid, IScheduledJob> _scheduled_Jobs = new Dictionary<Guid, IScheduledJob>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        public void AddJob(IScheduledJob job)
        {
            if (!_scheduled_Jobs.ContainsKey(job.JobId))
            {
                _scheduled_Jobs.Add(job.JobId, job);
            }

            //Update the configuration for currently scheduled jobs
            SaveScheduledJobConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        public void RemoveJob(IScheduledJob job)
        {
            if (_scheduled_Jobs.ContainsKey(job.JobId))
            {
                _scheduled_Jobs.Remove(job.JobId);
            }

            //Update the configuration for currently scheduled jobs
            SaveScheduledJobConfig();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<IScheduledJob> GetScheduledJobs()
        {
            return _scheduled_Jobs.Values.ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobStatus"></param>
        public void SetJobStatus(Guid jobId, JobStatus jobStatus)
        {
            if (_scheduled_Jobs.ContainsKey(jobId))
            {
                IScheduledJob job = _scheduled_Jobs[jobId];
                job.JobStatus = jobStatus;
            }

            //Update the configuration for currently scheduled jobs
            SaveScheduledJobConfig();
        }

        /// <summary>
        /// Save the config for the scheduled job. This config will be used by the 
        /// window service to run the jobs.
        /// </summary>
        private void SaveScheduledJobConfig()
        {
            //Save the JobId, JobName and Scheduled time in the config file for the window service.
            //The Window service will load that config, match the scheduled time and execute the job.

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appSection = config.AppSettings;

            foreach (IScheduledJob job in _scheduled_Jobs.Values)
            {
                appSection.Settings.Add(job.JobId.ToString(), job.ScheduledTime.ToShortTimeString());
            }
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
