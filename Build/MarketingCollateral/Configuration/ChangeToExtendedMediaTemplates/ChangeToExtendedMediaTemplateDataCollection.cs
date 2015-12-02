using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.MarketingCollateral.Configuration
{
    public class ChangeToExtendedMediaTemplateDataCollection : ConfigurationElementCollection, IEnumerable<ChangeToExtendedMediaTemplateData>
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
            return new ChangeToExtendedMediaTemplateData();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ChangeToExtendedMediaTemplateData)element).Name;
        }

        public ChangeToExtendedMediaTemplateData this[int index]
        {
            get
            {
                return (ChangeToExtendedMediaTemplateData)BaseGet(index);
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

        new public ChangeToExtendedMediaTemplateData this[string name]
        {
            get
            {
                return (ChangeToExtendedMediaTemplateData)BaseGet(name);
            }
        }

        public int IndexOf(ChangeToExtendedMediaTemplateData data)
        {
            return BaseIndexOf(data);
        }

        public void Add(ChangeToExtendedMediaTemplateData data)
        {
            BaseAdd(data);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(ChangeToExtendedMediaTemplateData data)
        {
            if (data != null && BaseIndexOf(data) >= 0)
            {
                BaseRemove(data.Name);
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
        public new IEnumerator<ChangeToExtendedMediaTemplateData> GetEnumerator()
        {
            int count = base.Count;

            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as ChangeToExtendedMediaTemplateData;
            }
        }
        #endregion
    }
}

