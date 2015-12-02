using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using ServerLogic.SitecoreExt;
using Sitecore.Data;
using Genworth.SitecoreExt.Helpers;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis.Standard;
using Genworth.SitecoreExt.Security;

namespace Genworth.SitecoreExt.Services.Events
{
    public class EventSearch
    {
        private bool bIsDirty;
        private Document[] oResultDocuments;
        private string[] sKeywordFields;
        private Dictionary<string, object> searchFilters;
        private bool bIsDefaultSort;
        private DateTime InclusiveDate;
        protected bool bIsInclusiveDateSearch;

        private string EventType { get; set; }

        internal string[] KeywordFields
        {
            get { return sKeywordFields; }
            set { sKeywordFields = value; }
        }

        private bool bKeywordSearch;

        private string sKeyword;
        public bool IsKeywordSearch
        {
            get { return bKeywordSearch; }
            set { bKeywordSearch = value; }
        }

        public string Keyword
        {
            get { return sKeyword; }

        }
        public bool IsDirty
        {
            get
            {
                return bIsDirty;
            }
        }

        public bool IsDefaultSort
        {
            get
            {
                return bIsDefaultSort;
            }
        }

        public Document[] ResultDocuments
        {
            get
            {
                //have we changed our data enough to be dirty?
                if (bIsDirty)
                {
                    //refetch the items from index
                    FetchItems();
                }

                //return the results (never null)
                return oResultDocuments ?? new Document[] { };
            }
        }

        public EventSearch()
        {
            this.bKeywordSearch = false;
            sKeywordFields = new string[0];
            searchFilters = new Dictionary<string, object>();
            Reset();

            Authorization oAuthorization = Authorization.CurrentAuthorization;
            if (oAuthorization.IsAgent)
            {
                Sitecore.Diagnostics.Log.Info("Event Search: Current User is an agent.", this);
                string[] pcStatus = oAuthorization.PC_Status;
                this.SetFilter(Constants.Event.SearchFilters.PCStatus, pcStatus);

                //Diagnostics
                StringBuilder pcStatusOutput = new StringBuilder();
                foreach (string s in pcStatus)
                {
                    pcStatusOutput.Append("[");
                    pcStatusOutput.Append(s);
                    pcStatusOutput.Append("]");
                }

                Sitecore.Diagnostics.Log.Info("Event Search: Pc Status: " + pcStatusOutput.ToString(), this);
            }
        }

        /// <summary>
        /// //Diagnostics method
        /// </summary>
        private void LogSearchFilters()
        {
            EventHelper.LogSearchFilters("Event Search", searchFilters, this);
        }

        public void Reset()
        {
            RemoveFilter(Constants.Event.SearchFilters.Keywords);
            RemoveFilter(Constants.Event.SearchFilters.RatioInMiles);
            RemoveFilter(Constants.Event.SearchFilters.ZipCode);
            bIsDefaultSort = true;

            //we are dirty
            bIsDirty = true;

            //Diagnostics
            Sitecore.Diagnostics.Log.Info("Event Search: Reset called. Showing search filters after reset.", this);
            LogSearchFilters();
        }

        private void SetFilter(string sField, object oValue, bool bScapeCharacters = true, bool bWilcardSearch = false)
        {
            //see if we already have a clause for this filter in the query
            if (searchFilters.ContainsKey(sField))
            {
                //remove the clause
                searchFilters.Remove(sField);
            }

            searchFilters.Add(sField, oValue);

            //we are dirty
            bIsDirty = true;
        }

        private void RemoveFilter(string sField)
        {
            if (searchFilters.ContainsKey(sField))
            {
                //remove the clause
                searchFilters.Remove(sField);
            }
            bIsDirty = true;
        }

        public void SetEventType(string sEventType)
        {
            if (!string.IsNullOrWhiteSpace(sEventType))
            {
                this.EventType = sEventType;
                SetFilter(Constants.Event.SearchFilters.EventType, sEventType);
            }
        }

        public void SetKeywordFilter(string sKeywords)
        {
            this.sKeyword = sKeywords;

            // Set only keyword filter
            SetFilter(Constants.Event.SearchFilters.Keywords, sKeywords);

            // Remove additional filters
            RemoveFilter(Constants.Event.SearchFilters.RatioInMiles);
            RemoveFilter(Constants.Event.SearchFilters.ZipCode);

            bKeywordSearch = true;

            //Diagnostics
            Sitecore.Diagnostics.Log.Info("Event Search: Set Keyword Filter called. Showing search filters after set.", this);
            LogSearchFilters();
        }

        public void SetDateFilter(DateTime dFromDate, DateTime dToDate, bool isDefault = false)
        {
            SetFilter(Constants.Event.SearchFilters.BeginDate, dFromDate);
            SetFilter(Constants.Event.SearchFilters.EndDate, dToDate);

            if (!isDefault)
            {
                // Remove additional filters
                RemoveFilter(Constants.Event.SearchFilters.RatioInMiles);
                RemoveFilter(Constants.Event.SearchFilters.ZipCode);
                RemoveFilter(Constants.Event.SearchFilters.Keywords);
            }

            //Diagnostics
            Sitecore.Diagnostics.Log.Info("Event Search: Set Date Filter called. Showing search filters after set.", this);
            LogSearchFilters();
        }

