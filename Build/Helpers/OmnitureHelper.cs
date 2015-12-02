using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Sitecore.Web.UI;
using System.Web.UI.HtmlControls;

namespace Genworth.SitecoreExt.Helpers
{
    public static class OmnitureHelper
    {
             
        internal static string GetOmnitureParameter(Item locationItem, Item item)
        {
            string result = string.Empty;
            string format = "CMS:: {0} :: {1}";

            if (locationItem != null && item != null)
            {
                result = string.Format(format, FormatItemId(locationItem), FormatItemId(GetContentItem(item)));
            }

            return result;
        }

        internal static string FormatItemId(Item item)
        {
            string result = string.Empty;
            string id = string.Empty;
            if (item != null && !item.ID.IsNull && !string.IsNullOrEmpty(id = item.ID.ToString()))
            {
                result = id.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
            }

            return result;
        }

        internal static Item GetContentItem(Item item)
        {
            Item result = item;
            FileField fileField = null;
            Field field = null;
            Item tmpItem = null;

            if (item != null)
            {
                if ((item.InstanceOfTemplate("Document Base") || item.InstanceOfTemplate("Brochure"))
                    && (field = item.GetField("Document", "File")) != null && (fileField = (FileField)field).MediaItem != null)
                {
                    result = fileField.MediaItem;
                }
                else if (item.InstanceOfTemplate("Video Base") && (tmpItem = item.GetListItem("Video", "Media")) != null)
                {
                    result = tmpItem;
                }
                else if (item.InstanceOfTemplate("Solution")
                     && (field = item.GetField("Documents", "Fact Sheet")) != null
                    && (tmpItem = field.GetItem())!= null)
                {
                    result = GetContentItem(tmpItem);
                }
                else if (item.InstanceOfTemplate("Asset Allocation Approach"))
                {
                    Item doc;
                    //first start with month performance
                    if ((doc = item.GetListItem("Documents", "Calendar Month Performance")) == null)
                    {
                        //if no month performance, move on to year performance
                        doc = item.GetListItem("Documents", "Calendar Year Performance");
                    }

                    if (doc != null && (tmpItem = GetContentItem(doc)) != null)
                    {
                        result = tmpItem;
                    }
                }
                else if (item.InstanceOfTemplate("Link") && (field = item.GetField("Link", "Item")) != null
                     && (tmpItem = field.GetItem()) != null)
                {
                    result = GetContentItem(tmpItem);
                }
                else if ((field = item.GetField("Document", "File")) != null && (fileField = (FileField)field).MediaItem != null)
                {
                    result = fileField.MediaItem;
                }
            }

            return result;
        }
    }
}
