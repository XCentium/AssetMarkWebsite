using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class DocumentContentItemTemplateSection : ConfigurationSection
    {
        // Declare the entity properties collection property
        [ConfigurationProperty("DocumentContentItemTemplates", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(DocumentContentItemTemplateCollection),
            AddItemName = "DocumentContentItemTemplate",
            ClearItemsName = "clearDocumentContentItemTemplate",
            RemoveItemName = "removeDocumentContentItemTemplate")]
        public DocumentContentItemTemplateCollection DocumentContentItemTemplates
        {
            get
            {
                DocumentContentItemTemplateCollection documentContentItemTemplates =
                    (DocumentContentItemTemplateCollection)base["DocumentContentItemTemplates"];
                return documentContentItemTemplates;
            }
        }
    }
}
