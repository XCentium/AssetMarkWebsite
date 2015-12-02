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

namespace Genworth.SitecoreExt.Services.Investments
{
    public class ManagerSearch
    {
        #region CURRENT RESEARCH ACCESS

        private const string CURRENT_RESEARCH_SESSION_KEY = "Genworth.SitecoreExt.Services.Investments.CurrentManagerSearch";

        public static ManagerSearch CurrentManagerSearch
        {
            get
            {
                object oObject;
                ManagerSearch oManagerSearch;
                oObject = System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY];
                if (oObject != null && oObject is ManagerSearch)
                {
                    oManagerSearch = (ManagerSearch)oObject;
                }
                else
                {
                    System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = oManagerSearch = new ManagerSearch();
                }
                return oManagerSearch;
            }
            set
            {
                System.Web.HttpContext.Current.Session[CURRENT_RESEARCH_SESSION_KEY] = value;
            }
        }

        #endregion

        #region SORTING

        private List<ResultSort> oSorts;

        private bool bIsSortDirty;

        #endregion

        #region HELPER OBJECT REFERENCES

        private Search oSearch;

        private ItemCache oItemCache;

        #endregion

        public ManagerSearch()
        {
            //create the sorts
            oSorts = new List<ResultSort>();

            //creaet an item cache
            oItemCache = new ItemCache();

            //create a new search object
            oSearch = new Search(oItemCache);
        }

        public Result[] GetResults()
        {
            //is sort dirty?
            if (bIsSortDirty)
            {
                //set sorts
                oSearch.ApplySort(oSorts, true);

                //sort is no longer dirty
                bIsSortDirty = false;
            }

            //return the paged results
            return oSearch.ResultDocuments.Select(oDocument => new Result(oDocument)).ToArray();
        }

        public void SetTemplate(string sTemplate)
        {
            oSearch.SetTemplate(sTemplate);
            bIsSortDirty = true;
        }

        public void Sort(string sField)
        {
            ResultSort oSort;

            //get the existing sort
            oSort = oSorts.Where(oItem => oItem.Field.Equals(sField)).FirstOrDefault();

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
                oSorts.Add(new ResultSort(sField, true));
            }

