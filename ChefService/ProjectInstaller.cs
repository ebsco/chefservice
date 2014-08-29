using System;
using System.ComponentModel;
using System.Configuration.Install;
using Microsoft.Win32;


namespace ChefService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {

            string regkey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\" + this.serviceInstaller1.ServiceName;

            //http://stackoverflow.com/a/18476058
            //This will not be active until the first reboot.  
            //This doesnt matter since this is being added for reboot functionality anyways

            if (Environment.OSVersion.Version.Major >= 6)
            {
                Registry.SetValue(regkey, "DelayedAutostart", 1, RegistryValueKind.DWord);
            }
        }
    }
}