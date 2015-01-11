using System.ComponentModel;
using System.Configuration.Install;
using System.Management;
using System.ServiceProcess;
using System.Linq;
using System;
using System.Threading;

namespace ChefService
{
    /// <summary>
    /// This class runs when installutil.exe is called on the executable ChefService.exe (myself)
    /// </summary>
    [RunInstaller(true)]
    public class ChefServiceInstallerDefinition : Installer
    {
        //Sent in via command line
        public static string user, pass;
        public static string ChefServiceName = "EISChef";

        public ServiceProcessInstaller processInstaller;
        public ServiceInstaller serviceInstaller;

        public ChefServiceInstallerDefinition()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();
            serviceInstaller.Committed += new InstallEventHandler(serviceInstaller_Committed);
            serviceInstaller.AfterInstall += new InstallEventHandler(serviceInstaller_AfterInstall);

            //set the privileges
            if (user != null && pass != null)
            {
                processInstaller.Account = ServiceAccount.User;
                processInstaller.Username = user;
                processInstaller.Password = pass;
            }
            else
            {
                processInstaller.Account = ServiceAccount.LocalSystem;
            }

            serviceInstaller.DisplayName = "EIS Chef Web Service";
            serviceInstaller.Description = "EIS Service to execute Chef Client";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //Tried this - was delayed way way too much
            //serviceInstaller.DelayedAutoStart = true;

            //must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = ChefServiceName;
            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);
        }

        void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            //This code is immediately starting the service.
            using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
            {
                sc.Start();
            }

            //Make sure it gets started
            bool starts = false;
            for (int i = 0; i < 20; i++)
            {
                using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        Console.WriteLine("Service not started yet...");
                    }
                    else
                    {
                        Console.WriteLine("Service started");
                        starts = true;
                        break;
                    }

                    Thread.Sleep(1000);
                }
            }
            if (!starts)
            {
                throw new Exception("Service never Started");
            }


        }

        void serviceInstaller_Committed(object sender, InstallEventArgs e)
        {
            //This code is making it so the Desktop Interaction checkbox is checked.

            ConnectionOptions coOptions = new ConnectionOptions();
            coOptions.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope mgmtScope = new ManagementScope(@"root\CIMV2", coOptions);
            mgmtScope.Connect();

            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + serviceInstaller.ServiceName + "'"))
            {
                using (ManagementBaseObject InParam = wmiService.GetMethodParameters("Change"))
                {
                    InParam["DesktopInteract"] = true;
                    ManagementBaseObject OutParam = wmiService.InvokeMethod("Change", InParam, null);
                }
            }
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            if (processInstaller.Account == ServiceAccount.User)
                Console.WriteLine("Installing Service using the User Account: " + user);
            else
                Console.WriteLine("Installing Service using the LocalSystem Account");

            base.Install(stateSaver);
        }

        void RemoveAlreadyInstalledVersion()
        {

            ServiceController ctl = ServiceController.GetServices()
                        .FirstOrDefault(s => s.ServiceName == ChefServiceName);
            if (ctl != null)
            {
                Console.WriteLine("Detected Service already installed - Uninstalling");
                ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
                InstallContext Context = new InstallContext();
                ServiceInstallerObj.Context = Context;
                ServiceInstallerObj.ServiceName = ChefServiceName;
                ServiceInstallerObj.Uninstall(null);
                Console.WriteLine("Uninstall Complete");
            }
            else
            {
                Console.WriteLine("Service is not installed, skipping any calls to uninstall");
            }
        }
        
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            RemoveAlreadyInstalledVersion();
            //base.Uninstall(savedState);
        }
    }
}
