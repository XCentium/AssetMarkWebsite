using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("check-data", ElementName = "check-data")]
    public class CheckData : RcToolsData
    {
        [XmlElement("checksum")]
        public string Checksum { get; set; }
    }
}
