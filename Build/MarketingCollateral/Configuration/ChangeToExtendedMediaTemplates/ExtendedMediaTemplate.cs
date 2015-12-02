using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class ExtendedMediaTemplate : ConfigurationElement
    {
        public ExtendedMediaTemplate()
        {
            this.TemplateId = string.Empty;
            this.TemplateFullName = string.Empty;
        }

        public ExtendedMediaTemplate(string templateId, string templateFullName)
        {
            this.TemplateId = templateId;
            this.TemplateFullName = templateFullName;
        }

        [ConfigurationProperty("templateId", IsRequired = true, IsKey = true)]
        public string TemplateId
        {
            get
            {
                return (string)this["templateId"];
            }
            set
            {
                this["templateId"] = value;
            }
        }

        [ConfigurationProperty("templateFullName", IsRequired = true)]
        public string TemplateFullName
        {
            get
            {
                return (string)this["templateFullName"];
            }
            set
            {
                this["templateFullName"] = value;
            }
        }
    }
}

