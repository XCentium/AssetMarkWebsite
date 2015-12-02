using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;

//using ServerLogic.Parsing.Csv;
using ServerLogic.SitecoreExt;
using System.Net;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using System.IO;
using DocumentEntities;
using DocumentProcessor.Helpers;
using Manatee.Json;
using System.Xml.Linq;
using System.Globalization;

namespace AdvisorApp.Helpers
{
    public class PortfolioBuilder
    {
        public static class CONSTANTS
        {
            public static string STRATEGY_DEFAULT_RISK_PROFILES = "{F8A05F01-22B2-4ABF-B67B-A7B7741BA226}";
        }

        public static string BuildJson(Item portfolioParent, Action<MediaItem> writeMediaItem)
        {
            JsonObject json = new JsonObject();
            json["Solutions"] = BuildSolutions(portfolioParent);
            json["AllocationApproaches"] = BuildAllocationApproaches(portfolioParent, writeMediaItem);
            json["PerformanceComparison"] = BuildPerformanceComparison(portfolioParent);
            json["ModelsPerformance"] = BuildModelsPerformance(portfolioParent);
            json["ProxyPerformance"] = BuildProxyPerformance(portfolioParent);
            json["ModelsAssetClass"] = BuildModelsAssetClass(portfolioParent);
            json["EmailAttachments"] = BuildEmailAttachments(portfolioParent);
            json["FocusedSolutions"] = BuildFocusedSolutions(portfolioParent, writeMediaItem);
            json["PerformancePeriods"] = BuildPerformancePeriods(portfolioParent);
            return json.ToString();
        }

        private static JsonArray BuildPerformancePeriods(Item portfolioParent)
        {
            JsonArray periods = new JsonArray();
            foreach (Item item in portfolioParent.GetChildByName("Performance Periods").Children)
            {
                JsonObject period = new JsonObject();
                period["StartYear"] = Int32.Parse(item["Start Year"], CultureInfo.InvariantCulture);
                period["StartMonth"] = Int32.Parse(item["Start Month"], CultureInfo.InvariantCulture);
                period["EndYear"] = Int32.Parse(item["End Year"], CultureInfo.InvariantCulture);
                period["EndMonth"] = Int32.Parse(item["End Month"], CultureInfo.InvariantCulture);
                periods.Add(period);
            }
            return periods;
        }

        private static JsonArray BuildFocusedSolutions(Item portfolioParent, Action<MediaItem> writeMediaItem)
        {
            JsonArray solutions = new JsonArray();

            foreach (Item item in portfolioParent.GetChildByName("Focused Solutions").Children)
            {
                JsonObject solution = new JsonObject();

                JsonArray solutionAllocationApproaches = new JsonArray();

                MultilistField approaches = item.GetField("Allocation Approaches");
                foreach (Item approachItem in approaches.GetItems())
                {
                    int index = approachItem.Parent.Children.IndexOf(approachItem.ID);
                    if (index >= 0)
                    {
                        solutionAllocationApproaches.Add(index);
                    }
                }

                solution["Name"] = item.GetText("Name");
                solution["AllocationApproaches"] = solutionAllocationApproaches;
                solution["Performance"] = BuildPerformanceData(item);

                FileField logo = item.GetField("Logo");

                InternalLinkField videoLink = item.GetField("Video");

                if (logo.MediaItem != null)
                    writeMediaItem(logo.MediaItem);

                solution["Description"] = item["Description"];
                solution["Video"] = videoLink.TargetItem != null ? videoLink.TargetID.Guid.ToString() : "";
                solution["Logo"] = logo.MediaItem != null ? logo.MediaItem.ID.Guid.ToString() + "." + ((MediaItem)logo.MediaItem).Extension : "";

                solutions.Add(solution);
            }

            return solutions;
        }

        private static JsonObject BuildSolutions(Item portfolioParent)
        {
            Item solutionsItem = portfolioParent.GetChildByName("Solutions");
            if (solutionsItem == null)
                return new JsonObject();
            else
                return BuildSolutionsFolder(solutionsItem);
        }

        private static JsonObject BuildSolutionsFolder(Item solutionsItem)
        {
            JsonObject solutions = new JsonObject();

            foreach (Item item in solutionsItem.GetChildren())
            {
                string name = item["Name"];

                if (item.TemplateID.ToString() == "{6F62EB4C-456C-4103-AFAD-D589F3096620}")
                {
                    solutions[name] = BuildSolutionsFolder(item);
                }
                else if (item.TemplateID.ToString() == "{264E1917-574F-4847-8010-38BACA46EE8D}")
                {
                    solutions[name] = BuildSolution(item);
                }
            }

            return solutions;
        }

