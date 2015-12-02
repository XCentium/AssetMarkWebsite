using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class ImporterAdaptorCollection : ConfigurationElementCollection, IEnumerable<ImporterAdaptor>
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
            return new ImporterAdaptor();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ImporterAdaptor)element).Name;
        }

        public ImporterAdaptor this[int index]
        {
            get
            {
                return (ImporterAdaptor)BaseGet(index);
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

        new public ImporterAdaptor this[string name]
        {
            get
            {
                return (ImporterAdaptor)BaseGet(name);
            }
        }

        public int IndexOf(ImporterAdaptor importerAdaptor)
        {
            return BaseIndexOf(importerAdaptor);
        }

        public void Add(ImporterAdaptor importerAdaptor)
        {
            BaseAdd(importerAdaptor);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(ImporterAdaptor importerAdaptor)
        {
            if (importerAdaptor != null && BaseIndexOf(importerAdaptor) >= 0)
            {
                BaseRemove(importerAdaptor.Name);
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
        public new IEnumerator<ImporterAdaptor> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as ImporterAdaptor;
            }
        }
        #endregion
    }
}