        public void SetLocation(double ratioInMiles, string zipCode)
        {
            // Set only location filter
            SetFilter(Constants.Event.SearchFilters.RatioInMiles, ratioInMiles);
            SetFilter(Constants.Event.SearchFilters.ZipCode, zipCode);

            // Remove additional filters
            RemoveFilter(Constants.Event.SearchFilters.Keywords);

            //Diagnostics
            Sitecore.Diagnostics.Log.Info("Event Search: Set Keyword Filter called. Showing search filters after set.", this);
            LogSearchFilters();
        }

        public void SetInclusiveDate(string date)
        {
            DateTime dateParsed;
            if (DateTime.TryParse(date, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dateParsed))
            {
                InclusiveDate = dateParsed;
                bIsInclusiveDateSearch = true;
                SetDateFilter(DateTime.MinValue, DateTime.MaxValue);

                //Diagnostics
                Sitecore.Diagnostics.Log.Info("Event Search: Set Inclusive Date Filter called. Showing search filters after set.", this);
                LogSearchFilters();
            }
        }

        private void FetchItems()
        {
            List<Document> events = EventHelper.SearchEvents(searchFilters);

            if ((events != null) && (events.Count > 0))
            {
                oResultDocuments = events.ToArray();
                Sitecore.Diagnostics.Log.Info("Event Search: FetchItems: Result Count = " + oResultDocuments.Length, this);

                if (bIsInclusiveDateSearch)
                {
                    FilterByInclusiveDate(oResultDocuments);
                    bIsInclusiveDateSearch = false;
                    Sitecore.Diagnostics.Log.Info("Event Search: FetchItems: After Inclusive Date Filter, Result Count = " + oResultDocuments.Length, this);
                }

                if (bIsDefaultSort)
                {
                    ApplyDefaultSort(oResultDocuments, new ResultSort(Constants.Event.Indexes.EventsIndex.Fields.Title, true));
                    bIsDefaultSort = false;
                }

                bIsDirty = false;
            }
        }

        internal void FilterByInclusiveDate(Document[] documents)
        {
            DateTime dBeginDate;
            DateTime dEndDate;
            List<Document> inclusiveDateEvents = new List<Document>();

            //Diagnostics
            Sitecore.Diagnostics.Log.Info("Event Search: filtering by inclusive Date: " + InclusiveDate.ToString(), this);

            foreach (Document doc in oResultDocuments)
            {
                EventHelper.GetDateFromString(EventHelper.GetField(doc, Constants.Event.Indexes.EventsIndex.Fields.BeginDate), out dBeginDate);
                EventHelper.GetDateFromString(EventHelper.GetField(doc, Constants.Event.Indexes.EventsIndex.Fields.EndDate), out dEndDate);

                if (InclusiveDate.Date >= dBeginDate.Date && InclusiveDate.Date <= dEndDate.Date)
                {
                    inclusiveDateEvents.Add(doc);
                }
            }

            oResultDocuments = inclusiveDateEvents.ToArray();
        }

        internal void ApplySort(IEnumerable<ResultSort> oSorts)
        {
            ApplySort(oSorts, false);
        }

        internal void ApplySort(IEnumerable<ResultSort> oSorts, bool bApplyAll)
        {
            ResultSort oSort;

            //are we applying all sorts?
            if (bApplyAll)
            {
                //we need to loop over all sorts
                foreach (ResultSort oResultSort in oSorts)
                {
                    ApplySort(oResultSort);
                }
            }
            else
            {
                //get the last sort
                if ((oSort = oSorts.LastOrDefault()) != null)
                {
                    ApplySort(oSort);
                }
            }
        }

        protected void ApplySort(ResultSort oSort)
        {
            IEnumerable<Document> oDocuments;
            Field oField;

            if (oSort.Order)
            {
                oDocuments = ResultDocuments.OrderBy(oDocument => (oField = oDocument.GetField(oSort.Field)) != null ? oField.StringValue : string.Empty);
            }
            else
            {
                oDocuments = ResultDocuments.OrderByDescending(oDocument => (oField = oDocument.GetField(oSort.Field)) != null ? oField.StringValue : string.Empty);
            }
            oResultDocuments = oDocuments.ToArray();
        }

        internal void ApplyDefaultSort(IEnumerable<Document> documents, ResultSort oSort)
        {
            Field oField;

            if (oSort.Order)
            {
                documents = oResultDocuments.OrderBy(oDocument => (oField = oDocument.GetField(oSort.Field)) != null ? oField.StringValue : string.Empty);
            }
            else
            {
                documents = oResultDocuments.OrderByDescending(oDocument => (oField = oDocument.GetField(oSort.Field)) != null ? oField.StringValue : string.Empty);
            }
            oResultDocuments = documents.ToArray();
        }

    }
}