        private static JsonObject BuildSolution(Item item)
        {
            return new JsonObject
            {
                { "Title", item.GetText("Solution", "Title") },
                { "FactSheet", ToAssetId(item, "Fact Sheet") },
                { "Performance", BuildPerformanceData(item) },
                { "AllowCustomization", ((CheckboxField)item.GetField("Allow Customization")).Checked },
                { "CustomCoverSheet", ToAssetId(item, "Custom Cover Sheet") }
            };
        }

        private static JsonObject BuildPerformanceData(Item item)
        {
            JsonArray modelIds = new JsonArray
            {
                item.GetText("Model ID Profile 1"),
                item.GetText("Model ID Profile 2"),
                item.GetText("Model ID Profile 3"),
                item.GetText("Model ID Profile 4"),
                item.GetText("Model ID Profile 5"),
                item.GetText("Model ID Profile 6")
            };

            return new JsonObject
            {
                { "Fee", item.GetText("Annual Fee") },
                { "ModelIDs", modelIds }
            };
        }

        private static JsonArray BuildEmailAttachments(Item portfolioParent)
        {
            Item emailsItem = portfolioParent.GetChildByName("Email Attachments");

            JsonArray emails = new JsonArray();
            foreach (Item item in emailsItem.Children)
            {
                emails.Add(new JsonObject
                {
                    { "Title", item.GetText("Attachment", "Title") },
                    { "AssetId", ToAssetId(item, "Asset") }
                });
            }

            return emails;
        }

        private static JsonObject BuildModelsPerformance(Item portfolioParent)
        {
            FileField xmlField = portfolioParent.GetField("Models Performance XML");
            if (xmlField.MediaItem == null)
                return new JsonObject();

            XDocument xml;
            using (Stream stream = ((MediaItem)xmlField.MediaItem).GetMediaStream())
            {
                xml = XDocument.Load(stream);
            }

            JsonObject models = new JsonObject();

            foreach (XElement modelElement in xml.Element("Models").Elements("Model"))
            {
                string id = modelElement.Attribute("Id").Value;
                models.Add(id, BuildRawReturns(modelElement));
            }

            return models;
        }

        private static JsonObject BuildProxyPerformance(Item portfolioParent)
        {
            FileField xmlField = portfolioParent.GetField("Proxy Performance XML");
            if (xmlField.MediaItem == null)
                return new JsonObject();

            XDocument xml;
            using (Stream stream = ((MediaItem)xmlField.MediaItem).GetMediaStream())
            {
                xml = XDocument.Load(stream);
            }

            JsonObject models = new JsonObject();

            foreach (XElement modelElement in xml.Element("Proxies").Elements("Proxy"))
            {
                string id = modelElement.Attribute("Id").Value;
                models.Add(id, BuildRawReturns(modelElement));
            }

            return models;
        }

        private static JsonObject BuildModelsAssetClass(Item portfolioParent)
        {
            FileField xmlField = portfolioParent.GetField("Models Asset Class XML");
            if (xmlField.MediaItem == null)
                return new JsonObject();

            XDocument xml;
            using (Stream stream = ((MediaItem)xmlField.MediaItem).GetMediaStream())
            {
                xml = XDocument.Load(stream);
            }

            JsonObject models = new JsonObject();

            foreach (XElement modelElement in xml.Element("modelAssetClass").Elements("Model"))
            {
                string id = modelElement.Attribute("ID").Value;

                var modelJson = new JsonObject();

                foreach (XElement assetType in modelElement.Elements("Assettype"))
                {
                    if (assetType.Attribute("assettype") == null) // Workaround bug in XML
                        continue;

                    var type = assetType.Attribute("assettype").Value;

                    var classesJson = new JsonArray();
                    foreach (XElement assetClass in assetType.Elements("Assetclass"))
                    {
                        if (assetClass.Attribute("assetclass") == null || assetClass.Attribute("weight") == null) // Workaround bug in XML
                            continue;

                        classesJson.Add(new JsonObject()
                        {
                            { "Class", assetClass.Attribute("assetclass").Value },
                            { "Weight", Convert.ToDouble(assetClass.Attribute("weight").Value) }
                        });
                    }

                    var assetTypeJson = new JsonObject()
                    {
                        { "Weight", Convert.ToDouble(assetType.Attribute("weight").Value) },
                        { "Classes", classesJson }
                    };

                    modelJson.Add(type, assetTypeJson);
                }

                models.Add(id, modelJson);
            }

            return models;
        }

