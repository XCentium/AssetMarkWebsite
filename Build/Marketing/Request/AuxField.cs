using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Genworth.SitecoreExt.Marketing.Request
{
    [XmlRoot("AuxField", ElementName = "AuxField")]
    public class AuxField
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}