            //apply the sorts
            oSearch.ApplySort(oSorts);
        }

        #region HELPER CLASSES

        public class ItemCache : Dictionary<string, bool> { }

        public class Search
        {
            private BooleanQueryContract oQuery;
            private bool bIsDirty;
            private Document[] oResultDocuments;
            private ItemCache oItemCache;
            private bool bBypassItems;
            private static Regex oLuceneSpecialCharacters = new Regex("[\\\\!\\(\\)\\:\\^\\]\\{\\}\\~\\?]");

            public Document[] ResultDocuments
            {
                get
                {
                    //have we changed our data enough to be dirty?
                    if (bIsDirty)
                    {
                        //refetch the items from index
                        FetchItems(bBypassItems);
                    }

                    //return the results (never null)
                    return oResultDocuments ?? new Document[] { };
                }
            }

            public Search() : this(new ItemCache()) { }

            public Search(ItemCache oItemCache) : this(oItemCache, false) { }

            public Search(ItemCache oItemCache, bool bBypassItems)
            {
                this.oItemCache = oItemCache;
                this.bBypassItems = bBypassItems;
                Reset();
            }

            public void Reset()
            {
                //create a query that we will use to search for results
                oQuery = new BooleanQueryContract();
                oQuery.Clauses = new List<BooleanClauseContract>();

                //we are dirty
                bIsDirty = true;
            }

            internal void SetTemplate(string sTemplate)
            {
                BooleanClauseContract oClause;

                //do we currently have clauses?
                if (oQuery.Clauses.Count > 0)
                {
                    //clear the clauses
                    oQuery.Clauses.Clear();
                }

                //loop over options
                oClause = new BooleanClauseContract();
                oClause.MultiFieldQuery = new MultiFieldQueryContract();

                //set up the field specific search
                if (!string.IsNullOrEmpty(sTemplate))
                {
                    oClause.MultiFieldQuery.SearchCriteria = sTemplate.Replace("}", "\\}").Replace("{", "\\{").Replace("-","");
                    oClause.MultiFieldQuery.Fields = new string[] { Constants.Investments.Indexes.Fields.Template };
                }
                else
                {
                    oClause.MultiFieldQuery.SearchCriteria = Constants.Investments.Indexes.Fields.Constant;
                    oClause.MultiFieldQuery.Fields = new string[] { Constants.Investments.Indexes.Fields.Constant };
                }

                //finish up the query construction
                oClause.MultiFieldQuery.DefaultOperator = QueryParser.Operator.OR.ToString();
                oClause.Occur = Occur.MUST.ToString();
                oQuery.Clauses.Add(oClause);

                //we are dirty
                bIsDirty = true;
            }

            private void FetchItems(bool bBypassItems)
            {
                List<Document> oDocuments;
                List<Document> oSearchDocuments;
                Field oIdField;
                string sItemId;
                bool bPermitted;

                //create a list of result objects
                oDocuments = new List<Document>();

                //get documents from search
                oSearchDocuments = SearchHelper.SearchLuceneIndex(oQuery, Constants.Investments.Indexes.InvestmentManagerStrategiesIndex);

                if (!bBypassItems)
                {
                    //do we have documents?
                    if (oSearchDocuments != null)
                    {
                        //loop over the documents
                        foreach (Document oDocument in oSearchDocuments)
                        {
                            //get the id field
                            if ((oIdField = oDocument.GetField(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.Id)) != null && !string.IsNullOrEmpty(sItemId = oIdField.StringValue))
                            {
                                //lock on the item cache
                                lock (oItemCache)
                                {
                                    //is this item in the item cache already?
                                    if (oItemCache.ContainsKey(sItemId))
                                    {
                                        //get the item
                                        bPermitted = oItemCache[sItemId];
                                    }
                                    else
                                    {

                                        //get the item from Sitecore
                                        bPermitted = ContextExtension.CurrentDatabase.GetItem(ItemPointer.Parse(sItemId).ItemID) != null;

                                        //add to the list
                                        oItemCache.Add(sItemId, bPermitted);
                                    }
                                }

                                //we only want to use this item if it is non-null
                                if (bPermitted)
                                {
                                    //add teh document to hte list
                                    oDocuments.Add(oDocument);
                                }
                            }
                        }
                    }

                    //store the results
                    oResultDocuments = oDocuments.ToArray();

                    //turn off dirty flag
                    bIsDirty = false;
                }
                else
                {
                    oResultDocuments = oSearchDocuments.ToArray();
                }
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

            internal void ApplySort(ResultSort oSort)
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
        }


        [DataContract]
        public class Result
        {
            private string sId;
            [DataMember(Name = "Path", Order = 1)]
            private string sPath;
            [DataMember(Name = "Template", Order = 2)]
            private string sTemplate;
            [DataMember(Name = "Title", Order = 3)]
            private string sTitle;
            [DataMember(Name = "Group", Order = 4)]
            private string sGroup;
            [DataMember(Name = "Manager", Order = 5)]
            private string sManager;
            [DataMember(Name = "ManagerId", Order = 6)]
            private string sManagerId;
            [DataMember(Name = "Style", Order = 7)]
            private string sStyle;
            [DataMember(Name = "StylePrefix", Order = 8)]
            private string sStylePrefix;
            [DataMember(Name = "StyleSuffix", Order = 9)]
            private string sStyleSuffix;
            [DataMember(Name = "IMA", Order = 10)]
            private string sIMA;
            [DataMember(Name = "ManagerSelect", Order = 11)]
            private string sManagerSelect;

            #region READ ONLY PROPERTIES

            public string Id { get { return sId; } }
            public string Path { get { return sPath; } }
            public string Template { get { return sTemplate; } }
            public string Title { get { return sTitle; } }
            public string Group { get { return sGroup; } }
            public string Manager { get { return sManager; } }
            public string ManagerId { get { return sManagerId; } }
            public string Style { get { return sStyle; } }
            public string StylePrefix { get { return sStylePrefix; } }
            public string StyleSuffix { get { return sStyleSuffix; } }
            public string IMA { get { return sIMA; } }
            public string ManagerSelect { get { return sManagerSelect; } }


            #endregion

            public Result(Document oDocument)
            {
                Field oField;
                int iIndex;

                //set the fields
                sId = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Id)) != null ? oField.StringValue : string.Empty;
                sPath = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Path)) != null ? oField.StringValue : string.Empty;
                sTemplate = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Template)) != null ? oField.StringValue : string.Empty;
                sTitle = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Title)) != null ? oField.StringValue : string.Empty;
                sGroup = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Group)) != null ? oField.StringValue : string.Empty;
                sManager = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Manager)) != null ? oField.StringValue : string.Empty;
                sManagerId = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.ManagerId)) != null ? oField.StringValue : string.Empty;
                sStyle = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Style)) != null ? oField.StringValue : string.Empty;
                if ((iIndex = sStyle.IndexOf("-")) > 0)
                {
                    sStylePrefix = sStyle.Substring(0, iIndex).Trim();
                    sStyleSuffix = sStyle.Substring(iIndex + 1).Trim();
                }
                else
                {
                    sStylePrefix = sStyle;
                    sStyleSuffix = string.Empty;
                }
                sIMA = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.IMA)) != null ? oField.StringValue : string.Empty;
                sManagerSelect = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.ManagerSelect)) != null ? oField.StringValue : string.Empty;
            }
        }

        /// <summary>
        /// The resultsort keeps track of the fields being sorted, their order and wether they are ascending or descending.
        /// </summary>
        [DataContract]
        public class ResultSort
        {
            [DataMember(Name = "Field", Order = 1)]
            private string sField;
            [DataMember(Name = "Order", Order = 2)]
            private bool bOrder;

            public string Field { get { return sField; } }
            public bool Order { set { bOrder = value; } get { return bOrder; } }

            public ResultSort(string sField, bool bOrder)
            {
                this.sField = sField;
                this.bOrder = bOrder;
            }
        }

        #endregion

        [DataContract]
        public class Strategy
        {
            [DataMember(Name = "Manager", Order = 1)]
            private string sManager;
            [DataMember(Name = "Strategy", Order = 2)]
            private string sStrategy;
            [DataMember(Name = "Information", Order = 3)]
            private string sInformation;
            [DataMember(Name = "Dimensions", Order = 4)]
            private List<Dimension> oDimensions;

            internal Strategy(Item oItem)
            {
                Item oTemp;

                //is our item non-null?
                if (oItem != null)
                {
                    //get manager for strategy
                    if ((oTemp = oItem.Parent) != null && oTemp.InstanceOfTemplate("Manager"))
                    {
                        //set manager name
                        sManager = oTemp.GetText("Manager", "Name");
                    }

                    //set the information
                    sStrategy = oItem.GetText("Strategy", "Title");
                    sInformation = oItem.GetText("Strategy", "Information");

                    //create our dimensions
                    oDimensions = new List<Dimension>();

                    //set up the dimensions
                    switch (oItem.TemplateID.ToString())
                    {
                        case Constants.Investments.Templates.ManagerStrategies.FixedIncome:
                            oDimensions.Add(new Dimension(Constants.Investments.Queries.StrategyFixedIncomeStatus, oItem.GetListItem("Strategy", "Status")));
                            oDimensions.Add(new Dimension(Constants.Investments.Queries.StrategyFixedIncomeStyle, oItem.GetListItem("Strategy", "Style")));
                            break;
                        case Constants.Investments.Templates.ManagerStrategies.InternationalGlobal:
                            oDimensions.Add(new Dimension(Constants.Investments.Queries.StrategyInternationalEmergence, oItem.GetListItem("Strategy", "Emerging")));
                            break;
                        case Constants.Investments.Templates.ManagerStrategies.Specialty:
                            oDimensions.Add(new Dimension(
                                new Dimension.Value[]{
									new Dimension.Value("REIT", oItem.GetText("Strategy", "REIT", "0").Equals("1")),
									new Dimension.Value("Multi Asset Income", oItem.GetText("Strategy", "Multi Asset Income", "0").Equals("1")),
									new Dimension.Value("Environmental, Social and Governance", oItem.GetText("Strategy", "Environment Social and Goverment", "0").Equals("1"))
								}.ToList()
                                ));
                            break;
                        case Constants.Investments.Templates.ManagerStrategies.USEquity:
                            oDimensions.Add(new Dimension(Constants.Investments.Queries.StrategyUSEquityCap, oItem.GetListItem("Strategy", "Cap")));
                            oDimensions.Add(new Dimension(Constants.Investments.Queries.StrategyUSEquityStyle, oItem.GetListItem("Strategy", "Style")));
                            break;
                    }
                }
            }

            [DataContract]
            public class Dimension
            {
                [DataMember(Name = "Values", Order = 1)]
                private List<Value> oValues;

                internal Dimension(string sQuery, Item oValue)
                {
                    string sValue;

                    sValue = oValue != null ? oValue.DisplayName : string.Empty;

                    //get the items
                    oValues = ContextExtension.CurrentDatabase.SelectItems(sQuery).OrderBy(x => x.Appearance.Sortorder).Select(oItem => new Value(oItem.DisplayName, oItem.DisplayName.Equals(sValue))).Reverse().ToList();
                }

                internal Dimension(List<Value> oValues)
                {
                    //set the values
                    this.oValues = oValues;
                }

                [DataContract]
                public class Value
                {
                    [DataMember(Name = "Value", Order = 1)]
                    private string sValue;
                    [DataMember(Name = "Selected", Order = 2)]
                    private bool bSelected;

                    internal Value(string sValue, bool bSelected)
                    {
                        this.sValue = sValue;
                        this.bSelected = bSelected;
                    }
                }
            }
        }
    }
}
