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

namespace AdvisorApp.Helpers
{
    public class GuidedPortfoliosBuilder
    {
        public static string BuildJson(Item guidedItem)
        {
            JsonObject json = new JsonObject();
            json["IntroductionText"] = guidedItem["Introduction Text"];
            json["Options"] = BuildOptions(guidedItem);
            return json.ToString();
        }

        private static JsonArray BuildOptions(Item guidedItem)
        {
            JsonArray array = new JsonArray();

            foreach (Item item in guidedItem.Children)
            {
                InternalLinkField videoLink = item.GetField("Guided Portfolios Option", "Video Link");

                JsonObject option = new JsonObject
                {
                    { "BriefText", item["Brief Text"] },
                    { "VideoLink", videoLink.TargetItem != null ? videoLink.TargetID.Guid.ToString() : "" }
                };

                array.Add(option);
            }

            return array;
        }
    }
}