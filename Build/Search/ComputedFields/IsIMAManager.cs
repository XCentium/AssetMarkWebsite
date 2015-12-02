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
    public class IsIMAManager : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string isIMA = item.GetText("Manager", "Individually Managed Accounts", "0");

            return isIMA;
        } 
    }
}




