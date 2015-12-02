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
    public class QuarterlyInvestmentReviewBuilder
    {
        public static string BuildJson(Item qirFolderItem)
        {
            JsonArray qirs = new JsonArray();

            foreach (Item qirItem in qirFolderItem.Children)
            {
                JsonArray sections = new JsonArray();

                foreach (Item qirSectionItem in qirItem.Children)
                {
                    JsonArray pages = new JsonArray();
                    foreach (Item qirPagesItem in qirSectionItem.Children)
                    {
                        JsonObject pageRange = new JsonObject();
                        pageRange["Id"] = qirPagesItem.ID.Guid.ToString();
                        pageRange["Name"] = qirPagesItem["Name"];
                        pageRange["StartPage"] = Int32.Parse(qirPagesItem["Start Page"]);
                        pageRange["EndPage"] = Int32.Parse(qirPagesItem["End Page"]);
                        pageRange["AlwaysIncluded"] = ((CheckboxField)qirPagesItem.GetField("Always Included")).Checked;
                        pages.Add(pageRange);
                    }

                    InternalLinkField pdfLink = qirSectionItem.GetField("PDF");

                    JsonObject section = new JsonObject();
                    section["Id"] = qirSectionItem.ID.Guid.ToString();
                    section["Name"] = qirSectionItem["Name"];
                    section["Pdf"] = pdfLink.TargetItem != null ? pdfLink.TargetID.Guid.ToString() : "";
                    section["Pages"] = pages;
                    sections.Add(section);
                }

                JsonObject qir = new JsonObject();
                qir["Id"] = qirItem.ID.Guid.ToString();
                qir["Name"] = qirItem["Name"];
                qir["Hidden"] = ((CheckboxField)qirItem.GetField("Hidden")).Checked;
                qir["Sections"] = sections;
                qirs.Add(qir);
            }

            JsonObject json = new JsonObject();
            json["QIRs"] = qirs;
            return json.ToString();
        }
    }
}