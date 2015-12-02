using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class RepositorySection : ConfigurationSection
    {
        #region MarketingCollateralImporter

        public RepositorySection(string name, int displayOrder)
        {
            this.Name = name;
            this.DisplayOrder = displayOrder;
        }

        public RepositorySection()
        {
            this.Name = string.Empty;
            this.DisplayOrder = 0;
        }

        #endregion MarketingCollateralImporter

        #region Public Properties

        /// <summary>
        /// Name of the Importer
        /// </summary>
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

        /// <summary>
        /// Name of the Importer
        /// </summary>
        [ConfigurationProperty("displayOrder", IsRequired = true)]
        public int DisplayOrder
        {
            get
            {
                return (int)this["displayOrder"];
            }
            set
            {
                this["displayOrder"] = value;
            }
        }

        // Declare the entity properties collection property
        [ConfigurationProperty("ImporterAdaptors", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ImporterAdaptorCollection),
            AddItemName = "ImporterAdaptor",
            ClearItemsName = "clearImporterAdaptor",
            RemoveItemName = "removeImporterAdaptor")]
        public ImporterAdaptorCollection ImporterAdaptors
        {
            get
            {
                ImporterAdaptorCollection importerAdaptors =
                    (ImporterAdaptorCollection)base["ImporterAdaptors"];
                return importerAdaptors;
            }
        }

        #endregion
    }
}

