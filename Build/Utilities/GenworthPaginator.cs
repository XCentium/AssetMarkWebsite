using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Data.Items;


namespace Genworth.SitecoreExt.Utilities
{
	public static class GenworthPaginator
	{
		/// <summary>
		/// Returns a Genworth specific paginator, which is customizable per section (e.g. a page or site area, such as Help Center)
		/// The default is to use the pagination settings specified in the config file. If you need to customize per section, add
		/// your code to the applicable case label.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="sPrefix"></param>
		/// <returns></returns>
		public static Paginator<Item> GetPaginator(IEnumerable<Item> items, string sSection = "", string sPrefix = "")
		{
			int iPerPage;
			int iMaxPerPage;
			int iMinPerPage;

			switch (sSection)
			{
				case "EventList":
				case "ArticleList":
				case "GlossaryList":
				case "FAQList":
				case "":
				default:
					// use default pagination settings
					//
					iPerPage = int.TryParse(Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pagination.PerPage"), out iPerPage) ? iPerPage : 10;
					iMaxPerPage = int.TryParse(Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pagination.MaxPerPage"), out iMaxPerPage) ? iMaxPerPage : 1000;
					iMinPerPage = int.TryParse(Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pagination.MinPerPage"), out iMinPerPage) ? iMinPerPage : 10;
					break;
			}

			return Paginator<Item>.GetPaginator(items, iPerPage, sPrefix, iMinPerPage, iMaxPerPage);
		}
	}
}
