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
    public class EventURL : Sitecore.ContentSearch.ComputedFields.IComputedIndexField
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
                eventURL = item.GetText("Event", "Event URL", string.Empty);
            }

            return eventURL;
        } 
    }
}



