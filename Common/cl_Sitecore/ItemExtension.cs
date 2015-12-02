using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore;
using Sitecore.Resources.Media;
using Sitecore.Data;
using Sitecore.Layouts;

using ServerLogic.Linq;
using System.Web;
using ServerLogic.SitecoreExt.Utilities;
using System.Collections.Specialized;

namespace ServerLogic.SitecoreExt
{
	public static class ItemExtension
	{
		private const string ROOT_ITEM_KEY = "ServerLogic.SitecoreExt.ItemExtension.RootItem";

		/// <summary>
		/// Returns the root item of the current Sitecore Context
		/// </summary>
		public static Item RootItem
		{
			get
			{
				object oObject;
				Item oRootItem;

				//get the object
				if ((oObject = HttpContext.Current.Items[ROOT_ITEM_KEY]) != null && oObject is Item)
				{
					//get the root item out of the http context
					oRootItem = (Item)oObject;
				}
				else
				{
                    //When trying to access the content from a service the context maybe null
                    if (Sitecore.Context.Site == null)
                    {
                        // query the DB for the root item (current language is accounted for)
                        oRootItem = ContextExtension.CurrentDatabase.GetItem(string.Concat(
                                                                                            Sitecore.Configuration.Settings.GetSetting("ServerLogic.SitecoreExtension.DefaultConentRootPath", string.Empty), 
                                                                                            Sitecore.Configuration.Settings.GetSetting("ServerLogic.SitecoreExtension.DefaultConentStartItem", string.Empty)
                                                                                           )
                                                                             );
                    }
                    else
                    {
                        // query the DB for the root item (current language is accounted for)
                        oRootItem = ContextExtension.CurrentDatabase.GetItem(Sitecore.Context.Site.SiteInfo.RootPath + Sitecore.Context.Site.SiteInfo.StartItem);
                    }
					//is the root item non-null?
					if (oRootItem != null)
					{
						//put the root item into the http context to ensure we can get it a second time
						HttpContext.Current.Items[ROOT_ITEM_KEY] = oRootItem;
					}
				}

				//return the root item
				return oRootItem;
			}
		}

		/// <summary>
		/// Returns the value of a field for use as a CSS style. Essentially, returns the value in lower case.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sField"></param>
		/// <returns>string</returns>
		public static string GetStyleValue(this Item oItem, string sField)
		{
			return oItem.GetStyleValue(string.Empty, sField);
		}

		/// <summary>
		/// Returns the value of a field by section for use as a CSS style. Essentially, returns the value in lower case.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <returns>string</returns>
		public static string GetStyleValue(this Item oItem, string sSection, string sField)
		{
			return oItem.GetStyleValue(sSection, sField, string.Empty);
		}

		/// <summary>
		/// Returns the value of a field by section or a provided default value for use as a CSS style. Essentially, returns the value in lower case.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetStyleValue(this Item oItem, string sSection, string sField, string sDefault)
		{
			return oItem.GetField(sSection, sField).GetStyleValue(sDefault);
        }

        /// <summary>
        /// Returns the value of a Sitecore Item field. Essentially, returns a string where carriage returns and new lines are replaced with HTML paragraph tags. (<p></p>)
        /// </summary>
        /// <param name="oItem"></param>
        /// <param name="sField"></param>
        /// <returns>string</returns>
        public static string GetMultiLineText(this Item oItem, string sField)
        {
            return oItem.GetMultiLineText(string.Empty, sField);
        }

        /// <summary>
        /// Returns the value of a Sitecore Item field by section. Essentially, returns a string where carriage returns and new lines are replaced with HTML paragraph tags. (<p></p>)
        /// </summary>
        /// <param name="oItem"></param>
        /// <param name="sSection"></param>
        /// <param name="sField"></param>
        /// <returns>string</returns>
        public static string GetMultiLineText(this Item oItem, string sSection, string sField)
        {
            return oItem.GetField(sSection, sField).GetMultiLineText();
        }

        /// <summary>
        /// Given an Item, set into an image control the media item parameters.
        /// </summary>
        /// <param name="oItem">Sitecore Item</param>
        /// <param name="sField">Field with the image media item</param>
        /// <param name="oImage">Image Web Control</param>
        public static void ConfigImage(this Item oItem, string sField, Image oImage)
        {
            oItem.ConfigImage(string.Empty, sField, oImage);
        }

