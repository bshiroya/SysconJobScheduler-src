using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syscon.ScheduledJob
{
    /// <summary>
    /// Interface for the job config.
    /// </summary>
    public interface IScheduledJobConfig
    {
        /// <summary>
        /// The SMB Directory path.
        /// </summary>
        string SMBDir
        {
            get;
            set;
        }

        /// <summary>
        /// The time scheduled to run this job.
        /// </summary>
        DateTime ScheduledTime
        {
            get;
            set;
        }

        IScheduledJob ScheduledJob { get; }

        string AssemblyName { get; }

        string AssemblyPath { get; }

        void LoadConfig();

        void SaveConfig();
    }
}
