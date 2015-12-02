using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Lucene.Net.Documents;
using System.Web;
using Lucene.Net.Search;
using Genworth.SitecoreExt.Providers;

namespace Genworth.SitecoreExt.Services.Events
{
    [DataContract]
    public abstract class EventsBase : IJsonCollectionProvider
    {
        public EventsBase()
        {
            this.Columns = new List<KeyValuePair<string, string>>(this.GetColumns());
            oSorts = new List<ResultSort>();
            oSorts.Add(new ResultSort(Constants.Event.Indexes.EventsIndex.Fields.Title, true));
        }

        protected EventSearch oSearch;
        protected bool bAutoUpdateFilterAvailability;

        private void SetDateFilterChanged()
        {
            //set the date filter
            oSearch.SetDateFilter(dFromDate, dToDate);

            //move to page 1
            iPage = 0;

            //update the filter availability
            AutoUpdateFilterAvailability();

            //sort is dirty
            bIsSortDirty = true;
        }

        [DataMember(Name = "FromDate")]
        public string FromDate
        {
            get { return dFromDate != DateTime.MinValue ? dFromDate.ToString(Constants.Event.DateFormat) : string.Empty; }
            set
            {
                if (!DateTime.TryParse(value, out dFromDate))
                {
                    dFromDate = DateTime.MinValue;
                }
            }
        }
        protected DateTime dFromDate;

        [DataMember(Name = "ToDate")]
        public string ToDate
        {
            get { return dToDate != DateTime.MaxValue ? dToDate.ToString(Constants.Event.DateFormat) : string.Empty; }
            set
            {
                if (!DateTime.TryParse(value, out dToDate))
                {
                    dToDate = DateTime.MaxValue;
                }

                //set the date filter changed
                SetDateFilterChanged();
            }
        }
        protected DateTime dToDate;

        #region SORTING

        [DataMember(Name = "Sort", Order = 8)]
        protected List<ResultSort> oSorts;

        protected bool bIsSortDirty;

        #endregion

        [DataMember(Name = "ResultCount")]
        protected virtual int ResultCount { get { return oSearch != null ? oSearch.ResultDocuments.Count() : 0; } set { } }

        [DataMember(Name = "ResultsPerPage")]
        protected int iResultsPerPage = Constants.Event.DefaultResultsPerPage;
        public int ResultsPerPage
        {
            set
            {
                int iTemp;

                //put the value in a temp
                iTemp = value;

                //a negative 1 means view all... if not negative 1, we must set a minimum
                if (iTemp != -1)
                {
                    //set minimum
                    iTemp = Math.Max(iTemp, Constants.Event.MinResultsPerPage);
                }

                //are we actually changing the per page?
                if (iTemp != iResultsPerPage)
                {
                    //set the results per page
                    iResultsPerPage = iTemp;

                    //move to page 1
                    iPage = 0;
                }
            }
        }

        [DataMember(Name = "Page")]
        protected int iPage;
        public int Page
        {
            set
            {
                int iTemp;

                //put the value in a temp
                iTemp = value;

                //is the page number too high?
                if (iTemp < 0 || iTemp > (ResultCount / iResultsPerPage))
                {
                    //the page is outside of range
                    iTemp = 0;
                }

                //set the page to the temp value
                iPage = iTemp;
            }
        }

        [DataMember(Name = "KeywordSearches")]
        protected List<string> oKeywordSearches = new List<string>();

        public abstract string Code
        {
            get;
        }

        public List<KeyValuePair<string, string>> Columns { get; private set; }

        protected abstract IEnumerable<KeyValuePair<string, string>> GetColumns();

        public abstract EventDataItem[] GetResults();

        public virtual bool SetURLFilters(System.Collections.Specialized.NameValueCollection oQueryString)
        {
            return false;
        }

        public virtual string URL
        {
            get { return string.Empty; }
        }

        public string JsonServiceUrl
        {
            get { return "/services/Events.svc/"; }
        }

        protected void AutoUpdateFilterAvailability()
        {
            if (bAutoUpdateFilterAvailability)
            {
                UpdateFilterAvailability();
            }
        }

        protected virtual void UpdateFilterAvailability(bool ApplyValues = true)
        {
            Document[] oDocuments = oSearch.ResultDocuments;
        }

        protected virtual void SetDefaultDateRange() { }

        protected virtual void SetSearchDateRange() { }

        public virtual void SetInclusiveDate(string date)
        {
            oSearch.SetInclusiveDate(date);
        }

        public virtual void Reset(bool bUseDefaultValues)
        {
            //reset the search
            oSearch.Reset();

            //set the keywords
            SetKeywords(string.Empty);
            SetLocation("0", "");

            //SetFilters();
            //clear the sorts
            oSorts.Clear();
            oSorts.Add(new ResultSort(Constants.Event.Indexes.EventsIndex.Fields.Title, true));

            //sorting is dirty by default
            bIsSortDirty = true;
        }

        internal void Sort(string sField)
        {
            ResultSort oSort;
            
            string[] fields = sField.Split(',');
            foreach (string field in fields)
            {
                oSort = oSorts.Where(oItem => oItem.Field.Equals(field)).FirstOrDefault();

                //is the sort null?
                if (oSort != null)
                {
                    //remove the sort
                    oSorts.Remove(oSort);

                    //add the sort
                    oSorts.Add(oSort);

                    //set the sort
                    oSort.Order = !oSort.Order;
                }
                else
                {
                    //create the sort
                    oSorts.Add(new ResultSort(field, true));
                }
            }

            Sort(oSorts);
        }

        protected virtual void Sort(List<ResultSort> oSorts)
        {
            //apply the sorts
            oSearch.ApplySort(oSorts);
        }

        internal void SetKeywords(string sKeywords)
        {
            //does the keyword search already contain these keywords?
            if (oKeywordSearches.Contains(sKeywords))
            {
                //remove the existing keywords
                oKeywordSearches.Remove(sKeywords);
            }

            if (!string.IsNullOrWhiteSpace(sKeywords))
            {
                //add the search
                oKeywordSearches.Insert(0, sKeywords);

                //Set search date ranges filter
                SetSearchDateRange();
            }
            else
            {
                // use default date range when setting no keywords
                SetDefaultDateRange();
            }

            //sort is dirty
            bIsSortDirty = true;

            //move to first page
            iPage = 0;            

            //tell the search to filter on keywords
            oSearch.SetKeywordFilter(sKeywords);
        }

        internal void SetLocation(string sRatio, string sZipCode)
        {
            //sort is dirty
            bIsSortDirty = true;

            //move to first page
            iPage = 0;

            double dRatio = 0;
            Double.TryParse(sRatio, out dRatio);

            if (!string.IsNullOrWhiteSpace(sZipCode))
            {
                //Use search date ranges filter
                SetSearchDateRange();
            }
            else
            {
                // use default date range when setting no zip code
                SetDefaultDateRange();
            }

            //tell the search to filter on keywords
            oSearch.SetLocation(dRatio, sZipCode);
        }
    }
}