        /// <summary>
        /// Given an Item, set into an image control a media item parameters.
        /// </summary>
        /// <param name="oItem">Sitecore Item</param>
        /// <param name="sSection">Sitecore Section</param>
        /// <param name="sField">Sitecore Field with the image media item</param>
        /// <param name="oImage">Image Web Control</param>
        public static void ConfigImage(this Item oItem, string sSection, string sField, Image oImage)
        {
            oItem.GetField(sSection, sField).ConfigImage(oImage);
        }

		/// <summary>
		/// Returns the string value of a field. Essentially, gets the text value of a field.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sField"></param>
		/// <returns>string</returns>
		public static string GetText(this Item oItem, string sField)
		{
			return GetText(oItem, string.Empty, sField);
		}

		/// <summary>
		/// Returns the string value of a field. Essentially, gets the text value of a field.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <returns>string</returns>
		public static string GetText(this Item oItem, string sSection, string sField)
		{
			return GetText(oItem, sSection, sField, string.Empty);
		}

		/// <summary>
		/// Returns the string value of a field. Essentially, gets the text value of a field.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetText(this Item oItem, string sSection, string sField, string sDefault)
		{
			return oItem.GetField(sSection, sField).GetText(sDefault);
		}

		/// <summary>
		/// Returns the decimal value of a field. Essentially, tries to parse the decimal value and returns it.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sField"></param>
		/// <returns>decimal</returns>
		public static decimal GetDecimal(this Item oItem, string sField)
		{
			return GetDecimal(oItem, string.Empty, sField);
		}

		/// <summary>
		/// Returns the decimal value of a field. Essentially, tries to parse the decimal value and returns it.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <returns>decimal</returns>
		public static decimal GetDecimal(this Item oItem, string sSection, string sField)
		{
			return GetDecimal(oItem, sSection, sField, 0M);
		}

		/// <summary>
		/// Returns the decimal value of a field. Essentially, tries to parse the decimal value and returns it.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <param name="dDefault"></param>
		/// <returns>decimal</returns>
		public static decimal GetDecimal(this Item oItem, string sSection, string sField, decimal dDefault)
		{
			return oItem.GetField(sSection, sField).GetDecimal(dDefault);
		}

		/// <summary>
		/// Returns a string for use as a URL for an image. Essentially, returns the path of an image for display
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sField"></param>
		/// <returns>string</returns>
		public static string GetImageURL(this Item oItem, string sField)
		{
			return oItem.GetImageURL(string.Empty, sField);
		}

		/// <summary>
		/// Returns a string for use as a URL for an image. Essentially, returns the path of an image for display
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <returns>string</returns>
		public static string GetImageURL(this Item oItem, string sSection, string sField)
		{
			return GetImageURL(oItem, sSection, sField, string.Empty);
		}

		/// <summary>
		/// Returns a string for use as a URL for an image. Essentially, returns the path of an image for display
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sSection"></param>
		/// <param name="sField"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetImageURL(this Item oItem, string sSection, string sField, string sDefault)
		{
			return oItem.GetField(sSection, sField).GetImageURL(sDefault);
		}

		/// <summary>
		/// Returns a string for use as a URL for a media item. Essentially, returns the path of a media item
		/// </summary>
		/// <param name="oItem"></param>
		/// <returns>string</returns>
		public static string GetMediaURL(this Item oItem)
		{
			return oItem.GetMediaURL(string.Empty);
		}

		/// <summary>
		/// Returns a string for use as a URL for a media item. Essentially, returns the path of a media item
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetMediaURL(this Item oItem, string sDefault)
		{
			return oItem != null ? MediaManager.GetMediaUrl(oItem) : sDefault;
		}

		#region GET MULTI-LIST ITEMS

		/// <summary>
		/// Returns a list of items. Essentially, returns a generic list of items in a Multilist control
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldName"></param>
		/// <returns>List<Item></Item></returns>
		public static List<Item> GetMultilistItems(this Item oItem, string sFieldName)
		{
			return oItem.GetMultilistItems(string.Empty, sFieldName);
		}

		/// <summary>
		/// Returns a list of items. Essentially, returns a generic list of items in a Multilist control
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldSection"></param>
		/// <param name="sFieldName"></param>
		/// <returns>List<Item></returns>
		public static List<Item> GetMultilistItems(this Item oItem, string sFieldSection, string sFieldName)
		{
			return oItem.GetMultilistItems(sFieldSection, sFieldName, new List<Item>());
		}

