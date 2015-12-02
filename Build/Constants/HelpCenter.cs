using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Constants
{
    /// <summary>
    /// Constants related to the help Center
    /// </summary>
    public static class HelpCenter
    {

        public static class Templates
        {

            public static class ContextSensitiveHelp
            {
                /// <summary>
                /// Context Sesnsitive Help template name (Item Name). This is the template used for Help Text
                /// </summary>
                public const string Name = "Context Sensitive Help";

                public const string ID = "{E984E030-6EB8-4A20-B4ED-1702E0D166A7}";

                public static class Sections
                {
                    public static class ContextSensitiveHelp
                    {
                        /// <summary>
                        /// Context Sesnsitive Help section name (Item Name).
                        /// </summary>
                        public const string Name = "Context Sensitive Help";

                        /// <summary>
                        /// Fields associated to the Context Sesnsitive Help section
                        /// </summary>
                        public static class Fields
                        {
                            /// <summary>
                            /// Title field name
                            /// </summary>
                            public const string TitleFieldName = "Title";

                            /// <summary>
                            /// Title field id as string
                            /// </summary>
                            public const string TitleFieldID = "{361DB7C1-213B-4C77-BD2A-89830A5E1B19}";

                            /// <summary>
                            /// Title field (Sitecore ID)
                            /// </summary>
                            public static ID Title = new ID(TitleFieldID);


                            /// <summary>
                            /// Text field name
                            /// </summary>
                            public const string TextFieldName = "Text";

                            /// <summary>
                            /// Text field id as string
                            /// </summary>
                            public const string TextFieldID = "{E38DA4AF-2B19-4CCF-BB98-99EA59E8FC6C}";

                            /// <summary>
                            /// Text field ID (Sitecore ID)
                            /// </summary>
                            public static ID Text = new ID(TextFieldID);

                            /// <summary>
                            /// Icon field name
                            /// </summary>
                            public const string IconFieldName = "Icon";

                            /// <summary>
                            /// Icon field id as string
                            /// </summary>
                            public const string IconFieldID = "{6F487507-5214-4904-BAC9-BE29D63FBFC1}";

                            /// <summary>
                            /// Icon field (Sitecore ID)
                            /// </summary>
                            public static ID Icon = new ID(IconFieldID);


                            /// <summary>
                            /// Icon field name
                            /// </summary>
                            public const string FieldSelectorFieldName = "Field Selector";

                            /// <summary>
                            /// Icon field id as string
                            /// </summary>
                            public const string FieldSelectorFieldID = "{4F23E6C7-6F94-4C98-8568-D99E4DCDAB77}";

                            /// <summary>
                            /// Icon field (Sitecore ID)
                            /// </summary>
                            public static ID FieldSelector = new ID(FieldSelectorFieldID);

                        }

                    }
                }
            }

            public static class GlossaryTerm
            {
                public const string Name = "Glossary Term";

                public const string TemplateID = "{19508B8E-FE7B-444D-A5B0-17F9A50CEB89}";

                public static class Sections
                {
                    public static class GlossaryTerm
                    {
                        public const string Name = "Glossary Term";

                        public const string SectionID = "{C5B7D9BC-CD59-449E-9B29-78E08FFB53DA}";

                        public static class Fields
                        {
                            /// <summary>
                            /// Term field name
                            /// </summary>
                            public const string TermFieldName = "Term";

                            /// <summary>
                            /// Term field id as string
                            /// </summary>
                            public const string TermFieldID = "{2463D139-BD73-4174-9286-451B6AEC4562}";

                            /// <summary>
                            /// Term field ID (Sitecore ID)
                            /// </summary>
                            public static ID Term = new ID(TermFieldID);


                            /// <summary>
                            /// Definition field name
                            /// </summary>
                            public const string DefinitionFieldName = "Definition";

                            /// <summary>
                            /// Definition field id as string
                            /// </summary>
                            public const string DefinitionFieldID = "{0ED4607D-BC41-41BB-95D3-DC52901E8814}";

                            /// <summary>
                            /// Term field ID (Sitecore ID)
                            /// </summary>
                            public static ID Definition = new ID(DefinitionFieldID);
                        }

                    }
                }
            }

            public static class FAQ
            {
                public const string Name = "FAQ";

                public const string TemplateID = "{BD37B234-C34D-4DB2-B6F3-0042DBA57460}";

                public static class Sections
                {
                    public static class FAQ
                    {
                        public const string Name = "FAQ";

                        public const string SectionID = "{958E55CE-1C37-4BB6-A268-3F643C8A0876}";

                        public static class Fields
                        {
                            /// <summary>
                            /// Question field name
                            /// </summary>
                            public const string QuestionFieldName = "Question";

                            /// <summary>
                            /// Question field id as string
                            /// </summary>
                            public const string QuestionFieldID = "{6F1E9FB3-C7E5-4E52-8840-65E0F9718BD0}";

                            /// <summary>
                            /// Question field ID (Sitecore ID)
                            /// </summary>
                            public static ID Question = new ID(QuestionFieldID);


                            /// <summary>
                            /// Answer field name
                            /// </summary>
                            public const string AnswerFieldName = "Answer";

                            /// <summary>
                            /// Answer field id as string
                            /// </summary>
                            public const string AnswerFieldID = "{8B062EAF-7A1A-409A-BF94-AC51F21DD336}";

                            /// <summary>
                            /// Answer Term field ID (Sitecore ID)
                            /// </summary>
                            public static ID Answer = new ID(AnswerFieldID);

                            /// <summary>
                            /// Answer field name
                            /// </summary>
                            public const string CategoryFieldName = "Category";

                            /// <summary>
                            /// Answer field id as string
                            /// </summary>
                            public const string CategoryFieldID = "{4EC61BD1-4F68-47DD-B61C-CD9BA83B9200}";

                            /// <summary>
                            /// Answer Term field ID (Sitecore ID)
                            /// </summary>
                            public static ID Category = new ID(CategoryFieldID);
                        }

                    }
                }
            }
        }

        public static class Indexes
        {
            public static class FAQIndex
            {
                public const string Name = "FAQIndex";


                public static class Fields
                {
                    public const string Question = "question";
                    public const string Answer = "answer";
                    public const string Category = "category";
                    public const string Id = "id";
                }

            }

            public static class GlossaryIndex
            {
                public const string Name = "sitecore_web_glossary_index";

                // this will be used to index the first letters of the Glossary Terms
                // avoiding the problems with the Stop Word from lucene 
                // (that makes the "a" letter difficult to search)
                public const string StartsWith = "startswith";

                public static class Fields
                {
                    public const string Term = "term";
                    public const string Definition = "definition";
                    public const string Id = "id";
                    public const string Prefix = "prefix";

                }

            }
        }

        public static class QueryParameters
        {
            // Shared
            public const string ID = "ID";

            // Specific for FAQ page
            public const string Category = "Category";

            // Specific for Glossary page
            public const string StartsWith = "StartsWith";

            public const string Keyword = "Keyword";

            public const string SearchAllValue = "*";

            public const string Sort = "sort";
            public const string DateAscending = "date";
            public const string DateDescending = "-date";
        }

        public static class Alphabet
        {
            public const string SearchAllDisplay = "ALL";
            public const string DefaultLetter = "A";
        }

        public static class FAQ
        {
            public const string CategoryAll = "All";
            public const string DefaultCategoryValue = "*";
            public const string DefaultSearchText = "Search FAQ";
        }

    }
}
