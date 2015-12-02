using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Constants
{
    public static class Page
    {
        public static class Templates
        {

            public static class PageBase
            {
                /// <summary>
                /// PageBase template name (Item Name). 
                /// </summary>
                public const string Name = "Page Base";

                public const string ID = "{F9FB183F-44EA-4233-A7C9-486BA9EA41B0}";

                public static class Sections
                {
                    public static class Page
                    {
                        /// <summary>
                        /// Page section name (Item Name).
                        /// </summary>
                        public const string Name = "Page";

                        /// <summary>
                        /// Fields associated to the Email section
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
                            public const string TitleFieldID = "{FF10254D-6E15-4817-A623-F3E12370A1EB}";

                            /// <summary>
                            /// Title field (Sitecore ID)
                            /// </summary>
                            public static ID Title = new ID(TitleFieldID);


                            /// <summary>
                            /// Sub Title field name
                            /// </summary>
                            public const string SubTitleFieldName = "Sub Title";

                            /// <summary>
                            /// Sub Title field id as string
                            /// </summary>
                            public const string SubTitleFieldID = "{B8D4C07B-44C1-41EB-94EC-A9B2D700F346}";

                            /// <summary>
                            /// Sub Title field ID (Sitecore ID)
                            /// </summary>
                            public static ID SubTitle = new ID(SubTitleFieldID);

                            /// <summary>
                            /// Summary field name
                            /// </summary>
                            public const string SummaryFieldName = "Summary";

                            /// <summary>
                            /// Summary field id as string
                            /// </summary>
                            public const string SummaryFieldID = "{62F2C276-3E99-4ADF-A772-8133EA0920C6}";

                            /// <summary>
                            /// Summary field (Sitecore ID)
                            /// </summary>
                            public static ID Summary = new ID(SummaryFieldID);


                        }

                    }
                }
            }

        }
    }
}