		/// <summary>
		/// Returns a list of items. Essentially, returns a generic list of the items within a Multilist control
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldSection"></param>
		/// <param name="sFieldName"></param>
		/// <param name="oItems"></param>
		/// <returns>List<Item></returns>
		public static List<Item> GetMultilistItems(this Item oItem, string sFieldSection, string sFieldName, List<Item> oItems)
		{
			return oItem.GetMultilistItems(oItem.GetField(sFieldSection, sFieldName), oItems);
		}

		/// <summary>
		/// Returns a list of items. Essentially, returns a generic list of the items within a Multilist control
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="oField"></param>
		/// <param name="oItems"></param>
		/// <returns>List<Item></returns>
		public static List<Item> GetMultilistItems(this Item oItem, Field oField, List<Item> oItems)
		{
			Item oSubItem;
			if (oField != null)
			{
				oField.Value.Split('|').ToList().ForEach(id =>
				{
					if ((oSubItem = ContextExtension.CurrentDatabase.GetItem(id)) != null)
					{
						oItems.Add(oSubItem);
					}
				});
			}
			return oItems;
		}

		#endregion

		#region GET LIST ITEM

		/// <summary>
		/// Returns a Sitecore item out of a ListItem control.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldName"></param>
		/// <returns>Item</returns>
		public static Item GetListItem(this Item oItem, string sFieldName)
		{
			return oItem.GetListItem(string.Empty, sFieldName);
		}

		/// <summary>
		/// Returns a Sitecore item out of a ListItem control.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldSection"></param>
		/// <param name="sFieldName"></param>
		/// <returns>Item</returns>
		public static Item GetListItem(this Item oItem, string sFieldSection, string sFieldName)
		{
			return oItem.GetListItem(sFieldSection, sFieldName, ContextExtension.CurrentDatabase);
		}

		/// <summary>
		/// Returns a Sitecore item out of a ListItem control.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldSection"></param>
		/// <param name="sFieldName"></param>
		/// <param name="oDatabase"></param>
		/// <returns>Item</returns>
		public static Item GetListItem(this Item oItem, string sFieldSection, string sFieldName, Database oDatabase)
		{
			return oItem.GetField(sFieldSection, sFieldName).GetItem(oDatabase);
		}

		#endregion

		/// <summary>
		/// Returns the value of the file size. Essentially, returns the numeric value of the file size.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldName"></param>
		/// <returns>double</returns>
		public static double GetFileSize(this Item oItem, string sFieldName)
		{
			return oItem.GetFileSize(sFieldName, 0);
		}

		/// <summary>
		/// Returns the value of the file size. Essentially, returns the numeric value of the file size.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldName"></param>
		/// <param name="dDefault"></param>
		/// <returns>double</returns>
		public static double GetFileSize(this Item oItem, string sFieldName, double dDefault)
		{
			return oItem.GetFileSize(string.Empty, sFieldName, dDefault);
		}

		/// <summary>
		/// Returns the value of the file size. Essentially, returns the numeric value of the file size.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldSection"></param>
		/// <param name="sFieldName"></param>
		/// <param name="dDefault"></param>
		/// <returns>double</returns>
		public static double GetFileSize(this Item oItem, string sFieldSection, string sFieldName, double dDefault)
		{
			return oItem.GetField(sFieldSection, sFieldName).GetFileSize(dDefault);
		}

        public static Dictionary<string, string> GetNameValueDictionary(this Item oItem, string sFieldName, string sFieldSection = null)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var collection = oItem.GetField(sFieldSection, sFieldName).GetNameValueCollection();

            foreach (var key in collection.AllKeys)
            {
                dict.Add(key, collection[key]);
            }

            return dict;
        }

		/// <summary>
		/// Returns a field from a Sitecore item either by explicit reference or by reading the entire content tree.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldName"></param>
		/// <returns>Field</returns>
		public static Field GetField(this Item oItem, string sFieldName)
		{
			return oItem.GetField(string.Empty, sFieldName);
		}

		/// <summary>
		/// Returns a field from a Sitecore item either by explicit reference or by reading the entire content tree.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldSection"></param>
		/// <param name="sFieldName"></param>
		/// <returns>Field</returns>
		public static Field GetField(this Item oItem, string sFieldSection, string sFieldName)
		{
			return oItem.GetField(sFieldSection, sFieldName, false);
		}

