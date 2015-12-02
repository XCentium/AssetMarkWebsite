using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Genworth.SitecoreExt.Marketing.Request
{
    [XmlRoot("ImpersonatingUser", ElementName = "ImpersonatingUser")]
    public class ImpersonatingUserData
    {
        [XmlElement("UserIdentity")]
        public string UserIdentity { get; set; }

        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        public string LastName { get; set; }

        [XmlArray("AuxFields")]
        [XmlArrayItem("AuxField")]
        public AuxField[] AuxFields { get; set; }
    }
}
