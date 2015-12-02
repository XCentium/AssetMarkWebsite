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
    public class StrategyStatusName : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string strategyStatus = null;
            Item managerItem;

            if ((managerItem = item.Parent).InstanceOfTemplate(Constants.Investments.Templates.Manager))
            {
                if (item.InstanceOfTemplate(Constants.Investments.Templates.ManagerStrategies.FixedIncome))
                {
                    strategyStatus = item.GetListItem("Strategy", "Status").DisplayName;
                }
                else if (item.InstanceOfTemplate(Constants.Investments.Templates.ManagerStrategies.USEquity))
                {
                    strategyStatus = item.GetListItem("Strategy", "Cap").DisplayName;
                }
            }

            return strategyStatus;
        } 
    }
}







