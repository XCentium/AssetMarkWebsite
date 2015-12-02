using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Documents;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Genworth.SitecoreExt.Services.Search;
using ServerLogic.SitecoreExt;
using Lucene.Net.QueryParsers;
using System.ServiceModel;

namespace Genworth.SitecoreExt.Helpers
{
    /// <summary>
    /// TODO: REFACTORING
    /// </summary>
    public static class HelpCenterHelper
    {

        private static List<Item> GetItemsFromLuceneDocuments(List<Document> oDocumentFound)
        {
            List<Item> oItemsToReturn;
            string sItemId;
            Item oItem;
            Lucene.Net.Documents.Field oIdField;

            oItemsToReturn = new List<Item>();

            if (oDocumentFound != null)
            {
                foreach (Document oDoc in oDocumentFound)
                {

                    oIdField = oDoc.GetField(Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Fields.Id);

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

        public static Dictionary<string, int> GetInitialLetters()
        {
            Dictionary<string, int> oResult = null;
            GenSearchService oSearchService;
            Query oQuery;
            oQuery = new WildcardQuery(new Term(Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Fields.Prefix, Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue));
            oSearchService = new GenSearchService();

            try
            {
                oResult = oSearchService.ExecuteGroupCountQuery(oQuery, Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Name, Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Fields.Prefix);
            }
            catch (Exception oSearchException)
            {
                Sitecore.Diagnostics.Log.Error("Error executing search request", oSearchException, typeof(EventHelper));
            }
            finally
            {
                oSearchService = null;
            }

            return oResult.ToDictionary(oKeypair => oKeypair.Key.Replace(Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.StartsWith, ""), oPair => oPair.Value);
        }

        public static List<Item> SearchFAQ(string sSearchKeywords, string sCategory)
        {
            List<Document> oQueryResults;
            Query oQuery;
            GenSearchService oSearchService;
            List<Item> oItemsToReturn = null;
            Term oTerm;
            bool bFilteredByCategory;
            bool bFilteredByKeyword;


            oSearchService = new GenSearchService();

            try
            {
                bFilteredByCategory = (!string.IsNullOrEmpty(sCategory)) && (!sCategory.Equals(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue));
                bFilteredByKeyword = (!string.IsNullOrEmpty(sSearchKeywords));

                // in case the user didn't searched for a specific category or keyword we should return all the FAQs
                if (!bFilteredByCategory && !bFilteredByKeyword)
                {
                    // we return all the FAQs by using the wildcard *
                    oTerm = new Term(Genworth.SitecoreExt.Constants.HelpCenter.Indexes.FAQIndex.Fields.Category, Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue);

                    oQuery = new WildcardQuery(oTerm);

                }
                // the user selected either a category or a keyword
                else
                {
                    Query oQueryTemp1 = null, oQueryTemp2 = null;
                    // if the user entered a keyword
                    if (bFilteredByKeyword)
                    {

                        oQueryTemp1 = SearchHelper.CreateMultiFieldQuery(new string[] { 
																Genworth.SitecoreExt.Constants.HelpCenter.Indexes.FAQIndex.Fields.Answer,																
																Genworth.SitecoreExt.Constants.HelpCenter.Indexes.FAQIndex.Fields.Question
																}, sSearchKeywords.AddWilcard());

                    }


                    // if the user selected a specific category
                    if (bFilteredByCategory)
                    {
                        sCategory = sCategory.Trim().ToLower().Replace("{", string.Empty).Replace("}", string.Empty);
                        oQueryTemp2 = SearchHelper.CreateMultiFieldQuery(new string[] { 																																
																			Genworth.SitecoreExt.Constants.HelpCenter.Indexes.FAQIndex.Fields.Category
																			}, sCategory);

                    }

                    oQuery = SearchHelper.CreateBooleanQuery(Occur.MUST, oQueryTemp1, oQueryTemp2);



                }

                oQueryResults = oSearchService.ExecuteQuery(oQuery, Genworth.SitecoreExt.Constants.HelpCenter.Indexes.FAQIndex.Name);

                oItemsToReturn = GetItemsFromLuceneDocuments(oQueryResults);

                oItemsToReturn = oItemsToReturn.OrderBy(i => i.GetText(
                                                        Genworth.SitecoreExt.Constants.HelpCenter.Templates.FAQ.Sections.FAQ.Name,
                                                        Genworth.SitecoreExt.Constants.HelpCenter.Templates.FAQ.Sections.FAQ.Fields.QuestionFieldName,
                                                        string.Empty
                                                        )).ToList();

            }
            catch (Exception oSearchException)
            {
                Sitecore.Diagnostics.Log.Error("Error executing search request", oSearchException, typeof(EventHelper));
            }
            finally
            {
                oSearchService = null;
            }


            return oItemsToReturn;
        }


        public static List<Item> SearchGlossary(string sSearchKeywords)
        {
            List<Document> oQueryResults;
            Query oQuery;
            GenSearchService oSearchService;
            List<Item> oItemsToReturn;

            oItemsToReturn = null;

            oSearchService = new GenSearchService();

            try
            {
                oQuery = SearchHelper.CreateMultiFieldQuery(new string[] { 
																Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Fields.Definition,
																Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Fields.Term
																}, sSearchKeywords.AddWilcard());

                oQueryResults = oSearchService.ExecuteQuery(oQuery, Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Name);
                oItemsToReturn = GetItemsFromLuceneDocuments(oQueryResults);
            }
            catch (Exception oSearchException)
            {
                Sitecore.Diagnostics.Log.Error("Error executing search request", oSearchException, typeof(HelpCenterHelper));
            }
            finally
            {
                oSearchService = null;
            }

            oItemsToReturn = oItemsToReturn.OrderBy(i => i.GetText(
                                        Genworth.SitecoreExt.Constants.HelpCenter.Templates.GlossaryTerm.Sections.GlossaryTerm.Name,
                                        Genworth.SitecoreExt.Constants.HelpCenter.Templates.GlossaryTerm.Sections.GlossaryTerm.Fields.TermFieldName,
                                        string.Empty
                                        )).ToList();

            return oItemsToReturn;
        }

        public static List<Item> SearchGlossaryByPrefix(string sPrefix)
        {
            List<Item> oItemsToReturn;
            WildcardQuery oQuery;
            Term oTerm;
            GenSearchService oSearchService;
            List<Document> oDocumentFound;
            string sItemId;
            Item oItem;
            Lucene.Net.Documents.Field oIdField;

            oItemsToReturn = new List<Item>();

            oSearchService = new GenSearchService();

            try
            {
                if (sPrefix.IndexOf('*') == -1)
                {
                    // add the prefix used in the glossary index for the prefix field 
                    sPrefix = String.Format("{0}{1}", Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.StartsWith, sPrefix).Trim().ToLower();
                }

                oTerm = new Term(Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Fields.Prefix, sPrefix);

                oQuery = new WildcardQuery(oTerm);

                oDocumentFound = oSearchService.ExecuteQuery(oQuery, Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Name);

                if (oDocumentFound != null)
                {
                    foreach (Document oDoc in oDocumentFound)
                    {

                        oIdField = oDoc.GetField(Genworth.SitecoreExt.Constants.HelpCenter.Indexes.GlossaryIndex.Fields.Id);

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

                    oItemsToReturn = oItemsToReturn.OrderBy(i => i.GetText(
                                                            Genworth.SitecoreExt.Constants.HelpCenter.Templates.GlossaryTerm.Sections.GlossaryTerm.Name,
                                                            Genworth.SitecoreExt.Constants.HelpCenter.Templates.GlossaryTerm.Sections.GlossaryTerm.Fields.TermFieldName,
                                                            string.Empty
                                                            )).ToList();
                }
            }
            catch (Exception oSearchException)
            {
                Sitecore.Diagnostics.Log.Error("Error executing search request", oSearchException, typeof(EventHelper));
            }
            finally
            {
                oSearchService = null;
            }

            return oItemsToReturn;


        }

    }
}
