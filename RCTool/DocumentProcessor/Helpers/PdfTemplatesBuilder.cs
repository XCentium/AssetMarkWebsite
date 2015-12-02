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
    public class PdfTemplatesBuilder
    {
        public static string BuildJson(Item pdfTemplatesItem)
        {
            Item questionnaireItem = pdfTemplatesItem.GetChildByName("Discovery Questionnaire");
            Item performanceItem = pdfTemplatesItem.GetChildByName("Performance");
            Item detailsItem = pdfTemplatesItem.GetChildByName("Portfolio Details");

            JsonObject json = new JsonObject();

            json["DiscoveryQuestionnaire"] = new JsonObject
            {
                { "TemplateId", ToAssetId(questionnaireItem, "Template") }
            };

            json["Performance"] = new JsonObject
            {
                { "GrossTemplateId", ToAssetId(performanceItem, "Gross Template") },
                { "NetTemplateId", ToAssetId(performanceItem, "Net Template") },
                { "HeaderColumn1", performanceItem["Header Column 1"] },
                { "HeaderColumn2", performanceItem["Header Column 2"] },
                { "HeaderColumn3", performanceItem["Header Column 3"] },
                { "HeaderColumn4", performanceItem["Header Column 4"] },
                { "HeaderColumn5", performanceItem["Header Column 5"] },
                { "HeaderColumn6", performanceItem["Header Column 6"] },
                { "HeaderColumn7", performanceItem["Header Column 7"] },
                { "HeaderColumn8", performanceItem["Header Column 8"] }
            };

            json["PortfolioDetails"] = new JsonObject
            {
                { "TemplateId", ToAssetId(detailsItem, "Template") }
            };

            return json.ToString();
        }

        private static string ToAssetId(Item item, string fieldName)
        {
            InternalLinkField link = item.GetField(fieldName);
            return link.TargetID.Guid.ToString();
        }
    }
}