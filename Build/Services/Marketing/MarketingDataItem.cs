using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Specialized;

namespace Genworth.SitecoreExt.Services.Marketing
{
    [DataContract]
    public class MarketingDataItem
    {
        [DataMember]
        public string ActionUrl { get; set; }

        [DataMember]
        public Dictionary<string,string> Fields { get; set; }
    }
}
