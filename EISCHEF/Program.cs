using System;
using System.Reflection;
using System.Diagnostics;

//Try simplifying... in the future?  Maybe this would work, need to test
//https://github.com/WinRb/vagrant-windows/blob/282f712ac8f319012f8232320b91c86372f3f8f3/lib/vagrant-windows/scripts/ps_runas.ps1.erb

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ClientWrapper cw = new ClientWrapper();
                int Exitcode = cw.RunChef(args);

                ChefService.EventWriter.Write("Exiting...", ChefService.EventLevel.Information, 1, Program.GetTitle());
                Environment.Exit(Exitcode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Crashed in Chef web Service Client wrapper:" + e);
                Environment.Exit(50);
            }
        }

        public static string GetTitle()
        {

            Assembly assembly = Assembly.GetExecutingAssembly();
            
            AssemblyTitleAttribute assemblyTitle = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
            return assemblyTitle.Title;
        }

    }
}