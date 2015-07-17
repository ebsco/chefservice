using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ChefService.WebService
{
    /// <summary>
    /// Since we are storing state (ChefRunner instance) we need to make this context mode single so we can always access to chef-client process class
    /// </summary>
    [ServiceBehavior(IncludeExceptionDetailInFaults = false, InstanceContextMode = InstanceContextMode.Single)]
    public class ChefWebService : IChefWebService
    {
        ChefRunner cr;
        static object lockobj = new object();

        /// <summary>
        /// Validate a valid instance of ChefRunner exists
        /// </summary>
        private void CheckValidChefRunnerInstance()
        {
            if (cr == null)
                throw new Exception("The Chef-Client run was never started, please call the StartChef Web Service method before calling any other methods");
        }

        /// <summary>
        /// Checks to make sure client is from a local ip address.
        /// </summary>
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

        /// <summary>
        /// Starts the Chef-Client run
        /// </summary>
        /// <param name="ChefArgs"></param>
        public void StartChef(string ChefArgs)
        {
            ValidateClientIPAddress();

            lock (lockobj)
            {
                if (cr != null)
                {
                    cr.Dispose();
                    cr = null;
                }
                cr = new ChefRunner(ChefArgs);
                cr.StartChef();
            }
        }

        /// <summary>
        /// Determines if Chef-Client process has exited
        /// </summary>
        /// <returns></returns>
        public bool HasExited()
        {
            ValidateClientIPAddress();
            CheckValidChefRunnerInstance();
            return cr.HasExited;
        }

        /// <summary>
        /// Gets the Exit code of Chef-Client process
        /// </summary>
        /// <returns></returns>
        public int GetExitCode()
        {
            ValidateClientIPAddress();
            CheckValidChefRunnerInstance();
            return cr.ExitCode;
        }

        /// <summary>
        /// Gets the Process output that is being stored in memory
        /// </summary>
        /// <returns></returns>
        public ProcessOutput GetProcessOutput()
        {
            ValidateClientIPAddress();
            CheckValidChefRunnerInstance();
            return cr.Output;
        }
    }
}