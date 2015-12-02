using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DocumentEntities
{
    [XmlRoot("file", ElementName = "file")]
    public class RCFile
    {
        //[XmlElement("name")]
        //public string Name { get; set; }

        [XmlElement("location")]
        public string Location { get; set; }

        [XmlElement("size")]
        public string Size { get; set; }

        [XmlElement("checksum")]
        public string Checksum { get; set; }

        [XmlElement("path")]
        public string Path { get; set; }

        //[XmlElement("extension")]
        //public string Extension { get; set; }

        //[XmlElement("contentType")]
        //public string ContentType { get; set; }
    }
}
