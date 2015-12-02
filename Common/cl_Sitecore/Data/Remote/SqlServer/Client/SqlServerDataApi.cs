using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server;
using ServerLogic.WCF;
using Sitecore.Data.DataProviders.Sql;
using Sitecore.Text;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client
{
	public class SqlServerDataApi : Sitecore.Data.SqlServer.SqlServerDataApi
	{
		private ResilientChannelFactory<ISqlServerDataApiService> oFactory;

		public SqlServerDataApi(string sConnectionString)
		{
			//create a factory to generate a channel to the service
			oFactory = new ResilientChannelFactory<ISqlServerDataApiService>("ISqlServerDataApiService", delegate(ISqlServerDataApiService oService) { oService.Initialize(sConnectionString); });

			//Sitecore.Diagnostics.Log.Info("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client.SqlServerDataApi", this);
		}

		protected override System.Data.IDbConnection CreateConnection()
		{
			//Sitecore.Diagnostics.Log.Info("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client.SqlServerDataApi.CreateConnection", this);

			return new DbConnection(oFactory.Service.CreateConnection());
		}

		public override Sitecore.Data.DataProviders.Sql.DataProviderCommand CreateCommand(string sql, params object[] parameters)
		{
			//Sitecore.Diagnostics.Log.Info("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client.SqlServerDataApi.CreateCommand", this);

			return new DataProviderCommand(oFactory.Service.CreateCommand(sql, parameters.Select(oParameter => GetSerializableParameter(oParameter)).ToArray()));
		}

		public override Sitecore.Data.DataProviders.Sql.DataProviderTransaction CreateTransaction()
		{
			//Sitecore.Diagnostics.Log.Info("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client.SqlServerDataApi.CreateTransaction", this);

			throw new NotSupportedException();
		}

		public override int Execute(string sql, params object[] parameters)
		{
			//Sitecore.Diagnostics.Log.Info("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client.SqlServerDataApi.Execute", this);

			return base.Execute(sql, parameters.Select(oParameter => GetSerializableParameter(oParameter)).ToArray());
		}

		private object GetSerializableParameter(object oParameter)
		{
			if (oParameter is BigString)
			{
				oParameter = ((BigString)oParameter).Value;
			}
			return oParameter;
		}
	}
}
