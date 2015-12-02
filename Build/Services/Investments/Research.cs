using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Genworth.SitecoreExt.Services.Search;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.Threading;
using Sitecore.Data;
using Genworth.SitecoreExt.Helpers;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Genworth.SitecoreExt.Constants;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Genworth.SitecoreExt.Search;

namespace Genworth.SitecoreExt.Services.Investments
{
    /// <summary>
    /// This class encapsulates the concept of Investment Research. 
    /// Users can use filter options to narrow down searches against a Lucene index to arrive at a list of documents.
    /// A user has 1 "sticky" research object that can be accessed via "Research.CurrentResearch".
    /// </summary>
    [DataContract]
    public class Research : InvestmentsSearchBase, IInvestmentsSearch
    {
        BooleanQuery oDefaultQuery;

        public const string CODE = "research";
        public override string Code { get { return CODE; } }

        #region CURRENT RESEARCH ACCESS

        private const string CURRENT_RESEARCH_SESSION_KEY = "Genworth.SitecoreExt.Services.Investments.CurrentResearch";
        public static IInvestmentsSearch Current
        {
            get
            {
                object oObject;
                IInvestmentsSearch oResearch;
                oObject = System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY];
                if (oObject != null && oObject is IInvestmentsSearch)
                {
                    oResearch = (IInvestmentsSearch)oObject;
                }
                else
                {
                    System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = oResearch = new Research();
                }
                return oResearch;
            }
            set
            {
                System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = value;
            }
        }


        #endregion

        #region FILTER SHORT CODE CONSTANTS

        public const string SHORT_CODE_CATEGORY = "ca";
        public const string SHORT_CODE_SOURCE = "so";
        public const string SHORT_CODE_STRATEGIST = "st";
        public const string SHORT_CODE_MANAGER = "ma";
        public const string SHORT_CODE_ALLOCATIONAPPROACH = "al";
        public const string SHORT_CODE_AUDIENCE = "au";

        #endregion

        #region FILTERS

        protected Filter oCategory;
        protected Filter oSource;
        protected Filter oStrategist;
        protected Filter oManager;
        protected Filter oAllocationApproach;
        private Filter oAudience;

        #endregion
        
        #region HELPER OBJECT REFERENCES
       
        public override ResultBase[] GetResults()
        {
            //Execute search 
            var docs = SearchHelper.SearchLuceneIndex(CreateBaseResearchQuery(), Constants.Investments.Indexes.InvestmentsResearchIndex);

            //Get results
            var resultList = docs.Select(oDocument => new Result(oDocument)).ToList();
            var results = resultList.Where(w => !string.IsNullOrEmpty(w.Title) && !string.IsNullOrEmpty(w.Id) && !(string.IsNullOrEmpty(w.Url) && string.IsNullOrEmpty(w.Path))).ToArray();
            return results;
        }

        protected Query GetDatesQuery(DateTime beginDate, DateTime endDate)
        {
            string dateFormat = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Indexing.DataFormat.DateTimeStringFormat, "yyyyMMdd");
            
            TermRangeQuery dateQuery = new TermRangeQuery(Constants.Investments.Indexes.Fields.Date, beginDate.ToString(dateFormat), endDate.ToString(dateFormat), true, true);

