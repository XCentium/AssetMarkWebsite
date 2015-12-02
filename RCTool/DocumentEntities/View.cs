using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("view", ElementName = "view")]
    public class View
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("available-orientation")]
        public AvailableOrientation AvailableOrientation { get; set; }

        [XmlArray("files")]
        [XmlArrayItem("file")]
        public List<RCFile> Files { get; set; }

        [XmlElement("view")]
        public List<View> SubViews { get; set; }

        [XmlElement("location")]
        public string Location { get; set; }
    }
}
