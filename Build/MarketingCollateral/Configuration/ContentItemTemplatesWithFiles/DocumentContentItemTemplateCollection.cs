using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class DocumentContentItemTemplateCollection : ConfigurationElementCollection, IEnumerable<DocumentContentItemTemplate>
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new DocumentContentItemTemplate();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((DocumentContentItemTemplate)element).TemplateName;
        }

        public DocumentContentItemTemplate this[int index]
        {
            get
            {
                return (DocumentContentItemTemplate)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public DocumentContentItemTemplate this[string name]
        {
            get
            {
                return (DocumentContentItemTemplate)BaseGet(name);
            }
        }

        public int IndexOf(DocumentContentItemTemplate importerAdaptor)
        {
            return BaseIndexOf(importerAdaptor);
        }

        public void Add(DocumentContentItemTemplate importerAdaptor)
        {
            BaseAdd(importerAdaptor);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(DocumentContentItemTemplate importerAdaptor)
        {
            if (importerAdaptor != null && BaseIndexOf(importerAdaptor) >= 0)
            {
                BaseRemove(importerAdaptor.TemplateName);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }

        #region IEnumerator
        public new IEnumerator<DocumentContentItemTemplate> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as DocumentContentItemTemplate;
            }
        }
        #endregion
    }
}

