using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Policies
{
    [DataContract]
    public class Policy
    {
        [DataMember]
        public string Body;

        [DataMember]
        public int Version;
    }
}
