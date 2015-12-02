using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("menu", ElementName = "menu")]
    public class Menu
    {
        [XmlElement("link")]
        public string Link { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("item")]
        public List<MenuItem> SubItems { get; set; }
    }
}