        private static JsonObject BuildRawReturns(XElement element)
        {
            SortedDictionary<DateTime, double> returns = new SortedDictionary<DateTime, double>();

            foreach (XElement returnElement in element.Elements("return"))
            {
                double value = Double.Parse(returnElement.Attribute("value").Value, CultureInfo.InvariantCulture);
                DateTime date = DateTime.ParseExact(returnElement.Attribute("date").Value, "yyyy-MM-dd", null);
                returns[date] = value;
            }

            JsonArray values = new JsonArray();
            int startYear = 3000;
            int startMonth = 1;

            foreach (DateTime date in returns.Keys)
            {
                if (date.Year >= 2000)
                {
                    startYear = Math.Min(startYear, date.Year);
                    if (startYear == date.Year)
                        startMonth = Math.Min(startMonth, date.Month);
                    values.Add(returns[date]);
                }
            }

            if (startYear == 3000)
                return null;

            return new JsonObject() {
                { "StartYear", startYear },
                { "StartMonth", startMonth },
                { "Values", values }
            };
        }

        private static string ToFactSheetAssetId(Item solutionsItem, string solution)
        {
            Item item = solutionsItem.GetChildByName(solution);
            if (item == null)
                return "";
            else
                return ToAssetId(item, "Fact Sheet");
        }

        private static JsonArray BuildAllocationApproaches(Item portfolioParent, Action<MediaItem> writeMediaItem)
        {
            JsonArray array = new JsonArray();

            Item approachesItem = portfolioParent.GetChildByName("Allocation Approaches");
            if (approachesItem == null)
                return array;

            foreach (Item item in approachesItem.Children)
            {
                JsonObject growWealth = new JsonObject
                {
                    { "Comprehensive", ToArray(item["Grow Wealth Comprehensive Included"]) },
                    { "ComprehensiveExcluded", ToArray(item["Grow Wealth Comprehensive Excluded"]) },
                    { "Focused", ToArray(item["Grow Wealth Focused Included"]) }
                };

                JsonObject gps = new JsonObject
                {
                    { "GrowWealth", growWealth },
                    { "GenerateIncome", ToArray(item["Generate Income Included"]) },
                    { "GenerateIncomeExcluded", ToArray(item["Generate Income Excluded"]) },
                    { "ReduceDownsideRisk", ToArray(item["Reduce Downside Risk"]) },
                    { "EnhanceIncome", ToArray(item["Enhance Income"]) }
                };

                JsonObject accumulateWealth = new JsonObject
                {
                    { "Default", ToArray(item.GetText("Goals Based", "Accumulate Wealth Default")) },
                    { "Min", ToArray(item.GetText("Goals Based", "Accumulate Wealth Min")) },
                    { "Max", ToArray(item.GetText("Goals Based", "Accumulate Wealth Max")) }
                };

                JsonObject distributeWealth = new JsonObject
                {
                    { "Default", ToArray(item.GetText("Goals Based", "Distribute Wealth Default")) },
                    { "Min", ToArray(item.GetText("Goals Based", "Distribute Wealth Min")) },
                    { "Max", ToArray(item.GetText("Goals Based", "Distribute Wealth Max")) }
                };

                JsonObject preserveWealth = new JsonObject
                {
                    { "Default", ToArray(item.GetText("Goals Based", "Preserve Wealth Default")) },
                    { "Min", ToArray(item.GetText("Goals Based", "Preserve Wealth Min")) },
                    { "Max", ToArray(item.GetText("Goals Based", "Preserve Wealth Max")) }
                };

                JsonObject openDesign = new JsonObject
                {
                    { "AccumulateWealth", accumulateWealth },
                    { "DistributeWealth", distributeWealth },
                    { "PreserveWealth", preserveWealth }
                };

                JsonObject customGpsSelect = new JsonObject
                {
                    { "EtfsAndCostAwarenessMin", ToArray(item.GetText("Custom GPS Select", "Grow Wealth ETFs Min")) },
                    { "EtfsAndCostAwarenessMax", ToArray(item.GetText("Custom GPS Select", "Grow Wealth ETFs Max")) },
                    { "ActiveMutualFundsMin", ToArray(item.GetText("Custom GPS Select", "Grow Wealth Active Mutual Funds Min")) },
                    { "ActiveMutualFundsMax", ToArray(item.GetText("Custom GPS Select", "Grow Wealth Active Mutual Funds Max")) },
                    { "GenerateIncomeMin", ToArray(item.GetText("Custom GPS Select", "Generate Income Min")) },
                    { "GenerateIncomeMax", ToArray(item.GetText("Custom GPS Select", "Generate Income Max")) },
                    { "PreserveWealthMin", ToArray(item.GetText("Custom GPS Select", "Preserve Wealth Min")) },
                    { "PreserveWealthMax", ToArray(item.GetText("Custom GPS Select", "Preserve Wealth Max")) },
                    { "ReduceVolatilityMin", ToArray(item.GetText("Custom GPS Select", "Reduce Volatility Min")) },
                    { "ReduceVolatilityMax", ToArray(item.GetText("Custom GPS Select", "Reduce Volatility Max")) },
                    { "EnhanceIncomeMin", ToArray(item.GetText("Custom GPS Select", "Enhance Income Min")) },
                    { "EnhanceIncomeMax", ToArray(item.GetText("Custom GPS Select", "Enhance Income Max")) },
                    { "ReduceDownsideRiskMin", ToArray(item.GetText("Custom GPS Select", "Reduce Downside Risk Min")) },
                    { "ReduceDownsideRiskMax", ToArray(item.GetText("Custom GPS Select", "Reduce Downside Risk Max")) }
                };

                JsonObject approach = new JsonObject
                {
                    { "Name", item["Name"] },
                    { "Gps", gps },
                    { "OpenDesign", openDesign },
                    { "CustomGpsSelect", customGpsSelect },
                    { "Strategists", BuildStrategists(item, writeMediaItem) },
                    { "Sailing", ((CheckboxField)item.GetField("Sailing")).Checked }
                };

                array.Add(approach);
            }

            return array;
        }

