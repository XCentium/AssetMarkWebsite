using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Data;
using System.ServiceModel.Activation;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server
{
	[ServiceContract(Namespace = "http://ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.IDbConnectionService", SessionMode = SessionMode.Required)]
	public interface IDbConnectionService
	{
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		void Initialize(Guid oGuid);

		[OperationContract(IsInitiating = false, Name = "BeginTransactionWithIsolationLevel")]
		IDbTransaction BeginTransaction(IsolationLevel il);

		[OperationContract(IsInitiating = false, Name = "BeginTransaction")]
		IDbTransaction BeginTransaction();

		[OperationContract(IsInitiating = false)]
		void ChangeDatabase(string databaseName);

		[OperationContract(IsInitiating = false)]
		void Close();

		string ConnectionString
		{
			[OperationContract(IsInitiating = false)]
			get;
			[OperationContract(IsInitiating = false)]
			set;
		}

		int ConnectionTimeout
		{
			[OperationContract(IsInitiating = false)]
			get;
		}

		[OperationContract(IsInitiating = false)]
		IDbCommand CreateCommand();

		string Database
		{
			[OperationContract(IsInitiating = false)]
			get;
		}

		[OperationContract(IsInitiating = false)]
		void Open();

		ConnectionState State
		{
			[OperationContract(IsInitiating = false)]
			get;
		}

		[OperationContract(IsInitiating = false)]
		void Dispose();
	}
}
