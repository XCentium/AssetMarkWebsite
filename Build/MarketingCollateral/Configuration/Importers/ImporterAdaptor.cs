using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class ImporterAdaptor : ConfigurationElement
    {

        #region ImporterAdaptor

        public ImporterAdaptor(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        public ImporterAdaptor()
        {
            this.Name = string.Empty;
            this.Type = string.Empty;
        }
        #endregion ImporterAdaptor

        #region Public Properties

        /// <summary>
        /// Name of the adaptor
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
        /// Name of the entity that will hold the adaptor definition.
        /// It has to be its Assembly name and class.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return (string)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }

        // Declare Sitecore properties collection property
        [ConfigurationProperty("RepositoryProperties", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RepositoryPropertyCollection),
            AddItemName = "Property",
            ClearItemsName = "clearProperty",
            RemoveItemName = "removeProperty")]
        public RepositoryPropertyCollection SitecoreProperties
        {
            get
            {
                RepositoryPropertyCollection sitecoreProperties =
                    (RepositoryPropertyCollection)base["RepositoryProperties"];
                return sitecoreProperties;
            }
        }

        #endregion
    }
}