		/// <summary>
		/// Returns a field from a Sitecore item either by explicit reference or by reading the entire content tree.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sFieldSection"></param>
		/// <param name="sFieldName"></param>
		/// <param name="bForceReadAll"></param>
		/// <returns>Field</returns>
		public static Field GetField(this Item oItem, string sFieldSection, string sFieldName, bool bForceReadAll)
		{
			Field oField;

			//should we force read all?
			if (bForceReadAll)
			{
				//read all of the fields
				oItem.Fields.ReadAll();
			}

			if (!string.IsNullOrEmpty(sFieldSection))
			{
				//now we can find the field we care about
				oField = oItem.Fields.Where(f => f.Section.ToLower().Equals(sFieldSection.ToLower()) && f.Name.ToLower().Equals(sFieldName.ToLower())).FirstOrDefault();

				//if we couldn't get the field, and we are not already forcing the read, try again
				if (oField == null && !bForceReadAll)
				{
					//try reading again, but with force read enabled
					oField = oItem.GetField(sFieldSection, sFieldName, true);
				}
			}
			else if (!string.IsNullOrEmpty(sFieldName))
			{
				//read by field name alone
				oField = oItem.Fields[sFieldName];

				//if we couldn't get the field, and we are not already forcing the read, try again
				if (oField == null && !bForceReadAll)
				{
					//try reading again, but with force read enabled
					oField = oItem.GetField(sFieldSection, sFieldName, true);
				}
			}
			else
			{
				//we absolutely could not find the field, return null
				oField = null;
			}

			return oField;
		}

		/// <summary>
		/// Returns a list of children items for use in finding item relationships. Essentially, returns a generic list of children for the provided item.
		/// </summary>
		/// <param name="oItem"></param>
		/// <returns>List<Item&gt;</returns>
		public static List<Item> GetChildren(this Item oItem)
		{
			return oItem.GetChildren(new List<Item>());
		}

		/// <summary>
		/// Returns a list of children items for use in finding item relationships. Essentially, returns a generic list of children for the provided item.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="oChildren"></param>
		/// <returns>List<Item&gt;</returns>
		private static List<Item> GetChildren(this Item oItem, List<Item> oChildren)
		{
			Item oPointerChild;

			//begin by looping over the children
			foreach (Item oChild in oItem.Children)
			{
				//if this child is specifically a POINTER, return the item being pointed to, not the pointer
				if (oChild.InstanceOfTemplate("Pointer"))
				{
					//get the pointer child
					oPointerChild = Sitecore.Context.Site.Database.GetItem(oChild.Fields["Item"].Value);

					//is the pointer child non-null?
					if (oPointerChild != null)
					{
						//add the child
						oChildren.Add(oPointerChild);
					}
				}
				else
				{
					//add the child
					oChildren.Add(oChild);
				}
			}

			return oChildren;
		}

        public static Item GetChildByName(this Item oItem, string childName)
        {
            return GetChildren(oItem).Where(oChild => oChild != null && oChild.Name != null && oChild.Name.ToLower().Equals(childName.ToLower())).FirstOrDefault();
        }

        public static List<Item> GetChildrenAndItemsOfTemplate(this Item oItem, string sTemplateName)
        {
            return oItem.GetChildrenAndItemsOfTemplate("Page", "Items", sTemplateName);
        }

        public static List<Item> GetChildrenAndItemsOfTemplate(this Item oItem, string[] sTemplateNames)
        {
            return oItem.GetChildrenAndItemsOfTemplate("Page", "Items", sTemplateNames);
        }

        public static List<Item> GetChildrenAndItemsOfTemplate(this Item oItem, string sFieldSection, string sFieldName, string sTemplateName)
        {
            return oItem.GetChildrenAndItemsOfTemplate(sFieldSection, sFieldName, new string[] { sTemplateName });
        }


        public static List<Item> GetChildrenAndItemsOfTemplate(this Item oItem, string sFieldSection, string sFieldName, string[] sTemplateNames)
        {
            List<Item> items = new List<Item>();
            items.AddRange(oItem.GetChildrenOfTemplate(sTemplateNames));
            items.AddRange(oItem.GetMultilistItems(sFieldSection, sFieldName).GetItemsOfTemplate(sTemplateNames));
            return items;
        }


        

