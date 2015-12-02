using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genworth.SitecoreExt.Constants;
using ServerLogic.SitecoreExt;
using Lucene.Net.Search;
using Genworth.SitecoreExt.Helpers;
namespace Genworth.SitecoreExt.Services.Investments
{
    public class ResearchClientView : Research
    {
        Query oAudienceQuery;
        public new const string CODE = "researchclientview";
        public override string Code { get { return CODE; } }

        #region CURRENT RESEARCH ACCESS

        private const string CURRENT_RESEARCH_SESSION_KEY = "Genworth.SitecoreExt.Services.Investments.CurrentResearchClientView";
        public static IInvestmentsSearch CurrentClientView
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
                    System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = oResearch = new ResearchClientView();
                }
                return oResearch;
            }
            set
            {
                System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = value;
            }
        }


        #endregion

        public ResearchClientView()
            : base(null)
        {
           
        }

        public override Filter[] Filters
        {
            get
            {
                oFilters = new List<Filter>();

                //load the filters list
                oFilters.Add(oCategory = new Filter(FilterGroup.General, "Category", SHORT_CODE_CATEGORY, Constants.Investments.Indexes.Fields.CategoryId, "/sitecore/content/Meta-Data/Lookups/Document/Category/*", oItem => oItem.DisplayName, oItem => oItem.DisplayName, false));
                oFilters.Add(oSource = new Filter(FilterGroup.General, "Source", SHORT_CODE_SOURCE, Constants.Investments.Indexes.Fields.SourceId, "/sitecore/content/Meta-Data/Lookups/Document/Source/*", oItem => oItem.DisplayName, oItem => oItem.DisplayName));
                oFilters.Add(oStrategist = new Filter(FilterGroup.General, "Strategist", SHORT_CODE_STRATEGIST, Constants.Investments.Indexes.Fields.StrategistId, "/sitecore/content/Shared Content/Investments/Strategists/*[@@templatename='Strategist' or @@templatename='Strategist No Allocation']", oItem => oItem.GetText("Name"), oItem => oItem.GetText("Code")));
                oFilters.Add(oManager = new Filter(FilterGroup.General, "Manager", SHORT_CODE_MANAGER, Constants.Investments.Indexes.Fields.ManagerId, "/sitecore/content/Shared Content/Investments/Managers/*[@@templatename='Manager']", oItem => oItem.GetText("Name"), oItem => oItem.GetText("Code")));
                oFilters.Add(oAllocationApproach = new Filter(FilterGroup.General, "Allocation Approach", SHORT_CODE_ALLOCATIONAPPROACH, Constants.Investments.Indexes.Fields.AllocationApproachId, "/sitecore/content/Shared Content/Investments/Asset Allocation Approaches/*[@@templatename='Asset Allocation Approach']", oItem => oItem.GetText("Title"), oItem => oItem.GetText("Code")));

                //audience is a special case, getting its values directly from the index rather than Sitecore.

                oCategory.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.ClientView.Research.CategoryChecked, string.Empty));
                oSource.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.ClientView.Research.SourceChecked, string.Empty));
                oStrategist.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.ClientView.Research.StrategistChecked, string.Empty));
                oManager.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.ClientView.Research.ManagerChecked, string.Empty));
                oAllocationApproach.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.ClientView.Research.AllocationApproachChecked, string.Empty));

                oCategory.DataField = Constants.Investments.DataFields.Category;
                oSource.DataField = Constants.Investments.DataFields.Source;
                oStrategist.DataField = Constants.Investments.DataFields.Strategist;
                oManager.DataField = Constants.Investments.DataFields.Manager;
                oAllocationApproach.DataField = Constants.Investments.DataFields.AllocationApproach;
            
                return oFilters.ToArray();
            }
        }

        private Query BuildInvestmentResearchQuery()
        {
            List<InvestmentHelper.Strategist> oStrategists;
            List<Sitecore.Data.Items.Item> oManagers;
            List<InvestmentHelper.Strategist> oAdditionalStrategists;

            oAudienceQuery = SearchHelper.CreateBooleanQuery(Occur.MUST, SearchHelper.CreateQuery(Constants.Investments.Indexes.Fields.Audience, Constants.Investments.Audiences.Client));

            Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.GetManagersAndStrategists(out oStrategists, out oManagers, out oAdditionalStrategists);

            List<string> allowedStrategists = new List<string>();
            List<string> allowedManagers = new List<string>();

            oStrategists.ForEach(strategist => allowedStrategists.Add(strategist.Item.ID.ToString()));
            oManagers.ForEach(manager => allowedManagers.Add(manager.ID.ToString()));
            oAdditionalStrategists.ForEach(additional => allowedStrategists.Add(additional.Item.ID.ToString()));

            allowedStrategists = allowedStrategists.Distinct().ToList();
            allowedManagers = allowedManagers.Distinct().ToList();

            List<Query> strategistsQueries = new List<Query>();
            List<Query> managersQueries = new List<Query>();
            List<Query> allQueries = new List<Query>();

            if (allowedStrategists.Count > 0)
            {
                allowedStrategists.ForEach(strategist => strategistsQueries.Add(
                SearchHelper.CreateBooleanQuery(Occur.MUST, oAudienceQuery,
                SearchHelper.CreateQuery(Constants.Investments.Indexes.Fields.StrategistId, strategist))));
                allQueries.AddRange(strategistsQueries);
            }
            if (allowedManagers.Count > 0)
            {
                allowedManagers.ForEach(manager => managersQueries.Add(
                SearchHelper.CreateBooleanQuery(Occur.MUST, oAudienceQuery,
                SearchHelper.CreateQuery(Constants.Investments.Indexes.Fields.ManagerId, manager))));
                allQueries.AddRange(managersQueries);
            }

            Query mainQuery = SearchHelper.CreateBooleanQuery(Occur.SHOULD, allQueries.ToArray());

            return mainQuery;
        }


        public override ResultBase[] GetResults()
        {
            //Create Query
            BooleanQuery query = CreateBaseResearchQuery();
  
            // Strategist & Managers filter            
            query.Add(BuildInvestmentResearchQuery(), Occur.MUST);

            //Execute search 
            var docs = SearchHelper.SearchLuceneIndex(query, Constants.Investments.Indexes.InvestmentsResearchIndex);

            //Get results
            var resultList = docs.Select(oDocument => new Result(oDocument)).ToList();
            var results = resultList.Where(w => !string.IsNullOrEmpty(w.Title) && !string.IsNullOrEmpty(w.Id) && !(string.IsNullOrEmpty(w.Url) && string.IsNullOrEmpty(w.Path))).ToArray();
            return results;
        }
    }
}
