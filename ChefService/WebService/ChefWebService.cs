using System.ServiceModel;
using System;
using System.ServiceModel.Channels;
using System.Net;
using System.Text;

namespace ChefService.WebService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = false, InstanceContextMode = InstanceContextMode.Single)]
    public class ChefWebService : IChefWebService
    {
        private void CheckValidChefRunnerInstance()
        {
            if (cr == null)
                throw new Exception("The Chef-Client run was never started, please call the StartChef Web Service method before calling any other methods");
        }
        private void ValidateClientIPAddress()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;
            
            if (!IPAddress.IsLoopback(IPAddress.Parse(ip)))
            {
                throw new Exception("Incoming message was not from local host, so stopping response, only support local host executions");
            }

            string abc = OperationContext.Current.EndpointDispatcher.EndpointAddress.Uri.Host;
            
            if (abc != "localhost")
                throw new Exception("Web Service endpoint is being hosted on something else besides localhost, this is an incorrect configuration");
        }
        
        ChefRunner cr;
        public void StartChef(string ChefArgs)
        {
            ValidateClientIPAddress();
            if (cr != null)
            {
                cr.Dispose();
                cr = null;
            }

            cr = new ChefRunner(ChefArgs);
            cr.StartChefThread();
        }

        public bool HasExited()
        {
            ValidateClientIPAddress();
            CheckValidChefRunnerInstance();
            return cr.HasExited;
        }

        public int GetExitCode()
        {
            ValidateClientIPAddress();
            CheckValidChefRunnerInstance();
            return cr.ExitCode;
        }

        public ProcessOutput GetProcessOutput()
        {
            ValidateClientIPAddress();
            CheckValidChefRunnerInstance();
            return cr.Output;
        }
    }
}