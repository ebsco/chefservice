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
            ManualResetEvent mre = null;
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                new ChefService().mystart();

                mre = new ManualResetEvent(false);
                mre.WaitOne();
            }
#else


            if (args.Length > 0)
            {
                Arguments arg = new Arguments(args, true, false);
                if (arg.ContainsKey("INSTALL"))
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
                    MyWindowsServiceInstaller.RemoveAlreadyInstalledVersion();
                    

                    bool val=SelfInstaller.InstallService(user, pass);
                    if (val)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        Environment.Exit(1);
                    }
                }
                else if (arg.ContainsKey("UNINSTALL"))
                {
                    bool val = SelfInstaller.UninstallService();
                    if (val)
                    {
                        Environment.Exit(0);
                    }
                    else
                    {
                        Environment.Exit(1);
                    }

                }
                else
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
    }
}
