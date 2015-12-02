using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Constants
{
    public sealed class Email
    {

        public static class Templates
        {

            public static class Email
            {
                /// <summary>
                /// Email template name (Item Name). This is the template used for email templates
                /// </summary>
                public const string Name = "Email";

                public const string ID = "{C408994A-2A11-4DC6-A906-72513F8B9E97}";

                public static class Sections
                {
                    public static class Email
                    {
                        /// <summary>
                        /// Email section name (Item Name).
                        /// </summary>
                        public const string Name = "Email";

                        /// <summary>
                        /// Fields associated to the Email section
                        /// </summary>
                        public static class Fields
                        {
                            /// <summary>
                            /// To field name
                            /// </summary>
                            public const string ToFieldName = "To";

                            /// <summary>
                            /// To field id as string
                            /// </summary>
                            public const string ToFieldID = "{71DD8198-7FDF-4C94-91BF-7C125460BD2F}";

                            /// <summary>
                            /// To field (Sitecore ID)
                            /// </summary>
                            public static ID To = new ID(ToFieldID);

                            /// <summary>
                            /// CC field name
                            /// </summary>
                            public const string CCFieldName = "CC";

                            /// <summary>
                            /// CC field id as string
                            /// </summary>
                            public const string CCFieldID = "{B2E429BB-4440-4558-8B06-38D017DCA9BD}";

                            /// <summary>
                            /// CC field (Sitecore ID)
                            /// </summary>
                            public static ID CC = new ID(CCFieldID);


                            /// <summary>
                            /// From field name
                            /// </summary>
                            public const string FromFieldName = "From";

                            /// <summary>
                            /// From field id as string
                            /// </summary>
                            public const string FromFieldID = "{5D9B371E-1CE0-4A84-AC5E-3D54EF3CF765}";

                            /// <summary>
                            /// From field ID (Sitecore ID)
                            /// </summary>
                            public static ID From = new ID(FromFieldID);

                            /// <summary>
                            /// From email description field name
                            /// </summary>
                            public const string FromEmailDescriptionFieldName = "From Email Description";

                            

                            /// <summary>
                            /// Subject field name
                            /// </summary>
                            public const string SubjectFieldName = "Subject";

                            /// <summary>
                            /// Subject field id as string
                            /// </summary>
                            public const string SubjectFieldID = "{47244432-C75C-4D41-99F7-0E4CC922DE10}";

                            /// <summary>
                            /// Subject field (Sitecore ID)
                            /// </summary>
                            public static ID Subject = new ID(SubjectFieldID);


                        }

                    }
                }
            }        
        }
    }
}
