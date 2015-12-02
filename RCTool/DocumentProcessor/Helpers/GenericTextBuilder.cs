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
    public class GenericTextBuilder
    {
        public static string BuildJson(Item textItemsFolder)
        {
            JsonObject json = new JsonObject();
            var textItems = textItemsFolder.Axes.GetDescendants().Where(
                                                                    itm => itm.TemplateID.ToString() == "{8EABDF36-4EF1-4AC6-BFD5-46A9706259C6}"
                                                                    ).ToList<Item>();

            foreach (var itm in textItems)
            {
                json[itm.Fields["Name"].Value] = itm["Body"];
            }
            return json.ToString();
        }
    }
}