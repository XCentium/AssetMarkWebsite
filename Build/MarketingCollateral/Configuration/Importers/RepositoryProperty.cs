using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class RepositoryProperty : ConfigurationElement
    {
        #region Constructors
        public RepositoryProperty(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public RepositoryProperty()
        {
            this.Name = string.Empty;
            this.Value = string.Empty;
            this.Format = string.Empty;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Name of the entity property.
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
        /// Field that holds the value of the property.
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return (string)this["value"];
            }
            set
            {
                this["value"] = value;
            }
        }

        /// <summary>
        /// Field that holds the format of the property.
        /// </summary>
        [ConfigurationProperty("format", IsRequired = false)]
        public string Format
        {
            get
            {
                return (string)this["format"];
            }
            set
            {
                this["format"] = value;
            }
        }

        #endregion
    }
}