		/// <summary>
		/// Returns a list of children, specifically for a template item. Essentially, returns a generic list of children for type TemplateItem
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sTemplateName"></param>
		/// <returns>List&lt;Item&gt;</returns>
		public static List<Item> GetChildrenOfTemplate(this Item oItem, string sTemplateName)
		{
			return GetChildren(oItem).Where(oChild => oChild.InstanceOfTemplate(sTemplateName)).ToList();
		}

		/// <summary>
		/// Returns a list of children, specifically for an array of template items. Essentially, returns a generic list of children for type TemplateItem
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sTemplateNames"></param>
		/// <returns>List&lt;Item&gt;</returns>
		public static List<Item> GetChildrenOfTemplate(this Item oItem, string[] sTemplateNames)
		{
			return GetChildren(oItem).Where(oChild => oChild.InstanceOfTemplate(sTemplateNames)).ToList();
		}

		/// <summary>
		/// Returns a list of items for a given template. Essentially, returns a generic list of items for type TemplateItem
		/// </summary>
		/// <param name="oItems"></param>
		/// <param name="sTemplateName"></param>
		/// <returns>List&lt;Item&gt;</returns>
		public static List<Item> GetItemsOfTemplate(this IEnumerable<Item> oItems, string sTemplateName)
		{
			return oItems.GetItemsOfTemplate(new string[] { sTemplateName });
		}

		/// <summary>
		/// Returns a list of items specifically for an array of templates. Essentially, returns a generic list of items for type TemplateItem
		/// </summary>
		/// <param name="oItems"></param>
		/// <param name="sTemplateNames"></param>
		/// <returns>List&lt;Item&gt;</returns>
		public static List<Item> GetItemsOfTemplate(this IEnumerable<Item> oItems, string[] sTemplateNames)
		{
			return oItems.Where(oChild => oChild.InstanceOfTemplate(sTemplateNames)).ToList();
		}

		/// <summary>
		/// Returns true if the supplied Template is assigned to the Item
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sTemplateName"></param>
		/// <returns>bool</returns>
		public static bool InstanceOfTemplate(this Item oItem, string sTemplateName)
		{
			bool bReturn;

			//does the item have a template?
			if (oItem != null && !string.IsNullOrEmpty(oItem.TemplateName) && !string.IsNullOrEmpty(sTemplateName))
			{
				//is the template name equal to the item?
				if (sTemplateName.Equals(oItem.TemplateName))
				{
					//set return to true
					bReturn = true;
				}
				else
				{
					//determine if any of the templates match the template provided
					bReturn = oItem.GetBaseTemplates().Any(t => t.IsTemplate(sTemplateName));
				}
			}
			else
			{
				//initialize return value to false
				bReturn = false;
			}

			return bReturn;
		}

		/// <summary>
		/// Returns true if the supplied Templates are assigned to the Item
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sTemplateNames"></param>
		/// <returns>bool</returns>
		public static bool InstanceOfTemplate(this Item oItem, string[] sTemplateNames)
		{
			bool bReturn;
			IEnumerable<TemplateItem> oTemplates;

			//initialize return value to false
			bReturn = false;

			//does the item have a template?
			if (oItem != null && sTemplateNames != null)
			{
				//get the base templates
				oTemplates = oItem.GetBaseTemplates();

				//determine our return value
				bReturn = oTemplates.Where(t => t.IsTemplate(sTemplateNames)).Count() > 0;
			}

			return bReturn;
		}

		/// <summary>
		/// Returns the base templates based on this item's template to determine inheritance.
		/// </summary>
		/// <param name="oItem"></param>
		/// <returns>IEnumerable&lt;TemplateItem&gt;</returns>
		public static IEnumerable<TemplateItem> GetBaseTemplates(this Item oItem)
		{
			//return the base templates based on this item's template
			return TemplateItemExtension.GetBaseTemplates(oItem);
		}

		/// <summary>
		/// Returns a list of parent items to determine location in tree. Essentially returns a list of parent items.
		/// </summary>
		/// <param name="oItem"></param>
		/// <returns>List&lt;Item&gt;</returns>
		public static List<Item> GetParentItems(this Item oItem)
		{
			return oItem.GetParentItems(new List<Item>());
		}

