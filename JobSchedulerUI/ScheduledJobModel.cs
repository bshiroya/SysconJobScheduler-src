using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Syscon.ScheduledJob;

namespace Syscon.JobSchedulerUI
{
    /// <summary>
    /// Represents Job details
    /// </summary>
    public class ScheduledJobModel
    {
        IScheduledJob _job;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="job"></param>
        public ScheduledJobModel(IScheduledJob job)
        {
            Job = job;
        }


        internal IScheduledJob Job
        {
            get { return _job; }
            private set { _job = value; }
        }


        private string _desc;
        /// <summary>
        /// Description of the Job
        /// </summary>
        public string Desc
        {
            get
            {
                return Job.JobDesc;
            }
        }

        /// <summary>
        /// Status of the job
        /// </summary>
        public JobStatus JobStatus
        {
            get
            {
                return Job.JobStatus;
            }
        }
       
        private string _logFile;
        /// <summary>
        /// Log file for the Job
        /// </summary>
        public string LogFile
        {
            get
            {
                return _logFile;
            }
            set
            {
                _logFile = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ScheduledTime
        {
            get
            {
                return Job.ScheduledTime.ToShortTimeString();
            }
        }

        /// <summary>
        /// Is the Job qeued or not
        /// </summary>
        public bool Enqueued
        {
            get
            {
                return Job.Enqueued;
            }
            set
            {
                Job.Enqueued = value;
            }
        }
        
    }   
   
}
