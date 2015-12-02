using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Data;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DataProviderCommandService : IDataProviderCommandService
	{
		private Sitecore.Data.DataProviders.Sql.DataProviderCommand oCommand;
		private Guid oGuid;

		private Sitecore.Data.DataProviders.Sql.DataProviderCommand Command
		{
			get
			{
				if (oCommand == null)
				{
					oCommand = SqlServerDataApiService.GetCommand(oGuid);
					if (oCommand == null)
					{
						throw new NullReferenceException(string.Format("Could not get command for Guid {0}", oGuid.ToString()));
					}
				}
				return oCommand;
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
			Sitecore.Diagnostics.Log.Debug(string.Format("Initializing {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataProviderCommandService", oGuid.ToString()),this);
		}

		public int ExecuteNonQuery()
		{
			try
			{
				return Command.ExecuteNonQuery();
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataProviderCommandService", "ExecuteNonQuery()", oException);
				throw oException;
			}
		}

		public Guid ExecuteReader()
		{
			IDataReader oDataReader;
			try
			{
				oDataReader = Command.ExecuteReader();
				return SqlServerDataApiService.PutReader(oDataReader);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataProviderCommandService", "ExecuteReader()", oException);
				throw oException;
			}
		}

		public void Dispose()
		{
			try
			{
				Command.Dispose();
				SqlServerDataApiService.RemoveCommand(oGuid);
                Sitecore.Diagnostics.Log.Debug(string.Format("Disposed {0} with Guid {1}", "ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataProviderCommandService", oGuid.ToString()), this);
			}
			catch (Exception oException)
			{
				LogError("ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations.DataProviderCommandService", "Dispose()", oException);
				throw oException;
			}
		}
	}
}
