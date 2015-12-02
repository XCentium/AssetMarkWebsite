using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;

using ServerLogic.Linq;

namespace ServerLogic.SitecoreExt
{
	public static class TemplateItemExtension
	{
		private static SortedDictionary<Guid, List<TemplateItem>> oItemBaseTemplates;
		private static object oItemBaseTemplatesSyncRoot = new object();

		private static List<TemplateItem> GetBaseTemplates(Guid gID)
		{
			List<TemplateItem> oBaseTemplates;

			//initialize the base templates to an empty list
			oBaseTemplates = null;

			//first, lock on the base templates list
			lock (oItemBaseTemplatesSyncRoot)
			{
				//does the list need to be instantiated?
				if (oItemBaseTemplates == null)
				{
					//instantiate the template dictionary
					oItemBaseTemplates = new SortedDictionary<Guid, List<TemplateItem>>();

					//log that we created a template list
					Sitecore.Diagnostics.Log.Debug("Instantiated shared base template list.");
				}

				//does the dictionary contain the item we are looking for?
				if (oItemBaseTemplates.ContainsKey(gID))
				{
					//get the item
					oBaseTemplates = oItemBaseTemplates[gID];
				}
			}

			//return the base templates
			return oBaseTemplates;
		}

		public static List<TemplateItem> GetBaseTemplates(Item oItem)
		{
			List<TemplateItem> oBaseTemplates;

			oBaseTemplates = GetBaseTemplates(oItem.TemplateID.Guid);
			if (oBaseTemplates == null)
			{
				oBaseTemplates = GetBaseTemplates(oItem.Template);
			}
			return oBaseTemplates;
		}

		public static List<TemplateItem> GetBaseTemplates(this TemplateItem oTemplate)
		{
			List<TemplateItem> oBaseTemplates;
			Guid gID;

			//get the template id
			gID = oTemplate.ID.Guid;

			//first, lock on the base templates list
			lock (oItemBaseTemplatesSyncRoot)
			{
				//try to get the base templates by id
				oBaseTemplates = GetBaseTemplates(oTemplate.ID.Guid);

				//does the dictionary contain the item we are looking for?
				if (oBaseTemplates == null)
				{
					//we need to create the list
					oItemBaseTemplates.Add(gID, oBaseTemplates = oTemplate.RecursiveToList(t => t.BaseTemplates).ToList());

					//log that we created a template list
					Sitecore.Diagnostics.Log.Debug("Created base template list for [" + oTemplate.FullName + "] containing [" + oBaseTemplates.Count() + "] base templates.");
				}
			}

			//return the base templates
			return oBaseTemplates;
		}

		public static bool IsTemplate(this TemplateItem oTemplate, string sTemplateName)
		{
			return oTemplate.Name.Equals(sTemplateName) || oTemplate.FullName.Equals(sTemplateName) || oTemplate.ID.ToString().Equals(sTemplateName);
		}

		public static bool IsTemplate(this TemplateItem oTemplate, string[] sTemplateNames)
		{
			int i;

			//loop over template names
			for (i = 0; i < sTemplateNames.Length; i++)
			{
				//check the name
				if (oTemplate.IsTemplate(sTemplateNames[i]))
				{
					//break out of the loop
					break;
				}
			}

			//if we read through the entire list and found no match, "i" would be equal to "sTemplateNames.lenth"
			return i < sTemplateNames.Length;
		}
	}
}
