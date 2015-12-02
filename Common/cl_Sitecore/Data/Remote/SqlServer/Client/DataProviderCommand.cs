using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerLogic.WCF;
using ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server;
using System.Data;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Client
{
	public class DataProviderCommand : Sitecore.Data.DataProviders.Sql.DataProviderCommand
	{
		private ResilientChannelFactory<IDataProviderCommandService> oFactory;

		private class EmptyDbCommand : IDbCommand
		{
			public void Cancel()
			{
				throw new NotImplementedException();
			}

			public string CommandText
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public int CommandTimeout
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public CommandType CommandType
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public IDbConnection Connection
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public IDbDataParameter CreateParameter()
			{
				throw new NotImplementedException();
			}

			public int ExecuteNonQuery()
			{
				throw new NotImplementedException();
			}

			public IDataReader ExecuteReader(CommandBehavior behavior)
			{
				throw new NotImplementedException();
			}

			public IDataReader ExecuteReader()
			{
				throw new NotImplementedException();
			}

			public object ExecuteScalar()
			{
				throw new NotImplementedException();
			}

			public IDataParameterCollection Parameters
			{
				get { throw new NotImplementedException(); }
			}

			public void Prepare()
			{
				throw new NotImplementedException();
			}

			public IDbTransaction Transaction
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public UpdateRowSource UpdatedRowSource
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}

		private static EmptyDbCommand oEmptyDbCommand = new EmptyDbCommand();

		public DataProviderCommand(Guid oGuid)
			: base(oEmptyDbCommand, false)
		{
			//create a factory to generate a channel to the service
			oFactory = new ResilientChannelFactory<IDataProviderCommandService>("IDataProviderCommandService", delegate(IDataProviderCommandService oService) { oService.Initialize(oGuid); });
		}

		public override int ExecuteNonQuery()
		{
			return oFactory.Service.ExecuteNonQuery();
		}

		public override System.Data.IDataReader ExecuteReader()
		{
			return new DataReader(oFactory.Service.ExecuteReader());
		}

		public override void Dispose()
		{
			oFactory.Service.Dispose();
		}
	}
}
