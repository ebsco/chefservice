﻿using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Threading;

namespace ChefService
{
    /// <summary>
    /// The installer for the ChefService.  Will be executed using installutil or when calling chefservice.exe with the command line parameters.
    /// </summary>
    [RunInstaller(true)]
    public class ChefServiceInstallerDefinition : Installer
    {
        public const string ChefServiceName = "ChefService";  //Service Name that shows up in SCM
        const int ServiceStartRetries = 20;
        ServiceProcessInstaller processInstaller;
        ServiceInstaller serviceInstaller;

        //Sent in via command line
        public static string user, pass;

        public ChefServiceInstallerDefinition()
        {
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();
            serviceInstaller.Committed += new InstallEventHandler(serviceInstaller_Committed);

            //set the privileges - Optional Username and Password to run as.
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

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            RemoveAlreadyInstalledVersion();
            //base.Uninstall(savedState);
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            if (processInstaller.Account == ServiceAccount.User)
                Console.WriteLine("Installing Service using the User Account: " + user);
            else
                Console.WriteLine("Installing Service using the LocalSystem Account");

            base.Install(stateSaver);
        }

        /// <summary>
        /// Will wait for the service to get started, before saying install is truly complete.  Also adds on the  Desktop Interaction checkbox setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serviceInstaller_Committed(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
            {
                sc.Start();
            }

            //Make sure it gets started
            WaitForServiceToStart();

            //This code is making it so the Desktop Interaction checkbox is checked.
            SetDesktopInteraction();
        }

        /// <summary>
        /// Waits for the Windows Service to Start
        /// </summary>
        private void WaitForServiceToStart()
        {
            bool starts = false;
            for (int i = 0; i < ServiceStartRetries; i++)
            {
                using (ServiceController sc = new ServiceController(serviceInstaller.ServiceName))
                {
                    if (sc.Status != ServiceControllerStatus.Running)
                    {
                        Console.WriteLine("Service not started yet..." + i + "/" + ServiceStartRetries);
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

        /// <summary>
        /// Allow service to interact with desktop
        /// </summary>
        private void SetDesktopInteraction()
        {
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

        /// <summary>
        /// Remove the currently installed version if it is found already.  Think rebootstrap, etc
        /// </summary>
        private void RemoveAlreadyInstalledVersion()
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

    }
}