        private static JsonArray BuildPerformanceComparison(Item portfolioParent)
        {
            JsonArray array = new JsonArray();

            Item riskReturnParentItem = portfolioParent.GetChildByName("Performance Comparison");
            if (riskReturnParentItem == null)
                return array;

            foreach (Item child in riskReturnParentItem.Children)
            {
                if (child.TemplateName == "Performance Comparison")
                {
                    array.Add(new JsonObject {
                        { "ProxyId", child.GetText("Performance Comparison", "Proxy ID") },
                        { "Name", child.GetText("Performance Comparison", "Name") },
                        { "LegendText", child.GetText("Performance Comparison", "Legend Text") },
                        { "HistoricalName", child.GetText("Performance Comparison", "Historical Name") },
                        { "HistoricalLegendText", child.GetText("Performance Comparison", "Historical Legend Text") }
                    });
                }
            }

            return array;
        }

        private static JsonArray BuildStrategists(Item approachItem, Action<MediaItem> writeMediaItem)
        {
            JsonArray array = new JsonArray();

            foreach (Item item in approachItem.Children)
            {
                JsonArray strategies = BuildStrategies(item);

                FileField logo = item.GetField("Strategist", "Logo");

                InternalLinkField videoLink = item.GetField("Strategist", "Video");

                if (logo.MediaItem != null)
                    writeMediaItem(logo.MediaItem);

                array.Add(new JsonObject {
                    { "Name", item["Name"] },
                    { "Strategies", strategies },
                    { "Description", item["Description"] },
                    { "Video", videoLink.TargetItem != null ? videoLink.TargetID.Guid.ToString() : "" },
                    { "Logo", logo.MediaItem != null ? logo.MediaItem.ID.Guid.ToString() + "." + ((MediaItem)logo.MediaItem).Extension : "" }
                });
            }

            return array;
        }

