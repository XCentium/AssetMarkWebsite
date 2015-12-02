using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server
{
	[ServiceContract(Namespace = "http://ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.IDataProviderCommandService", SessionMode = SessionMode.Required)]
	public interface IDataProviderCommandService
	{
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		void Initialize(Guid oGuid);

		[OperationContract(IsInitiating = false)]
		int ExecuteNonQuery();

		[OperationContract(IsInitiating = false)]
		Guid ExecuteReader();

		[OperationContract(IsInitiating = false)]
		void Dispose();
	}
}
