using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Syscon.ScheduledJob;
using SysconCommon.Common.Environment;

namespace Syscon.ScheduledJobRunner
{
    public class Program
    {
        static void Main(string[] args)
        {
            Guid jobId;

            Env.Log("--------------------------------------------------------------------------------------------");
            // Test if input arguments were supplied: 
            if (args.Length == 0 || args.Length > 1)
            {
                Console.WriteLine("The program expects a valid job id to run. The job id is a Guid that uniquely identifies the job dll."
                    + "\nThe proper way to supply job id is to run the JobSchedulerUI and assign the job to windows scheduler");
                Env.Log("The program expects a valid job id to run. The job id is a Guid that uniquely identifies the job dll."
                    + "\nThe proper way to supply job id is to run the JobSchedulerUI and assign the job to windows scheduler");

                Console.WriteLine("Usage: ScheduledJobRunner <JobGuid>");
                Env.Log("Usage: ScheduledJobRunner <JobGuid>");

                return;
            }
            try
            {

                if (!Guid.TryParse(args[0], out jobId))
                {
                    Console.WriteLine("The given JobId is invalid or in wrong format.");
                    Env.Log("The given JobId is invalid or in wrong format.");
                    Console.WriteLine("Usage: ScheduledJobRunner <JobGuid>");
                    return;
                }

                Env.Log("Supplied Job Id - {0}", jobId);
                Env.Log("Started loading of the PlugIn dlls");

                JobLoader jobLoader = new JobLoader();
                jobLoader.LoadJobPlugIns();

                Env.Log("Finished loading of the PlugIn dlls. Number of loaded PlugIns-{0}", jobLoader.ScheduledJobs.Count);

                foreach (IScheduledJob job in jobLoader.ScheduledJobs)
                {
                    if (job.JobId == jobId)
                    {
                        Env.Log("Running Job - {0}", job.JobDesc);
                        //Run the job.
                        job.ExceuteJob();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ScheduledJobRunner program\n" + ex.Message);
                Env.Log("Exception in ScheduledJobRunner program\n" + ex.Message);
                return;
            }
        }
    }

    /// <summary>
    /// This class just loads the available job PlugIns.
    /// </summary>
    internal class JobLoader
    {
        private CompositionContainer _container = null;
        private ObservableCollection<IScheduledJob> _allJobs = null;

        /// <summary>
        /// Ctor
        /// </summary>
        internal JobLoader()
        {
            _allJobs = new ObservableCollection<IScheduledJob>();
        }

        /// <summary>
        /// List of job plug-ins
        /// </summary>
        [ImportMany(typeof(IScheduledJob))]
        internal ObservableCollection<IScheduledJob> ScheduledJobs
        {
            get { return _allJobs; }
            set { _allJobs = value; }
        }

        /// <summary>
        /// Method to load the MEF job plug-ins
        /// </summary>
        internal void LoadJobPlugIns()
        {
            try
            {
                _allJobs.Clear();

                //Creating an instance of aggregate catalog. It aggregates other catalogs
                var aggregateCatalog = new AggregateCatalog();

                //Build the directory path where the parts will be available
                var directoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "PlugIns");

                //Load parts from the available DLLs in the specified path 
                //using the directory catalog
                var directoryCatalog = new DirectoryCatalog(directoryPath, "*.Plugin.dll");

                //Load parts from the current assembly if available
                var asmCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly());

                //Add to the aggregate catalog
                aggregateCatalog.Catalogs.Add(directoryCatalog);
                aggregateCatalog.Catalogs.Add(asmCatalog);

                //Crete the composition container
                _container = new CompositionContainer(aggregateCatalog);

                // Composable parts are created here i.e. 
                // the Import and Export components assembles here
                _container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                Console.WriteLine("Exception in loading Job PlugIns" + ex.Message);
                Env.Log("Exception in loading Job PlugIns" + ex.Message);
                throw ex;
            }
        }
    }
}
