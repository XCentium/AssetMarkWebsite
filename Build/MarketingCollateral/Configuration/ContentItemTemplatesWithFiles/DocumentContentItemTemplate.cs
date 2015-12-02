using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class DocumentContentItemTemplate : ConfigurationElement
    {
        public DocumentContentItemTemplate()
        {
            this.TemplateName = string.Empty;
            this.SectionName = string.Empty;
            this.FieldName = string.Empty;
        }

        public DocumentContentItemTemplate(string templateName, string sectionName, string fieldName)
        {
            this.TemplateName = templateName;
            this.SectionName = sectionName;
            this.FieldName = FieldName;
        }

        [ConfigurationProperty("templateName", IsRequired = true, IsKey = true)]
        public string TemplateName
        {
            get
            {
                return (string)this["templateName"];
            }
            set
            {
                this["templateName"] = value;
            }
        }

        [ConfigurationProperty("sectionName", IsRequired = true)]
        public string SectionName
        {
            get
            {
                return (string)this["sectionName"];
            }
            set
            {
                this["sectionName"] = value;
            }
        }

        [ConfigurationProperty("fieldName", IsRequired = true)]
        public string FieldName
        {
            get
            {
                return (string)this["fieldName"];
            }
            set
            {
                this["fieldName"] = value;
            }
        }
    }
}
