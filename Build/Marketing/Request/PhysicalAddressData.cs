using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Genworth.SitecoreExt.Marketing.Request
{
    [XmlRoot("Address", ElementName = "Address")]
    public class PhysicalAddressData
    {
        [XmlElement("Line1")]
        public string Line1 { get; set; }

        [XmlElement("Line2")]
        public string Line2 { get; set; }

        [XmlElement("City")]
        public string City { get; set; }

        [XmlElement("State")]
        public string State { get; set; }

        [XmlElement("PostalCode")]
        public string PostalCode { get; set; }
    }
}
