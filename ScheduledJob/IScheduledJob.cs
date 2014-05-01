using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using System.ComponentModel.Composition;
using System.Net.Security;

namespace Syscon.ScheduledJob
{

    /// <summary>
    /// Interface definition for scheduled job.
    /// </summary>
    public interface IScheduledJob
    {
        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        void ExceuteJob();

        /// <summary>
        /// Set the configuration settings for this job.
        /// </summary>
        void SetJobConfiguration();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IScheduledJobConfig JobConfig { get; }

        /// <summary>
        /// The unique Guid of this job for identification.
        /// </summary>
        Guid JobId 
        {
            get; 
        }

        /// <summary>
        /// Job description.
        /// </summary>
        string JobDesc 
        {
            get;  
        }

        /// <summary>
        /// The job status
        /// </summary>
        JobStatus JobStatus
        {
            get;
            set; 
        }

        /// <summary>
        /// The config file path.
        /// </summary>
        string ConfigFilePath
        {
            get;
        }

        /// <summary>
        /// The log file path.
        /// </summary>
        string LogFilePath
        {
            get;
        }

        /// <summary>
        /// Whether the job is enqueued in the scheduler service.
        /// </summary>
        bool Enqueued
        {
            get;
            set;
        }

        /// <summary>
        /// The time scheduled to run this job.
        /// </summary>
        string ScheduledTime
        {
            get;
            set;
        }
    }
}
