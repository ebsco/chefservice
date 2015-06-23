using System.ServiceModel;
using System.ServiceModel.Web;

namespace ChefService.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(ProtectionLevel = System.Net.Security.ProtectionLevel.None )]
    public interface IChefWebService
    {

        /// <summary>
        /// Start the chef-client process
        /// </summary>
        /// <param name="ChefArgs">The args to pass to the chef-client process</param>
        [OperationContract]
        [WebInvoke(UriTemplate = "StartChef?ChefArgs={ChefArgs}", Method = "GET")]
        void StartChef(string ChefArgs);

        /// <summary>
        /// Gets the exit code of chef-client
        /// </summary>
        /// <returns>integer representing the exit code of the process</returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "GetExitCode", Method = "GET")]
        int GetExitCode();

        /// <summary>
        /// Gets a portion of the chef-client output
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "GetProcessOutput", Method = "GET")]
        ProcessOutput GetProcessOutput();

        /// <summary>
        /// Determines if the chef-client process has exited or not
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "HasExited", Method = "GET")]
        bool HasExited();
   
        
    }
}
