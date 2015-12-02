using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Constants
{
    public static class Navigation
    {
        public static class Templates
        {
            /// <summary>
            /// Constants for Navigation base template
            /// </summary>
            public static class NavigationBase
            {
                //Item name for the navigation base template
                public const string Name = "Navigation Base";

                public static class Sections
                {
                    public static class Navigation
                    {
                        public const string Name = "Navigation";

                        public static class Fields
                        {

                            /// <summary>
                            /// Include in Navigation field name
                            /// </summary>
                            public const string IncludeInNavigationFieldName = "Include in Navigation";

                            /// <summary>
                            /// Include in Navigation field ID (sitecore Item ID)
                            /// </summary>
                            public const string IncludeInNavigationID = "{3D703D20-E979-4752-9970-ED8617B30248}";

                            /// <summary>
                            /// Sitecore ID for the Include in Navigation field
                            /// </summary>
                            public static ID IncludeInNavigation = new ID(IncludeInNavigationID);

                            /// <summary>
                            /// External Notification field name
                            /// </summary>
                            public const string ExternalNotificationFieldName = "External Notification";
                            /// <summary>
                            /// External Notification field ID as string (sitecore Item ID)
                            /// </summary>
                            public const string ExternalNotificationID = "{4B3E0E0D-2B7A-4A35-BBE2-DCA8931301ED}";

                            /// <summary>
                            /// Sitecore ID for the External Notification field
                            /// </summary>
                            public static ID ExternalNotification = new ID(ExternalNotificationID);

                            /// <summary>
                            /// Show on Meeting Mode field name
                            /// </summary>
                            public const string ShowOnMeetingModeFieldName = "Show on Meeting Mode";
                            /// <summary>
                            /// Show on Meeting Mode field ID as string (sitecore Item ID)
                            /// </summary>
                            public const string ShowOnMeetingModeID = "{40B577AE-DA84-4A6D-BA12-474F3DD40A6A}";

                            /// <summary>
                            /// Sitecore ID for the show on meeting mode field
                            /// </summary>
                            public static ID ShowOnMeetingMode = new ID(ShowOnMeetingModeID);

                            /// <summary>
                            /// User Status Visibility field name
                            /// </summary>
                            public const string UserStatusVisibilityFieldName = "User Status Visibility";

                            /// <summary>
                            /// User Status Visibility ID as string (Sitecore Item ID)
                            /// </summary>
                            public const string UserStatusVisibilityID = "{BF51C167-FF6E-490E-9CCB-49D8C051DCA3}";

                            /// <summary>
                            ///  Sitecore ID for User Status Visibility Field
                            /// </summary>
                            public static ID UserStatusVisibility = new ID(UserStatusVisibilityID);
                        }
                    }

                }
            }

            public static class LinkBase
            {
                public const string Name = "Link Base";

                public static class Sections
                {
                    public static class Link
                    {
                        public const string Name = "Link";

                        public static class Fields
                        {
                            public const string URLFieldName = "URL";

                            public const string URLID = "{EF50CF2F-DEA7-46B9-89AC-3F88A25B1FF8}";

                            public static ID URL = new ID(URLID);



                        }
                    }
                }
            }

            public static class UrlAlias
            {
                public const string Name = "Url Alias";

                public static class Sections
                {
                    public static class Data
                    {
                        public const string Name = "Data";

                        public static class Fields
                        {
                            public const string StartsWith = "Starts With";
                        }
                    }
                }
            }

        }
    }
}
