using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Sitecore.Data;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using System.ServiceModel.Activation;
using Genworth.SitecoreExt.Services.Contracts.Data;



namespace Genworth.SitecoreExt.Services.Content
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class GenHelpContentService : IGenHelpContentService
	{		
		 
		/// <summary>
		/// Executes the given fast query to retrieve the help content items
		/// </summary>
		/// <param name="sQuery"></param>
		/// <returns></returns>
		private List<HelpTextContract> GetHelpContent(string sQuery)
		{
			List<HelpTextContract> oHelpTextToReturn;
			Item[] oHelpTextItems;

			oHelpTextToReturn = new List<HelpTextContract>();

			if (!string.IsNullOrEmpty(sQuery))
			{

				oHelpTextItems = ContextExtension.CurrentDatabase.SelectItems(sQuery);

				if (oHelpTextItems != null)
				{
					try
					{
						foreach (var oHelpContentItem in oHelpTextItems)
						{

							oHelpTextToReturn.Add(new HelpTextContract()
							{
								Title = oHelpContentItem.GetField(
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TitleFieldName
																 ).GetText(string.Empty),
								Text = oHelpContentItem.GetField(
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TextFieldName
																 ).GetMultiLineText(string.Empty),
								IconURL = oHelpContentItem.GetField(
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.IconFieldName
																 ).GetImageURL(string.Empty),
								FieldSelector = oHelpContentItem.GetField(
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																	Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.FieldSelectorFieldName
																 ).GetText(string.Empty)
							}
												  );
						}
					}
					catch (Exception oLoadingItemsException)
					{
						Sitecore.Diagnostics.Log.Error("Error while loading help text content", oLoadingItemsException, this);
					}
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Error("GetPageHelpContentById - Unable parse a valid sitecore ID ", this);
			}

			return oHelpTextToReturn;
		}

		/// <summary>
		/// Returns all the help content items associated to the given item Id
		/// </summary>
		/// <param name="sItemId"></param>
		/// <returns></returns>
		public List<HelpTextContract> GetHelpContentByItemId(string sItemId)
		{
			ID oItemID;
			StringBuilder oQueryBuilder;
			string sQuery;
			List<HelpTextContract> oHelpTextToReturn;
			Item oItem;

			oHelpTextToReturn = null;
			if (ID.TryParse(sItemId, out oItemID))
			{

				oItem = ContextExtension.CurrentDatabase.GetItem(oItemID);

				if (oItem != null)
				{

					if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Name))
					{
						oHelpTextToReturn = new List<HelpTextContract>();

						oHelpTextToReturn.Add(new HelpTextContract()
														{
															Title = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TitleFieldName
																							 ).GetText(string.Empty),
															Text = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TextFieldName
																							 ).GetMultiLineText(string.Empty),
															IconURL = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.IconFieldName
																							 ).GetImageURL(string.Empty),
															FieldSelector = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.FieldSelectorFieldName
																							 ).GetText(string.Empty)
														}
											);
					}
					else
					{
						oQueryBuilder = new StringBuilder("fast://*[@@parentid='")
															  .Append(oItemID)
															  .Append("'and @@templateid = '")
															  .Append(Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.ID)
															  .Append("']");

						sQuery = oQueryBuilder.ToString();

						oHelpTextToReturn = GetHelpContent(sQuery);
					}
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Error("GetPageHelpContentById - Unable parse a valid sitecore ID ", this);
			}

			return oHelpTextToReturn;

		}

		/// <summary>
		/// Returns all the help content items associated to the item with the given name. If multiple items with the same name exists only the first Item will be considered
		/// </summary>
		/// <param name="sItemName"></param>
		/// <returns></returns>
		public List<HelpTextContract> GetHelpContentByItemName(string sItemName)
		{
			StringBuilder oQueryBuilder;
			string sQuery;
			Item[] oItems;
			Item oItem;
			List<HelpTextContract> oHelpTextToReturn;

			oHelpTextToReturn = null;

			if (!string.IsNullOrEmpty(sItemName))
			{

				oQueryBuilder = new StringBuilder("fast://*[@@name = '")
													  .Append(sItemName)
													  .Append("']");

				sQuery = oQueryBuilder.ToString();

				oItems = ContextExtension.CurrentDatabase.SelectItems(sQuery);
				if (oItems != null && oItems.Length > 0)
				{
					oItem = oItems.FirstOrDefault();


					if (oItem != null)
					{

						if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Name))
						{
							oHelpTextToReturn = new List<HelpTextContract>();

							oHelpTextToReturn.Add(new HelpTextContract()
															{
																Title = oItem.GetField(
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TitleFieldName
																								 ).GetText(string.Empty),
																Text = oItem.GetField(
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TextFieldName
																								 ).GetMultiLineText(string.Empty),
																IconURL = oItem.GetField(
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.IconFieldName
																								 ).GetImageURL(string.Empty),
																FieldSelector = oItem.GetField(
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																									Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.FieldSelectorFieldName
																								 ).GetText(string.Empty)
															}
												);
						}
						else
						{

							oQueryBuilder = new StringBuilder("fast://*[@@parentid = '")
																  .Append(oItem.ID)
																  .Append("'and @@templateid = '")
																  .Append(Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.ID)
																  .Append("']");

							sQuery = oQueryBuilder.ToString();

							oHelpTextToReturn = GetHelpContent(sQuery);
						}
					}
				}
			}

			return oHelpTextToReturn;
		}


		/// <summary>
		/// Returns all the help content items associated to the item with the given path.
		/// </summary>
		/// <param name="sItemName"></param>
		/// <returns></returns>
		public List<HelpTextContract> GetHelpContentByItemPath(string sItemPath)
		{
			StringBuilder oQueryBuilder;
			string sQuery;
			Item oItem;
			List<HelpTextContract> oHelpTextToReturn;

			oHelpTextToReturn = null;

			if (!string.IsNullOrEmpty(sItemPath))
			{				
				oItem = ContextExtension.CurrentDatabase.SelectSingleItem(sItemPath);

				if (oItem != null)
				{

					if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Name))
					{
						oHelpTextToReturn = new List<HelpTextContract>();

						oHelpTextToReturn.Add(new HelpTextContract()
														{
															Title = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TitleFieldName
																							 ).GetText(string.Empty),
															Text = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TextFieldName
																							 ).GetMultiLineText(string.Empty),
															IconURL = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.IconFieldName
																							 ).GetImageURL(string.Empty),
															FieldSelector = oItem.GetField(
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
																								Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.FieldSelectorFieldName
																							 ).GetText(string.Empty)
														}
											);
					}
					else
					{
						oQueryBuilder = new StringBuilder("fast://*[@@parentid = '")
															  .Append(oItem.ID)
															  .Append("'and @@templateid = '")
															  .Append(Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.ID)
															  .Append("']");

						sQuery = oQueryBuilder.ToString();

						oHelpTextToReturn = GetHelpContent(sQuery);
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Warn(string.Format("GetPageHelpContentByName - Unable to find item with path {0}", sItemPath), this);
				}
			}


			return oHelpTextToReturn;
		}
	}
}
