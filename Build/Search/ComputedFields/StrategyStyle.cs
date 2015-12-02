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
    public class StrategyStyle : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            Item managerItem;
            string styleValue = null;

            if ((managerItem = item.Parent).InstanceOfTemplate(Constants.Investments.Templates.Manager))
            {
                var style = new List<string>();

                //add the template name to the style
                style.Add(item.Template.DisplayName);

                //is this item a us equity?
                if (item.InstanceOfTemplate(Constants.Investments.Templates.ManagerStrategies.FixedIncome))
                {
                    //append information
                    style.Add(string.Format("{0} {1}", item.GetListItem("Strategy", "Status").DisplayName, item.GetListItem("Strategy", "Style").DisplayName).Trim());
                }
                else if (item.InstanceOfTemplate(Constants.Investments.Templates.ManagerStrategies.InternationalGlobal))
                {
                    //append information
                    style.Add(string.Format("{0}", item.GetListItem("Strategy", "Emerging").DisplayName).Trim());
                }
                else if (item.InstanceOfTemplate(Constants.Investments.Templates.ManagerStrategies.Specialty))
                {
                    //append information
                    style.Add(string.Format("{0} {1} {2}",
                        item.GetText("Strategy", "REIT", "0").Equals("1") ?
                            "REIT" :
                            string.Empty,
                        item.GetText("Strategy", "Multi Asset Income", "0").Equals("1") ?
                            "Multi Asset Income" :
                            string.Empty,
                        item.GetText("Strategy", "Environment Social and Goverment", "0").Equals("1") ?
                            "Environmental, Social and Governance" :
                            string.Empty
                     ).Trim());
                }
                else if (item.InstanceOfTemplate(Constants.Investments.Templates.ManagerStrategies.USEquity))
                {
                    //append information
                    style.Add(string.Format("{0} {1}", item.GetListItem("Strategy", "Cap").DisplayName, item.GetListItem("Strategy", "Style").DisplayName).Trim());
                }

                //store the style information
                styleValue = string.Join(" - ", style.ToArray());
            }

            return styleValue;
        } 
    }
}