        private static JsonArray BuildStrategies(Item strategistItem)
        {
            JsonArray array = new JsonArray();

            foreach (Item item in strategistItem.Children)
            {
                JsonObject gpsSelect = new JsonObject {
                    { "GenerateIncome", ToArray(item["Generate Income"]) },
                    { "PreserveWealth", ToArray(item["Preserve Wealth"]) },
                    { "ReduceDownsideRisk", ToArray(item["Reduce Downside Risk"]) },
                    { "EnhanceIncome", ToArray(item["Enhance Income"]) },
                    { "ReduceVolatility", ToArray(item["Reduce Volatility"]) },
                    { "EtfsAndCostAwareness", ToArray(item["Etfs and Cost Awareness"]) },
                    { "ActiveMutualFunds", ToArray(item["Active Mutual Funds"]) }
                };

                JsonArray modelIds = new JsonArray {
                    item.GetText("Model ID Profile 1"),
                    item.GetText("Model ID Profile 2"),
                    item.GetText("Model ID Profile 3"),
                    item.GetText("Model ID Profile 4"),
                    item.GetText("Model ID Profile 5"),
                    item.GetText("Model ID Profile 6")
                };

                JsonObject performance = new JsonObject {
                    { "AllowedProfiles", ToArray(item["Allowed Profiles"]) },
                    { "ModelIDs", modelIds },
                    { "Fee", item.GetText("Maximum Annual Fee") },
                };

                InternalLinkField assetLink = item.GetField("Strategy", "Fact Sheet");

                JsonObject gpsSelectDefaultRiskProfiles = null;

                foreach (Item itm in item.Children)
                {
                    if (itm.TemplateID.ToString() == CONSTANTS.STRATEGY_DEFAULT_RISK_PROFILES)
                    {
                        gpsSelectDefaultRiskProfiles = new JsonObject
                        {
                            {"EtfsandCostAwareness" , ToArray(itm.GetText("GPS Select Default Risk Profiles", "Etfs and Cost Awareness"))},
                            {"ActiveMutualFunds" , ToArray(itm.GetText("GPS Select Default Risk Profiles", "Active Mutual Funds"))},
                            {"GenerateIncome" , ToArray(itm.GetText("GPS Select Default Risk Profiles", "Generate Income"))},
                            {"PreserveWealth" , ToArray(itm.GetText("GPS Select Default Risk Profiles", "Preserve Wealth"))},
                            {"ReduceDownsideRisk" , ToArray(itm.GetText("GPS Select Default Risk Profiles", "Reduce Downside Risk"))},
                            {"EnhanceIncome" , ToArray(itm.GetText("GPS Select Default Risk Profiles", "Enhance Income"))},
                            {"ReduceVolatility" , ToArray(itm.GetText("GPS Select Default Risk Profiles", "Reduce Volatility"))}
                        };
                        // only one default risk profiles setting...
                        break;
                    }
                }

                if (gpsSelectDefaultRiskProfiles == null)
                {
                    array.Add(new JsonObject {
                        { "Name", item["Name"] },
                        { "FactSheet", assetLink.TargetID.Guid.ToString() },
                        { "CustomGpsSelectAvailable", ((CheckboxField)item.GetField("Available in Custom GPS Select")).Checked },
                        { "ByGoalsAndByStrategistAvailable", ((CheckboxField)item.GetField("Available in By Goals and By Strategists")).Checked },
                        { "MultiStrategyEligible", ((CheckboxField)item.GetField("Eligible for Multi Strategy")).Checked },
                        { "GpsSelect", gpsSelect },
                        { "Performance", performance }
                    });
                }
                else
                {
                    array.Add(new JsonObject {
                        { "Name", item["Name"] },
                        { "FactSheet", assetLink.TargetID.Guid.ToString() },
                        { "CustomGpsSelectAvailable", ((CheckboxField)item.GetField("Available in Custom GPS Select")).Checked },
                        { "ByGoalsAndByStrategistAvailable", ((CheckboxField)item.GetField("Available in By Goals and By Strategists")).Checked },
                        { "MultiStrategyEligible", ((CheckboxField)item.GetField("Eligible for Multi Strategy")).Checked },
                        { "GpsSelect", gpsSelect },
                        { "Performance", performance },
                        { "gpsSelectDefaultRiskProfiles", gpsSelectDefaultRiskProfiles }
                    });
                }
            }

            return array;
        }

        private static string ToAssetId(Item item, string fieldName)
        {
            InternalLinkField link = item.GetField(fieldName);
            return link.TargetID.Guid.ToString();
        }

        private static JsonArray ToArray(string s)
        {
            JsonArray array = new JsonArray();
            foreach (string profile in s.Split(','))
            {
                array.Add(Convert.ToDouble(profile.Trim()));
            }
            return array;
        }
    }
}