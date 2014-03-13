using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Syscon.ScheduledJob
{

    /// <summary>
    /// Enum for depicting the job status.
    /// </summary>
    [DataContract(Name = "JobStatus")]
    public enum JobStatus
    {
        Active = 0,
        Suspended
    }
}
