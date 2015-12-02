using System.Configuration;
using System.Globalization;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;

namespace Genworth.SitecoreExt.Search
{
    public static class LuceneSettings
    {
        #region Private Members
        private static Analyzer analyzer;
        private static int maxResults;
        private static int maxResultsByCategory;
        private static float minSimilarity;
        private static int maxStringLength;
        private static int minCharsPrefixQuery;

        #endregion

        #region Properties

        /// <summary>
        /// Property for default Lucene version
        /// </summary>
        static internal Lucene.Net.Util.Version LuceneVersion
        {
            get
            {
                return Lucene.Net.Util.Version.LUCENE_30;
            }
        }

        /// <summary>
        /// Property that holds the Lucene Analyzer
        /// </summary>
        static internal Analyzer Analyzer
        {
            // For now the Analyzer will be a StandardAnalyzer
            get
            {
                if (analyzer == null)
                {
                    analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(LuceneVersion);
                }
                return analyzer;
            }
        }

        /// <summary>
        /// Max number of results
        /// </summary>
        static internal int MaxResults
        {
            get
            {
                if (maxResults < 1)
                {
                    maxResults = GetMaxResults();
                }
                return maxResults;
            }
        }

        /// <summary>
        /// Max number of results
        /// </summary>
        static internal int MaxResultsByCategory
        {
            get
            {
                if (maxResultsByCategory < 1)
                {
                    maxResultsByCategory = GetMaxResultsByCategory();
                }
                return maxResultsByCategory;
            }
        }

        /// <summary>
        /// Minimum similarity for search.
        /// </summary>
        static internal float MinSimilarity
        {
            get
            {
                if (minSimilarity == 0.0f)
                {
                    minSimilarity = GetMinimumSimilarity();
                }
                return minSimilarity;
            }
        }

        /// <summary>
        /// Max number of results
        /// </summary>
        static internal int MaxStringLength
        {
            get
            {
                if (maxStringLength < 1)
                {
                    maxStringLength = GetMaxStringLength();
                }
                return maxStringLength;
            }
        }

        /// <summary>
        ///Minimun chars for prefix query
        /// </summary>
        static internal int MinCharsPrefixQuery
        {
            get
            {
                if (minCharsPrefixQuery < 1)
                {
                    minCharsPrefixQuery = GetMinCharsPrefixQuery();
                }
                return minCharsPrefixQuery;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        ///  Method to get the path of the index from the config file
        /// </summary>
        /// <returns></returns>
        private static int GetMaxResults()
        {
            int maxResults = 0;

            string maxResultsConfig = Sitecore.Configuration.Settings.GetSetting("Lucene.MaxResults");

            if (!string.IsNullOrWhiteSpace(maxResultsConfig))
            {
                if (int.TryParse(maxResultsConfig, out maxResults))
                {
                    return int.Parse(maxResultsConfig, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new Exception("The config key Lucene.MaxResults is not an integer.");
                }
            }
            else
            {
                throw new Exception("The config key Lucene.MaxResults is missing.");
            }
        }

        /// <summary>
        ///  Method to get the path of the index from the config file
        /// </summary>
        /// <returns></returns>
        private static int GetMaxResultsByCategory()
        {
            int maxResultsByCat = 0;

            string maxResultsByCatConfig = Sitecore.Configuration.Settings.GetSetting("Lucene.MaxResultsByCategory");

            if (!string.IsNullOrWhiteSpace(maxResultsByCatConfig))
            {
                if (int.TryParse(maxResultsByCatConfig, out maxResultsByCat))
                {
                    return int.Parse(maxResultsByCatConfig, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new Exception("The config key Lucene.MaxResultsByCategory is not an integer.");
                }
            }
            else
            {
                throw new Exception("The config key Lucene.MaxResultsByCategory is missing.");
            }
        }

        private static float GetMinimumSimilarity()
        {
            float minSimilarity = 0;

            string minSimilarityConfig = Sitecore.Configuration.Settings.GetSetting("Lucene.MinSimilarity");

            if (!string.IsNullOrWhiteSpace(minSimilarityConfig))
            {
                if (float.TryParse(minSimilarityConfig, out minSimilarity))
                {
                    return float.Parse(minSimilarityConfig, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new Exception("The config key Lucene.MinSimilarity is not a float.");
                }
            }
            else
            {
                throw new Exception("The config key Lucene.MinSimilarity is missing.");
            }
        }

        private static int GetMaxStringLength()
        {
            int maxStringLength = 0;

            string maxStringLengthConfig = Sitecore.Configuration.Settings.GetSetting("Lucene.MaxStringLength");

            if (!string.IsNullOrWhiteSpace(maxStringLengthConfig))
            {
                if (int.TryParse(maxStringLengthConfig, out maxStringLength))
                {
                    return int.Parse(maxStringLengthConfig, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new Exception("The config key Lucene.MaxStringLength is not an integer.");
                }
            }
            else
            {
                throw new Exception("The config key Lucene.MaxStringLength is missing.");
            }
        }

        private static int GetMinCharsPrefixQuery()
        {
            int minCharsPrefixQuery = 0;

            string minCharsPrefixQueryConfig = Sitecore.Configuration.Settings.GetSetting("Lucene.MinCharsPrefixQuery");

            if (string.IsNullOrWhiteSpace(minCharsPrefixQueryConfig))
            {
                if (int.TryParse(minCharsPrefixQueryConfig, out minCharsPrefixQuery))
                {
                    return int.Parse(minCharsPrefixQueryConfig, CultureInfo.InvariantCulture);
                }
                else
                {
                    throw new Exception("The config key Lucene.MinCharsPrefixQuery is not an integer.");
                }
            }
            else
            {
                throw new Exception("The config key Lucene.MinCharsPrefixQuery is missing.");
            }
        }
        #endregion

        #region Internal Methods

        /// <summary>
        /// Method to get the IndexReader
        /// </summary>
        /// <param name="path">Path of the index directory.</param>
        /// <param name="isReadOnly">The index reader directory is going to be opened as read only.</param>
        /// <returns>IndexReader</returns>
        static internal IndexReader GetIndexReader(string path, bool readOnly = true)
        {
            var directory = Lucene.Net.Store.FSDirectory.Open(path);
            return IndexReader.Open(directory, readOnly);
        }

        /// <summary>
        /// Method to get the IndexSearcher
        /// </summary>
        /// <param name="reader">Current IndexReader</param>
        /// <returns>IndexSearcher</returns>
        static internal IndexSearcher GetIndexSearcher(IndexReader reader)
        {
            return new IndexSearcher(reader);
        }
        #endregion
    }
}

