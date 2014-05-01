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
    public enum JobStatus
    {
        // Summary:
        //     The state of the job is unknown.
        Unknown = 0,
        //
        // Summary:
        //     The job is registered but is disabled and no instances of the job are queued
        //     or running. The job cannot be run until it is enabled.
        Disabled = 1,
        //
        // Summary:
        //     Instances of the job are queued.
        Queued = 2,
        //
        // Summary:
        //     The job is ready to be executed, but no instances are queued or running.
        Ready = 3,
        //
        // Summary:
        //     One or more instances of the job is running.
        Running = 4,
    }
}
