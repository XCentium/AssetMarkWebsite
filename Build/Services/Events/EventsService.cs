using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Sitecore.Data.Items;
using System.ServiceModel.Web;
using System.Web;
using Genworth.SitecoreExt.Helpers;

namespace Genworth.SitecoreExt.Services.Events
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EventsService : IEventsService
    {
        public string Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public EventsBase Eventing(string sType)
        {
            SetNoChaching();
            return (EventsBase)EventHelper.GetProvider(sType);
        }

        public EventDataItem[] GetResults(string sType)
        {
            SetNoChaching();

            //use the research to get hte documents
            return ((EventsBase)Eventing(sType)).GetResults();
        }

        public EventsBase SetDateRange(string sType, string sFromDate, string sToDate)
        {
            SetNoChaching();
            EventsBase oEvents;

            //set the dates
            oEvents = Eventing(sType);
            oEvents.FromDate = sFromDate;
            oEvents.ToDate = sToDate;

            //return the research
            return oEvents;
        }

        public EventsBase SetDate(string sType, string sDate)
        {
            SetNoChaching();
            EventsBase oEvents;

            //set the dates
            (oEvents = Eventing(sType)).SetInclusiveDate(sDate);

            //return the research
            return oEvents;
        }

        public EventsBase SetSort(string sType, string sField)
        {
            SetNoChaching();
            EventsBase oEvents;

            //we are using a local variable to skip cycles when we return
            (oEvents = Eventing(sType)).Sort(sField);

            //return the research
            return oEvents;
        }

        public EventsBase SetResultsPerPage(string sType, string sResultsPerPage)
        {
            SetNoChaching();
            EventsBase oEvents;
            int iResultsPerPage;

            //parse the results per page
            int.TryParse(sResultsPerPage, out iResultsPerPage);

            //set the results per page
            (oEvents = Eventing(sType)).ResultsPerPage = iResultsPerPage;

            //return the research
            return oEvents;
        }

        public EventsBase SetPage(string sType, string sPage)
        {
            SetNoChaching();
            EventsBase oEvents;
            int iPage;

            //parse the page
            int.TryParse(sPage, out iPage);

            //set the page
            (oEvents = Eventing(sType)).Page = iPage;

            //return the research
            return oEvents;
        }

        public EventsBase SetKeywords(string sType, string sKeywords)
        {
            SetNoChaching();
            EventsBase oEvents;

            //set the keywords
            (oEvents = Eventing(sType)).SetKeywords(sKeywords); 
            

            //return the research
            return oEvents;
        }

        public EventsBase SetLocation(string sType, string sRatio, string sZipCode)
        {
            SetNoChaching();
            EventsBase oEvents;

            // TODO: Set Set Location functionality
            (oEvents = Eventing(sType)).SetLocation(sRatio, sZipCode);
            //return the research
            return oEvents;
        }

        public EventsBase Reset(string sType)
        {
            SetNoChaching();
            EventsBase oEvents = null;

            //set the keywords
            (oEvents = Eventing(sType)).Reset(true);

            //return the research
            return oEvents;
        }

        internal void SetNoChaching()
        {
            HttpContext.Current.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
        }
    }
}
