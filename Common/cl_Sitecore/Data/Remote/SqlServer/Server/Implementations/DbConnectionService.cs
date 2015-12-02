using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DbConnectionService : IDbConnectionService
	{
		private IDbConnection oDbConnection;
		private Guid oGuid;
        
		private IDbConnection DbConnection
		{
			get
			{
				if (oDbConnection == null)
				{
					oDbConnection = SqlServerDataApiService.GetConnection(oGuid);
					if (oDbConnection == null)
					{
						throw new NullReferenceException(string.Format("Could not get connection for Guid {0}", oGuid.ToString()));
					}
				}
				return oDbConnection;
			}
		}

		private void LogError(string sClass, string sOperation, Exception oException)
		{
			Sitecore.Diagnostics.Log.Error(
				string.Format("Exception in {0} with Guid {1} while executing: {2}", sClass, oGuid != null ? oGuid.ToString() : "null", sOperation)
				, oException, this);
		}

		public void Initialize(Guid oGuid)
		{
			this.oGuid = oGuid;
			Sitecore.Diagnostics.Log.Debug(string.Format("Initializing {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", oGuid.ToString()), this);
		}

		public System.Data.IDbTransaction BeginTransaction(System.Data.IsolationLevel il)
		{
			try
			{
				return DbConnection.BeginTransaction(il);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "BeginTransaction(System.Data.IsolationLevel il)", oException);
				throw oException;
			}
		}

		public System.Data.IDbTransaction BeginTransaction()
		{
			try
			{
				return DbConnection.BeginTransaction();
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "BeginTransaction()", oException);
				throw oException;
			}
		}

		public void ChangeDatabase(string databaseName)
		{
			try
			{
				DbConnection.ChangeDatabase(databaseName);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "ChangeDatabase(string databaseName)", oException);
				throw oException;
			}
		}

		public void Close()
		{
			try
			{
				DbConnection.Close();
                Sitecore.Diagnostics.Log.Debug(string.Format("Closed {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", oGuid.ToString()), this);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "Close()", oException);
				throw oException;
			}
		}

		public string ConnectionString
		{
			get
			{
				try
				{
					return DbConnection.ConnectionString;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "ConnectionString:get", oException);
					throw oException;
				}
			}
			set
			{
				try
				{
					DbConnection.ConnectionString = value;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "ConnectionString:set", oException);
					throw oException;
				}
			}
		}

		public int ConnectionTimeout
		{
			get
			{
				try
				{
					return DbConnection.ConnectionTimeout;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "ConnectionTimeout", oException);
					throw oException;
				}
			}
		}

		public System.Data.IDbCommand CreateCommand()
		{
			try
			{
				return DbConnection.CreateCommand();
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "CreateCommand()", oException);
				throw oException;
			}
		}

		public string Database
		{
			get
			{
				try
				{
					return DbConnection.Database;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "Database", oException);
					throw oException;
				}
			}
		}

		public void Open()
		{
			try
			{
				DbConnection.Open();
                Sitecore.Diagnostics.Log.Debug(string.Format("Opened {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", oGuid.ToString()), this);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "Open()", oException);
				throw oException;
			}
		}

		public System.Data.ConnectionState State
		{
			get
			{
				try
				{
					return DbConnection.State;
				}
				catch (Exception oException)
				{
					LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "State", oException);
					throw oException;
				}
			}
		}

		public void Dispose()
		{
			try
			{
				DbConnection.Dispose();
				SqlServerDataApiService.RemoveConnection(oGuid);
                Sitecore.Diagnostics.Log.Debug(string.Format("Disposed {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", oGuid.ToString()), this);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DbConnectionService", "Dispose()", oException);
				throw oException;
			}
		}
	}
}
