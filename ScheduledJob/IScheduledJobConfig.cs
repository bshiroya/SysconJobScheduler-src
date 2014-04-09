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

        /// <summary>
        /// Reference of ScheduledJob object
        /// </summary>
        IScheduledJob ScheduledJob { get; }

        /// <summary>
        /// Assembly name
        /// </summary>
        string AssemblyName { get; }

        /// <summary>
        /// Assembly path
        /// </summary>
        string AssemblyPath { get; }

        /// <summary>
        /// Log file path
        /// </summary>
        string LogFilePath { get; set; }

        /// <summary>
        /// Load the config
        /// </summary>
        void LoadConfig();

        /// <summary>
        /// Save the config
        /// </summary>
        void SaveConfig();
    }
}
