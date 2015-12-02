using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Content
{
    [DataContract]
    public class ContentDataItem
    {
        [DataMember]
        public string ItemId { get; set; }

        [DataMember]
        public string ItemName { get; set; }

        [DataMember]
        public string TemplateId { get; set; }

        [DataMember]
        public string TemplateName { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public FieldDataItem[] Fields { get; set; }

        [DataMember(Name = "children")]
        public List<ContentDataItem> Children { get; set; }
    }
}
