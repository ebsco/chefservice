using System;
using System.ServiceProcess;
using System.Threading;

namespace ChefService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                new ChefService().mystart();

                ManualResetEvent mre = new ManualResetEvent(false);
                mre.WaitOne();
            }
#else

            if (args.Length > 0)
            {
                ExecArguments(args);
                throw new Exception("Processed args and should have exited");
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			        { 
				        new ChefService() 
			        };
                ServiceBase.Run(ServicesToRun);
            }
#endif
        }

        static void ExecArguments(string[]  args)
        {
            //We are representing installutil here where -install and -uninstall will install the ChefService Windows Service using the same ChefService executable.  
            //Got rid of the problem knowing which installutil version to call.

            Arguments arg = new Arguments(args, true, false);
            if (arg.ContainsKey("INSTALL"))
            {
                RunServiceInstall(arg);
                throw new Exception("Should not get here");
            }
            else if (arg.ContainsKey("UNINSTALL"))
            {
                RunServiceUninstall();
            }
            else
            {
                Usage();
                throw new Exception("Should not get through past Usage");
            }
        }
        static void RunServiceInstall(Arguments arg)
        {
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
            bool val = SelfInstaller.InstallService(user, pass);

            //Since we are calling an install command, exit.
            if (val)
            {
                Environment.Exit(0);
            }
            else
            {
                Environment.Exit(1);
            }
        }
        static void RunServiceUninstall(bool ShouldExit=true)
        {
            //Run uninstall command and exit
            bool val = SelfInstaller.UninstallService();
            if (val)
            {
                if (ShouldExit)
                Environment.Exit(0);
            }
            else
            {
                if (ShouldExit)
                Environment.Exit(1);

                throw new Exception("Failed to call uninstall of service");
            }
        }
        static void Usage()
        {
            Console.WriteLine("Usage:");

            Console.WriteLine("-uninstall - Removes the service");
            Console.WriteLine("-install  - Tells the exe to install the service");
            Console.WriteLine("Optional install params - Will use local system if not specified");
            Console.WriteLine("     -username name  - Username for the service");
            Console.WriteLine("     -password pass  - Password ofr the service");
            Console.WriteLine("");

            Environment.Exit(1);
        }
    }
}
