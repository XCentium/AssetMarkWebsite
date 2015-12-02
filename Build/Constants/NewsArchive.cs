using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Constants
{
    /// <summary>
    /// Constants related to the archive news section
    /// </summary>
    public static class NewsArchive
    {

        public static class Templates
        {

            public static class Article
            {
                public const string Name = "Article";

                public static class Sections
                {
                    public static class Page
                    {

                    }
                }
            }
        }

        public static class Indexes
        {
            public static class ArticlesIndex
            {
                public const string Name = "sitecore_web_articles_index";


                public static class Fields
                {
                    public const string Title = "title";
                    public const string SubTitle = "sub_title";
                    public const string Tags = "tags";
                    public const string Body = "body";
                    public const string Id = "id";
                    public const string Path = "itemnavigationpath";
                    public const string Type = "articletype";
                }

                public static class Types
                {
                    public const string General = "general";
                }
            }
        }
    }
}
