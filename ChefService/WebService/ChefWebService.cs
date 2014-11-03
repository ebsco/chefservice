using System.ServiceModel;
using System;
using System.ServiceModel.Channels;

namespace ChefService.WebService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = false, InstanceContextMode = InstanceContextMode.Single)]
    public class ChefWebService : IChefWebService
    {
        ChefRunner cr;
        public void StartChef(string ChefArgs)
        {
            
            if (cr != null)
                cr.Dispose();

            cr = new ChefRunner(ChefArgs);
            cr.StartChefThread();
        }

        public bool HasExited()
        {
            return cr.HasExited;
        }

        public int GetExitCode()
        {
            return cr.ExitCode;
        }

        public ProcessOutput GetProcessOutput()
        {
            return cr.Output;
        }
    }
}