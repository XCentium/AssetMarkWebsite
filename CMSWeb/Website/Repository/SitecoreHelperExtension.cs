using Sitecore.Data.Items;
using System.Web;
using Sitecore.Mvc;
using Sitecore.Mvc.Helpers;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Collections;
using ServerLogic.SitecoreExt;
using Sitecore.Mvc.Presentation;
namespace MvcAssetmark.Repository
{
    public static class SitecoreFieldHelper
    {
        public static HtmlString ImageLink(this SitecoreHelper helper, Item item, string urlField = "Url", string imageField = "Image")
        {
            return ContentLink(helper, item, urlField, imageField);
        }

        public static HtmlString ContentLink(this SitecoreHelper helper, Item item, string urlField, string contentField)
        {
            return helper.Field(urlField, item, new { text = helper.Field(contentField, item) });
        }

        public static HtmlString LinkDisplayName(this SitecoreHelper helper, string urlField, Item item, SafeDictionary<string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new SafeDictionary<string>();
            }

            parameters.Add("text", item.DisplayName);

            var response = helper.LinkItem(urlField, item, parameters);
            return response;
        }

        public static HtmlString LinkItem(this SitecoreHelper helper, string urlField, Item item, SafeDictionary<string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new SafeDictionary<string>();
            }

            HtmlString response = null;

            if (item.InstanceOfTemplate("Link"))
            {
                response = helper.Field(urlField, item, new { Parameters = parameters });
            }
            else
            {
                var param = string.Join(" ", parameters.Select(f => string.Format("{0}='{1}'", f.Key, f.Value)));
                response = new HtmlString(string.Format("<a href='{0}' {2}>{1}</a>", item.GetURL(), parameters["text"], param));
            }
            return response;
        }

        public static string LinkItem(this Item item)
        {
            return string.Join(" - ", item.GetParentItems().Select(f => f.DisplayName));
        }

        public static HtmlString LayoutTitle(this SitecoreHelper helper, System.Func<Item, Item, string> predicate, int childRootLevel = 1, string instanceOfTemplate = "Web Page")
        {
            var contextItem = RenderingContext.Current.ContextItem;
            var parents = contextItem.GetParentItems();
            var rootItem = parents.LastOrDefault();
            Item currentItem = null;

            // Avoid extra work if there are no children to process
            if (parents.Count > childRootLevel)
            {
                // jump to the home element, where the children items are and get the elements that has the correct template
                currentItem = parents.Take(parents.Count - childRootLevel).Where(f => f.InstanceOfTemplate(instanceOfTemplate)).FirstOrDefault();
            }

            // Func<Item, Item, string>
            var title = predicate(rootItem, currentItem);
            return new HtmlString(title);
        }
    }
}
