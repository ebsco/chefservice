using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace ChefRunner
{
    
    #if debug
    //Test Class
    public class mymain
    {
        public static void Main(string[] args)
        {
            ChefWrap.PerformChefCall();
            ChefWrap.ResetRegistryEntry();
        }
    }
       #endif

    public class ChefWrap
    {
        const string RegKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Ebsco\Chef";
        const string RegValue = @"ShouldRun";

        public static void ResetRegistryEntry()
        {
            //Reset Reg entry
            Trace.WriteLine("Start Resetting registry entry to false");
            Registry.SetValue(RegKey, RegValue, false);
            Trace.WriteLine("End Resetting registry entry");
        }
        public static void PerformChefCall()
        {
            Trace.WriteLine("Determing Chef Call based on registry entry");
            try
            {
                object value = Registry.GetValue(RegKey, RegValue, false);
                if (value != null)
                {
                    bool shouldRun = false;
                    if (Boolean.TryParse(value.ToString(), out shouldRun))
                    {
                        if (shouldRun)
                        {
                            Trace.WriteLine("Registry entry set to true - Calling Chef");
                            Process p = new Process();
                            p.StartInfo = new ProcessStartInfo();
                            p.StartInfo.FileName = "cmd";
                            p.StartInfo.Arguments = "/c chef-client.bat";
                            p.Start();
                            Trace.WriteLine("Chef Process Started");
                        }
                        else
                        {
                            Trace.WriteLine("Registry entry sent to false - No Chef Client call");
                        }
                    }
                    else
                    {
                        Trace.WriteLine("Registry entry unable to parse - No Chef Client call");
                    }
                }
                else
                {
                    Trace.WriteLine("Registry entry does not exist - No Chef Client call");
                }
            }
            catch (Exception e)
            {
                if (!EventLog.SourceExists("ChefWrap"))
                    EventLog.CreateEventSource("ChefWrap", "Application");

                EventLog.WriteEntry("ChefWrap", "Exception calling Chef-Client:"+e, EventLogEntryType.Error, 1234);
                Trace.WriteLine(e);
            }
        }
    }
}
