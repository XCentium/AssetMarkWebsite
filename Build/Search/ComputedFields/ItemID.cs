using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerLogic.SitecoreExt;
using System.Linq;

using Assert = Sitecore.Diagnostics.Assert;
using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;

using SC = Sitecore;
using Sitecore.Links;

namespace Genworth.SitecoreExt.Search.ComputedFields
{
    public class ItemID : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            var result = item.ID.ToGuid();

            return result.ToString();
        }
    }
}

