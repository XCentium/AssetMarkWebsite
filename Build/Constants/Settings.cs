using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Constants
{
    public sealed class Settings
    {
        /// <summary>
        /// Constants related to the security settings
        /// </summary>
        public sealed class Security
        {
            /// <summary>
            /// Setting that stores the key for the Authorization Object kept in the http context during the http request
            /// </summary>
            public const string AuthorizationObject = "Genworth.SitecoreExt.Security.AuthorizationObject";

            /// <summary>
            /// Setting that stores the login page url. 
            /// </summary>
            public const string LoginPage = "Genworth.SitecoreExt.Security.LoginPage";

            /// <summary>
            /// Setting that stores the login page url. 
            /// </summary>
            public const string LoginPageQueryString = "Genworth.SitecoreExt.Security.LoginPage.QueryString";

            /// <summary>
            /// Setting that stores a enables SWT Test Mode.
            /// </summary>
            public const string SWTTestMode = "Genworth.SitecoreExt.Security.SWTTestMode";

            /// <summary>
            /// Setting that stores a enables SWT Test Mode Configuration.
            /// </summary>
            public const string SWTTestModeConfiguration = "Genworth.SitecoreExt.Security.SWTTestModeConfiguration";

            /// <summary>
            /// Setting that stores the test SSO Guid that will be used in Genworth test mode
            /// </summary>
            public const string TestSSOGuid = "Genworth.SitecoreExt.Security.TestSSOGuid";

            /// <summary>
            /// Setting that indicates whether we are using a test SSO guid
            /// </summary>
            public const string UseTestSSOGuid = "Genworth.SitecoreExt.Security.UseTestSSOGuid";

            /// <summary>
            /// Setting that indicates which Sitecore item contains the code for the client Role Id. Code used in the Authorization process to filter content from EWM
            /// </summary>
            public const string ClientRoleItem = "Genworth.SitecoreExt.Security.ClientRoleItem";

            /// <summary>
            /// Setting that indicates which Sitecore item contains the code for the agent Role Id. Code used in the Authorization process to filter content from EWM
            /// </summary>
            public const string AgentRoleItem = "Genworth.SitecoreExt.Security.AgentRoleItem";

            /// <summary>
            /// Setting that indicates which Sitecore item contains the code for the Osj Role Id. Code used in the Authorization process to filter content from EWM
            /// </summary>
            public const string OsjRoleItem = "Genworth.SitecoreExt.Security.OsjRoleItem";

            public const string ClientSecuredSectionsThroughClientApprovedField = "Genworth.SitecoreExt.Security.ClientSecuredSectionsThroughClientApprovedField";
        }


        /// <summary>
        /// Constants related to the search settings
        /// </summary>
        public sealed class Search
        {
            /// <summary>
            /// Setting that stores the setting that holds the default datbase to be used by the search service 
            /// </summary>
            public const string SearchServiceDefaultDatabase = "Genworth.SitecoreExt.Search.SearchServiceDefaultDatabase";
            public const string DefaultSearchableArticleTypes = "Genworth.SitecoreExt.Search.DefaultSearchableArticleTypes";
        }

        /// <summary>
        /// Constants related to the Genworth Custom Provider
        /// </summary>
        public sealed class ContentProvider
        {
            /// <summary>
            /// Setting that stores the key for the language code to be used as default if no language is provided in call to the Content Provider Service
            /// </summary>
            public const string DefaultLanguage = "Serverlogic.SitecoreExtension.ContentProvider.DefaultLanguage";

            /// <summary>
            /// Setting that stores the key for the version number to be used as default if no language is provided in call to the Content Provider Service
            /// </summary>
            public const string DefaultVersion = "Serverlogic.SitecoreExtension.ContentProvider.DefaultVersion";

            /// <summary>
            /// Setting that stores the key for the default database to be used in the Content Provider Service
            /// </summary>
            public const string DefaultDatabase = "Serverlogic.SitecoreExtension.ContentProvider.DefaultDatabase";

        }


        public sealed class Pages
        {
            public sealed class Marketing
            {
                public const string TabId = "Genworth.SitecoreExt.Pages.Marketing.TabId";

                public const string Ref = "Genworth.SitecoreExt.Pages.Marketing.Ref";

                public const string RDDBaseURL = "Genworth.SitecoreExt.Pages.Marketing.RDDBaseURL";

                public const string RDDAccount = "Genworth.SitecoreExt.Pages.Marketing.RDDAccount";

                public const string RDDPassword = "Genworth.SitecoreExt.Pages.Marketing.RDDPassword";

                public const string GlobalsoftBaseURL = "Genworth.SitecoreExt.Pages.Marketing.GlobalsoftBaseURL";

                public const string StandardRegisterIssuer = "Genworth.SitecoreExt.Pages.Marketing.StandardRegisterIssuer";

                public const string SourceSystemName = "Genworth.SitecoreExt.Pages.Marketing.SourceSystemName";

                public const string CostCenter = "Genworth.SitecoreExt.Pages.Marketing.CostCenter";

                public const string StandardRegisterLogonURL = "Genworth.SitecoreExt.Pages.Marketing.StandardRegisterLogonURL";

                public const string StandardRegisterXmlNamespace = "Genworth.SitecoreExt.Pages.Marketing.StandardRegisterXmlNamespace";

                public const string FailureMessage = "Genworth.SitecoreExt.Pages.Marketing.FailureMessage";

                public const string ReturnURL = "Genworth.SitecoreExt.Pages.Marketing.ReturnURL";

                public sealed class MarcomCentral
                {
                    public const string PartnerCredentialsToken = "AssetMark.SitecoreExt.Pages.Marketing.MarcomCentral.PartnerCredentialsToken";

                    public const string ImpersonatingSettingsPath = "AssetMark.SitecoreExt.Pages.Marketing.MarcomCentral.ImpersonatingSettingsPath";
                }

            }

            public sealed class Header
            {
                public const string HtmlURL = "Genworth.SitecoreExt.Integrations.Header.URL";

                public const string Encoding = "Genworth.SitecoreExt.Integrations.Header.Encoding";
            }


            public sealed class Footer
            {
                public const string HtmlURL = "Genworth.SitecoreExt.Integrations.Footer.URL";

                public const string Encoding = "Genworth.SitecoreExt.Integrations.Footer.Encoding";
            }

            public static class Events
            {
                public const string RegistrationPage = "Genworth.SitecoreExt.Pages.Events.RegistrationPage";
                public const string DefaultRegistrationEmailTemplate = "Genworth.SitecoreExt.Pages.Events.DefaultRegistrationEmailTemplate";
            }

            public static class Administration
            {
                public const string Documents = "Genworth.SitecoreExt.Pages.Administration.Documents";
            }

            public static class ModelPorfolio
            {
                public const string SolutionTypesChecked = "Genworth.SitecoreExt.Services.Investments.ModelPortfolio.SolutionTypeChecked";
                public const string CustodiansChecked = "Genworth.SitecoreExt.Services.Investments.ModelPortfolio.CustodiansChecked";
            }
            public sealed class Investments
            {
                public static class Research
                {
                    public const string RemoveDocumentsFromTemplate = "Genworth.SitecoreExt.Services.Investments.Research.RemoveDocumentsFromTemplate";
                    public const string CategoryChecked = "Genworth.SitecoreExt.Services.Investments.Research.CategoryChecked";
                    public const string SourceChecked = "Genworth.SitecoreExt.Services.Investments.Research.SourceChecked";
                    public const string StrategistChecked = "Genworth.SitecoreExt.Services.Investments.Research.StrategistChecked";
                    public const string ManagerChecked = "Genworth.SitecoreExt.Services.Investments.Research.ManagerChecked";
                    public const string AllocationApproachChecked = "Genworth.SitecoreExt.Services.Investments.Research.AllocationApproachChecked";
                    public const string AudienceChecked = "Genworth.SitecoreExt.Services.Investments.Research.AudienceChecked";
                }
                public sealed class ClientView
                {
                    public sealed class Research
                    {
                        public const string CategoryChecked = "Genworth.SitecoreExt.Services.Investments.ClientView.Research.CategoryChecked";
                        public const string SourceChecked = "Genworth.SitecoreExt.Services.Investments.ClientView.Research.SourceChecked";
                        public const string StrategistChecked = "Genworth.SitecoreExt.Services.Investments.ClientView.Research.StrategistChecked";
                        public const string ManagerChecked = "Genworth.SitecoreExt.Services.Investments.ClientView.Research.ManagerChecked";
                        public const string AllocationApproachChecked = "Genworth.SitecoreExt.Services.Investments.ClientView.Research.AllocationApproachChecked";
                    }
                }
                public sealed class ModelPortfolio
                {
                    public const string AllowedCustodiansForDynamicDocuments = "Genworth.SitecoreExt.Services.Investments.ModelPortfolio.AllowedCustodiansForDynamicDocuments";
                    public const string AllowedSolutionsForDynamicDocuments = "Genworth.SitecoreExt.Services.Investments.ModelPortfolio.AllowedSolutionsForDynamicDocuments";
                }
            }
        }

        public sealed class Integrations
        {
            public sealed class Google
            {
                public const string UseAPIKey = "Genworth.SitecoreExt.Integrations.Google.UseAPIKey";

                public const string APIKey = "Genworth.SitecoreExt.Integrations.Google.APIKey";

                public const string StaticMapBaseURL = "Genworth.SitecoreExt.Integrations.Google.StaticMapBaseURL";

                public const string GeocodingBaseURL = "Genworth.SitecoreExt.Integrations.Google.GeocodingBaseURL";

                public const string MapsClienSideBaseURL = "Genworth.SitecoreExt.Integrations.Google.MapsClienSideBaseURL";
            }

            public sealed class Ewm
            {
                public const string BundleURL = "Genworth.SitecoreExt.Integrations.Ewm.BundleURL";

                public const string Encoding = "Genworth.SitecoreExt.Integrations.Ewm.Encoding";
            }
        }

        public sealed class Email
        {
            /// <summary>
            /// Setting that stores the stmp server address to be used for sending email notifications
            /// </summary>
            public const string SMTPServer = "Genworth.SitecoreExt.Email.SMTPServer";
        }

        public sealed class BundleTypes
        {
            public const string Scripts = "Scripts";

            public const string Styles = "Styles";
        }

        public sealed class Indexing
        {
            public sealed class DataFormat
            {
                public const string DateTimeStringFormat = "Genworth.SitecoreExt.Indexing.DataFormat.DateTimeStringFormat";
            }
        }
    }
}
