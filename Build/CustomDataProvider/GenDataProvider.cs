using System.Diagnostics;
using Sitecore.Collections;
using Sitecore.Globalization;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using Genworth.SitecoreExt.Services.Providers;
using System.Collections.Generic;
using System;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Sitecore.Xml;
using System.Collections.ObjectModel;
using Sitecore;
using Sitecore.Caching;
using System.Threading;
using Sitecore.Configuration;
using System.Text;
using System.Collections;
using System.ServiceModel;

namespace Genworth.SitecoreExt.CustomDataProvider
{
    /// <summary>
    /// Custom data provider that extracts sitecore content from the Genworth Content Service 
    /// </summary>
    public class GenDataProvider : DataProvider
    {
        #region VARIABLES

        #region CONSTANTS

        private const string GenworthProviderCacheName = "Genworth - SqlDataProvider - Prefetch data";

        private const string ProviderName = "GenDataProvider";

        #endregion


        private readonly Collection<Pair<string, ID>> oPrefetchSpecifications;

        private int oPrefetchChildLimit;

        private bool bLogPrefetchStatistics;

        protected readonly object oPrefetchCacheLock;

        private long lPrefetchCacheSize;

        private Cache oPrefetchCache;

        protected readonly ReaderWriterLock oDescendantsLock;

        private bool bInitialDataPrefetched;

        private readonly object oPrefetchLock;

        private string sName;

        private readonly SafeDictionary<ID, int> oPrefetchStatistics;

        private string sCurrentDatabase;


        #endregion

        #region PROPERTIES


        protected LanguageCollection Languages { get; set; }

        /// <summary>
        /// Database being used in the current data access
        /// </summary>
        private string CurrentDatabase
        {
            get
            {
                return sCurrentDatabase;
            }
            set
            {
                sCurrentDatabase = value;
            }
        }



        /// <summary>
        /// Provicer Cache
        /// </summary>
        protected Cache PrefetchCache
        {
            get
            {
                string sCacheName;
                Cache oNamedInstance;

                if (this.oPrefetchCache == null)
                {
                    lock (this.oPrefetchCacheLock)
                    {
                        if (this.oPrefetchCache == null)
                        {
                            sCacheName = "SqlDataProvider - Prefetch data";
                            if (!string.IsNullOrEmpty(this.sName))
                            {
                                sCacheName = string.Format("{0}({1})", sCacheName, this.sName);
                            }
                            oNamedInstance = Cache.GetNamedInstance(sCacheName, this.lPrefetchCacheSize);
                            if (this.CacheOptions.DisableAll)
                            {
                                oNamedInstance.Enabled = false;
                            }
                            this.oPrefetchCache = oNamedInstance;
                        }
                    }

                }
                return this.oPrefetchCache;
            }
        }


        /// <summary>
        /// Exposes this provider name
        /// </summary>
        public virtual string Name
        {
            get
            {
                string sNameToReturn;

                if (string.IsNullOrEmpty(this.sName))
                {
                    sNameToReturn = ProviderName;
                }
                else
                {
                    sNameToReturn = this.sName;
                }

                return sNameToReturn;
            }
            set
            {
                this.sName = value;
            }
        }


        #endregion


        #region CONSTRUCTOR


        public GenDataProvider()
            : base()
        {
            this.oDescendantsLock = new ReaderWriterLock();
            this.oPrefetchCacheLock = new object();
            this.oPrefetchLock = new object();
            this.oPrefetchSpecifications = new Collection<Pair<string, ID>>();
            this.oPrefetchStatistics = new SafeDictionary<ID, int>();
            this.lPrefetchCacheSize = Settings.Caching.DefaultDataCacheSize;
            this.oPrefetchChildLimit = 10000;
        }

        #endregion

        #region METHODS


        /// <summary>
        /// Gets the item definition, which tells Sitecore what template the item inherits from and the item’s name
        /// </summary>
        /// <param name="oItemId">Sitecore Item ID</param>
        /// <param name="oContext">Sitecore Call context</param>
        /// <returns>Item definition</returns>
        public override ItemDefinition GetItemDefinition(ID oItemId, CallContext oContext)
        {
            #region VARIABLES

            ItemDefinition oItemDefinition;
            PrefetchData oPrefetchData;

            //Add variables here...

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("GenDataProvider.GetItemDefinition {0}", oItemId), this);

            oItemDefinition = null;

            CurrentDatabase = oContext.DataManager.Database.Name;

