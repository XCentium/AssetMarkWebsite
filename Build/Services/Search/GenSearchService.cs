using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Lucene.Net.Search;
using Sitecore.Data;
using Sitecore.Data.Indexing;
using System.Runtime.Serialization.Formatters.Binary;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using System.ServiceModel.Activation;
using System.IO;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using Lucene.Net.Index;
using Genworth.SitecoreExt.Search;

namespace Genworth.SitecoreExt.Services.Search
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class GenSearchService : IGenSearchService
    {
        static GenSearchService()
        {
            Occur oDummy = Occur.MUST;
        }
        #region VARIABLES

        #region CONSTANTS

        /// <summary>
        /// Database that will be used to execute que lucene queries if no other was defines in the web.config settings (Genworth.SitecoreExt.Search.SearchServiceDefaultDatabase)
        /// </summary>
        private string sDefaultDatabase = "web";

        #endregion

        private Database oQueryDatabase;

        private ISearchIndex oIndex;

        private IndexSearcher oIndexSearcher;


        #endregion

        #region PROPERTIES

        /// <summary>
        /// Exposes the name of the databse that will be used to execute the queries
        /// </summary>
        private string DatabaseName
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Search.SearchServiceDefaultDatabase, sDefaultDatabase);
            }
        }

        /// <summary>
        /// Esposes the database that will be used to execute the queries
        /// </summary>
        private Database QueryDatabase
        {
            get
            {
                if (oQueryDatabase == null)
                {
                    oQueryDatabase = Sitecore.Data.Database.GetDatabase(DatabaseName);
                }
                return oQueryDatabase;
            }
        }

        private Lucene.Net.Util.Version LuceneVersion
        {
            get
            {
                return Lucene.Net.Util.Version.LUCENE_30;
            }
        }

        #endregion

        #region METHODS

        #region SERVICE OPERATIONS

        public List<Lucene.Net.Documents.Document> ExecuteBooleanQuery(BooleanQueryContract oQuery, string sIndexName)
        {
            #region VARIABLES

            List<Lucene.Net.Documents.Document> oLuceneQueryResults;
            Analyzer oAnalyzer;
            MultiFieldQueryParser oQueryParser;
            Query oLuceneQuery;
            BooleanQuery oLuceneBooleanQuery;

            #endregion

            Sitecore.Diagnostics.Log.Info("GenSearchService - ExecuteBooleanQuery - Begins", this);

            Sitecore.Diagnostics.Log.Info(string.Format("Executing query  on Index {0}", sIndexName), this);

            oLuceneQueryResults = null;

            if (oQuery != null && oQuery.Clauses != null)
            {
                oLuceneBooleanQuery = new BooleanQuery();

                /*
                 *An Analyzer builds TokenStreams to analyze the text. 
                 *The analyzer represents a policy for extracting index terms from the text. 
                 *A common implementation is to build a Tokenizer, which breaks the stream 
                 *of characters from the Reader into raw Tokens. 
                 *One or more TokenFilters may then be applied to the output of the Tokenizer. 
                 *spurious (stop) words using English dictionary 
                 */
                oAnalyzer = new StandardAnalyzer(LuceneVersion);

                try
                {

                    foreach (var oClause in oQuery.Clauses)
                    {

                        if (oClause.MultiFieldQuery != null && oClause.MultiFieldQuery.Fields != null && oClause.MultiFieldQuery.Fields.Length > 0 && !string.IsNullOrEmpty(oClause.MultiFieldQuery.SearchCriteria))
                        {
                            /*
                             * QueryParser obtains token stream using the Analizer and creates the Query
                             * baseing on internal rules. 
                             * QueryParser need to know content field in index to be able to parse queries like
                             * "Search" or "Example". These queries will be transformed into 
                             * [content_field]:Search and [content_field]:Example
                            */

                            oQueryParser = new MultiFieldQueryParser(LuceneVersion, oClause.MultiFieldQuery.Fields, oAnalyzer);


                            /*
                             * In default mode (OR_OPERATOR) terms without any modifiers are considered optional: 
                             * for example Test search is equal to Test OR search.
                             * In AND_OPERATOR mode terms are considered to be in conjuction: the above mentioned 
                             * query is parsed as Test AND search 
                             */
                            oQueryParser.DefaultOperator = QueryParser.Operator.OR;


                            /*
                             * This method converts string query to object Query that can be used in searching
                             */
                            oLuceneQuery = oQueryParser.Parse(oClause.MultiFieldQuery.SearchCriteria);


                            oLuceneBooleanQuery.Add(oLuceneQuery, ParseBooleanClauseOccur(oClause.Occur));

                        }
                    }

                    oLuceneQueryResults = ExecuteQuery(oLuceneBooleanQuery, sIndexName);
                }
                catch (Exception oSearchException)
                {
                    Sitecore.Diagnostics.Log.Error("Error executing search request", oSearchException, typeof(GenSearchService));
                }
            }

            if (oLuceneQueryResults == null)
            {
                Sitecore.Diagnostics.Log.Warn(string.Format(" Search on index {0} returned NULL", sIndexName), this);
            }
            else
            {
                Sitecore.Diagnostics.Log.Info(string.Format("Query executed result documents {0}", oLuceneQueryResults.Count), this);
            }
            Sitecore.Diagnostics.Log.Info("GenExternalSearchService - ExecuteBooleanQuery - Ends", this);

            return oLuceneQueryResults;
        }

        private Occur ParseBooleanClauseOccur(string sOccur)
        {
            switch (sOccur)
            {
                case "MUST":
                    return Occur.MUST;
                case "MUST_NOT":
                    return Occur.MUST_NOT;
                case "SHOULD":
                    return Occur.SHOULD;
                default:
                    return Occur.MUST;
            }
        }
        /// <summary>
        /// Executes the given query against the given index and the configured database for remote queries in the authoring environment
        /// </summary>
        /// <param name="oQuery">Lucene Query to execute</param>
        /// <param name="sIndexName">Index that will used to execute the query</param>
        /// <returns>A List with Lucene Documents obtained from the Hits object returned by the query</returns>
        public List<Lucene.Net.Documents.Document> ExecuteQuery(Query oQuery, string sIndexName)
        {
            #region VARIABLES

            TopScoreDocCollector oCollector;

            List<Lucene.Net.Documents.Document> odocumentMatches;
            Lucene.Net.Documents.Document oCurrentDocument;
            int iDocumentsFound;

            #endregion


            oCollector = null;
            odocumentMatches = new List<Lucene.Net.Documents.Document>();
            oCollector = TopScoreDocCollector.Create(LuceneSettings.MaxResults, true);
            //Check if we have a query to execute
            if (oQuery != null)
            {
                oIndex = ContentSearchManager.GetIndex(sIndexName);
                Sitecore.ContentSearch.LuceneProvider.ILuceneProviderIndex luceneProvider = oIndex as Sitecore.ContentSearch.LuceneProvider.ILuceneProviderIndex;
                Lucene.Net.Store.Directory indexDirectory = luceneProvider.Directory;
                var indexReader = Lucene.Net.Index.IndexReader.Open(indexDirectory, true);

                using (oIndexSearcher = new Lucene.Net.Search.IndexSearcher(indexReader))
                {
                    if (oIndexSearcher != null)
                    {
                        try
                        {
                            oIndexSearcher.Search(oQuery, oCollector);

                            if (oCollector != null)
                            {
                                iDocumentsFound = oCollector.TotalHits;
                                var scoreDocs = oCollector.TopDocs().ScoreDocs;
                                for (int iDocumentIndex = 0; iDocumentIndex < iDocumentsFound; iDocumentIndex++)
                                {
                                    var docId = scoreDocs[iDocumentIndex].Doc;
                                    oCurrentDocument = oIndexSearcher.Doc(docId);
                                    if (oCurrentDocument != null)
                                    {
                                        odocumentMatches.Add(oCurrentDocument);
                                    }
                                }
                            }
                        }
                        catch (Exception queryException)
                        {
                            Sitecore.Diagnostics.Log.Error(string.Format("Failed to execute query:{0} on Index:{1}", oQuery.ToString(), sIndexName), queryException, this);
                        }
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error(string.Format("Unable to create IndexSearcher over Database:{0} and Index:{1}", oQueryDatabase.Name, sIndexName), this);
                    }
                }
            }

            return odocumentMatches;

        }

        public Dictionary<string, int> ExecuteGroupCountQuery(Query oQuery, string sIndexName, string sfield)
        {
            List<Lucene.Net.Documents.Document> oDocuments = ExecuteQuery(oQuery, sIndexName);
            Field oField;

            var oResult = from oDoc in oDocuments
                          group oDoc by ((oField = oDoc.GetField(sfield)) != null ? oField.StringValue : string.Empty) into oDocGroup
                          select oDocGroup;

            return oResult.ToDictionary(oDocGroupKey => oDocGroupKey.Key, oDocGroupCount => oDocGroupCount.Count());

        }

        #endregion

        #endregion
    }
}
