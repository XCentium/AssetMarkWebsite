using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace DocumentEntities
{
    [XmlInclude(typeof(CheckData))]
    [XmlInclude(typeof(ContentData))]
    [XmlRoot("content", ElementName = "content")]
    public class RcToolsData
    {
    }
}
