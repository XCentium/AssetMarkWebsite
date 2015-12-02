using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;

namespace Genworth.SitecoreExt.Constants
{
    public sealed class Investments
    {
        public const string Root = "{F6F0FAA4-B904-42C0-B094-98434A0E06AC}";
        public const int MinResultsPerPage = 20;
        public const int DefaultResultsPerPage = 20;
        public const string DateFormat = "MM/dd/yyyy";
        public const int DefaultMonthRange = -1; //All //3;
        public const int DefaultMonthRangeFromExternalLink = -1; //All
        public const int MinimumMonthRange = 3;
        public const int MaxSideResearchItemsPerCategory = 5;
        public const string GridJsonServiceUrl = "/services/InvestmentResearch.svc/";


        public sealed class Queries
        {
            public const string StrategyFixedIncomeStatus = "/sitecore/Content/Meta-Data/Lookups/Ranges/Taxable-TaxExempt/*";
            public const string StrategyFixedIncomeStyle = "/sitecore/Content/Meta-Data/Lookups/Ranges/Core-Opportunistic-Sector/*";
            public const string StrategyInternationalEmergence = "/sitecore/content/Meta-Data/Lookups/Ranges/Low-Medium-High-Emerging/*";
            public const string StrategyUSEquityCap = "/sitecore/Content/Meta-Data/Lookups/Ranges/All-Large-MidSmall/*";
            public const string StrategyUSEquityStyle = "/sitecore/Content/Meta-Data/Lookups/Ranges/Value-Blend-Growth/*";
        }

        public sealed class Templates
        {
            public const string Manager = "{FB181357-DA5D-4F72-9CE6-3A78CF5A8DF7}";
            public const string Strategist = "{F512A0F1-9384-490A-A828-B5B27FC8CE0A}";
            public const string Strategy = "{C3B6234A-68FD-42B0-8158-2D9468457153}";
            public const string AllocationApproach = "{DC9607FC-60E7-4D7C-8BDB-910F359E9E8F}";

            public const string ManagerStrategy = "{8309E5F9-94B4-413D-801E-3EF41F4AB5A4}";
            public sealed class ManagerStrategies
            {
                public const string FixedIncome = "{283FFF07-5754-4F79-B45A-F0A081E33747}";
                public const string InternationalGlobal = "{8688A948-F018-4843-A0B5-829EAD73FE5C}";
                public const string Specialty = "{78825C54-E491-4C2B-9C35-035E3DACBD34}";
                public const string USEquity = "{08D6D914-718F-4138-B551-BA3BC3921519}";
            }

            public sealed class Names
            {
                public const string Strategy = "Strategy";
                public const string Solution = "Solution";
                public const string StrategistNoAllocation = "Strategist No Allocation";
                public const string Strategist = "Strategist";
                public const string Manager = "Manager";
                public const string SimpleLinks = "Simple Links";
            }

            public sealed class SolutionTypeFields
            {
                public const string Mandate = "Mandate";
            }

        }

        public sealed class Items
        {
            public static readonly string ResearchItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.Research");
            private static Item oResearchItem = !string.IsNullOrEmpty(ResearchItemId) ? ContextExtension.CurrentDatabase.GetItem(ResearchItemId) : null;
            public static Item ResearchItem { get { return !string.IsNullOrEmpty(ResearchItemId) ? ContextExtension.CurrentDatabase.GetItem(ResearchItemId) : null; ; } }

            public static readonly string CompareItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.Comparison");
            public static Item CompareItem { get { return !string.IsNullOrEmpty(CompareItemId) ? ContextExtension.CurrentDatabase.GetItem(CompareItemId) : null; } }

            public static readonly string PerformanceItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.Performance");
            public static Item PerformanceItem { get { return !string.IsNullOrEmpty(PerformanceItemId) ? ContextExtension.CurrentDatabase.GetItem(PerformanceItemId) : null; } }

            public static readonly string DocumentViewerItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.DocumentViewer");
            public static Item DocumentViewerItem { get { return !string.IsNullOrEmpty(DocumentViewerItemId) ? ContextExtension.CurrentDatabase.GetItem(DocumentViewerItemId) : null; } }
            public static readonly string DocumentViewerClientViewItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.DocumentViewerClientView");
            public static Item DocumentViewerClientViewItem { get { return !string.IsNullOrEmpty(DocumentViewerClientViewItemId) ? ContextExtension.CurrentDatabase.GetItem(DocumentViewerClientViewItemId) : null; } }

