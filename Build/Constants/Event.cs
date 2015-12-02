using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Constants
{
    /// <summary>
    /// Constants related to events.
    /// </summary>
    public static class Event
    {

        public const int DefaultResultsPerPage = 20;
        public const int MinResultsPerPage = 20;
        public const string DateFormat = "MM/dd/yyyy";


        public static String fPhoneNumber(String number)
        {
            String fNumber = number.Substring(0, 3) + "." +
                                     number.Substring(3, 3) + "." +
                                     number.Substring(6, 4);

            return fNumber;
        }


        public static class Indexes
        {
            public static class EventsIndex
            {
                public const string Name = "sitecore_web_events_index";


                public static class Fields
                {
                    public const string Title = "title";
                    public const string SubTitle = "sub_title";
                    public const string Tags = "tags";
                    public const string Summary = "summary";
                    public const string BeginDate = "begin_date";
                    public const string EndDate = "end_date";
                    public const string Body = "body";
                    public const string Speakers = "speakers";
                    public const string Latitude = "latitude";
                    public const string Longitude = "longitude";
                    public const string Id = "_uniqueid";
                    public const string Path = "itemnavigationpath";
                    public const string Type = "eventtype";
                    public const string City = "city";
                    public const string State = "state";
                    public const string IsGwnHosted = "isamhosted";
                    public const string PcStatusSec = "pcstatussec";
                    public const string Url = "url";
                    public const string ArchiveUrl = "archiveUrl";
                    public const string InvitationOnly = "by_invitation_only";
                    public const string InvitationOnlyTxt = "by_invitation_only_text";
                }

            }
        }
        public static class Types
        {
            public const string InPerson = "In-Person";
            public const string Webinar = "Webinar";
            public const string Conference = "Conference Call";
        }

        public static class SearchType
        {
            public const string All = "events-all";
            public const string InPerson = "events-inperson";
            public const string Archive = "events-archive";
            public const string Webinar = "events-webinar";
        }

        public static class SearchFilters
        {
            public const string Keywords = "keywords";
            public const string BeginDate = "beginDate";
            public const string EndDate = "endDate";
            public const string EventType = "eventType";
            public const string PCStatus = "pcStatus";

            public const string RatioInMiles = "ratioInMiles";
            public const string ZipCode = "zipCode";
            public const string Latitude = "latitude";
            public const string Longitude = "longitude";
        }

        public static class Templates
        {

            public static class EventBase
            {
                public const string Name = "Event Base";

                public static class Sections
                {
                    public static class Event
                    {

                    }
                }
            }

            /// <summary>
            /// On-Site template constants  
            /// </summary>
            public static class OnSite
            {
                /// <summary>
                /// On-Site template name (Item Name)
                /// </summary>
                public const string Name = "On-Site";

                /// <summary>
                /// Sections associated to the On-Site template
                /// </summary>
                public static class Sections
                {

                    /// <summary>
                    /// Event Section associated to On-Site template
                    /// </summary>
                    public static class Event
                    {
                        /// <summary>
                        /// Event Section Name
                        /// </summary>
                        public const string Name = "Event";

                        /// <summary>
                        /// Fields associated to the event section in On-Site template
                        /// </summary>
                        public static class Fields
                        {
                            /// <summary>
                            /// Map link field name
                            /// </summary>
                            public const string MapLinkFieldName = "Map Link";

                            /// <summary>
                            /// String version for the ID associated to the Map Link field Item ID
                            /// </summary>
                            public const string MapLinkID = "{62DA0D73-8A7E-4F4A-9FFB-9AF5F368017E}";

                            /// <summary>
                            /// Sitecore Item ID for the Map Link Field
                            /// </summary>
                            public static ID MapLink = new ID(MapLinkID);


                            /// <summary>
                            /// Address field name
                            /// </summary>
                            public const string AddressFieldName = "Address";

                            /// <summary>
                            /// String version for the ID associated to the Address field Item ID
                            /// </summary>
                            public const string AddressID = "{62DA0D73-8A7E-4F4A-9FFB-9AF5F368017E}";

                            /// <summary>
                            /// Sitecore Item ID for the Address Field
                            /// </summary>
                            public static ID Address = new ID(AddressID);

                            /// <summary>
                            /// City field name
                            /// </summary>
                            public const string CityFieldName = "City";


                            /// <summary>
                            /// State field name
                            /// </summary>
                            public const string StateFieldName = "State";

                            /// <summary>
                            /// Zip Code field name
                            /// </summary>
                            public const string ZipCodeFieldName = "Zip Code";

                            /// <summary>
                            /// Telephone field name
                            /// </summary>
                            public const string TelephoneFieldName = "Telephone";
                        }
                    }

                    /// <summary>
                    /// Event Section associated to On-Site template
                    /// </summary>
                    public static class Geolocation
                    {
                        /// <summary>
                        /// Event Section Name
                        /// </summary>
                        public const string Name = "Geolocation";

                        /// <summary>
                        /// Fields associated to the geolocation section in On-Site template 
                        /// (/sitecore/templates/Genworth/Base/Add Ons/Event/On-Site/Geolocation)
                        /// {E759556E-9F4B-4649-A4BE-95E9FFFEFC5F}
                        /// </summary>
                        public static class Fields
                        {
                            /// <summary>
                            /// Latitude field name
                            /// </summary>
                            public const string LatitudeFieldName = "Latitude";

                            /// <summary>
                            /// String version for the ID associated to the Latitude field Item ID
                            /// </summary>
                            public const string LatitudeID = "{62DA0D73-8A7E-4F4A-9FFB-9AF5F368017E}";

                            /// <summary>
                            /// Sitecore Item ID for the Latitude Field
                            /// </summary>
                            public static ID Latitude = new ID(LatitudeID);


                            /// <summary>
                            /// Longitude field name
                            /// </summary>
                            public const string LongitudeFieldName = "Longitude";

                            /// <summary>
                            /// String version for the ID associated to the Longitude field Item ID
                            /// </summary>
                            public const string LongitudeID = "{62DA0D73-8A7E-4F4A-9FFB-9AF5F368017E}";

                            /// <summary>
                            /// Sitecore Item ID for the Longitude Field
                            /// </summary>
                            public static ID Longitude = new ID(LongitudeID);


                        }
                    }
                }



            }

            public static class Webinar
            {
                public const string Name = "Webinar";
            }

            public static class ConferenceCall
            {
                public const string Name = "Conference-Call";
            }

            public static class PremierConsultantUpcomingMeeting
            {
                public const string Name = "Premier Consultant Upcoming Meeting";
            }

            /// <summary>
            /// Mastery Program Event template constants
            /// </summary>
            public static class MasteryProgramEvent
            {
                /// <summary>
                /// Template Name
                /// </summary>
                public const string Name = "Mastery Program Event";

                public static class Sections
                {
                    public static class Event
                    {
                        public const string PCStatus = "PC Status";
                        public const string QualifyingAUM = "Qualifying AUM";
                        public const string ContinuingEducationCredits = "Continuing Education Credits";
                        public const string Internal = "Internal";
                        public const string External = "External";
                        public const string Broker = "Broker";
                        public const string Dealer = "Dealer";
                    }
                }
            }

            public static class PCTemplate
            {
                /// <summary>
                /// Sitecore Item ID for Premier Consultant Upcoming Meeting
                /// </summary>
                public const string PremierEventName = "{6EB0FE7E-CEA7-4614-ABD3-C662813B96F3}";
            }

            public static class BDConference
            {
                /// <summary>
                /// Sitecore Item ID for Premier Consultant Upcoming Meeting
                /// </summary>
                public const string BDConfEventName = "{F5A1690C-7FAF-4B44-AC5C-F4276ECA5A51}";
            }


            public static class MasteryProgram
            {
                /// <summary>
                /// Sitecore Item ID for Premier Consultant Upcoming Meeting
                /// </summary>
                public const string MasteryPrgEventName = "{FCEFD211-440F-4537-AF15-AE537098EC97}";
            }



            public static class RegistrationPage
            {
                /// <summary>
                /// Template Name
                /// </summary>
                public const string Name = "RegistrationPage";
            }

            public static class ConferenceCallEvent
            {
                /// <summary>
                /// Template Name
                /// </summary>
                // Sitecore Item ID for Conference Call
                public const string ConferenceCallEventName = "{5123C7F4-7B3C-428A-ADBF-4B6555D70B56}"; //Conference Call

                public static class Sections
                {
                    public static class Event
                    {
                        public const string DialPhoneFieldName = "Event Dial-In Phone";
                        public const string RecPhoneFieldName = "Recording Phone";

                    }
                }
            }

            public static class WebinarEvent
            {
                /// <summary>
                /// Template Name
                /// </summary>
                //public const string Name = "Webinar";
                public const string WebinarEventName = "{6726FA6D-EF11-46CF-9CEC-39AE4EFB8986}"; //Webinar

                public static class Sections
                {
                    public static class Event
                    {
                        public const string EveURLFieldName = "Event URL";
                        public const string ArcURLFieldName = "Archive URL";
                        public const string HostFieldName = "Host";
                    }
                }
            }
        }

        public static class RegistrationPage
        {
            public const string URLFormat = "{0}?{1}={2}";

            public const string EventIdQueryStringKey = "EventId";

        }
    }
}
