using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Content
{
    [DataContract]
    public class FieldDataItem
    {
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string FieldName { get; set; }
    }
}
