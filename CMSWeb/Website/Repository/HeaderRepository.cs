using MvcAssetmark.Models.Navigation;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServerLogic.SitecoreExt;
using Sitecore.Mvc.Presentation;

namespace MvcAssetmark.Repository
{
    public class HeaderRepository
    {
        private Item DataSourceItem;
        private RenderingContext renderingContext;
        private Item currentPage { get; set; }


        public HeaderRepository(RenderingContext renderingContext)
        {
            this.renderingContext = renderingContext;
            this.DataSourceItem = renderingContext.Rendering.Item;
            this.currentPage = renderingContext.PageContext.Item;
        }

        internal List<MenuItem> GetMenuItems()
        {
            var response = new List<MenuItem>();
            var webPages = DataSourceItem.GetChildren().Where(f => f.GetText("Include in Navigation") == "1");

            foreach (var page in webPages)
            {
                response.Add(new MenuItem()
                {
                    Name = page.DisplayName,
                    Url = page.GetURL(),
                    Selected = page.InSelectedPath()
                });
            }
            return response;
        }

        internal List<Item> GetImageMenuItems()
        {
            return DataSourceItem.GetChildrenOfTemplate("Image Link");
        }


        internal Item GetHomeLinkImage()
        {
            return ContextExtension.CurrentDatabase.GetItem(Sitecore.Configuration.Settings.GetSetting("Assetmark.HomeImageLink", string.Empty));
        }

        internal List<KeyValuePair<Item, bool>> GetGpsMenu()
        {
            var response = GetItemsActive(DataSourceItem);

            return response;
        }

        internal List<KeyValuePair<Item, bool>> GetGpsSecondaryMenu()
        {
            var currentItem = currentPage.GetParentItems().Reverse<Item>().Skip(2).FirstOrDefault();
            var response = GetItemsActive(currentItem);
            
            return response;
        }

        private List<KeyValuePair<Item, bool>> GetItemsActive(Item currentItem)
        {
            var items = currentItem.GetChildren().Where(f => f.GetText("Include in Navigation") == "1");
            var response = new List<KeyValuePair<Item, bool>>();
            var parentIds = currentPage.GetParentItems().Select(oItem => oItem.ID).ToList();

            foreach (var i in items)
            {
                bool selected = parentIds.Contains(i.ID);
                response.Add(new KeyValuePair<Item, bool>(i, selected));
            }
            return response;
        }

        internal List<KeyValuePair<Item, bool>> GetRetirementMenu()
        {
            var response = GetItemsActive(DataSourceItem);

            return response;
        }
    }
}