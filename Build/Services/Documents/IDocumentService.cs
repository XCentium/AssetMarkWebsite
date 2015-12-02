using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;

namespace Genworth.SitecoreExt.Services.Documents
{
	[ServiceContract]
	public interface IDocumentService
	{
		[OperationContract]
		void SubmitGreeting(Document oDocument);

		[OperationContract]
		List<Document> GetGreetings();

		[OperationContract]
		List<KeyValuePair<string, string>> GetItemById(string Ciid);

		[OperationContract]
		List<KeyValuePair<string, string>> GetItemByPath(string Path);

		[OperationContract]
		Stream GetFileById(string Ciid);

		[OperationContract]
		Stream GetFileByPath(string Path);

        [OperationContract]
        Stream GetImageBySSO(string key);

        [OperationContract]
        Stream GetImageByPath(string path);

        [OperationContract]
        string GetImageUriBySSO(string sKey, int iWidth, int iHeight);

	}
}
