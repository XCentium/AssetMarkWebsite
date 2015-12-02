using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server
{
	[ServiceContract(Namespace = "http://ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.IBlobService")]
	public interface IBlobService
	{
		[OperationContract]
		MemoryStream GetBlobStream(Guid oGuid);
	}
}
