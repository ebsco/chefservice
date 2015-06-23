using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleApplication1.ChefWebService;

namespace ConsoleApplication1
{
    class ClientWrapper
    {
        /// <summary>
        /// Mutex to make sure only one instance of the client is running at once
        /// </summary>
        Mutex mtx;

        /// <summary>
        /// Runs the chef client until completion
        /// </summary>
        /// <param name="args">Arguments from the CLI to pass to chef-client</param>
        /// <returns></returns>
        public int RunChef(string[] args)
        {
            ObtainClientMutex();
            string commandLine = chefcommandlineparse(args);
            ChefWebServiceClient client = new ChefWebServiceClient();

            try
            {
                client.StartChef(commandLine);
                WaitForChefClientToFinish(client);
            }
            finally
            {
                mtx.ReleaseMutex();
                Console.WriteLine("Mutex released");
            }

            int Exitcode = client.GetExitCode();
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
            bool morelinesPotential = true;
            while (morelinesPotential)
            {
                morelinesPotential = false;

                foreach (var line in client.GetProcessOutput().Lines)
                {
                    Console.WriteLine(line);
                    morelinesPotential = true;
                }
            }
        }

        /// <summary>
        /// Waits for Chef-client to finish while continuing to process the output
        /// </summary>
        /// <param name="client"></param>
        private void WaitForChefClientToFinish(ChefWebServiceClient client)
        {
            while (!client.HasExited())
            {
                LogChefOutput(client);
                Thread.Sleep(250); //dont hit the service too hard
            }

            //chef-client has Exited read until all lines are grabbed
            //The chef service will only send  50 lines at once to make sure the data packet doesnt get too large.
            LogChefOutput(client);
        }
        
        /// <summary>
        /// Only one chef-client run at once
        /// </summary>
        private void ObtainClientMutex()
        {
            bool alreadyexists;
            mtx = new Mutex(false, "ChefClientStarter", out alreadyexists);
            Console.WriteLine("Waiting for singular mutex to start Chef run");

            while (!mtx.WaitOne(1000))
            {
                Console.Write(".");
            }

            Console.WriteLine("Mutex obtained");
        }
    }
}
