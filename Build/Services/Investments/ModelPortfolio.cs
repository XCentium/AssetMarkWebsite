using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServerLogic.SitecoreExt;
using System.Threading.Tasks;
using Genworth.SitecoreExt.Helpers;
using Lucene.Net.Documents;
using System.Web;
using Genworth.SitecoreExt.Constants;
using ServerLogic.SitecoreExt;

namespace Genworth.SitecoreExt.Services.Investments
{
    [DataContract]
    public class ModelPortfolio : InvestmentsSearchBase, IInvestmentsSearch
    {
        private const string CURRENT_RESEARCH_SESSION_KEY = "Genworth.SitecoreExt.Services.Investments.ModelPortfolio";
        public const string CODE = "modelportfolio";

        public override string Code { get { return CODE; } }
        private ModelPortfolioResult[] oResultData;
        private ModelPortfolioResult[] oServiceResultData;
        private ModelPortfolioResult[] oFilteredServiceResultData;
        Task<List<ModelPortfolioResult>> oExtrernaldata;
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
                    ModelPortfolio obj = new ModelPortfolio();
                    System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = oResearch = obj;

                }
                return oResearch;
            }
            set
            {
                System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = value;
            }
        }


        protected override Filter GetFilter(string sFilter)
        {
            Filter oFilter;
            //which filter are we looking for?
            switch (sFilter.ToLower().Trim())
            {
                case SHORT_CODE_CUSTODIANS:
                    oFilter = oCustodians;
                    break;
                case SHORT_CODE_SOLUTION_TYPE:
                    oFilter = oSolution_Types;
                    break;
                default:
                    oFilter = null;
                    break;
            }
            return oFilter;
        }

        public override List<KeyValuePair<string, string>> Columns
        {
            get
            {
                List<KeyValuePair<string, string>> sColumns;

                //set up the columns
                sColumns = new List<KeyValuePair<string, string>>();
                sColumns.Add(new KeyValuePair<string, string>("Title", Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.Title));
                sColumns.Add(new KeyValuePair<string, string>("Strategist", Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.Strategist));
                sColumns.Add(new KeyValuePair<string, string>("Solution Type", Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.SolutionType));
                sColumns.Add(new KeyValuePair<string, string>("Custodian", Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.Custodian));
                sColumns.Add(new KeyValuePair<string, string>("Date", Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.Date));
                sColumns.Add(new KeyValuePair<string, string>("Download", "Download"));

                return sColumns;
            }
        }

        public override bool SetURLFilters(System.Collections.Specialized.NameValueCollection oQueryString)
        {
            return false;
        }

        public override string URL
        {
            get { throw new NotImplementedException(); }
        }
        public static ModelPortfolio CurrentModelPortfolio
        {
            get
            {
                object oObject;
                ModelPortfolio oResearch;
                oObject = System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY];
                if (oObject != null && oObject is Research)
                {
                    oResearch = (ModelPortfolio)oObject;
                }
                else
                {
                    System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = oResearch = new ModelPortfolio();
                }
                return oResearch;
            }
            set
            {
                System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = value;
            }
        }
        #region FILTER SHORT CODE CONSTANTS

        public const string SHORT_CODE_CUSTODIANS = "cu";
        public const string SHORT_CODE_SOLUTION_TYPE = "st";

        #endregion
        #region FILTERS
        private Filter oCustodians;
        private Filter oSolution_Types;




        #endregion
        public ModelPortfolio()
            : base(true)
        {
            oSearch = new Search(oItemCache, true, SearchHelper.CreateQuery(Constants.Investments.Indexes.Fields.Template, Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.Research.RemoveDocumentsFromTemplate).Replace("}", "\\}").Replace("{", "\\{").Replace("|", " ").Replace("-", ""), false));
            oSearch.KeywordFields = new string[] { Constants.Investments.Indexes.Fields.Title, Constants.Investments.Indexes.Fields.Custodian, Constants.Investments.Indexes.Fields.Strategist, Constants.Investments.Indexes.Fields.SolutionType };
            //force to call the static constructor before enter to the thread.
            object scustodians = ModelPortfolioResult.CustodiansList;
            //load the filters list
            oFilters.Add(oCustodians = new Filter(FilterGroup.General, "Custodians", SHORT_CODE_CUSTODIANS, Constants.Investments.Indexes.Fields.CustodianId, "/sitecore/content/Meta-Data/Security/Custodians/*", oItem => oItem.DisplayName, oItem => oItem.GetText("Code")));
            oFilters.Add(oSolution_Types = new Filter(FilterGroup.General, "Solution Type", SHORT_CODE_SOLUTION_TYPE, Constants.Investments.Indexes.Fields.SolutionTypeId, "/sitecore/content/Meta-Data/Solution Types/*", oItem => oItem.DisplayName, oItem => oItem.GetText("Code")));

            //Get Dynamic documents
            HttpContext oContext = HttpContext.Current;
            oExtrernaldata = new Task<List<ModelPortfolioResult>>(() =>
            {
                string sSetting = Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.ModelPortfolio.AllowedCustodiansForDynamicDocuments);
                HttpContext.Current = oContext;
                string[] sCustodians = string.IsNullOrWhiteSpace(sSetting) ? oCustodians.Options.Select(oCustodian => oCustodian.Code).ToArray() : sSetting.Split(',');
                sSetting = Sitecore.Configuration.Settings.GetSetting(Settings.Pages.Investments.ModelPortfolio.AllowedSolutionsForDynamicDocuments);
                //change request 16670
                string[] sSolution_Type = string.IsNullOrWhiteSpace(sSetting) ? oSolution_Types.Options.Select(oCustodian => oCustodian.Code).ToArray() : sSetting.Split(',');
                //oSolution_Types.Options.Select(oSolution_Type => oSolution_Type.Code).ToArray();
                return InvestmentHelper.GetModelPortfolioDocuments(sCustodians, sSolution_Type);
                /*List<ModelPortfolioResult> result = new List<ModelPortfolioResult>();
                for (int i=0;i<10 ;i++)
                    result.Add(new ModelPortfolioResult(string.Format("test {0}", i), "", "GSAM", DateTime.Today.ToShortDateString(), "FID", "DS"));
                return result;*/

            });


            oExtrernaldata.Start();
            //Check defaul values for solution Types
            oSolution_Types.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.ModelPorfolio.SolutionTypesChecked, ""));
            oCustodians.SetDefaultValues(Sitecore.Configuration.Settings.GetSetting(Settings.Pages.ModelPorfolio.CustodiansChecked, ""));

            //remove date
            Months = -1;
            bAutoUpdateFilterAvailability = false;

            UpdateFilterAvailability(true);
        }

        public override Filter[] Filters
        {
            get { return oFilters.ToArray(); }
        }

        private ModelPortfolioResult[] ExternalData
        {
            get
            {
                if (oServiceResultData == null)
                {
                    oServiceResultData = oExtrernaldata.Result.ToArray();
                    oFilteredServiceResultData = oServiceResultData;
                }
                return oServiceResultData;
            }
        }
        private void GetAvilableFilters()
        {

        }
        public override ResultBase[] GetResults()
        {
            FetchData();
            //is sort dirty?
            if (bIsSortDirty)
            {
                //set sorts
                Sort(oSorts);

                //sort is no longer dirty
                bIsSortDirty = false;
            }
            //return the paged results
            return (iResultsPerPage == -1 ? oResultData : oResultData.Skip(iResultsPerPage * iPage).Take(iResultsPerPage)).ToArray();
        }
        [DataMember(Name = "ResultCount", Order = 5)]
        protected override int ResultCount { get { FetchData(); return oResultData != null ? oResultData.Count() : 0; } set { } }
        protected override void Sort(List<ResultSort> oSorts)
        {
            IEnumerable<ModelPortfolioResult> oResults;
            foreach (ResultSort oSort in oSorts)
            {
                if (oSort.Order)
                {
                    oResults = oResultData.OrderBy(oResult => oResult.GetSortableField(oSort.Field));
                }
                else
                {
                    oResults = oResultData.OrderByDescending(oResult => oResult.GetSortableField(oSort.Field));
                }
                oResultData = oResults.ToArray();
            }
        }






        protected override void UpdateFilterAvailability(bool IsfirtTime = true)
        {
            // Quick fix no filter bar, updateFilterAvailability is not required
            /*
            FetchData();
            base.UpdateFilterAvailability(false);
            HashSet<string> ohCustodians = new HashSet<string>();
            HashSet<string> ohPrograms = new HashSet<string>();
            foreach (ModelPortfolioResult oResult in oFilteredServiceResultData)
            {
                if (!ohCustodians.Contains(oResult.CustodianCode))
                {
                    ohCustodians.Add(oResult.CustodianCode);
                }
                if (!ohPrograms.Contains(oResult.SolutionTypeCode))
                {
                    ohPrograms.Add(oResult.SolutionTypeCode);
                }
            }
            oCustodians.SetAvailableValue(ohCustodians);
            oSolution_Types.SetAvailableValue(ohPrograms);
            oFilters.AsParallel().ForAll(oFilter => oFilter.ApplyAvailableValues());
            */

        }
        private void FetchData()
        {
            if (oSearch.IsDirty)
            {
                ModelPortfolioResult[] oLocalDocuments;

                oLocalDocuments = oSearch.ResultDocuments.Select(oDocument => new ModelPortfolioResult(oDocument)).ToArray();
                if (oSearch.IsKeywordSearch && !string.IsNullOrWhiteSpace(oSearch.Keyword))
                {

                    KeywordSeachDynamicData(oSearch.Keyword);

                }
                else
                    FilterDynamicData();
                oSearch.IsKeywordSearch = false;
                oResultData = oLocalDocuments.Concat(oFilteredServiceResultData).ToArray();
                bIsSortDirty = true;
            }
        }
        public override void Reset(bool bUseDefaultValues)
        {

            //we want to be sure we do not update filter availability while we are resetting data (saves cycles)
            bAutoUpdateFilterAvailability = false;
            oFilteredServiceResultData = ExternalData;
            base.Reset(bUseDefaultValues);
            Months = -1;
            //we are done resetting our data, turn on the auto-update
            bAutoUpdateFilterAvailability = true;

            //update availability
            UpdateFilterAvailability(true);
        }
        private void FilterDynamicData()
        {
            string[] sSolutionTypesFiltered = oSolution_Types.Options.Where(oFilter => oFilter.Available && oFilter.Filtered).Select(oFilter => oFilter.Code).ToArray();
            string[] sCustodians = oCustodians.Options.Where(oFilter => oFilter.Available && oFilter.Filtered).Select(oFilter => oFilter.Code).ToArray();
            if (sSolutionTypesFiltered.Count() == 0 && sCustodians.Count() == 0)
            {
                oFilteredServiceResultData = ExternalData;
            }
            else
                oFilteredServiceResultData = ExternalData.AsParallel().Where(oExternalResult => sCustodians.Contains(oExternalResult.CustodianCode) || sSolutionTypesFiltered.Contains(oExternalResult.SolutionTypeCode)).ToArray();

        }
        private void KeywordSeachDynamicData(string sKeyword)
        {
            oFilteredServiceResultData = ExternalData.AsParallel().Where(oExternalResult =>
            {
                if (oExternalResult.Title.IndexOf(sKeyword, StringComparison.OrdinalIgnoreCase) > -1 ||
                    oExternalResult.Srategist.LastIndexOf(sKeyword, StringComparison.OrdinalIgnoreCase) > -1 ||
                    oExternalResult.Custodian.LastIndexOf(sKeyword, StringComparison.OrdinalIgnoreCase) > -1 ||
                    oExternalResult.SolutionType.LastIndexOf(sKeyword, StringComparison.OrdinalIgnoreCase) > -1)
                    return true;
                else
                    return false;
            }).ToArray();
        }
    }
}
