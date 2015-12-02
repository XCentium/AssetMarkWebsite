using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAssetmark.Models.Navigation
{
    public class HeaderModel
    {
        public List<Item> ImageLinkCollection { get; set; }

        public List<MenuItem> MenuItemCollection { get; set; }

        public Item HomeLink { get; set; }
    }
}