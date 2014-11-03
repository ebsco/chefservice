using System;
using System.Configuration.Install;
using System.Reflection;

namespace ChefService
{
    public static class SelfInstaller
    {
        private static readonly string _exePath =Assembly.GetExecutingAssembly().Location;


        public static bool InstallService(string username=null, string password=null)
        {
            MyWindowsServiceInstaller.user = username;
            MyWindowsServiceInstaller.pass = password;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { _exePath });
            }
            catch
            {
                Console.WriteLine("Install Failed");
                return false;
            }


            Console.WriteLine("Install Succeeded");
            return true;
        }

        public static bool UninstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { "/u", _exePath });
            }
            catch
            {
                Console.WriteLine("UnInstall Failed");
                return false;
            }
            Console.WriteLine("UnInstall Succeeded");
            return true;
        }
    }
}
