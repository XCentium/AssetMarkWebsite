using MvcAssetmark.Models.Navigation;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServerLogic.SitecoreExt;
using MvcAssetmark.Repository;

namespace MvcAssetmark.Controllers
{
    public class NavigationController : Controller
    {
        public ActionResult Header()
        {
            var repository = new HeaderRepository(RenderingContext.Current);
            var model = new HeaderModel()
            {
                ImageLinkCollection = repository.GetImageMenuItems(),
                MenuItemCollection = repository.GetMenuItems(),
                HomeLink = repository.GetHomeLinkImage()
            };
            return View(model);
        }

        public ActionResult Menu()
        {
            var model = new MenuModel();
            return View(model);
        }

        public ActionResult GpsMenu()
        {
            var repository = new HeaderRepository(RenderingContext.Current);
            var model = repository.GetGpsMenu();
            return View("~/Views/Microsites/Gps/Navigation/Menu.cshtml", model);
        }

        public ActionResult GpsSecondaryMenu()
        {
            var repository = new HeaderRepository(RenderingContext.Current);
            var model = repository.GetGpsSecondaryMenu();
            return View("~/Views/Microsites/Gps/Navigation/MenuSecondary.cshtml", model);
        }

        public ActionResult RetirementMenu()
        {
            var repository = new HeaderRepository(RenderingContext.Current);
            var model = repository.GetRetirementMenu();
            return View("~/Views/Microsites/Retirement/Menu.cshtml", model);
        }
    }
}