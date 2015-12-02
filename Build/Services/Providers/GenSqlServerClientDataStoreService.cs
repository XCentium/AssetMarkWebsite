using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Sitecore.Data.SqlServer;
using Sitecore.Data.DataProviders.Sql;
using System.Configuration;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Providers
{

	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
	public class GenSqlServerClientDataStoreService : SqlServerClientDataStore, IGenSqlServerClientDataStoreService
	{

		#region VARIABLES

		#region CONSTANTS

		/// <summary>
		/// Web.config setting that specifies the database that should be used to retrieve the roles information
		/// </summary>
		private static string sDatabaseNameSettingName = "GenSqlServerClientDataStoreServiceDatabaseName";

		/// <summary>
		/// Default database to be used in case that no database is defined in Web.config settings
		/// </summary>
		private static string sDefaultDatabaseName = "web";	

		
		#endregion
		
		
		#endregion

		#region PROPERTIES

		/// <summary>
		/// Connection string that determines which database is hit
		/// </summary>
		private static string ConnectionString
		{
			get
			{
				if (ConfigurationManager.ConnectionStrings[DatabaseName] != null)
				{
					return ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
				}				

				return string.Empty;
			}
		}


		/// <summary>
		/// Database used to get the roles information
		/// </summary>
		private static string DatabaseName
		{
			get {
				return Sitecore.Configuration.Settings.GetSetting(sDatabaseNameSettingName, sDefaultDatabaseName);
				}
		}
		
		
		#endregion

		public GenSqlServerClientDataStoreService() : base(new SqlServerDataApi(), new TimeSpan(0,10,0).ToString())
		{
			this._api = new SqlServerDataApi(ConnectionString);	
		}
		

		public void CompactDataOperation()
		{		    
			this.CompactData();
		}

		public string LoadDataOperation(string sKey)
		{
			#region VARIABLE

			string sDataOperationResult;

			#endregion

			sDataOperationResult = this.LoadData(sKey);

			return sDataOperationResult;
			
		}

		public void RemoveDataOperation(string sKey)
		{			
			this.RemoveDataOperation(sKey);
		}

		public void SaveDataOperation(string sKey, string sData)
		{
			this.SaveDataOperation(sKey, sData);
		}
	}
}
