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
                //while (true){

                ChefWebServiceClient client = new ChefWebServiceClient();
                string commandLine = chefcommandlineparse(args);
                client.StartChef(commandLine);
                WaitForChefClientToFinish(client);


                //Get exit code  from chef client run and exit based on it
                int Exitcode = client.GetExitCode();
                Console.WriteLine("Exit code from Chef :" + Exitcode);
                Console.WriteLine("Finished...");
                //}
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
        static bool LogChefOutput(ChefWebServiceClient client)
        {
            bool linewritten = false;
            foreach (var line in client.GetProcessOutput().Lines)
            {
                Console.WriteLine(line);
                linewritten = true;
                //linecount++;
            }
            return linewritten;
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
            bool morelines = true;
            while (morelines)
            {
                morelines = LogChefOutput(client);
                Thread.Sleep(50);
            }
        }
        
    }
}