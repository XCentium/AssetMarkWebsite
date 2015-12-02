using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using System.Web.UI.WebControls;

using Sitecore.Data.Fields;
using ServerLogic.SitecoreExt;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Genworth.SitecoreExt.Helpers;

namespace Genworth.SitecoreExt
{
    public enum DocumentType
    {
        Invalid,
        Pdf,
        Excel,
        Word,
        Video,
        PowerPoint,
        Unknown,
    }

    public static class GenworthItemExtension
    {
        public static void ConfigureHyperlink(this Item oItem, HyperLink oHyperLink)
        {
            string sTarget;

            //is the item non-null?
            if (oItem != null)
            {
                //configure the link to either open in a specified target OR open in a shadow box
                if (oItem.InstanceOfTemplate("Link"))
                {
                    if ((sTarget = oItem.GetText("Target")).Length > 0)
                    {
                        oHyperLink.Target = sTarget;
                    }

                    //set the url on the link
                    oHyperLink.NavigateUrl = oItem.GetURL();
                }
                else if (oItem.InstanceOfTemplate("Document Base") && !oItem.InstanceOfTemplate("Web Base"))
                {
                    //link directly to document file
                    oHyperLink.Attributes["href"] = oItem.GetImageURL("Document", "File");

                    string strLinkText = oItem.GetText("Document", "Link Text");
                    if (!String.IsNullOrEmpty(strLinkText))
                    {
                        oHyperLink.Text = strLinkText;
                    }
                }
                else
                {
                    //set the url on the link
                    oHyperLink.NavigateUrl = oItem.GetURL();
                }

                //is this a shadow?
                if (oItem.InstanceOfTemplate("Shadow Box"))
                {
                    oHyperLink.Attributes["rel"] = "shadowbox;options{dimensions:height=640;width=940}";
                }

                
            }
        }

        public static void ConfigureVideoShadowbox(this Item oItem, HyperLink oHyperLink)
        {
            if (oItem != null && oItem.InstanceOfTemplate("Video"))
            {
                // build the shadowbox URL
                //
                oHyperLink.NavigateUrl = string.Format("{0}?shadowmode=1", oItem.GetURL());

                oHyperLink.Attributes.Add("rel", string.Format("shadowbox;height={0};width={1};",
                    oItem.GetText("Height"), oItem.GetText("Width")));
            }
        }

		public static string ConfigureDocumentHyperlink(this Item oItem, HyperLink oHyperLink, string sSection, string sField)
        {
            string sClassName = null;
			if (oHyperLink != null && oItem != null)
			{
				var mediaItem = oItem.GetMediaItem(sSection, sField);
				sClassName = ConfigureDocumentHyperlink(mediaItem, oHyperLink);
			}
			return sClassName;
		}


		public static string ConfigureDocumentHyperlink(MediaItem oMediaItem, HyperLink oHyperLink)
            {
			string sClassName = null;
			if (oMediaItem != null && oHyperLink != null)
                {
                    switch (GetDocumentType(oMediaItem))
                    {
                        case DocumentType.Pdf:
                            sClassName = "pdf";
                            break;

                        case DocumentType.Excel:
                            sClassName = "xls";
                            break;

                        case DocumentType.Word:
                            sClassName = "doc";
                            break;

                        case DocumentType.PowerPoint:
                            sClassName = "ppt";
                            break;

                        default:
                            break;
                    }

                    oHyperLink.Target = "_blank";
                    oHyperLink.NavigateUrl = String.Format("~/{0}", ItemExtension.GetMediaURL(oMediaItem));
                }
			return sClassName;
		}

		public static void ConfigureDocumentHyperlink(this Item oItem, HyperLink oHyperLink, AttributeCollection oAttributes = null)
		{
			MediaItem oMediaItem;
			string sClassName = null;
			bool bIsLink;

			if (oHyperLink != null && oItem != null)
			{
				if ((oMediaItem = GetMediaItem(oItem)) != null)
				{
					sClassName = ConfigureDocumentHyperlink(oMediaItem, oHyperLink);
				}
                else if (oItem.InstanceOfTemplate("Custodial Service and Form"))
                {
                    sClassName = "form";
                    oHyperLink.NavigateUrl = oItem.GetURL();
                }
                else if (bIsLink = oItem.InstanceOfTemplate("Link") || oItem.InstanceOfTemplate("Article") || oItem.InstanceOfTemplate("Web Page"))
                {
                    sClassName = "html";
                    oItem.ConfigureHyperlink(oHyperLink);

                    // if is an external link open it in a new window even if the content editor
                    // forgot to enter "_blank" on the "Target" field
                    if (bIsLink && oItem.GetText("URL").Trim().Count() > 0)
                    {
                        oHyperLink.Target = "_blank";
                    }
                }

                if (oAttributes != null && sClassName != null)
                {
                    oAttributes.Add("class", sClassName);
                }

                
            }
        }

