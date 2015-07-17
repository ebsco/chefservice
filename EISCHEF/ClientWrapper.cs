using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleApplication1.ChefWebService;
using System.Diagnostics;
using System.Reflection;

namespace ConsoleApplication1
{
    class ClientWrapper
    {
        /// <summary>
        /// Runs the chef client until completion
        /// </summary>
        /// <param name="args">Arguments from the CLI to pass to chef-client</param>
        /// <returns></returns>
        public int RunChef(string[] args)
        {
            
            string commandLine = chefcommandlineparse(args);
            ChefWebServiceClient client = new ChefWebServiceClient();

            ValidateOneClientRun(client);

            client.StartChef(commandLine);
            ChefService.EventWriter.Write("Chef Started", ChefService.EventLevel.Information, 1, Program.GetTitle());
            WaitForChefClientToFinish(client);
            ChefService.EventWriter.Write("Chef Finished", ChefService.EventLevel.Information, 1, Program.GetTitle());


            int Exitcode = client.GetExitCode();
            ChefService.EventWriter.Write("Exit code from Chef:" + Exitcode, ChefService.EventLevel.Information, 1, Program.GetTitle());
            Console.WriteLine("Exit code from Chef :" + Exitcode);
            Console.WriteLine("Finished...");
            return Exitcode;
        }

        /// <summary>
        /// Parse the args and make sure to append quotes when necessary
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private string chefcommandlineparse(string[] args)
        {
            string commandLine = "";
            foreach (var arg in args)
            {
                commandLine += " ";

                if (arg.Contains(" "))
                {
                    commandLine += "\"" + arg + "\"";
                }
                else
                {
                    commandLine += arg;
                }
            }
            return commandLine;
        }

        /// <summary>
        /// Process lines from the web service and print to console.  This will automatically be redirected over winrm
        /// </summary>
        /// <param name="client"></param>
        private void LogChefOutput(ChefWebServiceClient client)
        {
            
            while (true)
            {
             
                //ChefService.EventWriter.Write("Checking for new lines...", ChefService.EventLevel.Information, 1, Program.GetTitle());
                var lines = client.GetProcessOutput().Lines;
                //ChefService.EventWriter.Write("total new lines: "+lines.Length, ChefService.EventLevel.Information, 1, Program.GetTitle());

                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }

                if (lines.Count == 0)
                    break;
            }
        }

        /// <summary>
        /// Waits for Chef-client to finish while continuing to process the output
        /// </summary>
        /// <param name="client"></param>
        private void WaitForChefClientToFinish(ChefWebServiceClient client)
        {
            bool hasexited = false;

            while (!hasexited)
            {
                LogChefOutput(client);
                Thread.Sleep(250); //dont hit the service too hard

                hasexited = client.HasExited();
                //ChefService.EventWriter.Write("Checking if exited...", ChefService.EventLevel.Information, 1, Program.GetTitle());
            }

            //chef-client has Exited read until all lines are grabbed
            //The chef service will only send  50 lines at once to make sure the data packet doesnt get too large.
            LogChefOutput(client);
        }
        
        /// <summary>
        /// Only one chef-client run at once
        /// </summary>
        private void ValidateOneClientRun(ChefWebServiceClient client)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string s = assembly.Location;
            string assembname = System.IO.Path.GetFileNameWithoutExtension(s);

            var procs = Process.GetProcessesByName(assembname);

            //Some error detecting our processes
            if (procs.Length == 0) {
                if (!Debugger.IsAttached)
                {
                    throw new Exception("Failed to find any other processes, this is a bug in client code");
                }
            }
            else if (procs.Length == 1) //This is our process
            {
                Console.WriteLine("No other " + assembname + " are running, good to run");
            }
            else
            {
                Console.WriteLine("Killing other processes that are still running. Total count:" + (procs.Length - 1));

                int myprocid = Process.GetCurrentProcess().Id;
                foreach (var item in procs)
                {
                    if (item.Id != myprocid)
                    {
                        Console.WriteLine("Killing:" + item.ProcessName);
                        item.Kill();
                    }
                }


                Console.WriteLine("Finished killing other processes, attempting to clear out error");
                Console.WriteLine("Killing any already running Chef-Client process");
                client.ClearError();
            }

        }
    }
}