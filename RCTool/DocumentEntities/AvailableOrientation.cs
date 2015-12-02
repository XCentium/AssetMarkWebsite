using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot(ElementName="available-orientation")]
    public class AvailableOrientation
    {
        [XmlElement("portrait")]
        public bool Portrait { get; set; }

        [XmlElement("landscape")]
        public bool Landscape { get; set; }
    }
}
