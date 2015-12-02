using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("data", ElementName = "data")]
    public class ViewMedia
    {
        [XmlElement("location")]
        public string Location { get; set; }
    }
}
