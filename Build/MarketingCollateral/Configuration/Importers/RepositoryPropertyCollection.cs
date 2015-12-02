using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class RepositoryPropertyCollection : ConfigurationElementCollection, IEnumerable<RepositoryProperty>
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
            return new RepositoryProperty();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((RepositoryProperty)element).Name;
        }

        public RepositoryProperty this[int index]
        {
            get
            {
                return (RepositoryProperty)BaseGet(index);
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

        new public RepositoryProperty this[string name]
        {
            get
            {
                return (RepositoryProperty)BaseGet(name);
            }
        }

        public int IndexOf(RepositoryProperty url)
        {
            return BaseIndexOf(url);
        }

        public void Add(RepositoryProperty element)
        {
            BaseAdd(element);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(RepositoryProperty element)
        {
            if (element != null && BaseIndexOf(element) >= 0)
            {
                BaseRemove(element.Name);
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
        public new IEnumerator<RepositoryProperty> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as RepositoryProperty;
            }
        }
        #endregion
    }

}

