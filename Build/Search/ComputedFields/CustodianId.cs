using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Assert = Sitecore.Diagnostics.Assert;
using Log = Sitecore.ContentSearch.Diagnostics.CrawlingLog;

using SC = Sitecore;
using Sitecore.Links;
using ServerLogic.SitecoreExt;
using Sitecore.Data;
using Sitecore.Resources.Media;
using Genworth.SitecoreExt.Helpers;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace Genworth.SitecoreExt.Search.ComputedFields
{
    public class CustodianId : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string custodianId = null;
            Sitecore.Data.Fields.Field field;
            if ((field = item.GetField("Custodians")) != null && !string.IsNullOrWhiteSpace(field.Value))
            {
                custodianId = field.Value;
            }

            return custodianId;
        }
    }
}




