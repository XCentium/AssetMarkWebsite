using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.Utilities;
using Genworth.SitecoreExt.Services.Search;
using Lucene.Net;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Util;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Lucene.Net.Index;
using System.Threading;
using System.Globalization;
using System.ServiceModel;
using Genworth.SitecoreExt.Services.Events;
using Genworth.SitecoreExt.Providers;
using GFWM.Shared.ServiceRequest;
using GFWM.Shared.ServiceRequestFactory;
using GFWM.App.GlobalSearch.Entities.Request;
using GFWM.App.GlobalSearch.Entities.Response;

namespace Genworth.SitecoreExt.Helpers
{
    public class EventHelper
    {
        #region VARIABLES

        static CultureInfo oCulture = Thread.CurrentThread.CurrentCulture;

        #endregion

        #region PROPERTIES
        private static readonly string sEventPageItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.EventSearch");
        private static Item oEventPageItem = !string.IsNullOrEmpty(sEventPageItemId) ? ContextExtension.CurrentDatabase.GetItem(sEventPageItemId) : null;

        public static Item EventPageItem
        {
            get
            {

                return oEventPageItem;
            }
        }

        public static readonly string sRegistrationPageItemId = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Events.RegistrationPage);
        private static Item oRegistrationPageItem = !string.IsNullOrEmpty(sRegistrationPageItemId) ? ContextExtension.CurrentDatabase.GetItem(sRegistrationPageItemId) : null;
        public static Item RegristationPageItem
        {
            get
            {
                return oRegistrationPageItem;
            }
        }

