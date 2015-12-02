using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class ChangeToExtendedMediaTemplateSection : ConfigurationSection
    {
        // Declare the entity properties collection property
        [ConfigurationProperty("ChangeToExtendedMediaTemplateDataEntries", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ChangeToExtendedMediaTemplateDataCollection),
            AddItemName = "ChangeToExtendedMediaTemplateData",
            ClearItemsName = "clearExtendedMediaTemplateData",
            RemoveItemName = "removeExtendedMediaTemplateData")]
        public ChangeToExtendedMediaTemplateDataCollection ChangeToExtendedMediaTemplateDataEntries
        {
            get
            {
                ChangeToExtendedMediaTemplateDataCollection changeToExtendedMediaTemplateDataEntries =
                    (ChangeToExtendedMediaTemplateDataCollection)base["ChangeToExtendedMediaTemplateDataEntries"];
                return changeToExtendedMediaTemplateDataEntries;
            }
        }

        [ConfigurationProperty("ExtendedMediaTemplateEntries", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ExtendedMediaTemplateCollection),
            AddItemName = "ExtendedMediaTemplate",
            ClearItemsName = "clearExtendedMediaTemplate",
            RemoveItemName = "removeExtendedMediaTemplate")]
        public ExtendedMediaTemplateCollection ExtendedMediaTemplateEntries
        {
            get
            {
                ExtendedMediaTemplateCollection extendedMediaTemplateEntries =
                    (ExtendedMediaTemplateCollection)base["ExtendedMediaTemplateEntries"];
                return extendedMediaTemplateEntries;
            }
        }
    }
}
