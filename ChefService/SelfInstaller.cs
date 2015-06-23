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

        /// <summary>
        /// Installs the chefservice with optional username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool InstallService(string username=null, string password=null)
        {
            ChefServiceInstallerDefinition.user = username;
            ChefServiceInstallerDefinition.pass = password;
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { _exePath });
            }
            catch(Exception e)
            {
                Console.WriteLine("Install Failed");
                Console.WriteLine("Exception:" + e);
                return false;
            }


            Console.WriteLine("Install Succeeded");
            return true;
        }

        /// <summary>
        /// Uninstalls the chefservice
        /// </summary>
        /// <returns></returns>
        public static bool UninstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(
                    new string[] { "/u", _exePath });
            }
            catch(Exception e)
            {
                Console.WriteLine("UnInstall Failed");
                Console.WriteLine("Exception:" + e);
                return false;
            }
            Console.WriteLine("UnInstall Succeeded");
            return true;
        }
    }
}
