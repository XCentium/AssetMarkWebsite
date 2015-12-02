using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Genworth.SitecoreExt.Marketing.Request
{
    [XmlRoot("Login", ElementName = "Login")]
    public class StandardRegisterLoginData
    {
        [XmlElement("UserIdentity")]
        public string UserIdentity { get; set; }

        [XmlElement("ReturnUrl")]
        public string ReturnUrl { get; set; }

        [XmlElement("FailureMessage")]
        public string FailureMessage { get; set; }

        [XmlElement("SourceSystemName")]
        public string SourceSystemName { get; set; }

        [XmlElement("Role")]
        public string Role { get; set; }

        [XmlElement("UpdateProfile")]
        public string UpdateProfile { get; set; }

        [XmlElement("CostCenter")]
        public string CostCenter { get; set; }

        [XmlElement("FirstName")]
        public string FirstName { get; set; }

        [XmlElement("LastName")]
        public string LastName { get; set; }

        [XmlElement("Address")]
        public PhysicalAddressData Address { get; set; }

        [XmlElement("Email")]
        public string Email { get; set; }

        [XmlElement("Phone")]
        public string Phone { get; set; }

        [XmlElement("SFDCID")]
        public string SFDCID { get; set; }

        [XmlElement("ManagerIdentity")]
        public string ManagerIdentity { get; set; }

        [XmlArray("AuxFields")]
        [XmlArrayItem("AuxField")]
        public AuxField[] AuxFields { get; set; }

        [XmlElement("ImpersonatingUser")]
        public ImpersonatingUserData ImpersonatingUser { get; set; }

    }
}
