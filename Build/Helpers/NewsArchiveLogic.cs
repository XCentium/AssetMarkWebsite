using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genworth.SitecoreExt.Services.Search;
using Lucene.Net.Documents;
using Genworth.SitecoreExt.Services.Contracts.Data;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;

namespace Genworth.SitecoreExt.Helpers
{
    public static class NewsArchiveHelper
    {
        public static List<Item> SearchNewsArchive(string sSearchKeywords, string category)
        {
            List<Item> oItemsToReturn;
            oItemsToReturn = SearchNewsArchiveInternal(sSearchKeywords, GetCategoryFilterValues(category));
            return oItemsToReturn;
        }

        public static List<Item> SearchNewsArchive(string sSearchKeywords, Dictionary<string, string[]> filterValues)
        {
            List<Item> oItemsToReturn;

            oItemsToReturn = SearchNewsArchiveInternal(sSearchKeywords, filterValues);

            return oItemsToReturn;
        }

        private static List<Item> SearchNewsArchiveInternal(string sSearchCriteria, Dictionary<string, string[]> filterValues)
        {
            List<Item> oQueryResults = null;

            try
            {
                Query oQuery = SearchHelper.CreateMultiFieldQuery(new string[] { 
																Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Fields.Title,
																Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Fields.SubTitle,
																Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Fields.Body,
																Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Fields.Tags																
																}, sSearchCriteria.AddWilcard());

                Query filterQuery = SearchHelper.BuildFilterQuery(filterValues);

                FilteredQuery filteredQuery = new FilteredQuery(oQuery, new QueryWrapperFilter(filterQuery));

                oQueryResults = SearchHelper.Search(filteredQuery,
                    Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Name,
                    Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Fields.Id);

            }
            catch (Exception oSearchException)
            {
                Sitecore.Diagnostics.Log.Error("SearchNewsArchiveInternal - Error executing search request", oSearchException, typeof(EventHelper));
            }

            return oQueryResults;
        }

        private static Dictionary<string, string[]> GetCategoryFilterValues(string category)
        {
            Dictionary<string, string[]> filterValues = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(category))
            {
                category = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Search.DefaultSearchableArticleTypes, string.Empty);
            }

            string field = Genworth.SitecoreExt.Constants.NewsArchive.Indexes.ArticlesIndex.Fields.Type;
            if (!string.IsNullOrWhiteSpace(category))
            {
                filterValues.Add(field, category.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                Sitecore.Diagnostics.Log.Error(string.Format("The setting '{0}' was not provided for Genworth.SitecoreExt.Search.DefaultSearchableArticleTypes", category), typeof(NewsArchiveHelper));
            }

            return filterValues;
        }

    }
}