            oPrefetchData = this.GetPrefetchData(oItemId);
            if (oPrefetchData == null)
            {
                oItemDefinition = null;
            }
            else
            {
                oItemDefinition = oPrefetchData.ItemDefinition;

                if (oItemDefinition != null)
                {
                    UpdateContext(oItemDefinition, oContext);
                }
            }
            return oItemDefinition;

        }

        /// <summary>
        ///  Fills the item’s fields
        /// </summary>
        /// <param name="oItemDefinition">Sitecore item definition</param>
        /// <param name="oVersionUri">Sitecore version uri</param>
        /// <param name="oContext">Sitecore call context</param>
        /// <returns>List with the fields associated to the item</returns>
        public override FieldList GetItemFields(ItemDefinition oItemDefinition, VersionUri oVersionUri, CallContext oContext)
        {
            #region VARIABLES

            FieldList oFieldList;
            PrefetchData oPrefetchData;

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("GetItemFields {0} Language {1}  Version {2}", oItemDefinition.ID, oVersionUri.Language.Name, oVersionUri.Version.Number), this);

            oFieldList = null;

            oPrefetchData = this.GetPrefetchData(oItemDefinition.ID);

            if (oPrefetchData != null)
            {
                oFieldList = oPrefetchData.GetFieldList((string.Equals(Language.Invariant.Name, oVersionUri.Language.Name) ? ("en") : (oVersionUri.Language.Name)),
                                                        oVersionUri.Version.Number
                                                        );
                if (oFieldList == null)
                {
                    oFieldList = oPrefetchData.GetSharedFields();
                }
            }

            if (oFieldList != null)
            {
                foreach (var oF in oFieldList.GetFieldIDs())
                {
                    Sitecore.Diagnostics.Log.Debug(string.Format("GetItemFields, Field {0}   Value {1}", oF, oFieldList[oF]), this);
                }
                UpdateContext(oFieldList, oContext);
            }

