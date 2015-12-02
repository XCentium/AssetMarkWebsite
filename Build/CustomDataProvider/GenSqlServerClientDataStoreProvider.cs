using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Configuration;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Data.SqlServer;
using Genworth.SitecoreExt.Services.Providers;
using System.ServiceModel;


namespace Genworth.SitecoreExt.CustomDataProvider
{
    class GenSqlServerClientDataStoreProvider : SqlServerClientDataStore
    {
        #region VARIABLES


        #endregion

        #region PROPERTIES


        #endregion

        public GenSqlServerClientDataStoreProvider(SqlDataApi api, string objectLifetime)
            : base(api, objectLifetime)
        { }

        public GenSqlServerClientDataStoreProvider(string connectionString, string objectLifetime)
            : base(connectionString, objectLifetime)
        { }

        protected override void CompactData()
        {
            try
            {
                using (var oGenSqlServerClientDataStoreService = new GenSqlServerClientDataStoreServiceProxy())
                {
                    oGenSqlServerClientDataStoreService.CompactDataOperation();
                }
            }
            catch (Exception oGenSqlServerClientDataStorageProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Sql Server Client Data Storage Provider Error", oGenSqlServerClientDataStorageProviderException, this);
                throw;
            }
        }

        protected override string LoadData(string sKey)
        {
            string sDataLoaded;

            using (var oGenSqlServerClientDataStoreService = new GenSqlServerClientDataStoreServiceProxy())
            {
                sDataLoaded = oGenSqlServerClientDataStoreService.LoadDataOperation(sKey);
            }

            return sDataLoaded;
        }

        protected override void RemoveData(string sKey)
        {
            try
            {
                using (var oGenSqlServerClientDataStoreService = new GenSqlServerClientDataStoreServiceProxy())
                {
                    oGenSqlServerClientDataStoreService.RemoveDataOperation(sKey);
                }
            }
            catch (Exception oGenSqlServerClientDataStorageProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Sql Server Client Data Storage Provider Error", oGenSqlServerClientDataStorageProviderException, this);
                throw;
            }
        }

        protected override void SaveData(string sKey, string sData)
        {
            try
            {
                using (var oGenSqlServerClientDataStoreService = new GenSqlServerClientDataStoreServiceProxy())
                {
                    oGenSqlServerClientDataStoreService.SaveDataOperation(sKey, sData); 
                }
            }
            catch (Exception oGenSqlServerClientDataStorageProviderException)
            {
                Sitecore.Diagnostics.Log.Error("Genworth Sql Server Client Data Storage Provider Error", oGenSqlServerClientDataStorageProviderException, this);
                throw;
            }
        }

        protected override void TouchData(string key)
        {
            this.RemoveDataFromCache(key);
        }
    }
}