            public static readonly string ManagersFolderItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.ShareContent.Investments.ManagersFolder");
            public static Item ManagersFolderItem { get { return !string.IsNullOrEmpty(ManagersFolderItemId) ? ContextExtension.CurrentDatabase.GetItem(ManagersFolderItemId) : null; } }

            public static readonly string AllocationApproachItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.ShareContent.Investments.AllocationApproachFolder");
            public static Item AllocationApproachFolderItem { get { return !string.IsNullOrEmpty(AllocationApproachItemId) ? ContextExtension.CurrentDatabase.GetItem(AllocationApproachItemId) : null; } }

            public static readonly string StrategitsFolderItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.ShareContent.Investments.StrategitsFolder");
            public static Item StrategitsFolderItem { get { return !string.IsNullOrEmpty(StrategitsFolderItemId) ? ContextExtension.CurrentDatabase.GetItem(StrategitsFolderItemId) : null; } }

            public static readonly string InvestmentsRoot = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.Root");
            public static Item InvestmentsRootItem { get { return !string.IsNullOrEmpty(InvestmentsRoot) ? ContextExtension.CurrentDatabase.GetItem(InvestmentsRoot) : null; } }

            public static readonly string ResearchClientViewItemId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.ResearchClientView");
            public static Item ResearchClientViewItem { get { return !string.IsNullOrEmpty(ResearchClientViewItemId) ? ContextExtension.CurrentDatabase.GetItem(ResearchClientViewItemId) : null; } }

            /// <summary>
            /// Item that represents the link displayed in the right side of the second level menu on all the investments page
            /// </summary>
            public static readonly string ProductLinkId = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.ShareContent.Links.Investments.ProductLink");
            public static Item ProductLink { get { return !string.IsNullOrEmpty(ProductLinkId) ? ContextExtension.CurrentDatabase.GetItem(ProductLinkId) : null; } }
        }

        public sealed class Audiences
        {
            public const string Advisor = "advisor";
            public const string Client = "client";
        }

        public sealed class AudienceOptions
        {
            public const string Advisor = "Advisor Only";
            public const string Client = "Client Facing";
        }


        public sealed class Indexes
        {
            public const string InvestmentsResearchIndex = "sitecore_web_investment_research_index";
            public const string InvestmentManagerStrategiesIndex = "sitecore_web_investment_manager_strategies_index";

            public sealed class Fields
            {
                public const string Id = "id";
                public const string Path = "path";
                public const string Url = "url";
                public const string Title = "title";
                public const string Content = "contentfile";
                public const string Manager = "manager";
                public const string ManagerId = "managerid";
                public const string Strategist = "strategist";
                public const string StrategistId = "strategistid";
                public const string AllocationApproach = "allocationapproach";
                public const string AllocationApproachId = "allocationapproachid";
                public const string Category = "category";
                public const string CategoryId = "categoryid";
                public const string Icon = "icon";
                public const string Source = "source";
                public const string SourceId = "sourceid";
                public const string Date = "date";
                public const string Audience = "audience";
                public const string Constant = "constant";
                public const string Template = "_template";
                public const string Style = "style";
                public const string IMA = "ima";
                public const string ManagerSelect = "managerselect";
                public const string Group = "group";
                public const string Custodian = "custodian";
                public const string CustodianId = "custodianid";
                public const string SolutionType = "solutionType";
                public const string SolutionTypeId = "solutionTypeid";
                public const string Extension = "extension";
                public const string OmnitureId = "omnitureId";
                public const string UpdatedDate = "_updated";
            }
        }

        public static class QueryParameters
        {
            public const string AllocationApproach = "allocationapproach";
        }

        /*To relate Result DataMember*/
        public static class DataFields
        {
            public const string Source = "Source";
            public const string Category = "Category";
            public const string Manager = "Manager";
            public const string Strategist = "Strategist";
            public const string AllocationApproach = "AllocationApproach";
            public const string Audience = "Audience";
            public const string Date = "Date";
            public const string Title = "Title";
        }
    }
}
