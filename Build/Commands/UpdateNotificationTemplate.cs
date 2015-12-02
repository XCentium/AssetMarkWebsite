using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Data.Items;
using System.Collections.Specialized;
using Sitecore.Data;
using Sitecore.Configuration;
using Sitecore.SecurityModel;
using Genworth.SitecoreExt;
using ServerLogic.SitecoreExt;
using Sitecore.Data.Fields;
using Sitecore;

namespace Genworth.SitecoreExt.Commands
{
	public class UpdateNotificationTemplate : Command
	{
		private const string sNotificationTemplateID = "{B9F0001A-08A9-435A-AEF2-21930DD3CB43}";
		private const string sNotificationOldTemplateID = "{10BA36D5-71BD-453D-99D0-D090EF077AFB}";
		private readonly string sQuery = "fast:/sitecore/content//*[@@templateid='" + sNotificationOldTemplateID + "']";
		private const string sCommandFolder = "/sitecore/templates/Genworth/Commands";
		private const string sDataBaseName = "master";
		private const string sInsertOptionsFieldName = "__Insert Rules";
		private const string sCommandID = "{D9022041-3952-436A-A810-DF594DFD04CB}";
		public override void Execute(CommandContext oContext)
		{
			if (oContext.Items.Length > 0)
			{
				Item oItem = oContext.Items[0];
				NameValueCollection oParameters = new NameValueCollection();
				oParameters["item"] = oItem.ID.ToString();
				Sitecore.Context.ClientPage.Start(this, "Run", oParameters);
			}
			
		}
		protected void Run(Sitecore.Web.UI.Sheer.ClientPipelineArgs oArgs)
		{
			Database db = Factory.GetDatabase(sDataBaseName);
			Item oNotificationItem = db.GetItem(sNotificationTemplateID);
			Item oNotificationOld;
			Item oItemCommand = db.GetItem(oArgs.Parameters["item"]);
			if (oNotificationItem != null)
			{
				TemplateItem oNotificationTemplate = new TemplateItem(oNotificationItem);
				if (oNotificationTemplate != null)
				{
					//find items with notifications template
					Item[] oItems = db.SelectItems(sQuery);
					using (new SecurityDisabler())
					{
						ChangeItemTemplate(oItems, oNotificationItem);
						
						oNotificationOld = db.GetItem(sNotificationOldTemplateID);
						
						foreach (Item ochild in oNotificationItem.Children)
						{
							ochild.CopyTo(oNotificationOld, ochild.Name);
						}

						Item oStandarValue = db.GetItem(oNotificationOld[FieldIDs.StandardValues]);
						using (new EditContext(oNotificationOld))
						{
							oNotificationOld.Fields[FieldIDs.BaseTemplate].Value = oNotificationItem[FieldIDs.BaseTemplate];
							oNotificationOld[FieldIDs.StandardValues] = oStandarValue.ID.ToString();
						}
						
						using (new EditContext(oStandarValue))
						{
							oStandarValue["Title"] = "$name";
						}

						oNotificationTemplate = new TemplateItem(oNotificationOld);
						ChangeItemTemplate(oItems, oNotificationTemplate);
						oNotificationItem.Delete();
						//remove inserted options
										
						using (new EditContext(oItemCommand))
						{
							oItemCommand[sInsertOptionsFieldName] = oItemCommand[sInsertOptionsFieldName].Replace(sCommandID,"").Replace("||", "|").TrimEnd('|');
						}
						
						//delete command
						db.GetItem(sCommandFolder).Delete();
						
					}
				}
			}
		}
		private void ChangeItemTemplate(Item[] oItems, TemplateItem oTemplate)
		{
			foreach (Item oItem in oItems)
			{
				using (new EditContext(oItem))
				{
					// change the template
					oItem.ChangeTemplate(oTemplate);
				}
			}
		}
	}
}
