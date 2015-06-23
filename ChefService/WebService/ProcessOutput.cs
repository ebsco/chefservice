using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChefService.WebService
{

    /// <summary>
    /// Process output DTO for redirecting chef-client output over the local webservice connection
    /// </summary>
    [DataContract]
    public class ProcessOutput
    {
        [DataMember]
        public List<string> Lines = new List<string>();
    }
}
