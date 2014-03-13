using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace Syscon.Services
{
    /// <summary>
    /// Installer class for the job scheduler service.
    /// </summary>
    [RunInstaller(true)]
    public partial class JobSchedulerWinServiceInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller _installerProcess;
        private ServiceInstaller _installer;

        public JobSchedulerWinServiceInstaller()
        {
            InitializeComponent();

            _installerProcess = new ServiceProcessInstaller();
            _installerProcess.Account = ServiceAccount.LocalSystem;
            _installer = new ServiceInstaller();
            _installer.ServiceName = "JobSchedulerService";
            _installer.Description = "Syscon job scheduler service";

            Installers.Add(_installerProcess);
            Installers.Add(_installer);
        }
    }
}
