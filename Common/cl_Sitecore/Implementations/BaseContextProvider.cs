using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using System.Web;
using Sitecore;
using System.Globalization;

namespace ServerLogic.SitecoreExt.Implementations
{
	public class BaseContextProvider : IContextProvider
	{
		private const string CURRENT_CULTURE_KEY = "ServerLogic.SitecoreExt.Implementations.BaseContextProvider.CurrentCulture";
		private Database oCurrentDatabase;

		public virtual Database CurrentDatabase
		{
			get
			{
				string sDatabaseName;

				//have we loaded the current database before
				if (oCurrentDatabase == null)
				{
					//check to see if a setting for the default databsae was provided
					if (!string.IsNullOrEmpty(sDatabaseName = Sitecore.Configuration.Settings.GetSetting("ServerLogic.SitecoreExtension.ContentDatabase")))
					{
						//a setting was provided, use it. If the database is not found, default to content database, otherwise default to database
						oCurrentDatabase = Sitecore.Data.Database.GetDatabase(sDatabaseName) ?? Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
					}
					else
					{
						//default databsae should be content database. If null, default to database
						oCurrentDatabase = Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database;
					}
				}

				//return the content databsae
				return oCurrentDatabase;
			}
		}

		public virtual Item CurrentItem
		{
			get
			{
				return Sitecore.Context.Item;
			}
		}

		public virtual Item CurrentParentItem
		{
			get
			{
				return CurrentItem.Parent;
			}
		}


		public virtual List<Item> CurrentParentItems
		{
			get
			{
				List<Item> oItems;

				//create the list
				oItems = new List<Item>();

				//add the current item to the list
				oItems.Add(CurrentItem);

				//continue adding to the list
				CurrentParentItem.GetParentItems(oItems);

				//return the items
				return oItems;
			}
		}

		public CultureInfo CurrentCulture
		{
			get
			{
				object oObject;
				CultureInfo oCurrentCulture;

				//get the object
				if ((oObject = HttpContext.Current.Items[CURRENT_CULTURE_KEY]) != null && oObject is CultureInfo)
				{
					//get the root item out of the http context
					oCurrentCulture = (CultureInfo)oObject;
				}
				else
				{
					// query the DB for the root item (current language is accounted for)
					oCurrentCulture = Sitecore.Context.Culture;

					//is the root item non-null?
					if (oCurrentCulture != null)
					{
						//put the root item into the http context to ensure we can get it a second time
						HttpContext.Current.Items[CURRENT_CULTURE_KEY] = oCurrentCulture;
					}
				}

				//return the root item
				return oCurrentCulture;
			}
		}

		public string CurrentLanguageCode
		{
			get
			{
				return CurrentCulture.TwoLetterISOLanguageName.ToLower();
			}
		}
	}
}
