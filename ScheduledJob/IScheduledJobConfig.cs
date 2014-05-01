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
        /// SMB database user id
        /// </summary>
         string UserId
        {
            get;
            set;
        }

        /// <summary>
         /// SMB database user password
        /// </summary>
        string Password
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
