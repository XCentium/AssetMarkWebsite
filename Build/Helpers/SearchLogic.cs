using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.Services.Search;
using System.ServiceModel;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System.Text.RegularExpressions;
using Lucene.Net.Index;

namespace Genworth.SitecoreExt.Helpers
{
    public static class SearchHelper
    {
        private static string _SEARCH_MECHANISM;

        public static string SEARCH_MECHANISM
        {
            get
            {
                if (string.IsNullOrEmpty(_SEARCH_MECHANISM))
                {
                    _SEARCH_MECHANISM = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Search.Mechanism", "local").ToLower().Trim();
                }
                return _SEARCH_MECHANISM;
            }
        }

        public static Lucene.Net.Util.Version LuceneVersion
        {
            get
            {
                return Lucene.Net.Util.Version.LUCENE_30;
            }
        }

        public static List<Item> Search(Query oQuery, string sIndexName, string fieldName)
        {
            List<Document> oDocumentsFound;
            List<Item> oItemsToReturn;
            string sItemId;
            Item oItem;
            Lucene.Net.Documents.Field oIdField;

            oItemsToReturn = new List<Item>();
            oDocumentsFound = SearchLuceneIndex(oQuery, sIndexName);

            if (oDocumentsFound != null)
            {
                foreach (Document oDoc in oDocumentsFound)
                {

                    oIdField = oDoc.GetField(fieldName);

                    if (oIdField != null)
                    {
                        sItemId = oIdField.StringValue;

                        if (!string.IsNullOrEmpty(sItemId))
                        {
                            oItem = ContextExtension.CurrentDatabase.GetItem(sItemId);
                            if (oItem != null)
                            {
                                oItemsToReturn.Add(oItem);
                            }
                        }
                    }
                }
            }

            return oItemsToReturn;
        }

        public static List<Item> Search(BooleanQueryContract oQuery, string sIndexName, string fieldName)
        {
            List<Document> oDocumentFound;
            List<Item> oItemsToReturn;
            string sItemId;
            Item oItem;
            Lucene.Net.Documents.Field oIdField;

            oItemsToReturn = new List<Item>();
            oDocumentFound = SearchLuceneIndex(oQuery, sIndexName);

            if (oDocumentFound != null)
            {
                foreach (Document oDoc in oDocumentFound)
                {
                    oIdField = oDoc.GetField(fieldName);

                    if (oIdField != null)
                    {
                        sItemId = oIdField.StringValue;

                        if (!string.IsNullOrEmpty(sItemId))
                        {
                            oItem = ContextExtension.CurrentDatabase.GetItem(sItemId);
                            if (oItem != null)
                            {
                                oItemsToReturn.Add(oItem);
                            }
                        }
                    }
                }
            }

            return oItemsToReturn;
        }

        public static List<Document> SearchLuceneIndex(Query oQuery, string sIndexName)
        {
            List<Document> oQueryResults;

            oQueryResults  = new GenSearchService().ExecuteQuery(oQuery, sIndexName);

            return oQueryResults;
        }


        public static List<Document> SearchLuceneIndex(BooleanQueryContract oQuery, string sIndexName)
        {
            List<Document> oQueryResults;

            oQueryResults  = new GenSearchService().ExecuteBooleanQuery(oQuery, sIndexName);

            return oQueryResults;
        }


        public static Query CreateMultiFieldQuery(string[] sField, string sSearchCriteria, bool bEscapeCharecters = false)
        {
            Analyzer oAnalyzer;
            Query oLuceneQuery;
            MultiFieldQueryParser oMultiFieldQueryParser;
            string sSearchCriteriaEscaped;

            oAnalyzer = new StandardAnalyzer(LuceneVersion);
            oMultiFieldQueryParser = new MultiFieldQueryParser(LuceneVersion, sField, oAnalyzer);
            oMultiFieldQueryParser.DefaultOperator = QueryParser.Operator.OR;
            sSearchCriteriaEscaped = bEscapeCharecters ? QueryParser.Escape(sSearchCriteria) : sSearchCriteria;
            oLuceneQuery = oMultiFieldQueryParser.Parse(sSearchCriteriaEscaped);
            return oLuceneQuery;
        }
        public static Query CreateQuery(string sField, string sSearchCriteria, bool bEscapeCharecters = true)
        {
            QueryParser oParser = new QueryParser(LuceneVersion, sField, new StandardAnalyzer(LuceneVersion));
            return oParser.Parse(bEscapeCharecters ? QueryParser.Escape(sSearchCriteria) : sSearchCriteria);
        }
        public static BooleanQuery CreateBooleanQuery(Occur oOccur, params Query[] oQueries)
        {
            BooleanQuery oBooleanQuery = new BooleanQuery();

            foreach (Query oQuery in oQueries)
            {
                if (oQuery != null)
                    oBooleanQuery.Add(oQuery, oOccur);
            }
            return oBooleanQuery;
        }

        public static string RemoveOperators(this string sKeywords)
        {
            Regex oRegex = new Regex(@"[\\/:\*\?<>|\+\[\]\(\)]");

            return oRegex.Replace(sKeywords, "");
        }
        public static string AddWilcard(this string sKeywords)
        {
            string sSearchCriteria = sKeywords.RemoveOperators();
            if (!sSearchCriteria.Contains("\""))
            {
                sSearchCriteria = string.Concat(sSearchCriteria.Replace(" ", "* "), "*");
            }
            return sSearchCriteria;
        }

        /// <summary>
        /// Build Filtered Query
        /// It receive a field and list of valid data on a <string, List<string>>
        /// 
        /// </summary>
        /// <param name="restrictedData">Dictionary<string, string[]> restrictedData</param>
        /// <returns>FilteredQuery</returns>
        public static Query BuildFilterQuery(Dictionary<string, string[]> filterValues)
        {
            BooleanQuery booleanQuery = new BooleanQuery();

            string field = string.Empty;
            BooleanQuery fieldQuery = null;
            string[] validValues = null;
            foreach (KeyValuePair<string, string[]> item in filterValues)
            {
                field = item.Key;
                fieldQuery = new BooleanQuery();
                validValues = item.Value;
                foreach (string validValue in validValues)
                {
                    //Use of ToLowerInvariant is necessary...
                    fieldQuery.Add(new TermQuery(new Term(field, validValue.ToLowerInvariant())), Occur.SHOULD);
                }

                fieldQuery.MinimumNumberShouldMatch = 1;

                booleanQuery.Add(fieldQuery, Occur.MUST);

            }

            return booleanQuery;
        }
    }
}
