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
        /// <summary>
        /// Lines of output to be sent back to CLI Utility
        /// </summary>
        [DataMember]
        public List<string> Lines = new List<string>();
    }
}
