using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Security;
using System.IO;
using System.Threading;
using ConsoleApplication1.ChefWebService;

//Try simplifying... in the future?
//https://github.com/WinRb/vagrant-windows/blob/282f712ac8f319012f8232320b91c86372f3f8f3/lib/vagrant-windows/scripts/ps_runas.ps1.erb

namespace ConsoleApplication1
{
    class Program
    {
        
        static void Main(string[] args)
        {
            try
            {
                bool alreadyexists;
                Mutex mtx = new Mutex(false, "ChefClientStarter", out alreadyexists);

                ChefWebServiceClient client = new ChefWebServiceClient();
                string commandLine = chefcommandlineparse(args);

                Console.WriteLine("Waiting for singular mutex to start Chef run");

                while (!mtx.WaitOne(1000))
                {
                    Console.Write(".");
                }
                
                Console.WriteLine("Mutex obtained");
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

                //Get exit code  from chef client run and exit based on it
                int Exitcode = client.GetExitCode();
                Console.WriteLine("Exit code from Chef :" + Exitcode);
                Console.WriteLine("Finished...");

                Environment.Exit(Exitcode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Crashed in Chef web Service Client wrapper:" + e);
                Environment.Exit(50);
            }
        }

        static string chefcommandlineparse(string[] args)
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
        static void LogChefOutput(ChefWebServiceClient client)
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
        static void WaitForChefClientToFinish(ChefWebServiceClient client)
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
        
    }
}