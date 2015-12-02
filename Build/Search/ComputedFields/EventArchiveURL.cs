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
    public class EventArchiveURL : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string eventURL = null;

            if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.Webinar.Name))
            {
                eventURL = item.GetText("Event", "Archive URL", string.Empty);
            }
            else if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.ConferenceCall.Name))
            {
                string archiveUrl = string.Empty;
                if (!string.IsNullOrEmpty(archiveUrl = item.GetImageURL("Event", "Event Recording")))
                {
                    eventURL = archiveUrl;
                }
            }

            return eventURL;
        } 
    }
}



