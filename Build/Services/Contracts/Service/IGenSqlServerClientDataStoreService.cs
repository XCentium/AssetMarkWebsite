using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Genworth.SitecoreExt.Services.Providers
{	
	[ServiceContract]
	public interface IGenSqlServerClientDataStoreService
	{
		[OperationContract]
		void CompactDataOperation();

		[OperationContract]
		string LoadDataOperation(string key);

		[OperationContract]
		void RemoveDataOperation(string key);

		[OperationContract]
		void SaveDataOperation(string key, string data);

	}
}
