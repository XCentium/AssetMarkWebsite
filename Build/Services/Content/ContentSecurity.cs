using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Content
{
    [DataContract]
    public class ContentSecurity
    {
        [DataMember]
        public string[] UserLevels;
    }
}
