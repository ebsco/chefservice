using System.ServiceModel;
using System.ServiceModel.Web;

namespace ChefService.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(ProtectionLevel = System.Net.Security.ProtectionLevel.None )]
    public interface IChefWebService
    {
        
        [OperationContract]
        [WebInvoke(UriTemplate = "StartChef?ChefArgs={ChefArgs}", Method = "GET")]
        void StartChef(string ChefArgs);

        [OperationContract]
        [WebInvoke(UriTemplate = "GetExitCode", Method = "GET")]
        int GetExitCode();

        [OperationContract]
        [WebInvoke(UriTemplate = "GetProcessOutput", Method = "GET")]
        ProcessOutput GetProcessOutput();

        [OperationContract]
        [WebInvoke(UriTemplate = "HasExited", Method = "GET")]
        bool HasExited();
   
        
    }
}
