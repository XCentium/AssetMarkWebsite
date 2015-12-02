using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("setting", ElementName = "setting")]
    public class Setting
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }
}
