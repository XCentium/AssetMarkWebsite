using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class ChangeToExtendedMediaTemplateData : ConfigurationElement
    {
        public ChangeToExtendedMediaTemplateData()
        {
            this.Name = string.Empty;
            this.OldTemplate = string.Empty;
            this.NewTemplate = string.Empty;
            this.MimeType = string.Empty;
        }

        public ChangeToExtendedMediaTemplateData(string name, string oldTemplate, string newTemplate, string mimeType)
        {
            this.Name = name;
            this.OldTemplate = oldTemplate;
            this.NewTemplate = newTemplate;
            this.MimeType = mimeType;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("oldTemplate", IsRequired = true, IsKey = true)]
        public string OldTemplate
        {
            get
            {
                return (string)this["oldTemplate"];
            }
            set
            {
                this["oldTemplate"] = value;
            }
        }

        [ConfigurationProperty("newTemplate", IsRequired = true)]
        public string NewTemplate
        {
            get
            {
                return (string)this["newTemplate"];
            }
            set
            {
                this["newTemplate"] = value;
            }
        }

        [ConfigurationProperty("mimeType", IsRequired = true)]
        public string MimeType
        {
            get
            {
                return (string)this["mimeType"];
            }
            set
            {
                this["mimeType"] = value;
            }
        }
    }
}
