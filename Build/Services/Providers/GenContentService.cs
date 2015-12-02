using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Collections;
using System.Xml.Linq;
using Sitecore.Globalization;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Resources.Media;
using System.IO;
using ServerLogic.SitecoreExt;


namespace Genworth.SitecoreExt.Services.Providers
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class GenContentService : IGenContentService
    {

        #region ATTRIBUTES

        #region CONSTANTS

        /// <summary>
        /// Default database to use when no database was provided in  a call to a service and no default database was defined in the Sitecore Settings section
        /// </summary>
        private const string sDefaultDatabase = "web";


        /// <summary>
        /// Default language to use when no language was provided in  a call to a service and no default language was defined in the Sitecore Settings section
        /// </summary>
        private const string sDefaultLanguage = "en";




        #endregion

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Default database to use if not database is provided in a call to this service
        /// </summary>
        private string DefaultDatabase
        {
            get {
                 return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.ContentProvider.DefaultDatabase, sDefaultDatabase);
                }
        }

        /// <summary>
        /// Default language to use if not language is provided in a call to this service
        /// </summary>
        private string DefaultLanguage
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.ContentProvider.DefaultLanguage, sDefaultLanguage);
            }
        }


        /// <summary>
        /// Default version to use if not version is provided in a call to this service
        /// </summary>
        private Sitecore.Data.Version DefaultVersion
        {
            get
            {
                string sVersionDefault;
                Sitecore.Data.Version oVersion;
                int iVersion;

                sVersionDefault = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.ContentProvider.DefaultVersion);

                if (int.TryParse(sVersionDefault, out iVersion))
                {
                    oVersion = Sitecore.Data.Version.Parse(iVersion);
                }
                else
                {
                    oVersion = Sitecore.Data.Version.First;
                }

                return oVersion;
            }
        }

        #endregion

        /// <summary>
		/// Builds a Sitecore CallContext, which keeps all the information related to the current execution of a data provider
		/// </summary>
		/// <param name="sDatabaseName">Sitecore database to be used in the context</param>
		/// <param name="iProviderCount">Sitecore provider count</param>
		/// <returns></returns>
		private CallContext BuildCallContext(string sDatabaseName)
		{
			#region VARIABLES

			CallContext oWebCallContext;
			DataManager oWebDataManager;
			Database oWebDatabase;

			#endregion
			

			oWebCallContext = null;

			if (!string.IsNullOrEmpty(sDatabaseName))
			{
				//Database initialization
				//oWebDatabase = new Database(sDatabaseName);
				if (Sitecore.Context.Database == null)
				{
					Sitecore.Context.Database = Sitecore.Data.Database.GetDatabase(sDatabaseName);
				}
			
				oWebDatabase = Sitecore.Context.Database;
				//DataManager initialization
				oWebDataManager = new DataManager(oWebDatabase);
				//Building CallContext that will be used to work with the data provider
				oWebCallContext = new CallContext(oWebDataManager, oWebDatabase.GetDataProviders().Count());
			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenContentService attempted to create a CallContext without a database", this);
			}
			return oWebCallContext;
		}


		/// <summary>
		/// Retrieves the data provider for the given database
		/// </summary>
		/// <param name="sDatabaseName">Sitecore database name</param>
		/// <returns>Data provider for the given database if found otherwise NULL</returns>       
		private DataProvider GetDataProvider(string sDatabaseName)
		{
			#region VARIABLES

			DataProvider[] oProviders;
			DataProvider oWebDataProvider;
			//Add method variables here...

			#endregion
			Database oDatabase;

			oWebDataProvider = null;

			if (!string.IsNullOrEmpty(sDatabaseName))
			{
				if (Sitecore.Context.Database == null)
				{
					Sitecore.Context.Database = Sitecore.Data.Database.GetDatabase(sDatabaseName);
				}
			
				oDatabase = Sitecore.Context.Database;
			
				//oDatabase = Sitecore.Data.Database.GetDatabase(sDatabaseName);
				oProviders = oDatabase.GetDataProviders();

				oWebDataProvider = (
										from
											oDataProvider
										in
											oProviders
										where
											string.Equals(oDataProvider.Database.Name, sDatabaseName)
										select
										   oDataProvider
									).FirstOrDefault();

				if (oWebDataProvider == null)
				{
					Sitecore.Diagnostics.Log.Error(string.Format("Unable to find data provide for database:{0}", sDatabaseName), this);
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Warn("GenContentService attempted to create a DataProvider without a database associated to it", this);
			}
			return oWebDataProvider;

		}

		/// <summary>
		/// Gets the item definition for the item with the given ID
		/// </summary>
		/// <param name="sItemId">Sitecore Item ID</param>
		/// <param name="sDatabaseName">Sitecore Database from where the item definition will be pulled out</param>
		/// <param name="iProviderCount">Sitecore CallContext provider count</param>
		/// <returns>Item's definition</returns>
		public ItemDefinitionContract GetItemDefinition(string sItemId, string sDatabaseName)
		{
			#region VARIABLES

			ItemDefinition oItemDefinition;
			ItemDefinitionContract oItemDefinitionContract;
			Sitecore.Data.ID oItemId;
			Sitecore.Data.ID oItemParentId;
			DataProvider oWebDataProvider;
			CallContext oWebCallContext;

			//Add variables here

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("Start GetItemDefinition {0}", sItemId), this);

			oItemDefinition = null;

			oItemDefinitionContract = null;
			if (!string.IsNullOrEmpty(sItemId))
			{
				oItemId = new Sitecore.Data.ID(sItemId);
				oWebCallContext = BuildCallContext(sDatabaseName);

				oWebDataProvider = GetDataProvider(sDatabaseName);

				if (oWebDataProvider != null)
				{
					oItemDefinition = oWebDataProvider.GetItemDefinition(oItemId, oWebCallContext);

					if (oItemDefinition != null)
					{
						oItemDefinitionContract = new ItemDefinitionContract()
						{
							Id = oItemDefinition.ID.Guid,
							Name = oItemDefinition.Name,
							TemplateId = oItemDefinition.TemplateID.ToString(),
							BranchId = oItemDefinition.BranchId.ToString()
						};

						oItemParentId = oWebDataProvider.GetParentID(oItemDefinition, oWebCallContext);

						if (!ID.IsNullOrEmpty(oItemParentId))
						{
							oItemDefinitionContract.ParentId = oItemParentId.ToString();
						}
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error("Unable to find data provide", this);
				}

			}

			Sitecore.Diagnostics.Log.Info(string.Format("End GetItemDefinition: ItemDefinition."), this);
			return oItemDefinition == null ? null : oItemDefinitionContract;
		}


	
		/// <summary>
		/// Gets the item definitions for the item with the given IDs
		/// </summary>
		/// <param name="oItemsIds">Sitecore IDs</param>
		/// <param name="sDatabaseName">Sitecore Database from where the item definition will be pulled out</param>
		/// <param name="iProviderCount">Sitecore CallContext provider count</param>
		/// <returns>A List with the Item definitions. It may return null if no definition is found</returns>
		public List<ItemDefinitionContract> GetItemDefinitions(List<string> oItemsIds, string sDatabaseName)
		{
			#region VARIABLES

			ItemDefinition oItemDefinition;
			ItemDefinitionContract oItemDefinitionContract;
			List<ItemDefinitionContract> oItemDefinitions;
			Sitecore.Data.ID oItemId;
			Sitecore.Data.ID oItemParentId;
			DataProvider oWebDataProvider;
			CallContext oWebCallContext;

			//Add variables here

			#endregion			
			
			Sitecore.Diagnostics.Log.Info("Start GetItemDefinitions", this);

			oItemDefinitions = null;
			oItemDefinition = null;
			oItemDefinitionContract = null;
			if (oItemsIds != null && oItemsIds.Count > 0)
			{
				oWebCallContext = BuildCallContext(sDatabaseName);

				oWebDataProvider = GetDataProvider(sDatabaseName);

				if (oWebDataProvider != null)
				{
					oItemDefinitions = new List<ItemDefinitionContract>();
					foreach (string sItemId in oItemsIds)
					{
						oItemId = new Sitecore.Data.ID(sItemId);

						oItemDefinition = oWebDataProvider.GetItemDefinition(oItemId, oWebCallContext);

						if (oItemDefinition != null)
						{
							oItemDefinitionContract = new ItemDefinitionContract()
							{
								Id = oItemDefinition.ID.Guid,
								Name = oItemDefinition.Name,
								TemplateId = oItemDefinition.TemplateID.ToString(),
								BranchId = oItemDefinition.BranchId.ToString()
							};

							oItemParentId = oWebDataProvider.GetParentID(oItemDefinition, oWebCallContext);

							if (!ID.IsNullOrEmpty(oItemParentId))
							{
								oItemDefinitionContract.ParentId = oItemParentId.ToString();
							}

							oItemDefinitions.Add(oItemDefinitionContract);
							Sitecore.Diagnostics.Log.Info(string.Format("ItemDefinition for {0} added", sItemId), this);
						}
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error("Unable to find data provide", this);
				}
			}

			Sitecore.Diagnostics.Log.Info("End GetItemDefinitions: ItemDefinition.", this);
			return oItemDefinitions;
		}

		/// <summary>
		/// Gets the fields associated to an item
		/// </summary>
		/// <param name="sItemId">Sitecore item ID</param>
		/// <param name="sLanguageName">Sitecore item Language</param>
		/// <param name="iItemVersionNumber">Sitecore item Version number</param>
		/// <param name="sDatabaseName">Sitecore Database to use</param>
		/// <param name="iProviderCount">Sitecore provider count</param>
		/// <returns>A List witht the fields associated to the item with the given id</returns>
		public List<FieldContract> GetItemFields(string sItemId, string sLanguageName, int iItemVersionNumber, string sDatabaseName)
		{
			#region VARIABLES

			List<FieldContract> oFieldListToReturn;
			FieldContract oFieldSerializable;
			Sitecore.Data.ID oItemId;
			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			FieldCollection oItemFields;
			Sitecore.Data.VersionUri oItemVersionUri;
			Sitecore.Globalization.Language oItemLanguage;
			Sitecore.Data.Version oItemVersion;
			int iNumberOfFields;			
			Field oCurrentField;
            Item oItem;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetItemFields Begin {0}", sItemId), this);

			//List that will contain the fields to return
			oFieldListToReturn = new List<FieldContract>();

			if (!string.IsNullOrEmpty(sItemId) && ID.IsID(sItemId) && !string.IsNullOrEmpty(sDatabaseName) && !string.IsNullOrEmpty(sLanguageName))
			{
				//Building CallContext and VersionUri to initialize the data provider
				oItemId = new Sitecore.Data.ID(sItemId);

                if (string.IsNullOrEmpty(sDatabaseName))
                {
                    sDatabaseName = this.DefaultDatabase;
                }

                if (string.IsNullOrEmpty(sLanguageName))
                {
                    sLanguageName = DefaultLanguage;
                }

                if (iItemVersionNumber == -1)
                {
                    oItemVersion = DefaultVersion;
                }
                else
                {
                    oItemVersion = new Sitecore.Data.Version(iItemVersionNumber);
                }
				oItemLanguage = Sitecore.Globalization.Language.Parse(sLanguageName);
                oItemLanguage.Origin.ItemId = oItemId;
				if (oItemLanguage != null)
				{
					oItemVersionUri = new VersionUri(oItemLanguage, oItemVersion);

					oWebCallContext = BuildCallContext(sDatabaseName); ;

					oWebDataProvider = GetDataProvider(sDatabaseName);

					if (oWebDataProvider != null)
					{
						//To get the item fields we first need to get the item definition						
						oItemFields = null;
                        oItem = oWebDataProvider.Database.GetItem(oItemId, oItemLanguage, oItemVersion);

                        if (oItem != null)
						{							
                            //We want to assure that no lazy loading is used 
                            oItem.Fields.ReadAll();

                            oItemFields = oItem.Fields;
							//Serialization
							if (oItemFields != null)
							{
								iNumberOfFields = oItemFields.Count;
								
								for (int iFieldIndex = 0; iFieldIndex < iNumberOfFields; iFieldIndex++)
								{
                                    oCurrentField = oItemFields[iFieldIndex];
									oFieldSerializable = new FieldContract()
									{
                                        Id = oCurrentField.ID.ToString(),
                                        Value = oCurrentField.Value										
									};                                    
									oFieldListToReturn.Add(oFieldSerializable);
									Sitecore.Diagnostics.Log.Error(string.Format("Field {0}   Value {1}", oFieldSerializable.Id, oFieldSerializable.Value), this);
								}
							}
						}
						else
						{
							Sitecore.Diagnostics.Log.Error(string.Format("Unable to get Item for item ID:{0}", sItemId), this);
						}
					}
					else
					{
						Sitecore.Diagnostics.Log.Error("Unable to find data provide", this);
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error(string.Format("Unable to find language {0}", sLanguageName), this);
				}
			}

			Sitecore.Diagnostics.Log.Info(string.Format("GetItemFields End, FieldsCount:{0}", oFieldListToReturn.Count), this);

			return oFieldListToReturn;
		}


        public Dictionary<string, List<FieldContract>> GetItemsFields(List<string> oItemsIds, string sDatabaseName)
        {
            #region VARIABLES

            Dictionary<string, List<FieldContract>> oItemsWithFieldsToReturn;
            List<FieldContract> oItemFieldsToReturn;
            FieldContract oFieldSerializable;
            Sitecore.Data.ID oItemId;
            Database oDatabase;
            FieldCollection oItemFields;
            Sitecore.Data.Version[] oItemVersions;
            LanguageCollection oLanguages;
            int iNumberOfFields;
            Field oCurrentField;
            Item oItem;

            //Add variables here...

            #endregion

            Sitecore.Diagnostics.Log.Info(string.Format("GetItemFields Begin {0}", oItemsIds), this);

            //List that will contain the fields to return
            oItemsWithFieldsToReturn = new Dictionary<string, List<FieldContract>>();

            if (oItemsIds != null && !string.IsNullOrEmpty(sDatabaseName))
            {
                oDatabase = Sitecore.Data.Database.GetDatabase(sDatabaseName);
                if (oDatabase != null)
                {
                    foreach (String sItemId in oItemsIds)
                    {
                        Sitecore.Diagnostics.Log.Info(string.Format("Getting fields for item {0}", sItemId), this);
                        if (ID.TryParse(sItemId, out oItemId) && !oItemsWithFieldsToReturn.ContainsKey(sItemId))
                        {
                            oItemFieldsToReturn = new List<FieldContract>();
                            oLanguages = oDatabase.GetLanguages();
                            oItem = oDatabase.GetItem(oItemId);
                            oItemVersions = oItem.Versions.GetVersionNumbers();

                            if (oItemVersions != null && oLanguages != null)
                            {
                                foreach (var oVersion in oItemVersions)
                                {
                                    foreach (var oItemLanguage in oLanguages)
                                    {
                                        oItem = oDatabase.GetItem(oItemId, oItemLanguage, oVersion);

                                        if (oItem != null)
                                        {
                                            //We want to assure that no lazy loading is used 
                                            oItem.Fields.ReadAll();

                                            oItemFields = oItem.Fields;
                                            //Getting everything ready for the serialization
                                            if (oItemFields != null)
                                            {
                                                iNumberOfFields = oItemFields.Count;

                                                for (int iFieldIndex = 0; iFieldIndex < iNumberOfFields; iFieldIndex++)
                                                {
                                                    oCurrentField = oItemFields[iFieldIndex];
                                                    oFieldSerializable = new FieldContract()
                                                    {
                                                        Id = oCurrentField.ID.ToString(),
                                                        Value = oCurrentField.Value,
                                                        Language = oItemLanguage.Name,
                                                        Version = oVersion.Number
                                                    };                                                    
                                                    oItemFieldsToReturn.Add(oFieldSerializable);
													Sitecore.Diagnostics.Log.Debug(string.Format("Field {0}   Value {1}", oFieldSerializable.Id, oFieldSerializable.Value), this);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Sitecore.Diagnostics.Log.Error(string.Format("Unable to get Item for item ID:{0}", oItemsIds), this);
                                        }

                                    }
                                }
                            }
                            oItemsWithFieldsToReturn.Add(sItemId, oItemFieldsToReturn);

                        }                        
                    }
                }
            }

            Sitecore.Diagnostics.Log.Info(string.Format("GetItemFields End, FieldsCount:{0}", oItemsWithFieldsToReturn.Count), this);

            return oItemsWithFieldsToReturn;
        }

		/// <summary>
		/// Retireves the information for all the versions of the given item
		/// </summary>
		/// <param name="sItemId">Sitecore Item ID</param>
		/// <param name="sDatabaseName">Sitecore Database To Use</param>
		/// <param name="iProviderCount">Sitecore CallContext ProviderCount</param>
		/// <returns>List of Versions</returns>
		public List<VersionUriContract> GetItemVersions(string sItemId, string sDatabaseName)
		{
			#region VARIABLES

			ItemDefinition oItemDefinition;
			List<VersionUriContract> oListOfItemVersionsToReturn;
			Sitecore.Data.ID oItemId;
			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			VersionUriList oVersions;
			VersionUriContract oVersionUriContract;
			VersionContract oVersionContract;
			LanguageContract oLanguageContract;

			//Add variables here
			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetItemVersions Begin {0}", sItemId), this);

			oListOfItemVersionsToReturn = new List<VersionUriContract>();

			if (!string.IsNullOrEmpty(sItemId))
			{
				oItemId = new Sitecore.Data.ID(sItemId);

				oWebCallContext = BuildCallContext(sDatabaseName);

				oWebDataProvider = GetDataProvider(sDatabaseName);

				if (oWebDataProvider != null && oWebCallContext != null)
				{
					oItemDefinition = oWebDataProvider.GetItemDefinition(oItemId, oWebCallContext);

					if (oItemDefinition != null)
					{
						oVersions = oWebDataProvider.GetItemVersions(oItemDefinition, oWebCallContext);
						if (oVersions != null)
						{
							int numberOfVersions = oVersions.Count;

							for (int versionIndex = 0; versionIndex < numberOfVersions; versionIndex++)
							{
								if (oVersions[versionIndex] != null && oVersions[versionIndex].Language != null && oVersions[versionIndex].Version != null)
								{
									oLanguageContract = new LanguageContract()
									{
										Name = oVersions[versionIndex].Language.Name
									};

									oVersionContract = new VersionContract()
									{
										Number = oVersions[versionIndex].Version.Number
									};

									oVersionUriContract = new VersionUriContract()
									{
										Version = oVersionContract,
										Language = oLanguageContract
									};

									oListOfItemVersionsToReturn.Add(oVersionUriContract);
								}
								 								
							}
						}
					}
					else
					{
						Sitecore.Diagnostics.Log.Error(string.Format("Unable to get ItemDefinition for item:{0}", sItemId), this);
					}
				}
				else
				{
					if (oWebDataProvider == null)
					{
						Sitecore.Diagnostics.Log.Error("Unable to find data provide", this);
					}
					if (oWebCallContext == null)
					{
						Sitecore.Diagnostics.Log.Error(string.Format("Unable to get CallContext using database:{0} ", sDatabaseName), this);
					}
				}
			}

			Sitecore.Diagnostics.Log.Info(string.Format("GetItemVersions End, Versions:{0}", oListOfItemVersionsToReturn), this);

			return oListOfItemVersionsToReturn;
		}

		/// <summary>
		/// Gets the IDs for the children of the given item
		/// </summary>
		/// <param name="sItemId">Sitecore Item ID</param>
		/// <param name="sDatabaseName">Sitecore Database to be used by the data provider</param>
		/// <param name="iProviderCount">Sitecore provider count for the call context</param>
		/// <returns>List of IDs serialized</returns>
		public List<Guid> GetChildIDs(string sItemId, string sDatabaseName)
		{
			#region VARIABLES

			ItemDefinition oItemDefinition;
			List<Guid> oChildIdsList;
			Sitecore.Data.ID oItemId;
			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			IDList oChildList;			

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetChildIDs Start {0}", sItemId), this);

			oChildIdsList = new List<Guid>();

			if (!string.IsNullOrEmpty(sItemId))
			{
				oItemId = new Sitecore.Data.ID(sItemId);

				oWebCallContext = BuildCallContext(sDatabaseName);

				oWebDataProvider = GetDataProvider(sDatabaseName);

				if (oWebDataProvider != null && oWebCallContext != null)
				{
					oItemDefinition = oWebDataProvider.GetItemDefinition(oItemId, oWebCallContext);

					if (oItemDefinition != null)
					{
						oChildList = oWebDataProvider.GetChildIDs(oItemDefinition, oWebCallContext);

						if (oChildList != null)
						{
							foreach (var child in oChildList)
							{										
								oChildIdsList.Add(((ID)child).Guid);
							}
						}
					}
					else
					{
						Sitecore.Diagnostics.Log.Error(string.Format("Unable to get ItemDefinition for item:{0}", sItemId), this);
					}
				}
				else
				{
					if (oWebDataProvider == null)
					{
						Sitecore.Diagnostics.Log.Error("Unable to find data provider", this);
					}
					if (oWebCallContext == null)
					{
						Sitecore.Diagnostics.Log.Error(string.Format("Unable to get CallContext using database:{0}", sDatabaseName), this);
					}
				}
			}

			Sitecore.Diagnostics.Log.Info(string.Format("GetChildIDs End, ChildCount:{0}", oChildIdsList.Count), this);

			return oChildIdsList;
		}

		/// <summary>
		/// Gets the IDs for the children of the given item
		/// </summary>
		/// <param name="sItemId">Sitecore Item ID</param>
		/// <param name="sDatabaseName">Sitecore Database to be used by the data provider</param>
		/// <param name="iProviderCount">Sitecore provider count for the call context</param>
		/// <returns>List of IDs serialized</returns>
		public Dictionary<Guid, List<Guid>> GetItemsChildIDs(List<string> oItemIds, string sDatabaseName)
		{
			#region VARIABLES

			ItemDefinition oItemDefinition;
			List<Guid> oChildIdsList;
			Sitecore.Data.ID oItemId;
			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			IDList oChildList;
			Dictionary<Guid, List<Guid>> oItemsWithChilds;

			//Add variables here...

			#endregion

			oItemsWithChilds = new Dictionary<Guid, List<Guid>>();

			if (oItemIds != null)
			{
				Sitecore.Diagnostics.Log.Info(string.Format("GetChildIDs Start For {0} item(s)", oItemIds.Count), this);

				oWebCallContext = BuildCallContext(sDatabaseName);

				oWebDataProvider = GetDataProvider(sDatabaseName);

				if (oWebDataProvider != null && oWebCallContext != null)
				{
					foreach (string sItemId in oItemIds)
					{
						oItemId = new Sitecore.Data.ID(sItemId);

						if (oItemsWithChilds.ContainsKey(oItemId.Guid))
						{
							continue;
						}

						oItemDefinition = oWebDataProvider.GetItemDefinition(oItemId, oWebCallContext);

						if (oItemDefinition != null)
						{
							oChildList = oWebDataProvider.GetChildIDs(oItemDefinition, oWebCallContext);

							if (oChildList != null)
							{
								oChildIdsList = new List<Guid>();

								foreach (var child in oChildList)
								{
									oChildIdsList.Add(((ID)child).Guid);
								}

								oItemsWithChilds.Add(oItemDefinition.ID.Guid, oChildIdsList);
							}
						}
						else
						{
							Sitecore.Diagnostics.Log.Error(string.Format("Unable to get ItemDefinition for item:{0}", sItemId), this);
						}
					}
				}
				else
				{
					if (oWebDataProvider == null)
					{
						Sitecore.Diagnostics.Log.Error("Unable to find data provider", this);
					}
					if (oWebCallContext == null)
					{
						Sitecore.Diagnostics.Log.Error(string.Format("Unable to get CallContext using database:{0}", sDatabaseName), this);
					}
				}
			}
			Sitecore.Diagnostics.Log.Info(string.Format("GetChildIDs End, ChildCount:{0}", oItemsWithChilds.Count), this);

			return oItemsWithChilds;
		}

		/// <summary>
		/// Gets the Sitecore ID associated to the parent of the given item
		/// </summary>
		/// <param name="sItemId">Sitecore Item ID</param>
		/// <param name="sDatabaseName">Sitecore Database to be used by the data provider</param>
		/// <param name="iProviderCount">Sitecore provider count for the call context</param>
		/// <returns>Sitecore ID serialized</returns>
		public Guid GetParentID(string sItemId, string sDatabaseName)
		{
			#region VARIABLES

			ItemDefinition oItemDefinition;
			Sitecore.Data.ID oItemId;
			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			Sitecore.Data.ID oParentID;
			Guid oParentGuid;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetParentID Start {0}", sItemId), this);

			oParentID = ID.Null;
			oParentGuid = Guid.Empty;
			if (!string.IsNullOrEmpty(sItemId))
			{
				oItemId = new Sitecore.Data.ID(sItemId);

				oWebCallContext = BuildCallContext(sDatabaseName);

				oWebDataProvider = GetDataProvider(sDatabaseName);

				if (oWebDataProvider != null && oWebCallContext != null)
				{
					oItemDefinition = oWebDataProvider.GetItemDefinition(oItemId, oWebCallContext);

					if (oItemDefinition != null)
					{
						oParentID = oWebDataProvider.GetParentID(oItemDefinition, oWebCallContext);
						if (!ID.IsNullOrEmpty(oParentID))
						{
							oParentGuid = oParentID.Guid;
						}
					}
					else
					{
						Sitecore.Diagnostics.Log.Error(string.Format("Unable to get ItemDefinition for item:{0}", sItemId), this);
					}
				}
				else
				{
					if (oWebDataProvider == null)
					{
						Sitecore.Diagnostics.Log.Error("Unable to find data provide", this);
					}
					if (oWebCallContext == null)
					{
						Sitecore.Diagnostics.Log.Error(string.Format("Unable to get CallContext using database:{0}", sDatabaseName), this);
					}
				}
			}			

			Sitecore.Diagnostics.Log.Info(string.Format("GetParentID End, Parentt ID:{0}", oParentGuid), this);

			return oParentGuid;
		}

		/// <summary>
		/// Gets the root item ID for given sitecore database
		/// </summary>
		/// <param name="sDatabaseName">Sitecore Database to be used by the data provider</param>
		/// <param name="iProviderCount">Sitecore provider count for the call context</param>
		/// <returns>Sitecore ID serialized</returns>
		public Guid GetRootID(string sDatabaseName)
		{
			#region VARIABLES

			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			ID oRootID;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetRootID Start, Database:{0}", sDatabaseName), this);

			oRootID = ID.Null;
			oWebCallContext = BuildCallContext(sDatabaseName);

			oWebDataProvider = GetDataProvider(sDatabaseName);

			if (oWebDataProvider != null && oWebCallContext != null)
			{
				oRootID = oWebDataProvider.GetRootID(oWebCallContext);
			}
			else
			{
				if (oWebDataProvider == null)
				{
					Sitecore.Diagnostics.Log.Error("Unable to find data provide", this);
				}
				if (oWebCallContext == null)
				{
					Sitecore.Diagnostics.Log.Error(string.Format("Unable to get CallContext using database:{0}", sDatabaseName), this);
				}
			}

			Sitecore.Diagnostics.Log.Info(string.Format("GetRootID End, Root ID:{0}", oRootID), this);

			return oRootID.Guid;
		}

		/// <summary>
		/// Gets all the templates IDs for the given Sitecore database
		/// </summary>
		/// <param name="sDatabaseName">Sitecore Database to be used by the data provider</param>
		/// <param name="iProviderCount">Sitecore provider count for the call context</param>
		/// <returns>Sitecore Templates IDs serialized</returns>
		public List<Guid> GetTemplateIDs(string sDatabaseName)
		{

			#region VARIABLES

			DataProvider oWebDataProvider;
			CallContext oWebCallContext;			
			IdCollection oTemplateIds;
			List<Guid> oTemplatesGuids;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetTemplateIDs Start, Database:{0}", sDatabaseName), this);

			oTemplatesGuids = new List<Guid>();

			oTemplateIds = null;

			oWebCallContext = BuildCallContext(sDatabaseName);

			oWebDataProvider = GetDataProvider(sDatabaseName);

			if (oWebDataProvider != null && oWebCallContext != null)
			{
				oTemplateIds = oWebDataProvider.GetTemplateItemIds(oWebCallContext);
			}
			else
			{
				if (oWebDataProvider == null)
				{
					Sitecore.Diagnostics.Log.Error("Unable to find data provide", this);
				}
				if (oWebCallContext == null)
				{
					Sitecore.Diagnostics.Log.Error(string.Format("Unable to get CallContext using database:{0}", sDatabaseName), this);
				}
			}

			if (oTemplateIds != null)
			{
				foreach (var oTemplateId in oTemplateIds)
				{
					oTemplatesGuids.Add(oTemplateId.Guid);
				}
			}

			Sitecore.Diagnostics.Log.Info(string.Format("GetTemplateIDs End, Template Count:{0}", oTemplatesGuids.Count), this);

			return oTemplatesGuids;
		}

		public Guid ResolvePath(string sItemPath, string sDatabaseName)
		{
			#region VARIABLES
			
			ID oItemId;
			Item oItem;
			Database oDatabase;

			//Add variables here...

			#endregion			

			oItemId = null;

			if (!string.IsNullOrEmpty(sDatabaseName))
			{
				oDatabase = Sitecore.Data.Database.GetDatabase(sDatabaseName);


				oItem = oDatabase.GetItem(sItemPath);

				if (oItem != null)
				{
					oItemId = oItem.ID;
				}
			}

			return !ID.IsNullOrEmpty(oItemId) ? oItemId.Guid : Guid.Empty;

		}

		public List<Guid> SelectIDs(string sQuery, string sDatabaseName)
		{
			#region VARIABLES
			
			Item[] oQueryResult;
			List<Guid> oIdsToReturn;
			Database oDatabase;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("SelectIDs Start, Query:{0}", sQuery), this);

			oIdsToReturn = new List<Guid>();
			  oDatabase = Sitecore.Data.Database.GetDatabase(sDatabaseName);

			  if (oDatabase != null)
			  {
				  oIdsToReturn = new List<Guid>();
				  if (!string.IsNullOrEmpty(sQuery))
				  {

					  oQueryResult = oDatabase.SelectItems(sQuery);

					  if (oQueryResult != null)
					  {
						  for (int iItemIndex = 0; iItemIndex < oQueryResult.Length; iItemIndex++)
						  {
							  oIdsToReturn.Add(oQueryResult[iItemIndex].ID.Guid);
						  }
					  }
				  }
			  }
			return oIdsToReturn;
		}


		public Guid SelectSingleID(string sQuery, string sDatabaseName)
		{
			#region VARIABLES

			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			ID oQueryResult;
			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("SelectSingleID Start, Query:{0}", sQuery), this);

			oQueryResult = ID.Null;

			oWebCallContext = BuildCallContext(sDatabaseName);

			oWebDataProvider = GetDataProvider(sDatabaseName);

			if (oWebDataProvider != null && !string.IsNullOrEmpty(sQuery))
			{
				oQueryResult = oWebDataProvider.SelectSingleID(sQuery, oWebCallContext);
			}

			return ID.IsNullOrEmpty(oQueryResult) ? oQueryResult.Guid : Guid.Empty;
		}

	
		/// <summary>
		/// Returns the BLOB with the associated id passed as parameter
		/// </summary>
		/// <param name="oBlobId">BLOB Id, this ID is usually diferent from the actual item ID</param>
		/// <param name="sDatabaseName"></param>
		/// <returns></returns>
		public System.IO.MemoryStream GetBlobStream(Guid oBlobId, string sDatabaseName)
		{
			#region VARIABLES

			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			Stream oBlobStream;			
			MemoryStream oMemoryStream;
			byte[] bytes;
			BinaryReader br;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetBlobStream Start, Id:{0}", oBlobId), this);

			oMemoryStream = null;
			oBlobStream = null;
			bytes = null;
			oWebCallContext = BuildCallContext(sDatabaseName);
			oWebDataProvider = GetDataProvider(sDatabaseName);

			if (oWebDataProvider != null)
			{				
				oBlobStream = oWebDataProvider.GetBlobStream(oBlobId, oWebCallContext);
				if (oBlobStream != null)
				{
					//we need to force the loading of the stream to know the actual size of the strem we will return					
					//otherwise we will get an exception saying that Stream.Length is not supported
					bytes = new byte[oBlobStream.Length];
					br = new BinaryReader(oBlobStream);
					bytes = br.ReadBytes((int)oBlobStream.Length);

					br.Close();
					oBlobStream.Close();

					if (bytes.Length > 0)
					{
						oMemoryStream = new MemoryStream(bytes);
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error(string.Format("GetBlobStream Unable to find blob with id {0}", oBlobId), this);
				}
			}
			else
			{
				Sitecore.Diagnostics.Log.Error("GetBlobStream Unable to parse ID", this);
			}

			Sitecore.Diagnostics.Log.Info(string.Format("GetBlobStream ends, Bytes:{0}", bytes != null? bytes.Length : 0), this);

			return oMemoryStream;
		}


		/// <summary>
		/// Determines whether a BLOB exists using its ID
		/// </summary>
		/// <param name="oBlobId">BLOB Id, this ID is usually diferent from the actual item ID</param>
		/// <param name="sDatabaseName"></param>
		/// <returns></returns>
		public bool BlobStreamExists(Guid oBlobId, string sDatabaseName)
		{
			#region VARIABLES

			DataProvider oWebDataProvider;
			CallContext oWebCallContext;
			bool bBlobExists;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("BlobStreamExists Start, Id:{0}", oBlobId), this);

			bBlobExists = false;
			oWebCallContext = BuildCallContext(sDatabaseName);

			oWebDataProvider = GetDataProvider(sDatabaseName);

			if (oWebDataProvider != null)
			{
				bBlobExists = oWebDataProvider.BlobStreamExists(oBlobId, oWebCallContext);
			}

			Sitecore.Diagnostics.Log.Info(string.Format("BlobStreamExists Ends, Found:{0}", bBlobExists), this);

			return bBlobExists;
		}

						
		/// <summary>
		/// Gets he languages available for the given database
		/// </summary>
		/// <param name="sDatabaseName"></param>
		/// <returns></returns>
		public List<LanguageContract> GetLanguages(string sDatabaseName)
		{
			#region VARIABLES

			DataProvider oWebDataProvider;
			CallContext oWebCallContext;			
			List<LanguageContract> oLanguages;
			LanguageCollection oSitecoreLanguages;

			//Add variables here...

			#endregion

			Sitecore.Diagnostics.Log.Info(string.Format("GetLanguages Start, Database:{0}", sDatabaseName), this);

			oLanguages = null;			

			oWebCallContext = BuildCallContext(sDatabaseName);

			
			oWebDataProvider = GetDataProvider(sDatabaseName);

			if (oWebDataProvider != null)
			{
				oLanguages = new List<LanguageContract>();			

				oSitecoreLanguages = oWebDataProvider.GetLanguages(oWebCallContext);

				if (oSitecoreLanguages != null)
				{
					foreach (Language oLanguage in oSitecoreLanguages)
					{
						oLanguages.Add(new LanguageContract() { Name = oLanguage.Name, OriginItemId = oLanguage.Origin.ItemId.ToString() });
					}
				}
			}

			return oLanguages;
		}
		
	}
}
