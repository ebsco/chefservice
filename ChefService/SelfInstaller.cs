using System;
using System.Configuration.Install;
using System.Reflection;

namespace ChefService
{
    //A class that mimics installutil so we dont need to worry about a specific version being installed on the machine.
    //Instead of the user finding installutil and calling installutil /i ChefService.exe.  THey can call CHefService.exe -install, using this class.
    public static class SelfInstaller
    {
        private static readonly string _exePath =Assembly.GetExecutingAssembly().Location;

        public static bool InstallService(string username=null, string password=null)
        {
            ChefServiceInstallerDefinition.user = username;
            ChefServiceInstallerDefinition.pass = password;
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