        private static string registrationPageUrl;
        public static string RegistrationPageUrl
        {
            get
            {
                if (registrationPageUrl == null)
                {
                    if (oRegistrationPageItem != null && oRegistrationPageItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.RegistrationPage.Name))
                    {
                        registrationPageUrl = oRegistrationPageItem.GetURL(true);
                    }
                }
                return registrationPageUrl;
            }
        }



        #endregion

        public static IJsonCollectionProvider GetProvider(string sProviderName)
        {
            IJsonCollectionProvider provider = null;

            switch (sProviderName.ToLower())
            {
                case EventsAll.CODE:
                    provider = EventInstance<EventsAll>();
                    break;
                case EventsInPerson.CODE:
                    provider = EventInstance<EventsInPerson>();
                    break;
                case EventsWebinar.CODE:
                    provider = EventInstance<EventsWebinar>();
                    break;
                case EventsArchive.CODE:
                    provider = EventInstance<EventsArchive>();
                    break;
            }

            return provider;
        }

        private static T EventInstance<T>() where T : class, new()
        {
            T provider = null;

            string sessionKey = typeof(T).FullName;

            if ((provider = System.Web.HttpContext.Current.Session[sessionKey] as T) == null)
            {
                System.Web.HttpContext.Current.Session[typeof(T).FullName] = provider = new T();
            }

            return provider;
        }

        public static List<Item> GetEventsByDateRange(DateTime dBeginDate, DateTime dEndDate)
        {
            Item oEventPageItem;
            IEnumerable<Item> result = new List<Item>();

            //if we have an event page item, let's query a range of events
            oEventPageItem = EventPageItem;
            if (oEventPageItem != null)
            {
                result = oEventPageItem.GetChildren().Where(oItem => oItem.InstanceOfTemplate("Event Base") && IsIteminDateRange(oItem, dBeginDate, dEndDate));
            }

            return result.OrderBy(oItem => oItem.GetText("Event", "Begin Date")).ToList();
        }

        public static bool IsIteminDateRange(Item oItem, DateTime dBeginDate, DateTime dEndDate)
        {
            DateTime oEnd;
            DateTime oBbegin;
            if (oItem != null && DateTime.TryParseExact(oItem.GetText("End Date"), "yyyyMMddTHHmmss", oCulture, DateTimeStyles.None, out oEnd) && DateTime.TryParseExact(oItem.GetText("Begin Date"), "yyyyMMddTHHmmss", oCulture, DateTimeStyles.None, out oBbegin))
            {
                return oEnd >= dBeginDate && oBbegin <= dEndDate;
            }
            else
                return false;
        }

        public static string GetDateString(Item oEventItem, string sFormat, string sRange)
        {
            StringBuilder sBuilder;
            string sBeginDate;
            string sEndDate;

            //create the string builder
            sBuilder = new StringBuilder();

            //get the end date
            sEndDate = oEventItem.GetField("Event", "End Date").GetDateString(sFormat, string.Empty);

            //bind the begin and end date
            if (!string.IsNullOrEmpty(sBeginDate = oEventItem.GetField("Event", "Begin Date").GetDateString(sFormat, string.Empty)))
            {
                //append the start date
                sBuilder.Append(sBeginDate);

                //do we have an end date, and is it different than the start date
                if (!string.IsNullOrEmpty(sEndDate) && !sEndDate.Equals(sBeginDate))
                {
                    //append the start date
                    sBuilder.Append(sRange).Append(sEndDate);
                }
            }
            else if (!string.IsNullOrEmpty(sEndDate))
            {
                //append the end date
                sBuilder.Append(sEndDate);
            }

            //return the date range
            return sBuilder.ToString();
        }

        public static List<Document> SearchEvents(Dictionary<string, object> searchFilters)
        {
            return SearchEventsInternal(searchFilters);
        }

        private static List<Document> SearchEventsInternal(Dictionary<string, object> searchFilters)
        {
            List<Document> results = new List<Document>();

            string eventType = searchFilters.ContainsKey(Constants.Event.SearchFilters.EventType) ?
                searchFilters[Constants.Event.SearchFilters.EventType].ToString() : string.Empty;

            string keywords = searchFilters.ContainsKey(Constants.Event.SearchFilters.Keywords) ?
                searchFilters[Constants.Event.SearchFilters.Keywords].ToString() : string.Empty;

            DateTime fromDate = searchFilters.ContainsKey(Constants.Event.SearchFilters.BeginDate) ?
                Convert.ToDateTime(searchFilters[Constants.Event.SearchFilters.BeginDate]) : DateTime.MinValue;

            DateTime toDate = searchFilters.ContainsKey(Constants.Event.SearchFilters.EndDate) ?
                Convert.ToDateTime(searchFilters[Constants.Event.SearchFilters.EndDate]) : DateTime.MaxValue;

            // hack of time, 'cause Lucene doesn't work with inclusive TO DATE, not even with 23:59:59, must be 1 full day
            if (toDate != DateTime.MaxValue)
                toDate = toDate.AddDays(1);

            string[] pcStatus = searchFilters.ContainsKey(Constants.Event.SearchFilters.PCStatus) ?
                (string[])searchFilters[Constants.Event.SearchFilters.PCStatus] : new string[0];

            // additional filters: By Location
            double ratioInMiles = searchFilters.ContainsKey(Constants.Event.SearchFilters.RatioInMiles) ?
                Convert.ToDouble(searchFilters[Constants.Event.SearchFilters.RatioInMiles]) : 0;

            string zipCode = searchFilters.ContainsKey(Constants.Event.SearchFilters.ZipCode) ?
                searchFilters[Constants.Event.SearchFilters.ZipCode].ToString() : string.Empty;

            //Diagnostics
            Sitecore.Diagnostics.Log.Info("Event Helper: Calling GS Service. Sending the following parameters:", new object());
            LogEventSearchFilters(eventType, keywords, fromDate, toDate, pcStatus, ratioInMiles, zipCode, typeof(EventHelper));

            if (string.IsNullOrWhiteSpace(keywords))
            {
                results = CallEventSearchService(
                    new EventsSearchRequest()
                    {
                        SearchType = GetEventSearchType(eventType),
                        BeginDate = fromDate,
                        EndDate = toDate,
                        PcStatus = pcStatus
                    });
            }
            else
            {
                results = CallEventSearchService(
                    new EventsKeywordSearchRequest()
                    {
                        SearchPhrase = keywords,
                        SearchType = GetEventSearchType(eventType),
                        BeginDate = fromDate,
                        EndDate = toDate,
                        PcStatus = pcStatus
                    });
            }

            if (ratioInMiles > 0 && !string.IsNullOrWhiteSpace(zipCode))
            {
                results = FilterSearchEventsByLocation(results, ratioInMiles, zipCode);
            }

            return results;
        }

        private static List<Document> CallEventSearchService(EventsKeywordSearchRequest request)
        {
            IServiceRequest client = ServiceRequestFactory.GetProxy(SERVICES.GLOBALSEARCH_SERVICE);
            List<Document> results = new List<Document>();
            EventsSearchResponse response = null;

            try
            {
                response = client.Request<EventsKeywordSearchRequest, EventsSearchResponse>(request);
                results = response.Results;
            }
            catch (Exception searchException)
            {
                Sitecore.Diagnostics.Log.Error("Error executing event search request", searchException, typeof(EventHelper));
            }

            return results;
        }

        private static List<Document> CallEventSearchService(EventsSearchRequest request)
        {
            IServiceRequest client = ServiceRequestFactory.GetProxy(SERVICES.GLOBALSEARCH_SERVICE);
            List<Document> results = new List<Document>();
            EventsSearchResponse response = null;

            try
            {
                response = client.Request<EventsSearchRequest, EventsSearchResponse>(request);
                results = response.Results;
            }
            catch (Exception searchException)
            {
                Sitecore.Diagnostics.Log.Error("Error executing event search request", searchException, typeof(EventHelper));
            }

            return results;
        }

        private static GFWM.App.GlobalSearch.Entities.Enums.EventSearchTypes GetEventSearchType(string type)
        {
            GFWM.App.GlobalSearch.Entities.Enums.EventSearchTypes searchType =
                GFWM.App.GlobalSearch.Entities.Enums.EventSearchTypes.All;

            switch (type)
            {
                case Constants.Event.SearchType.Archive:
                    searchType = GFWM.App.GlobalSearch.Entities.Enums.EventSearchTypes.Archive;
                    break;
                case Constants.Event.SearchType.InPerson:
                    searchType = GFWM.App.GlobalSearch.Entities.Enums.EventSearchTypes.InPerson;
                    break;
                case Constants.Event.SearchType.Webinar:
                    searchType = GFWM.App.GlobalSearch.Entities.Enums.EventSearchTypes.WebinarAndConference;
                    break;
            }

            return searchType;
        }

        private static List<Document> FilterSearchEventsByLocation(List<Document> documents, double ratioInMiles, string zipCode)
        {
            List<Document> documentsToReturn;
            Dictionary<string, string> coordinatesOrigin;
            double latitude;
            double longitude;

            coordinatesOrigin = Geography.LookupCoordinates(zipCode);
            documentsToReturn = new List<Document>();

            if (coordinatesOrigin != null && coordinatesOrigin.Keys.Contains(Geography.sLatitudeKey) && coordinatesOrigin.Keys.Contains(Geography.sLongitudeKey))
            {
                if (double.TryParse(coordinatesOrigin[Geography.sLatitudeKey], out latitude) && double.TryParse(coordinatesOrigin[Geography.sLongitudeKey], out longitude))
                {
                    documentsToReturn = FilterDocumentsByLatitudeAndLongitude(documents, latitude, longitude, ratioInMiles);
                }
                else
                {
                    Sitecore.Diagnostics.Log.Error("SearchEvents - Unable to obtain latitude/longitude for origin location", typeof(EventHelper));
                }
            }

            return documentsToReturn;
        }

        private static List<Document> FilterDocumentsByLatitudeAndLongitude(List<Document> documents, double dLatitude, double dLongitude, double dRatioInMiles)
        {
            List<Document> oDocumentsToReturn;

            oDocumentsToReturn = new List<Document>();
            foreach (Document oDoc in documents)
            {
                if (WithinLatitudeAndLongitude(oDoc, dLatitude, dLongitude, dRatioInMiles))
                {
                    oDocumentsToReturn.Add(oDoc);
                }
            }
            return oDocumentsToReturn;
        }

        public static bool WithinLatitudeAndLongitude(Document doc, double latitude, double longitude, double ratioInMiles)
        {
            double latitude2 = 0;
            double longitude2 = 0;

            Field latitudeField = doc.GetField("latitude");
            if (latitudeField != null && double.TryParse(latitudeField.StringValue, out latitude2))
            {
                Field longitudeField = doc.GetField("longitude");
                if (longitudeField != null && double.TryParse(longitudeField.StringValue, out longitude2))
                {
                    return WithinLatitudeAndLongitude(latitude, longitude, latitude2, longitude2, ratioInMiles);
                }
            }

            return false;
        }

        public static bool WithinLatitudeAndLongitude(double dLatitude, double dLongitude, double dLatitude2, double dLongitude2, double dRatioInMiles)
        {
            if (Geography.CalcDistance(dLatitude, dLongitude, dLatitude2, dLongitude2) <= dRatioInMiles)
                return true;
            return false;
        }

        public static Item GetItemByDateRange(string sItemId, DateTime dBeginDate, DateTime dEndDate)
        {
            Item oItem = ContextExtension.CurrentDatabase.GetItem(sItemId);
            return IsIteminDateRange(oItem, dBeginDate, dEndDate) ? oItem : null;
        }

        public static string GetRegisterPageURL(string id)
        {
            return string.Format(
                Genworth.SitecoreExt.Constants.Event.RegistrationPage.URLFormat,
                RegistrationPageUrl,
                Genworth.SitecoreExt.Constants.Event.RegistrationPage.EventIdQueryStringKey,
                id
            );
            //return registerUrl;
        }

        public static string GetField(Document doc, string field)
        {
            Field oField;
            return (oField = doc.GetField(field)) != null ? oField.StringValue : string.Empty;
        }

        public static string GetDateFromString(string stringValue, out DateTime dDate)
        {
            dDate = DateTime.MinValue;
            string parseFormat = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Indexing.DataFormat.DateTimeStringFormat, "yyyyMMdd");
            if (!string.IsNullOrEmpty(stringValue) && DateTime.TryParseExact(stringValue, parseFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dDate))
            {
                //format the date properly

                stringValue = dDate.ToString("MM/dd/yyyy");
            }

            return stringValue;
        }

        /// <summary>
        /// //Diagnostics method
        /// </summary>
        public static void LogSearchFilters(string from, Dictionary<string, object> searchFilters, object owner)
        {
            foreach (string s in searchFilters.Keys)
            {
                string filter;
                if (!String.IsNullOrEmpty(searchFilters[s] as string))
                {
                    filter = searchFilters[s].ToString();
                }
                else
                {
                    filter = "null";
                }

                Sitecore.Diagnostics.Log.Info(from + ": Search filters: " + s + ": " + filter, owner);
            }
        }

        public static void LogEventSearchFilters(string eventType, string keywords, DateTime fromDate, DateTime toDate, string[] pcStatus, double ratioInMiles, string zipCode, object owner)
        {
            string format = "Event Helper: Search filters: eventType: [{0}], keyworkds: [{1}], fromDate: [{2}], toDate: [{3}], pcStatus: [{4}], ratioInMiles: [{5}], zipCode: [{6}]";
            Sitecore.Diagnostics.Log.Info(string.Format(format, eventType, keywords, fromDate.ToString("yyyy-MM-ddTHH:mm:ss"), toDate.ToString("yyyy-MM-ddTHH:mm:ss"), string.Join(" ", pcStatus), ratioInMiles, zipCode), owner);
        }

        class ItemComparer : IEqualityComparer<Item>
        {
            public bool Equals(Item x, Item y)
            {
                return x.ID == y.ID;
            }
            public int GetHashCode(Item oItem)
            {
                return oItem.ID.GetHashCode();
            }
        }

    }
}
