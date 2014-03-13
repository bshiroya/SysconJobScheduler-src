using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Syscon.ScheduledJob
{

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "JobStatus")]
    public enum JobStatus
    {
        Active = 0,
        Suspended
    }
}
