using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Data.Items;
using System.Web;

namespace ServerLogic.SitecoreExt.Utilities
{
	/// <summary>
	/// A Sticky Link is a type of link that has a default URL, 
	/// but if you happen to navigate to a page beneath the 
	/// Sticky Link, the Sticky Link will no longer point to 
	/// the default URL, but to the page beneath the Sticky Link 
	/// that you visited. Take for instance the following 
	/// navigation tree:
	///   Home
	///   |- Products 
	///      |- Products Home
	///      |- Visual Studio
	///      |- SQL Server
	///      |- Sitecore
	///      |- Windows
	///   |- Services
	///   |- Company
	/// 
	/// In this case, Home/Products is a Sticky Link with a default destimation of: Home/Products/Products Home.
	/// 
	/// If a user clicks on Home/Products in the navigation, they will be taken to Home/Products/Products Home. 
	/// If the user should visit Home/Products/Visual Studio, subsequent clicks on Home/Products in the navigation will take the user to Home/Products/Visual Studio instead.
	/// </summary>
	public class StickyLinkManager
	{
		/// <summary>
		/// This string format represents the sticky session key used to find a session key by Layout and Sticky Link
		/// </summary>
		private const string STICKY_SESSION_KEY = "ServerLogic.SitecoreExt.Utilities.StickyLinkManager:{0}|{1}";

		/// <summary>
		/// This string represents the name of the template used for sticky links.
		/// </summary>
		public const string STICKY_LINK_TEMPLATE_NAME = "Sticky Link";

		/// <summary>
		/// Calculates a session key used to store or retrieve the Sticky URL from the user's current session.
		/// </summary>
		private static string CalculateSessionKey(Item oStickyLinkItem)
		{
			return string.Format(STICKY_SESSION_KEY, Sitecore.Context.Item.Visualization.GetLayoutID(Sitecore.Context.Device), oStickyLinkItem.ID.ToString());
		}

		/// <summary>
		/// This method ONLY returns a value IF there is a sticky URL associated with this Sticky Link. 
		/// </summary>
		/// <param name="oItem"></param>
		/// <returns></returns>
		internal static string GetStickyLinkURL(Item oStickyLinkItem)
		{
			//we only get sticky link URLs for pages that are actually sticky links.
			return oStickyLinkItem.InstanceOfTemplate(STICKY_LINK_TEMPLATE_NAME) ? GetStickyLinkURL(oStickyLinkItem, System.Web.HttpContext.Current) : null;
		}

		private static string GetStickyLinkURL(Item oStickyLinkItem, HttpContext oContext)
		{
			string sKey;
			object oObject;

			//get the key for the current layout and item
			sKey = CalculateSessionKey(oStickyLinkItem);

			//get the object from the session, be sure it is not null and that it is a string
			return (oObject = oContext.Session[sKey]) != null && oObject is string ? (string)oObject : string.Empty;
		}

		/// <summary>
		/// This method looks at the current Item being viewed. If the current 
		/// item is not a sticky link, all parent sticky links will have a 
		/// session value created pointing back to this item.
		/// </summary>
		public static void Stick()
		{
			Item oCurrentItem;

			//get the current item
			oCurrentItem = ContextExtension.CurrentItem;

			if (oCurrentItem != null)
			{
				//output some debugging info
				Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Utilities.StickyLinkManager.Stick - Attempting to Stick to Item {0}.", oCurrentItem.ID.ToString()), typeof(StickyLinkManager));

				//we only want to perform a stick operation IF the current item is NOT a sticky link.
				if (!oCurrentItem.InstanceOfTemplate(STICKY_LINK_TEMPLATE_NAME))
				{
					//continue to stick
					Stick(oCurrentItem.GetURL(), System.Web.HttpContext.Current);
				}
			}
			else
			{
				//output some debugging info
				Sitecore.Diagnostics.Log.Info("ServerLogic.SitecoreExt.Utilities.StickyLinkManager.Stick - The current item is null. Cannot stick to a null item.", typeof(StickyLinkManager));
			}
		}

		private static void Stick(string sURL, HttpContext oContext)
		{
			List<Item> oStickyLinkItems;
			string sKey;

			//get a collection of current parent items that are sticky links
			oStickyLinkItems = ContextExtension.CurrentParentItems.GetItemsOfTemplate(STICKY_LINK_TEMPLATE_NAME);

			//do we have sticky link items?
			if (oStickyLinkItems.Count > 0)
			{
				//set the session value
				foreach (Item oStickyLinkItem in oStickyLinkItems)
				{
					//get the key for the current layout and item
					sKey = CalculateSessionKey(oStickyLinkItem);

					//output some debugging info
					Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Utilities.StickyLinkManager.Stick - Sticking Item {0} to Sticky Link {1} with Session Key {2}.", sURL, oStickyLinkItem.ID.ToString(), sKey), typeof(StickyLinkManager));

					//store the URL
					oContext.Session[sKey] = sURL;
				}
			}
			else
			{
				//output some debugging info
				Sitecore.Diagnostics.Log.Info(string.Format("ServerLogic.SitecoreExt.Utilities.StickyLinkManager.Stick - Item {0} has no parent Sticky Links.", sURL), typeof(StickyLinkManager));
			}
		}
	}
}
