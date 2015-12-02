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

namespace Genworth.SitecoreExt.Services.Investments
{
    /// <summary>
    /// This helper object constructs search information to query the Lucene index.
    /// </summary>
    public class Search
    {
        private static Query ALL_ITEMS_QUERY = CreateAllItemsQuery();
        private Dictionary<string, Query> oQueries;
        private Query oQuery;
        private Query oDefaultQuery;
        private bool bIsDirty;
        private Document[] oResultDocuments;
        private ItemCache oItemCache;
        private bool bBypassItems;
        private string[] sKeywordFields;

        internal string[] KeywordFields
        {
            get { return sKeywordFields; }
            set { sKeywordFields = value; }
        }


        private bool bKeywordSearch;

        private static Regex oLuceneSpecialCharacters = new Regex("[\\\\!\\(\\)\\:\\^\\]\\{\\}\\~\\?]");
        private string sKeyword;
        public bool IsKeywordSearch
        {
            get { return bKeywordSearch; }
            set { bKeywordSearch = value; }
        }
        public bool BypassItems
        {
            get { return bBypassItems; }

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
        internal Dictionary<string, Query> Queries
        {
            get
            {
                return oQueries;
            }
        }
        public int AppliedFilters { get { return oQueries.Count; } }

        public Search() : this(new ItemCache()) { }

        public Search(ItemCache oItemCache) : this(oItemCache, false) { }
        public Search(ItemCache oItemCache, Query oDefaultQuery) : this(oItemCache, false, oDefaultQuery) { }
        public Search(ItemCache oItemCache, bool bBypassItems, Query oDefault = null)
        {
            this.oItemCache = oItemCache;
            this.bKeywordSearch = false;
            this.bBypassItems = bBypassItems;
            this.oDefaultQuery = oDefault;
            Reset();
            sKeywordFields = new string[] { Constants.Investments.Indexes.Fields.Title };
        }

        private static Query CreateAllItemsQuery()
        {

            //return the query
            return SearchHelper.CreateQuery(Constants.Investments.Indexes.Fields.Constant, Constants.Investments.Indexes.Fields.Constant);
        }

        public void Reset()
        {
            //create a query that we will use to search for results
            oQueries = new Dictionary<string, Query>();
            oQuery = new BooleanQuery();


            //we are dirty
            bIsDirty = true;
        }




        internal void SetFilter(Filter oFilter)
        {
            //set the filter using a filer object
            SetFilter(oFilter.IndexField, oFilter.Options.Where(oOption => oOption.Filtered).Select(oOption => oOption.Id));
        }

        public void SetFilter(string sIndexField, IEnumerable<string> sOptions)
        {

            SetFilter(sIndexField, string.Join(" ", sOptions.ToArray()).Replace("}", "\\}").Replace("{", "\\{"));


        }

        public void SetDateFilter(DateTime dFromDate, DateTime dToDate)
        {
            SetFilter(Constants.Investments.Indexes.Fields.Date, string.Format("[{0} TO {1}]", dFromDate.ToString("yyyyMMddTHHmmss"), dToDate.ToString("yyyyMMddTHHmmss")), false);

        }
        private void SetFilter(string sField, string squery, bool bScapeCharacters = true, bool bWilcardSearch = false)
        {
            Query oNewQuery;
            //see if we already have a clause for this filter in the query
            if (oQueries.ContainsKey(sField))
            {
                //remove the clause
                oQueries.Remove(sField);

                //we are dirty
                bIsDirty = true;
            }

            //does this filter have any options selected?
            if (!string.IsNullOrWhiteSpace(squery))
            {
                //loop over options
                if (bWilcardSearch)
                {
                    oNewQuery = SearchHelper.CreateMultiFieldQuery(new string[] { sField }, squery, bScapeCharacters);

                }
                else
                    oNewQuery = SearchHelper.CreateQuery(sField, squery, bScapeCharacters);
                oQueries.Add(sField, oNewQuery);
                //we are dirty
                bIsDirty = true;
            }
        }
        private void SetFilter(string skey, string[] sFields, string squery, bool bScapeCharacters = true, bool bWilcardSearch = false)
        {
            Query oNewQuery;
            //see if we already have a clause for this filter in the query
            if (oQueries.ContainsKey(skey))
            {
                //remove the clause
                oQueries.Remove(skey);

                //we are dirty
                bIsDirty = true;
            }

            //does this filter have any options selected?
            if (!string.IsNullOrWhiteSpace(squery))
            {
                //loop over options
                oNewQuery = SearchHelper.CreateMultiFieldQuery(sFields, squery, bScapeCharacters);
                oQueries.Add(skey, oNewQuery);
                //we are dirty
                bIsDirty = true;
            }
        }

        public void SetKeywordFilter(string sKeywords)
        {

            if (!string.IsNullOrWhiteSpace(sKeywords))
            {
                SetFilter(Constants.Investments.Indexes.Fields.Title, sKeywordFields, oLuceneSpecialCharacters.Replace(sKeywords, "\\$0").Trim().AddWilcard(), false, true);
                this.sKeyword = sKeywords;

            }
            else
            {
                oQueries.Remove(Constants.Investments.Indexes.Fields.Title);
                this.sKeyword = string.Empty;
                bIsDirty = true;
            }
            bKeywordSearch = true;
        }

        private void FetchItems(bool bBypassItems)
        {
            List<Document> oDocuments;
            List<Document> oSearchDocuments;
            Field oIdField;
            string sItemId;
            bool bPermitted = true;

            //create a list of result objects
            oDocuments = new List<Document>();

            //do we have any query attributes?
            if (oQueries.Count > 0)
            {
                if (oQueries.ContainsKey(Constants.Investments.Indexes.Fields.Title))
                {

                    oQuery = oQueries[Constants.Investments.Indexes.Fields.Title];
                    oQueries.Remove(Constants.Investments.Indexes.Fields.Title);
                }
                else
                {
                    oQuery = SearchHelper.CreateBooleanQuery(Occur.MUST, oQueries.Values.ToArray());
                }
                if (oDefaultQuery != null)
                {
                    oQuery = SearchHelper.CreateBooleanQuery(Occur.MUST, oQuery, oDefaultQuery);
                }
                //get documents from search
                oSearchDocuments = SearchHelper.SearchLuceneIndex(oQuery, Constants.Investments.Indexes.InvestmentsResearchIndex);
            }
            else
            {
                //get all documents
                oSearchDocuments = SearchHelper.SearchLuceneIndex(ALL_ITEMS_QUERY, Constants.Investments.Indexes.InvestmentsResearchIndex);
            }

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


            }
            else
            {
                oResultDocuments = oSearchDocuments.ToArray();
            }
            //turn off dirty flag
            bIsDirty = false;
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
    }
}
