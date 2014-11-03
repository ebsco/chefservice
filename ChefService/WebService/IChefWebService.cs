using System.ServiceModel;
using System.ServiceModel.Web;

namespace ChefService.WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(ProtectionLevel = System.Net.Security.ProtectionLevel.None )]
    public interface IChefWebService
    {
        [WebInvoke(UriTemplate = "StartChef?ChefArgs={ChefArgs}", Method = "GET")]
        void StartChef(string ChefArgs);

        [WebInvoke(UriTemplate = "GetExitCode", Method = "GET")]
        int GetExitCode();


        [WebInvoke(UriTemplate = "GetProcessOutput", Method = "GET")]
        ProcessOutput GetProcessOutput();

        [WebInvoke(UriTemplate = "HasExited", Method = "GET")]
        bool HasExited();
   
        
    }
}
