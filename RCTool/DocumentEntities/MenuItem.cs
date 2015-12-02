using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DocumentEntities
{
    [XmlRoot("item", ElementName = "item")]
    public class MenuItem
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("default-index")]
        public bool DefaultIndex { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("icon")]
        public string Icon { get; set; }

        [XmlElement("align-right")]
        public bool AlignRight { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public List<MenuItem> MenuItems { get; set; }
    }
}
