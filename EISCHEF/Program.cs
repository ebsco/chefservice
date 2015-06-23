using System;

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
                Environment.Exit(Exitcode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Crashed in Chef web Service Client wrapper:" + e);
                Environment.Exit(50);
            }
        }
    }
}