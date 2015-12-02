using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("image", ElementName = "image")]
    public class Image
    {
        [XmlElement("file")]
        public RCFile File { get; set; }
    }
}
