using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Constants
{
    public static class Policies
    {
        public static class Templates
        {

            public static class Policy
            {
                /// <summary>
                /// Policy template name (Item Name). This is the template used for Policies such as EULA
                /// </summary>
                public const string Name = "Policy";

                public const string ID = "{2CAEE771-6815-477A-AF79-1135A1EC2E99}";

                public static class Sections
                {
                    public static class Policy
                    {
                        /// <summary>
                        /// Policy section name (Item Name). 
                        /// </summary>
                        public const string Name = "Policy";

                        /// <summary>
                        /// Fields associated to the Policy section
                        /// </summary>
                        public static class Fields
                        {
                            /// <summary>
                            /// Body field name (/sitecore/templates/Genworth/Data/Policy/Policy/Body)
                            /// </summary>
                            public const string BodyFieldName = "Body";

                            /// <summary>
                            /// Body field id as string
                            /// </summary>
                            public const string BodyeFieldID = "{B73446C2-318D-4893-84F7-2A5D921489D2}";

                            /// <summary>
                            /// Body field (Sitecore ID)
                            /// </summary>
                            public static ID Body = new ID(BodyeFieldID);
                         

                        }

                    }
                }
            }            

            
        }
    }
}
