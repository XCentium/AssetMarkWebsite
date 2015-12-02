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
    public class EventType : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
    {
        public string FieldName { get; set; }

        public string ReturnType { get; set; }

        public object ComputeFieldValue(Sitecore.ContentSearch.IIndexable indexable)
        {
            var item = IndexingUtility.ValidIndexableItem(indexable, this);
            if (item == null)
                return null;

            string eventType = string.Empty;

            if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Name))
            {
                eventType = Constants.Event.Types.InPerson;
            }
            else if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.Webinar.Name))
            {
                eventType = Constants.Event.Types.Webinar;
            }
            else if (item.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.ConferenceCall.Name))
            {
                eventType = Constants.Event.Types.Conference;
            }

            return eventType;
        } 
    }
}


