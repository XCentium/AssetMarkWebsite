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
    public class ArticleType : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string result = Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Types.General;

            if (item != null && item.InstanceOfTemplate("Article with Left Sidebar"))
            {
                Sitecore.Data.Fields.LookupField field = item.Fields["Category"];

                if (field != null && !string.IsNullOrEmpty(field.Value))
                {
                    result = field.Value;
                }
            }

            return result;
        }
    }
}
