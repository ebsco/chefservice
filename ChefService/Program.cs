﻿using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace ChefService
{
    static class Program
    {
        private static readonly string _exePath = Assembly.GetExecutingAssembly().Location;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            DebugMode();

            //If there are args - we are usually installing/uninstalling the service.
            if (args.Length > 0)
            {
                ExecArguments(args);
                throw new Exception("Processed args and should have exited");
            }
            else
            {
                //We are running in the Service Manager in this use case
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			        { 
				        new ChefService() 
			        };
                ServiceBase.Run(ServicesToRun);
            }

        }

        /// <summary>
        /// Running in debug mode attached to visual studio
        /// </summary>
        static void DebugMode()
        {
            //When debugging in Visual Studio
            #if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                new ChefService().StartWebService();

                ManualResetEvent mre = new ManualResetEvent(false);
                mre.WaitOne();
                Environment.Exit(1);
            }
            #endif
        }

        /// <summary>
        /// Method executed when CLI parameters are supplied
        /// </summary>
        /// <param name="args">The CLI args</param>
        static void ExecArguments(string[] args)
        {
            //We are representing installutil here where -install and -uninstall will install the ChefService Windows Service using the same ChefService executable.  
            //Got rid of the problem knowing which installutil version to call.

            Arguments arg = new Arguments(args, true, false);
            if (arg.ContainsKey("INSTALL"))
            {
                RunServiceInstall(arg);
                throw new Exception("Should not get here - install");
            }
            else if (arg.ContainsKey("UNINSTALL"))
            {
                RunServiceUninstall(true);
                throw new Exception("Should not get here - uninstall");
            }
            else
            {
                Usage();
                throw new Exception("Should not get through past Usage");
            }
        }

        /// <summary>
        /// Installs the ChefService.
        /// </summary>
        /// <param name="arg">The full CLI Arguments parsed into a class</param>
        static void RunServiceInstall(Arguments arg)
        {
            //We support running with a username and password, or without (See ChefServiceInstallerDefinition for default when not using username/password)
            string user = null, pass = null;

            if (arg.ContainsKey("USERNAME"))
            {
                user = arg["USERNAME"];

                if (arg.ContainsKey("PASSWORD"))
                {
                    pass = arg["PASSWORD"];
                }
                else
                {
                    throw new Exception("need password too");
                }
            }

            //Am I already installed?
            //If so remove ChefService Windows Service and then call the install command on myself using the installutil C# class
            RunServiceUninstall(false);

            //Since we are calling an install command, exit.
            ChefServiceInstallerDefinition.user = user;
            ChefServiceInstallerDefinition.pass = pass;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { _exePath });
                Console.WriteLine("Install Succeeded");
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Install Failed");
                Console.WriteLine("Exception:" + e);
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Will uninstall the service.  Needs to support exiting, and not exiting(install use case)
        /// </summary>
        /// <param name="ShouldExit">Default of true - controls if it exits the console application</param>
        static void RunServiceUninstall(bool ShouldExit = true)
        {
            //Run uninstall command
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { "/u", _exePath });
                Console.WriteLine("UnInstall Succeeded");
                
                if (ShouldExit)
                    Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("UnInstall Failed");
                Console.WriteLine("Exception:" + e);
                
                if (ShouldExit)
                    Environment.Exit(1);
            }
        }

        /// <summary>
        /// Shows usage of the Main executable
        /// </summary>
        static void Usage()
        {
            Console.WriteLine("Usage:");

            Console.WriteLine("-uninstall - Removes the service");
            Console.WriteLine("-install  - Tells the exe to install the service");
            Console.WriteLine("Optional install params - Will use local system if not specified");
            Console.WriteLine("     -username name  - Username for the service. If using a local user prefix with .\\");
            Console.WriteLine("     -password pass  - Password ofr the service");
            Console.WriteLine("");

            Environment.Exit(1);
        }
    }
}