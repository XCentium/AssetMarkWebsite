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

namespace Genworth.SitecoreExt.Search.ComputedFields
{
    public class EventPCStatus : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string defaultPcStatus = "NotAvailable";
            string pcStatus = string.Empty;

            if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Name))
            {
                if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.PremierConsultantUpcomingMeeting.Name))
                {
                    pcStatus = GetPcStatusSecurity(item, defaultPcStatus);
                }
                else
                {
                    pcStatus = defaultPcStatus;
                }
            }
            else if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.Webinar.Name) ||
                     item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.ConferenceCall.Name)) 
            {
                pcStatus = defaultPcStatus;
            }

            return pcStatus;
        }

        private string GetPcStatusSecurity(Sitecore.Data.Items.Item oItem, string defaultPcStatus)
        {
            StringBuilder sbPcStatus = new StringBuilder();
            string comma = ",";

            if (oItem != null)
            {
                List<Sitecore.Data.Items.Item> pcStatusList = oItem.GetMultilistItems("Security", "PC-Status");

                foreach (var pc in pcStatusList)
                {
                    string code = pc.GetText("PC-Status", "Code", string.Empty);
                    sbPcStatus.Append(code);
                    sbPcStatus.Append(comma);
                }

                if (sbPcStatus.Length > 0)
                {
                    sbPcStatus.Remove(sbPcStatus.Length - 1, 1);
                }
                else
                {
                    sbPcStatus.Append(defaultPcStatus);
                }
            }

            return sbPcStatus.ToString();
        }
    }
}



