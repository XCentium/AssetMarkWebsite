using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server
{
	[ServiceContract(Namespace = "http://ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.ISqlServerDataApiService", SessionMode = SessionMode.Required)]
	[ServiceKnownType(typeof(Sitecore.Data.ID))]
	public interface ISqlServerDataApiService
	{
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		void Initialize(string sConnectionString);

		[OperationContract(IsInitiating = false)]
		Guid CreateConnection();

		[OperationContract(IsInitiating = false)]
		Guid CreateCommand(string sSql, object[] oParameters);

		[OperationContract(IsInitiating = false)]
		Guid CreateTransaction();

		[OperationContract(IsInitiating = false)]
		int Execute(string sSql, object[] parameters);
	}
}
