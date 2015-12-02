using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ServerLogic.WCF;
using ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client
{
	public class DbConnection : IDbConnection
	{
		private ResilientChannelFactory<IDbConnectionService> oFactory;

		public DbConnection(Guid oGuid)
		{
			//create a factory to generate a channel to the service
			oFactory = new ResilientChannelFactory<IDbConnectionService>("IDbConnectionService", delegate(IDbConnectionService oService) { oService.Initialize(oGuid); });
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			throw new NotSupportedException();
		}

		public IDbTransaction BeginTransaction()
		{
			throw new NotSupportedException();
		}

		public void ChangeDatabase(string databaseName)
		{
			oFactory.Service.ChangeDatabase(databaseName);
		}

		public void Close()
		{
			oFactory.Service.Close();
		}

		public string ConnectionString
		{
			get
			{
				return oFactory.Service.ConnectionString;
			}
			set
			{
				oFactory.Service.ConnectionString = value;
			}
		}

		public int ConnectionTimeout
		{
			get { return oFactory.Service.ConnectionTimeout; }
		}

		public IDbCommand CreateCommand()
		{
			throw new NotSupportedException();
		}

		public string Database
		{
			get { return oFactory.Service.Database; }
		}

		public void Open()
		{
			oFactory.Service.Open();
		}

		public ConnectionState State
		{
			get { return oFactory.Service.State; }
		}

		public void Dispose()
		{
			oFactory.Service.Dispose();
		}
	}
}
