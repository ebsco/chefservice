using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ChefService.WebService
{
    [DataContract]
    public class ProcessOutput
    {
        [DataMember]
        public List<string> Lines = new List<string>();
    }
}
