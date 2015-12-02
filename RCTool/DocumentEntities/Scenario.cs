using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("scenario", ElementName = "scenario")]
    public class Scenario
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlText()]
        public string Value { get; set; }
    }
}