            return oFieldList;
        }


        /// <summary>
        ///  Tells how many languages and version an item contains
        /// </summary>
        /// <param name="oItemDefinition">Sitecore item definition</param>
        /// <param name="oContext">Sitecore call context</param>
        /// <returns>List o versions associated to the item </returns>
        public override VersionUriList GetItemVersions(ItemDefinition oItemDefinition, CallContext oContext)
        {
            #region VARIABLES

            VersionUriList oVersionUriList;
            PrefetchData oPrefetchData;

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("GetItemVersions {0}", oItemDefinition.ID), this);

            oPrefetchData = this.GetPrefetchData(oItemDefinition.ID);

            if (oPrefetchData == null)
            {
                oVersionUriList = null;
            }
            else
            {
                oVersionUriList = oPrefetchData.GetVersionUris();
                if (oVersionUriList != null)
                {
                    UpdateContext(oVersionUriList, oContext);
                }
            }

            return oVersionUriList;
        }

        /// <summary>
        /// Indicates whether the given ItemDefinition has children
        /// </summary>
        /// <param name="itemDefinition"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool HasChildren(ItemDefinition itemDefinition, CallContext context)
        {
            bool hasChildren;
            PrefetchData prefetchData;

            hasChildren = true;

            prefetchData = this.GetPrefetchData(itemDefinition.ID);
            if (prefetchData == null)
            {
                hasChildren = false;
            }
            else
            {
                hasChildren = (prefetchData.GetChildIds().Count > 0);
            }

            return hasChildren;
        }

        /// <summary>
        /// Sitecore request and item children IDs
        /// </summary>
        /// <param name="oItemDefinition">Sitecore item definition</param>
        /// <param name="oContext">Sitecore call context</param>
        /// <returns>IDList with the IDs for the children items of the given item </returns>
        public override IDList GetChildIDs(ItemDefinition oItemDefinition, CallContext oContext)
        {
            #region VARIABLES

            IDList oIDsList;
            object[] oPrefetchParameters;
            ArrayList oIdsToPrefetch;

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("GetChildIDs {0}", oItemDefinition.ID), this);

            oIDsList = null;


            PrefetchData prefetchData = this.GetPrefetchData(oItemDefinition.ID);
            if (prefetchData != null)
            {
                oIDsList = prefetchData.GetChildIds();
                if (oIDsList != null && oIDsList.Count > 1)
                {
                    oIdsToPrefetch = new ArrayList();
                    foreach (ID oItemId in oIDsList)
                    {
                        if (!this.PrefetchCache.ContainsKey(oItemId))
                        {
                            oIdsToPrefetch.Add("ItemId");
                            oIdsToPrefetch.Add(oItemDefinition.ID);
                        }
                    }

                    if (oIdsToPrefetch.Count > 0)
                    {
                        oPrefetchParameters = oIdsToPrefetch.ToArray(typeof(object)) as object[];
                        PrefetchItems(oPrefetchParameters);
                    }
                }
            }


            return oIDsList;
        }

        /// <summary>
        /// Retrieves the parent ID for the given sitecore item ID
        /// </summary>
        /// <param name="oItemDefinition">Sitecore item definition</param>
        /// <param name="oContext">Sitecore call context</param>
        /// <returns>ID associated to the item parent of item passed as a parameter</returns>
        public override ID GetParentID(ItemDefinition oItemDefinition, CallContext oContext)
        {
            #region VARIABLES

            ID oParentId;
            PrefetchData oPrefetchData;

            #endregion

            oParentId = null;

            Sitecore.Diagnostics.Log.Debug(string.Format("GetParentID {0}", oItemDefinition.ID), this);

            oPrefetchData = this.GetPrefetchData(oItemDefinition.ID);

            if ((oPrefetchData != null) && !oPrefetchData.ParentId.IsNull)
            {
                oParentId = oPrefetchData.ParentId;
                UpdateContext(oParentId, oContext);
            }
            else
            {
                oParentId = null;
            }

            return oParentId;
        }

        /// <summary>
        /// Gets the root item for the context environment
        /// </summary>
        /// <param name="oContext">Sitecore call context</param>
        /// <returns>ID associated to the root item in the given context</returns>
        public override ID GetRootID(CallContext oContext)
        {
            #region VARIABLES

            //Add variables here...

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("GetRootID {0}", oContext.ToString()), this);

            UpdateContext(ItemIDs.RootID, oContext);

            return ItemIDs.RootID;
        }

        public IdCollection GetTemplateIds(string sDatabaseName)
        {
            #region VARIABLES

            List<Guid> oGuidList;
            IdCollection oTemplateIdsCollection;

            #endregion

            oTemplateIdsCollection = null;

            using (var oContentService = new GenContentServiceProxy())
            {
                oGuidList = oContentService.GetTemplateIDs(sDatabaseName);
            }

            if (oGuidList != null && oGuidList.Count > 0)
            {
                oTemplateIdsCollection = new IdCollection();

                if (oGuidList != null)
                {
                    foreach (var oGuid in oGuidList)
                    {
                        oTemplateIdsCollection.Add(new ID(oGuid));
                    }
                }
            }

            return oTemplateIdsCollection;
        }


        /// <summary>
        /// Gets a collection with the Sitecore IDs of all the templates in the current database
        /// </summary>
        /// <param name="oContext"></param>
        /// <returns></returns>
        public override IdCollection GetTemplateItemIds(CallContext oContext)
        {
            #region VARIABLES

            IdCollection oTemplateIdsCollection;

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("GetTemplateItemIds {0}", oContext.ToString()), this);

            oTemplateIdsCollection = null;

            try
            {
                oTemplateIdsCollection = GetTemplateIds(oContext.DataManager.Database.Name);
                if (oTemplateIdsCollection != null)
                {
                    UpdateContext(oTemplateIdsCollection, oContext);
                    oContext.Abort();
                    Sitecore.Diagnostics.Log.Debug(string.Format("GetTemplateItemIds, Templates Count:{0}", oTemplateIdsCollection.Count), this);
                }
            }
            catch
            {
                Sitecore.Diagnostics.Log.Error(string.Format("Unable to get Get templateItem ids for database:{0}", oContext.DataManager.Database.Name), this);
                throw;
            }


            return oTemplateIdsCollection;
        }


        /// <summary>
        /// Required by the Sitecore Sql DataProvider (Related to Preteching data from the configuration file)
        /// </summary>
        /// <param name="configNode"></param>
        public void AddPrefetch(XmlNode configNode)
        {
            Sitecore.Diagnostics.Log.Debug(string.Format("AddPrefetch localName:{0} InnerText:{1}", configNode.LocalName, configNode.InnerText), this);

            string localName = configNode.LocalName;
            if (localName != null)
            {
                if (!(localName == "cacheSize"))
                {
                    if (!(localName == "children"))
                    {
                        if (!(localName == "item"))
                        {
                            if (!(localName == "logStats"))
                            {
                                if (!(localName == "childLimit"))
                                {
                                    if (localName == "template")
                                    {
                                        this.oPrefetchSpecifications.Add(new Pair<string, ID>("TemplateID", ID.Parse(XmlUtil.GetValue(configNode))));
                                    }
                                    return;
                                }
                                this.oPrefetchChildLimit = int.Parse(XmlUtil.GetValue(configNode));
                                return;
                            }
                            this.bLogPrefetchStatistics = XmlUtil.GetValue(configNode) == "true";
                            return;
                        }
                        this.oPrefetchSpecifications.Add(new Pair<string, ID>("ID", ID.Parse(XmlUtil.GetValue(configNode))));
                        return;
                    }
                }
                else
                {
                    lock (this.oPrefetchCacheLock)
                    {
                        this.lPrefetchCacheSize = StringUtil.ParseSizeString(XmlUtil.GetValue(configNode));
                        this.oPrefetchCache = null;
                        return;
                    }
                }
                this.oPrefetchSpecifications.Add(new Pair<string, ID>("ParentID", ID.Parse(XmlUtil.GetValue(configNode))));

            }
        }

        private Pair<string, object[]> GetPrefetchSpecifications()
        {
            #region VARIABLES

            Pair<string, object[]> oPrefetchSpecificatiosToReturn;
            StringBuilder oSpecificationsBuilder;
            int iSpecificationIndex;
            string sSpecificationIdentifier;
            ArrayList oSpecificationsList;
            string sPrefetchSpecificationsIdentifier;

            #endregion

            Log.Debug("GetPrefetchSpecifications Begins", this);

            oPrefetchSpecificatiosToReturn = null;

            if (this.oPrefetchSpecifications.Count > 0)
            {

                oSpecificationsBuilder = new StringBuilder();
                oSpecificationsList = new ArrayList();
                iSpecificationIndex = 0;
                foreach (Pair<string, ID> pair in this.oPrefetchSpecifications)
                {
                    sSpecificationIdentifier = "p" + iSpecificationIndex;
                    oSpecificationsBuilder.Append("{0}" + pair.Part1 + "{1}={2}");
                    oSpecificationsBuilder.Append(sSpecificationIdentifier);
                    oSpecificationsBuilder.Append("{3} OR ");
                    oSpecificationsList.Add(sSpecificationIdentifier);
                    Log.Debug(sSpecificationIdentifier, this);
                    oSpecificationsList.Add(pair.Part2);
                    iSpecificationIndex++;
                }
                sPrefetchSpecificationsIdentifier = oSpecificationsBuilder.ToString(0, oSpecificationsBuilder.Length - 4);
                oPrefetchSpecificatiosToReturn = new Pair<string, object[]>(sPrefetchSpecificationsIdentifier, oSpecificationsList.ToArray(typeof(object)) as object[]);
            }

            Log.Debug("GetPrefetchSpecifications Ends", this);

            return oPrefetchSpecificatiosToReturn;
        }


        /// <summary>
        /// Verifies if prefetch specifications have been loaded
        /// </summary>
        private void EnsureInitialPrefetch()
        {
            #region VARIABLES

            Pair<string, object[]> oPrefetchSpecificationsToCheck;
            object[] oParameters;

            #endregion

            if (!this.bInitialDataPrefetched)
            {
                lock (this.oPrefetchLock)
                {
                    if (!this.bInitialDataPrefetched)
                    {
                        oPrefetchSpecificationsToCheck = this.GetPrefetchSpecifications();
                        if (oPrefetchSpecificationsToCheck != null)
                        {
                            oParameters = oPrefetchSpecificationsToCheck.Part2;
                            this.PrefetchItems(oParameters);
                            this.AddPrefetchDataToCache(ID.Null, new PrefetchData(ItemDefinition.Empty, ID.Null));
                        }
                        this.bInitialDataPrefetched = true;
                    }
                }
            }
        }

        /// <summary>
        /// Saves and item data to the cache
        /// </summary>
        /// <param name="oItemId"></param>
        /// <param name="oData"></param>
        private void AddPrefetchDataToCache(ID oItemId, PrefetchData oData)
        {
            #region VARIABLES
            ItemDefinition oItemDefinition;
            ID oTemplateID;

            #endregion
            this.PrefetchCache.Add(oItemId, oData, oData.GetDataLength());
            if (this.bLogPrefetchStatistics && this.bInitialDataPrefetched)
            {
                oItemDefinition = oData.ItemDefinition;
                if (!oItemDefinition.IsEmpty)
                {
                    oTemplateID = oItemDefinition.TemplateID;
                    lock (this.oPrefetchStatistics.SyncRoot)
                    {
                        this.oPrefetchStatistics[oTemplateID] += 1;
                    }
                }
            }
        }


        /// <summary>
        /// Gets the prefetch data from cache
        /// </summary>
        /// <param name="oItemId"></param>
        /// <returns></returns>
        protected PrefetchData GetPrefetchDataFromCache(ID oItemId)
        {
            #region VARIABLES

            PrefetchData oData;

            #endregion

            oData = this.PrefetchCache[oItemId] as PrefetchData;
            if ((oData != null) && oData.ItemDefinition.IsEmpty)
            {
                oData = null;
            }

            return oData;
        }

        /// <summary>
        /// Gets an item from cache
        /// </summary>
        /// <param name="oItemId"></param>
        /// <returns></returns>
        private PrefetchData PrefetchItem(ID oItemId)
        {
            SafeDictionary<ID, PrefetchData> oDictionary;

            oDictionary = this.PrefetchItems(new object[] { "itemId", oItemId });
            if (oDictionary == null)
            {
                return null;
            }
            return oDictionary[oItemId];
        }


        /// <summary>
        /// Tries to dull an item data from cache trying to load it if it has not been already loaded
        /// </summary>
        /// <param name="oItemId"></param>
        /// <returns></returns>
        private PrefetchData GetPrefetchData(ID oItemId)
        {
            PrefetchData oData;

            this.EnsureInitialPrefetch();

            oData = this.PrefetchCache[oItemId] as PrefetchData;
            if (oData == null)
            {

                oData = this.PrefetchItem(oItemId);
                if (oData == null)
                {
                    oData = new PrefetchData(ItemDefinition.Empty, ID.Null);
                    lock (this.PrefetchCache.SyncRoot)
                    {
                        if (!this.PrefetchCache.ContainsKey(oItemId))
                        {
                            this.PrefetchCache.Add(oItemId, oData, oData.GetDataLength());
                        }
                    }
                    oData = this.GetPrefetchDataFromCache(oItemId);
                }
            }
            else
            {
                if (oData.ItemDefinition.IsEmpty)
                {
                    oData = null;
                }
            }

            return oData;
        }



        private SafeDictionary<ID, PrefetchData> PrefetchItems(params object[] oParameters)
        {
            SafeDictionary<ID, PrefetchData> oPrefetchData;

            oPrefetchData = new SafeDictionary<ID, PrefetchData>();
            this.LoadItemDefinitions(oParameters, oPrefetchData);
            if (oPrefetchData.Count == 0)
            {
                return null;
            }
            this.LoadItemFields(oParameters, oPrefetchData);
            this.LoadChildIds(oParameters, oPrefetchData);
            foreach (PrefetchData oData in oPrefetchData.Values)
            {
                this.AddPrefetchDataToCache(oData.ItemDefinition.ID, oData);
            }
            this.ShowPrefetchStats();
            return oPrefetchData;
        }



        /// <summary>
        /// Load the childs for the items passed through the parameters
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="parameters"></param>
        /// <param name="prefetchData"></param>
        protected virtual void LoadChildIds(object[] parameters, SafeDictionary<ID, PrefetchData> prefetchData)
        {
            List<string> oItemIds;
            PrefetchData data = null;
            ID @null;
            ID oCurrentItemId;
            List<Guid> oChildIds;
            Dictionary<Guid, List<Guid>> oItemsWithChilds;
            @null = ID.Null;

            oItemIds = ParseIDsFromParameters(parameters);

            if (oItemIds != null)
            {
                using (var oContentService = new GenContentServiceProxy())
                {
                    oItemsWithChilds = oContentService.GetItemsChildIDs(oItemIds, this.CurrentDatabase);
                }

                foreach (var oItemChild in oItemsWithChilds)
                {
                    oCurrentItemId = ID.Parse(oItemChild.Key);

                    if (oCurrentItemId != @null)
                    {
                        data = prefetchData[oCurrentItemId];
                        @null = oCurrentItemId;
                    }
                    if (data != null)
                    {
                        oChildIds = oItemChild.Value;
                        foreach (Guid oChild in oChildIds)
                        {
                            data.AddChildId(ID.Parse(oChild));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Logs cache statistics when loging is enabled
        /// </summary>
        private void ShowPrefetchStats()
        {
            if (this.bLogPrefetchStatistics && this.bInitialDataPrefetched)
            {
                Log.Debug(string.Format("--- Template usage begin({0})", this.Name), this);
                lock (this.oPrefetchStatistics.SyncRoot)
                {
                    foreach (KeyValuePair<ID, int> pair in this.oPrefetchStatistics)
                    {
                        Log.Debug(string.Concat(new object[] { "Count: ", pair.Value, ", Template: ", pair.Key }), this);
                    }
                }
                Log.Debug(string.Format("--- Template usage end({0})", this.Name), this);
            }
        }

        /// <summary>
        /// Gets all the languages available in the current database
        /// </summary>
        /// <returns></returns>
        protected virtual LanguageCollection LoadLanguages()
        {
            #region VARIABLES

            LanguageCollection oLanguages;
            List<LanguageContract> oGenLanguages;
            ID oLanguageOriginItemId;
            Language oLanguage;

            #endregion

            oLanguages = null;

            using (var oContentService = new GenContentServiceProxy())
            {
                oGenLanguages = oContentService.GetLanguages(this.CurrentDatabase);
            }

            if (oGenLanguages != null)
            {
                oLanguages = new LanguageCollection();
                foreach (LanguageContract sGenLanguage in oGenLanguages)
                {
                    if (!string.IsNullOrEmpty(sGenLanguage.OriginItemId) && !string.IsNullOrEmpty(sGenLanguage.Name) && !string.Equals(sGenLanguage.Name, "*") && ID.TryParse(sGenLanguage.OriginItemId, out oLanguageOriginItemId))
                    {
                        if (Language.TryParse(sGenLanguage.Name, out oLanguage) && !oLanguages.Contains(oLanguage))
                        {
                            oLanguage.Origin.ItemId = oLanguageOriginItemId;
                            oLanguages.Add(oLanguage);
                        }
                    }
                }
            }

            return oLanguages;

        }

        /// <summary>
        /// Returns the languages available in the current Sitecore database
        /// </summary>
        /// <returns></returns>
        public override LanguageCollection GetLanguages(CallContext oContext)
        {
            return GetLanguages();
        }

        /// <summary>
        /// Gets a collection of languages containig the available languages for the environment
        /// </summary>
        /// <returns></returns>
        public LanguageCollection GetLanguages()
        {
            LanguageCollection languages;
            languages = this.Languages;
            if (languages == null)
            {
                languages = this.LoadLanguages();
                if (languages == null)
                {
                    languages = new LanguageCollection();
                }
                this.Languages = languages;
            }
            return languages;
        }

        /// <summary>
        /// Loads the fields for the collection of items passed as parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="prefetchData"></param>
        protected virtual void LoadItemFields(object[] parameters, SafeDictionary<ID, PrefetchData> prefetchData)
        {
            LanguageCollection languages = this.GetLanguages();
            List<string> oItemIds;
            PrefetchData data = null;
            ID oCurrentItemId;
            List<FieldContract> oGenFieldsForItem;
            ID @null = ID.Null;
            Dictionary<string, List<FieldContract>> oItemsWithFields;

            oItemIds = ParseIDsFromParameters(parameters);

            Sitecore.Diagnostics.Log.Debug("LoadItemFields - Begins", this);

            if (oItemIds != null)
            {
                using (var oContentService = new GenContentServiceProxy())
                {
                    oItemsWithFields = oContentService.GetItemsFields(oItemIds, this.CurrentDatabase);
                }

                foreach (string sItemId in oItemIds)
                {
                    if (ID.TryParse(sItemId, out oCurrentItemId))
                    {
                        if (oCurrentItemId != @null)
                        {
                            data = prefetchData[oCurrentItemId];
                            if (data != null)
                            {
                                data.InitializeFieldLists(languages);
                            }
                            @null = oCurrentItemId;
                        }

                        oGenFieldsForItem = oItemsWithFields[sItemId];

                        if ((data != null) && oGenFieldsForItem != null && oGenFieldsForItem.Count > 0)
                        {
                            foreach (FieldContract oGenField in oGenFieldsForItem)
                            {
                                data.AddField(oGenField.Language, oGenField.Version, ID.Parse(oGenField.Id), oGenField.Value);
                                Sitecore.Diagnostics.Log.Debug(string.Format("Field:{0}   Value:{1}  Language:{2} Version:{3}", oGenField.Id, oGenField.Value, oGenField.Language, oGenField.Version), this);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Helpers that allow the extraction of the sitecore Ids from an array of object in the form of {"ItemId", "{Sitecore Id}"}
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private List<string> ParseIDsFromParameters(object[] parameters)
        {
            #region VARIABLES
            List<string> oItemIds;
            int iParameterIndex;
            string sItemId;
            //Add variables here...

            #endregion

            oItemIds = new List<string>();

            if (parameters != null && parameters.Length > 1)
            {
                oItemIds = new List<string>();
                for (iParameterIndex = 0; iParameterIndex < parameters.Length; iParameterIndex += 2)
                {
                    if (ID.IsID(parameters[iParameterIndex + 1].ToString()))
                    {
                        sItemId = parameters[iParameterIndex + 1].ToString();

                        oItemIds.Add(sItemId);
                    }
                }
            }

            return oItemIds;
        }

        /// <summary>
        /// Loads a set item definitions into the cache by using the parameters array passed as parameter. Parameters are in the form of {"ItemId", "{Sitecore Id}"}
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="prefetchData"></param>
        protected virtual void LoadItemDefinitions(object[] parameters, SafeDictionary<ID, PrefetchData> prefetchData)
        {
            #region VARIABLES

            ItemDefinition oItemDefinition;
            string sItemId;
            int iParameterIndex;
            List<string> oItemIds;
            List<ItemDefinitionContract> oItemDefinitions;
            PrefetchData oData;

            //Add variables here...

            #endregion

            if (parameters != null && parameters.Length > 1)
            {
                oItemIds = new List<string>();
                for (iParameterIndex = 0; iParameterIndex < parameters.Length; iParameterIndex += 2)
                {
                    if (ID.IsID(parameters[iParameterIndex + 1].ToString()))
                    {
                        sItemId = parameters[iParameterIndex + 1].ToString();

                        oItemIds.Add(sItemId);

                    }
                }

                if (oItemIds.Count > 0)
                {
                    using (var oContentService = new GenContentServiceProxy())
                    {
                        oItemDefinitions = oContentService.GetItemDefinitions(oItemIds, this.CurrentDatabase);
                    }

                    if (oItemDefinitions != null && oItemDefinitions.Count > 0)
                    {
                        try
                        {
                            foreach (ItemDefinitionContract oItemDefinitionContract in oItemDefinitions)
                            {
                                oItemDefinition = new Sitecore.Data.ItemDefinition(
                                                                            new Sitecore.Data.ID(oItemDefinitionContract.Id),
                                                                            !string.IsNullOrEmpty(oItemDefinitionContract.Name) ? oItemDefinitionContract.Name : string.Empty,
                                                                            !string.IsNullOrEmpty(oItemDefinitionContract.TemplateId) ? new Sitecore.Data.ID(oItemDefinitionContract.TemplateId) : null,
                                                                            !string.IsNullOrEmpty(oItemDefinitionContract.BranchId) ? new Sitecore.Data.ID(oItemDefinitionContract.BranchId) : null
                                                                            );

                                oData = new PrefetchData(oItemDefinition, !string.IsNullOrEmpty(oItemDefinitionContract.ParentId) ? new Sitecore.Data.ID(oItemDefinitionContract.ParentId) : ID.Null);

                                prefetchData[oItemDefinition.ID] = oData;

                                Sitecore.Diagnostics.Log.Debug(string.Format("GetItemDefinition, Item Cached:{0}", oItemDefinition.ID.ToString()), this);
                            }
                        }
                        catch
                        {
                            Sitecore.Diagnostics.Log.Error("GetItemDefinition, Unable to get item definitions", this);
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Execute the selection 
        /// </summary>
        /// <param name="sQuery"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override IDList SelectIDs(string sQuery, CallContext context)
        {
            List<Guid> oResults;
            IDList oSelectedIds;
            bool bFound;

            Sitecore.Diagnostics.Log.Debug(string.Format("SelectIDs Start, Query:{0}", sQuery), this);

            oSelectedIds = new IDList();
            bFound = false;

            using (var oContentService = new GenContentServiceProxy())
            {
                oResults = oContentService.SelectIDs(sQuery, context.DataManager.Database.Name);
            }

            if (oResults != null && oResults.Count > 0)
            {
                foreach (var oItemId in oResults)
                {
                    oSelectedIds.Add(ID.Parse(oItemId));
                }
                bFound = true;
            }

            if (bFound)
            {
                UpdateContext(oSelectedIds, context);
                context.Abort();
            }

            return oSelectedIds;
        }

        public override ID SelectSingleID(string query, CallContext context)
        {
            Guid oResult;
            ID oSelectedId;
            bool bFound;
            oSelectedId = ID.Null;
            bFound = false;

            using (var oContentService = new GenContentServiceProxy())
            {
                oResult = oContentService.SelectSingleID(query, context.DataManager.Database.Name);
            }

            if (oResult != null && oResult == Guid.Empty)
            {
                oSelectedId = ID.Parse(oSelectedId);
                bFound = true;
            }

            if (bFound)
            {
                UpdateContext(oSelectedId, context);
                context.Abort();
            }
            return oSelectedId;
        }

        public override System.IO.Stream GetBlobStream(Guid oBlobId, CallContext context)
        {

            #region VARIABLES

            System.IO.MemoryStream oBlobStream;
            //Add variables here...

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("GetBlobStream Start, Id:{0}", oBlobId), this);

            using (var oContentService = new GenContentServiceProxy())
            {
                oBlobStream = oContentService.GetBlobStream(oBlobId, context.DataManager.Database.Name);
            }

            if (oBlobStream == null)
            {
                Sitecore.Diagnostics.Log.Error(string.Format("GetBlobStream Unable to get BLOB with Id:{0}", oBlobId), this);
            }

            return oBlobStream;
        }

        public override bool BlobStreamExists(Guid oBlobId, CallContext context)
        {
            #region VARIABLES

            bool bBlobExists;
            //Add variables here...

            #endregion

            Sitecore.Diagnostics.Log.Debug(string.Format("BlobStreamExists Start, Id:{0}", oBlobId), this);

            using (var oContentService = new GenContentServiceProxy())
            {
                bBlobExists = oContentService.BlobStreamExists(oBlobId, context.DataManager.Database.Name);
            }

            Sitecore.Diagnostics.Log.Debug(string.Format("BlobStreamExists Ends, Exits:{0}", bBlobExists), this);

            return bBlobExists;
        }

        public override Sitecore.Eventing.EventQueue GetEventQueue()
        {
            return new RemoteClientEventQueue();
        }


        #region CONTEXT HELPERS

        /// <summary>
        /// Updates the context result with the given Id Collection
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        private static void UpdateContext(IdCollection result, CallContext context)
        {
            if (result != null)
            {
                IdCollection currentResult = context.CurrentResult as IdCollection;
                if (currentResult != null)
                {
                    result = IdCollection.Combine(currentResult, result);
                }
                context.CurrentResult = result;
            }
            context.SetNextIndex(context.Index + 1);
        }

        /// <summary>
        /// Updates the context result with the given Id list
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        private static void UpdateContext(IDList result, CallContext context)
        {
            if (result != null)
            {
                IDList currentResult = context.CurrentResult as IDList;
                context.CurrentResult = IDList.Add(currentResult, result);
            }
            context.SetNextIndex(context.Index + 1);
        }

        /// <summary>
        /// Updates the context result with the given Id
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        private static void UpdateContext(ID result, CallContext context)
        {
            if (!ID.IsNullOrEmpty(result))
            {
                context.CurrentResult = result;
            }
            context.SetNextIndex(context.Index + 1);
        }

        /// <summary>
        /// Updates the context result with the given Language Collection
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        private static void UpdateContext(LanguageCollection result, CallContext context)
        {
            if (result != null)
            {
                LanguageCollection currentResult = context.CurrentResult as LanguageCollection;
                if (currentResult != null)
                {
                    Language[] languageArray = (Language[])MainUtil.AddArrays(currentResult.ToArray(), result.ToArray(), typeof(Language));
                    result = new LanguageCollection();
                    for (int i = 0; i < languageArray.Length; i++)
                    {
                        result.Add(languageArray[i]);
                    }
                }
                context.CurrentResult = result;
            }
            context.SetNextIndex(context.Index + 1);
        }

        /// <summary>
        /// Updates the context result with the given Version Uri List
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        private static void UpdateContext(VersionUriList result, CallContext context)
        {
            if ((result != null) && (result.Count > 0))
            {
                VersionUriList currentResult = context.CurrentResult as VersionUriList;
                context.CurrentResult = VersionUriList.Add(currentResult, result);
            }
            context.SetNextIndex(context.Index + 1);
        }

        /// <summary>
        /// Updates the context result with the given Field List
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        private static void UpdateContext(FieldList result, CallContext context)
        {
            if ((result != null) && (result.Count > 0))
            {
                FieldList currentResult = context.CurrentResult as FieldList;
                context.CurrentResult = FieldList.Add(currentResult, result);
            }
            context.SetNextIndex(context.Index + 1);
        }


        /// <summary>
        /// Updates the context result with the given ItemDefinition
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        private static void UpdateContext(ItemDefinition result, CallContext context)
        {
            if (result != null)
            {
                context.CurrentResult = result;
            }
            context.SetNextIndex(context.Index + 1);
        }


        #endregion



        #endregion
    }
}
