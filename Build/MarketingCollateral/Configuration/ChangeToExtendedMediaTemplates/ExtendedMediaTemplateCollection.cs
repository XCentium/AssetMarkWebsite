using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class ExtendedMediaTemplateCollection : ConfigurationElementCollection, IEnumerable<ExtendedMediaTemplate>
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
            return new ExtendedMediaTemplate();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ExtendedMediaTemplate)element).TemplateId;
        }

        public ExtendedMediaTemplate this[int index]
        {
            get
            {
                return (ExtendedMediaTemplate)BaseGet(index);
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

        new public ExtendedMediaTemplate this[string name]
        {
            get
            {
                return (ExtendedMediaTemplate)BaseGet(name);
            }
        }

        public int IndexOf(ExtendedMediaTemplate data)
        {
            return BaseIndexOf(data);
        }

        public void Add(ExtendedMediaTemplate data)
        {
            BaseAdd(data);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(ExtendedMediaTemplate data)
        {
            if (data != null && BaseIndexOf(data) >= 0)
            {
                BaseRemove(data.TemplateId);
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
        public new IEnumerator<ExtendedMediaTemplate> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as ExtendedMediaTemplate;
            }
        }
        #endregion
    }
}


