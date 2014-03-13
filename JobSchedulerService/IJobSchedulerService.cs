using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections.ObjectModel;

using Syscon.ScheduledJob;

namespace Syscon.Services
{
    /// <summary>
    ///  Define a service contract.
    /// </summary>
    [ServiceContract(Namespace = "http://Syscon.Services")]
    public interface IJobSchedulerService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        [OperationContract]
        void AddJob(IScheduledJob job);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="job"></param>
        [OperationContract]
        void RemoveJob(IScheduledJob job);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<IScheduledJob> GetScheduledJobs();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="jobStatus"></param>
        [OperationContract]
        void SetJobStatus(Guid jobId, JobStatus jobStatus);
    }
}