            return dateQuery;
        }

        #endregion

        public Research() : base(true) { }
        
        public Research(object oNone) : base(oNone) { }
        
        #region HELPER METHODS


        protected BooleanQuery CreateDocumentExclusionQuery()
        {
            BooleanQuery oQuery = new BooleanQuery();

            oQuery.Add(new MatchAllDocsQuery(), Occur.MUST);

            QueryParser oParser = new QueryParser(LuceneSettings.LuceneVersion, Constants.Investments.Indexes.Fields.Template, new StandardAnalyzer(LuceneSettings.LuceneVersion));
            string sTemplatesIds = Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.RemoveDocumentsFromTemplate).Replace("}", "\\}").Replace("{", "\\{").Replace("|", " ");
            oQuery.Add(oParser.Parse(sTemplatesIds), Occur.MUST_NOT);
            return oQuery;
        }

        protected BooleanQuery CreateBaseResearchQuery()
        {
            //Create Query
            BooleanQuery query = new BooleanQuery();
            query.Add(CreateDocumentExclusionQuery(), Occur.MUST);
            query.Add(SearchHelper.CreateQuery(Constants.Investments.Indexes.Fields.Constant, Constants.Investments.Indexes.Fields.Constant), Occur.MUST);
            query.Add(GetDatesQuery(DateTime.Today.AddYears(-2), DateTime.Today), Occur.MUST);

            return query;
        }

        protected override Filter GetFilter(string sFilter)
        {
            Filter oFilter;
            //which filter are we looking for?
            switch (sFilter.ToLower().Trim())
            {
                case SHORT_CODE_CATEGORY:
                case "category":
                case "categories":
                    oFilter = oCategory;
                    break;
                case SHORT_CODE_SOURCE:
                case "source":
                case "sources":
                    oFilter = oSource;
                    break;
                case SHORT_CODE_STRATEGIST:
                case "strategist":
                case "strategists":
                    oFilter = oStrategist;
                    break;
                case SHORT_CODE_MANAGER:
                case "manager":
                case "managers":
                    oFilter = oManager;
                    break;
                case SHORT_CODE_ALLOCATIONAPPROACH:
                case "allocationapproach":
                case "allocation approach":
                case "allocationapproaches":
                case "allocation approaches":
                    oFilter = oAllocationApproach;
                    break;
                case SHORT_CODE_AUDIENCE:
                case "audience":
                case "audiences":
                    oFilter = oAudience;
                    break;
                default:
                    oFilter = null;
                    break;
            }
            return oFilter;
        }

        public override Filter[] Filters
        {
            get
            {
                oFilters = new List<Filter>();

                //audience is a special case, getting its values directly from the index rather than Sitecore.
                oFilters.Add(oAudience = new Filter(FilterGroup.General, "Audience", SHORT_CODE_AUDIENCE, Constants.Investments.Indexes.Fields.Audience));
                oAudience.AddOption(Constants.Investments.Audiences.Advisor, Constants.Investments.AudienceOptions.Advisor, Constants.Investments.Audiences.Advisor);
                oAudience.AddOption(Constants.Investments.Audiences.Client, Constants.Investments.AudienceOptions.Client, Constants.Investments.Audiences.Client);

                //load the filters list
                oFilters.Add(oCategory = new Filter(FilterGroup.General, "Category", SHORT_CODE_CATEGORY, Constants.Investments.Indexes.Fields.CategoryId, "/sitecore/content/Meta-Data/Lookups/Document/Category/*", oItem => oItem.DisplayName, oItem => oItem.DisplayName, true));
                oFilters.Add(oSource = new Filter(FilterGroup.General, "Source", SHORT_CODE_SOURCE, Constants.Investments.Indexes.Fields.SourceId, "/sitecore/content/Meta-Data/Lookups/Document/Source/*", oItem => oItem.DisplayName, oItem => oItem.DisplayName));
                oFilters.Add(oStrategist = new Filter(FilterGroup.General, "Strategist", SHORT_CODE_STRATEGIST, Constants.Investments.Indexes.Fields.StrategistId, "/sitecore/content/Shared Content/Investments/Strategists/*[@@templatename='Strategist' or @@templatename='Strategist No Allocation']", oItem => oItem.GetText("Name"), oItem => oItem.GetText("Code")));
                oFilters.Add(oManager = new Filter(FilterGroup.General, "Manager", SHORT_CODE_MANAGER, Constants.Investments.Indexes.Fields.ManagerId, "/sitecore/content/Shared Content/Investments/Managers/*[@@templatename='Manager']", oItem => oItem.GetText("Name"), oItem => oItem.GetText("Code")));
                oFilters.Add(oAllocationApproach = new Filter(FilterGroup.General, "Approach", SHORT_CODE_ALLOCATIONAPPROACH, Constants.Investments.Indexes.Fields.AllocationApproachId, "/sitecore/content/Shared Content/Investments/Asset Allocation Approaches/*[@@templatename='Asset Allocation Approach']", oItem => oItem.GetText("Title"), oItem => oItem.GetText("Code"), oItem => oItem.GetText("Display options", "Display on Research Filter").Equals("1")));
                
                oCategory.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.CategoryChecked,string.Empty));
                oSource.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.SourceChecked, string.Empty));
                oStrategist.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.StrategistChecked, string.Empty));
                oManager.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.ManagerChecked, string.Empty));
                oAllocationApproach.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.AllocationApproachChecked, string.Empty));
                oAudience.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.AudienceChecked, string.Empty));

                oCategory.DataField = Constants.Investments.DataFields.Category;
                oSource.DataField = Constants.Investments.DataFields.Source;
                oStrategist.DataField = Constants.Investments.DataFields.Strategist;
                oManager.DataField = Constants.Investments.DataFields.Manager;
                oAllocationApproach.DataField = Constants.Investments.DataFields.AllocationApproach;
                oAudience.DataField = Constants.Investments.DataFields.Audience;

                return oFilters.ToArray();
            }
        }

        public override List<KeyValuePair<string, string>> Columns
        {
            get
            {
                List<KeyValuePair<string, string>> sColumns;

                //set up the columns
                sColumns = new List<KeyValuePair<string, string>>();

                sColumns.Add(new KeyValuePair<string, string>("Title", Constants.Investments.DataFields.Title));
                sColumns.Add(new KeyValuePair<string, string>("Category", Constants.Investments.DataFields.Category));
                sColumns.Add(new KeyValuePair<string, string>("Source", Constants.Investments.DataFields.Source));
                sColumns.Add(new KeyValuePair<string, string>("Strategist", Constants.Investments.DataFields.Strategist));
                sColumns.Add(new KeyValuePair<string, string>("Manager", Constants.Investments.DataFields.Manager));
                sColumns.Add(new KeyValuePair<string, string>("Allocation Approach", Constants.Investments.DataFields.AllocationApproach));
                sColumns.Add(new KeyValuePair<string, string>("Date", Constants.Investments.DataFields.Date));
                return sColumns;
            }
        }

        public override bool SetURLFilters(NameValueCollection oQueryString)
        {
            bool bResult = false;
            string sStrategist;
            string sAllocationApproach;
            string sManager;
            string sCategory;
            string sSource;

            if (!string.IsNullOrEmpty(sStrategist = oQueryString["strategist"])
                | !string.IsNullOrEmpty(sAllocationApproach = oQueryString["allocationapproach"])
                | !string.IsNullOrEmpty(sManager = oQueryString["manager"])
                | !string.IsNullOrEmpty(sCategory = oQueryString["category"])
                | !string.IsNullOrEmpty(sSource = oQueryString["source"]))
            {
                bResult = true;
                //reset the search
                Reset(false);

                if (!string.IsNullOrEmpty(sCategory))
                {
                    //set the parameters
                    SetFilterOption(Research.SHORT_CODE_CATEGORY, sCategory.ToLower(), true);
                }
                if (!string.IsNullOrEmpty(sSource))
                {
                    //set the parameters
                    SetFilterOption(Research.SHORT_CODE_SOURCE, sSource.ToLower(), true);
                }
                if (!string.IsNullOrEmpty(sStrategist))
                {
                    //set the parameters
                    SetFilterOption(Research.SHORT_CODE_STRATEGIST, sStrategist.ToLower(), true);
                }
                if (!string.IsNullOrEmpty(sAllocationApproach))
                {
                    //set the parameters
                    SetFilterOption(Research.SHORT_CODE_ALLOCATIONAPPROACH, sAllocationApproach.ToLower(), true);
                }
                if (!string.IsNullOrEmpty(sManager))
                {
                    //set the parameters
                    SetFilterOption(Research.SHORT_CODE_MANAGER, sManager.ToLower(), true);
                }
            }
            return bResult;
        }

        public override string URL
        {
            get
            {
                return ServerLogic.SitecoreExt.ContextExtension.CurrentItem.GetURL(); //Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.GetURL();
            }

        }

        #endregion

        public override void Reset(bool bUseDefaultValues)
        {
            //we want to be sure we do not update filter availability while we are resetting data (saves cycles)
            bAutoUpdateFilterAvailability = false;

            base.Reset(bUseDefaultValues);
            //set default date range
            //the default date is different from a search in the page or when the search was made from another page
            //based on parameters
            Months = bUseDefaultValues ? Constants.Investments.DefaultMonthRange : Constants.Investments.DefaultMonthRangeFromExternalLink;
            //we are done resetting our data, turn on the auto-update
            bAutoUpdateFilterAvailability = true;
            //update availability
            AutoUpdateFilterAvailability();
        }

    }
}
