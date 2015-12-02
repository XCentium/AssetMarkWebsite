using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Configuration;
using Genworth.SitecoreExt;
using ServerLogic.SitecoreExt;
using Sitecore.SecurityModel;

namespace Genworth.SitecoreExt.Commands
{
	/// <summary>
	/// Commmand to change template, create all meta data needed.
	/// </summary>
	public class UpdateContextSensitveHelp: Command
	{
		private const string METADATAPATH = "/sitecore/content/Meta-Data";
		private const string HELPTEXTFOLDERNAME = "Help Text";
		private const string TITLEFOLDERNAME = "Title";
		private const string TEXTFOLDERNAMR = "Text";
		private const string HELPTEXTFOLDERID = "{005B8CDD-C849-4931-97B7-BB9275DAFD39}";
		private const string TITLEFOLDERID = "{6F64FA64-71CC-4961-9E02-FBA7B7AFE649}";
		private const string TEXTFOLDERID = "{978504C9-6AE3-4F80-95C2-A6410C53669A}";
		public override void Execute(CommandContext oContext)
		{
			if (oContext.Items.Length > 1)
			{
				Item oItem = oContext.Items[0];
				NameValueCollection oParameters = new NameValueCollection();
				oParameters["database"] = oItem.Database.Name;
				Sitecore.Context.ClientPage.Start(this, "Run", oParameters);

			}
		}
		protected void Run(Sitecore.Web.UI.Sheer.ClientPipelineArgs oArgs)
		{
			Database db = Factory.GetDatabase(oArgs.Parameters["database"]);
			Dictionary<string, Guid> oText = new Dictionary<string, Guid>();
			Dictionary<string, Guid> oTitle = new Dictionary<string, Guid>();
			Item oHelpText;
			Item oTextFolder;
			Item oTitleFolder;
			Item oMetaData ;

			if ((oMetaData = db.GetItem(METADATAPATH)) != null)
			{
				using(new SecurityDisabler())
				if ((oHelpText = oMetaData.Children[HELPTEXTFOLDERNAME]) == null)
				{
					oHelpText = oMetaData.Add(HELPTEXTFOLDERNAME, new TemplateID(new ID(HELPTEXTFOLDERID)));
				}
				if ((oTitleFolder = oMetaData.Children[TITLEFOLDERNAME]) == null)
				{
					oTitleFolder = oMetaData.Add(TITLEFOLDERNAME, new TemplateID(new ID(TITLEFOLDERID)));
				}
				if ((oTextFolder = oMetaData.Children[TEXTFOLDERNAMR]) == null)
				{
					oTextFolder = oMetaData.Add(TEXTFOLDERNAMR, new TemplateID(new ID(TEXTFOLDERID)));
				}
				db.SelectItemsUsingXPath("fast:/sitecore/content//*[@@templatename=\"Context Sensitive Help\"]");
			}
		}

	}
}