        /// <summary>
        /// Given an item, returns the document type of the Sitecore MediaItem it refers to.
        /// For example, items inheriting from "Document Base" have a "File" field containing a media item.
        /// This function returns the document type of that media item.
        /// </summary>
        /// <param name="oItem"></param>
        /// <returns></returns>
        //public static DocumentType GetDocumentType(this Item oItem)
        //{
        //    MediaItem oMediaItem;
        //    DocumentType retVal = DocumentType.Invalid;

        //    // first check for video documents and links to other pages (aka unknow documents)
        //    //
        //    if (oItem.InstanceOfTemplate("Video Base"))
        //    {
        //        retVal = DocumentType.Video;
        //    }
        //    else if (oItem.InstanceOfTemplate("Link Base"))
        //    {
        //        retVal = DocumentType.Unknown;
        //    }
        //    else if ((oMediaItem = GetMediaItem(oItem)) != null)
        //    {
        //        retVal = GetDocumentType(oMediaItem);
        //    }

        //    return retVal;
        //}

        private static DocumentType GetDocumentType(MediaItem oMediaItem)
        {
            DocumentType retVal = DocumentType.Invalid;

            switch (oMediaItem.Extension)
            {
                case "pdf":
                    retVal = DocumentType.Pdf;
                    break;
                case "xls":
                case "xlsx":
                    retVal = DocumentType.Excel;
                    break;
                case "doc":
                case "docx":
                    retVal = DocumentType.Word;
                    break;
                case "ppt":
                case "pptx":
                    retVal = DocumentType.PowerPoint;
                    break;
            }

            return retVal;
        }

        /// <summary>
        /// Given an item, returns the Sitecore MediaItem it refers to. For example, items inheriting
        /// from "Document Base" have a "File" field containing a media item.
        /// </summary>
        /// <param name="oItem">Items inheriting from Video Base or Document Base</param>
        /// <returns>Sitecore MediaItem. Null if none exists.</returns>
        /// 
        public static MediaItem GetMediaItem(this Item oItem)
        {
            Item retVal = null;

            if (oItem.InstanceOfTemplate("Document Base"))
            {
                // using this methiod to retreive the media item, as there seems to be a bug in
                // this call, which is returning null --> oItem.GetListItem("Document", "File");
                //
                retVal = ((FileField)oItem.GetField("Document", "File")).MediaItem;
            }
            else if (oItem.InstanceOfTemplate("Video Base"))
            {
                retVal = oItem.GetListItem("Video", "Media");
            }

            return retVal;
        }
        
		public static MediaItem GetMediaItem(this Item oItem, string Section, string Field)
		{
			Item response = null;
			FileField oField = (FileField)oItem.GetField(Section, Field);
			if (oField != null)
			{
				response = oField.MediaItem;
			}
			return response;
		}

        #region Omniture Methods

        /// <summary>
        /// Function to set omniture tags to a web control
        /// </summary>
        /// <param name="item"></param>
        /// <param name="hyperLink"></param>
        /// <param name="additionalInfo"></param>
        public static void ConfigureOmnitureControl(this Item item, Item locationItem, WebControl hyperLink)
        {
            hyperLink.CssClass += " " + Genworth.SitecoreExt.Constants.Omniture.CssClass;
            hyperLink.Attributes.Add(Genworth.SitecoreExt.Constants.Omniture.Attribute, OmnitureHelper.GetOmnitureParameter(locationItem, item));
        }

        /// <summary>
        /// Function to set omniture tags to a html control
        /// </summary>
        /// <param name="item"></param>
        /// <param name="hyperLink"></param>
        /// <param name="additionalInfo"></param>
        public static void ConfigureOmnitureControl(this Item item, Item locationItem, HtmlControl hyperLink)
        {
            hyperLink.Attributes["class"] += " " + Genworth.SitecoreExt.Constants.Omniture.CssClass;
            hyperLink.Attributes.Add(Genworth.SitecoreExt.Constants.Omniture.Attribute, OmnitureHelper.GetOmnitureParameter(locationItem, item));
        }

        #endregion 

    }
}
