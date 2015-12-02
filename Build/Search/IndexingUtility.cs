using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sitecore.Data;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Sitecore.Diagnostics;

using Assert = Sitecore.Diagnostics.Assert;
using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;

using SC = Sitecore;

namespace Genworth.SitecoreExt.Search
{
    public class IndexingUtility
    {
        public static string GetMultilistValues(Sitecore.Data.Fields.Field oField, string sFieldName)
        {
            Database oDatabase = oField.Database;
            Item oSubItem;
            StringBuilder oResult = new StringBuilder();
            if (oField != null)
            {
                oField.Value.Split('|').ToList().ForEach(id =>
                {
                    if ((oSubItem = oDatabase.GetItem(id)) != null)
                    {
                        oResult.Append(oSubItem.GetText(sFieldName));
                        oResult.Append(" , ");

                    }
                });
            }
            return oResult.ToString().TrimEnd(new char[] { ' ', ',', ' ' });
        }

        public static Item ValidIndexableItem(Sitecore.ContentSearch.IIndexable indexable, Sitecore.ContentSearch.ComputedFields.IComputedIndexField computedIndexField)
        {
            Assert.ArgumentNotNull(indexable, "indexable");
            SC.ContentSearch.SitecoreIndexableItem scIndexable =
              indexable as SC.ContentSearch.SitecoreIndexableItem;

            if (scIndexable == null)
            {
                Log.Log.Warn(
                  computedIndexField + " : unsupported IIndexable type : " + indexable.GetType());
                return null;
            }

            SC.Data.Items.Item item = (SC.Data.Items.Item)scIndexable;

            if (item == null)
            {
                Log.Log.Warn(
                  computedIndexField + " : unsupported SitecoreIndexableItem type : " + scIndexable.GetType());
                return null;
            }

            if (item.Name == "__Standard Values")
            {
                Log.Log.Warn(
                  computedIndexField + " : standard values' items are not indexed : " + scIndexable.GetType());
                return null;
            }

            return item;
        }
    }
}
