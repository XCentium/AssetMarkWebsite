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
    public class AssetAllocationBuilder
    {
        public static string BuildJson(Item assetAllocationItem)
        {
            InternalLinkField videoLink = assetAllocationItem.GetField("Asset Allocation 1", "Video Link");

            JsonObject allocation1 = new JsonObject();
            allocation1["SailingText"] = assetAllocationItem["Sailing Text"];
            allocation1["RowingText"] = assetAllocationItem["Rowing Text"];
            allocation1["AlternativeText"] = assetAllocationItem["Alternative Text"];
            allocation1["Video"] = videoLink.TargetItem != null ? videoLink.TargetID.Guid.ToString() : "";

            JsonObject allocation2 = new JsonObject();
            allocation2["IntroductionText"] = assetAllocationItem["Introduction Text"];
            allocation2["StrategicText"] = assetAllocationItem["Strategic Text"];
            allocation2["TacticalConstrainedText"] = assetAllocationItem["Tactical Constrained Text"];
            allocation2["TacticalUnconstrainedText"] = assetAllocationItem["Tactical Unconstrained Text"];
            allocation2["AbsoluteReturnText"] = assetAllocationItem["Absolute Return Text"];
            allocation2["AlternativeInvestmentsText"] = assetAllocationItem["Alternative Investments Text"];

            JsonObject json = new JsonObject();
            json["Allocation1"] = allocation1;
            json["Allocation2"] = allocation2;
            return json.ToString();
        }
    }
}