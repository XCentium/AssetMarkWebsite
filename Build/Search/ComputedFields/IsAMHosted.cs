using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assert = Sitecore.Diagnostics.Assert;
using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;

using SC = Sitecore;
using Sitecore.Links;

namespace Genworth.SitecoreExt.Search.ComputedFields
{
    public class IsAMHosted : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string isAMHosted = "0";

            if (item != null)
            {
                Sitecore.Data.Fields.LookupField field = item.Fields["Host"];

                if (field != null && field.InnerField != null
                    && !string.IsNullOrWhiteSpace(field.InnerField.Value)
                    && field.InnerField.Value == "Genworth")
                {
                    isAMHosted = "1";
                }
            }

            return isAMHosted;
        } 
    }
}

