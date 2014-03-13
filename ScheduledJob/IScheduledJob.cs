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
    [ServiceContract(Namespace="Syscon.ScheduledJob", 
        Name="ScheduledJob",
        ProtectionLevel=ProtectionLevel.EncryptAndSign)]
    public interface IScheduledJob
    {
        /// <summary>
        /// Execute the instructions for this job.
        /// </summary>
        /// <remarks>This method should contain all the logic to be executed for this job.</remarks>
        [OperationContract]
        void ExceuteJob();

        /// <summary>
        /// Set the configuration settings for this job.
        /// </summary>
        [OperationContract]
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
            [OperationContract]
            get; 
        }

        /// <summary>
        /// Job description.
        /// </summary>
        string JobDesc 
        {
            [OperationContract]
            get;  
        }

        /// <summary>
        /// The job status
        /// </summary>
        JobStatus JobStatus
        {
            [OperationContract]
            get;
            [OperationContract]
            set; 
        }

        /// <summary>
        /// The config file path.
        /// </summary>
        string ConfigFilePath
        {
            [OperationContract]
            get;
        }

        /// <summary>
        /// The log file path.
        /// </summary>
        string LogFilePath
        {
            [OperationContract]
            get;
        }

        /// <summary>
        /// Whether the job is enqueued in the scheduler service.
        /// </summary>
        bool Enqueued
        {
            [OperationContract]
            get;
            [OperationContract]
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
    }
}