		/// <summary>
		/// Returns a list of parent items to determine location in tree. Essentially returns a list of parent items.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="oItems"></param>
		/// <returns>List&lt;Item&gt;</returns>
        public static List<Item> GetParentItems(this Item oItem, List<Item> oItems)
        {
            Item oCurrentItem;
            ID oStopId;
            Item oRootItem;
            if ((oRootItem = RootItem) == null || oRootItem.Parent == null)
                oStopId = Sitecore.ItemIDs.ContentRoot;
            else
                oStopId = oRootItem.Parent.ID;
            //loop from the item in question up to the root to determine if this item is selected
            for (oCurrentItem = oItem; oCurrentItem != null && oCurrentItem.ID != oStopId; oCurrentItem = oCurrentItem.Parent)
            {
                //add the item to the list
                oItems.Add(oCurrentItem);
            }

            //return the items
            return oItems;
        }


		/// <summary>
		/// Returns true if Item appears in the path between root and the path item.
		/// </summary>
		/// <param name="oItem"></param>
		/// <returns>bool</returns>
		public static bool InSelectedPath(this Item oItem)
		{
			//does oItem exist between root and the selected item?
			return oItem.InSelectedPath(Context.Item);
		}

		/// <summary>
		/// Returns true if Item appears in the path between root and the path item.
		/// </summary>
		/// <param name="oItem">The item we are testing.</param>
		/// <param name="oPathItem">The end of the path starting at root.</param>
		/// <returns>bool</returns>
		public static bool InSelectedPath(this Item oItem, Item oPathItem)
		{
			List<Item> oParentItems;
			string sKey;

			//set the key
			sKey = string.Format("parent-items-{0}", oPathItem.ID.ToString());

			//do we havfe parent items already?
			if ((oParentItems = (List<Item>)Sitecore.Context.Items[sKey]) == null)
			{
				//get and store the items in the context
				Sitecore.Context.Items.Add(sKey, oParentItems = oPathItem.GetParentItems());
			}

			//return whether or not we have the items we care about
			return oItem.InSelectedPath(oParentItems);
		}

		/// <summary>
		/// Returns true if Item appears in the path between root and the path item.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="oPathItems"></param>
		/// <returns>bool</returns>
		public static bool InSelectedPath(this Item oItem, List<Item> oPathItems)
		{
			//return whether or not we have the items we care about
			return oPathItems.Where(o => o.ID.Equals(oItem.ID)).Count() > 0;
		}

		/// <summary>
		/// Returns the attribute that links to this item. Essentially returns a string path to the item.
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="sTrueAttribute"></param>
		/// <param name="sFalseAttribute"></param>
		/// <returns>string</returns>
		public static string GetSelectionAttribute(this Item oItem, string sTrueAttribute, string sFalseAttribute)
		{
			bool bReturn;
			Item oTemp;
			if (oItem.InstanceOfTemplate("Link") && (oTemp = ContextExtension.CurrentDatabase.GetItem(oItem.Fields["Item"].Value)) != null)
			{
				bReturn = ContextExtension.CurrentItem.ID.Equals(oTemp.ID);
			}
			else
			{
				bReturn = oItem.InSelectedPath(ContextExtension.CurrentParentItems);
			}
			return bReturn ? sTrueAttribute : sFalseAttribute;
		}

		/// <summary>
		/// Returns a string value for use as an item URL. Essentially, returns the path of a Sitecore Item
		/// </summary>
		/// <param name="oItem"></param>
		/// <returns>string</returns>
		public static string GetURL(this Item oItem)
		{
			return oItem.GetURL(false);
		}

		/// <summary>
		/// This version of GetURL is intended to allow the caller to specify whether "Sticky Link" testing should be bypassed. 
		/// </summary>
		/// <param name="oItem"></param>
		/// <param name="bBypassStickyTest"></param>
		/// <returns></returns>
		public static string GetURL(this Item oItem, bool bBypassStickyTest)
		{
			string sURL;
			Item oLinkedItem;

			if (oItem.InstanceOfTemplate("Link"))
			{
				//is the item a sticky link?
				if (bBypassStickyTest || !oItem.InstanceOfTemplate(StickyLinkManager.STICKY_LINK_TEMPLATE_NAME) || string.IsNullOrEmpty(sURL = StickyLinkManager.GetStickyLinkURL(oItem)))
				{
					if (string.IsNullOrEmpty(sURL = oItem.GetText("URL")) && (oLinkedItem = ContextExtension.CurrentDatabase.GetItem(oItem.Fields["Item"].Value)) != null)
					{
						sURL = oLinkedItem.GetURL();
					}
				}
			}
			else
			{
				sURL = Sitecore.Links.LinkManager.GetItemUrl(oItem);
			}
			return sURL;
		}
	}
}
