using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Xml.Linq;



namespace Genworth.SitecoreExt.Services.Providers
{	
	[ServiceContract]	
	public interface IGenContentService
	{
		[OperationContract]
		ItemDefinitionContract GetItemDefinition(string sItemId, string sDatabaseName);

		[OperationContract]
		List<ItemDefinitionContract> GetItemDefinitions(List<string> oItemsIds, string sDatabaseName);

		[OperationContract]
		List<FieldContract> GetItemFields(string sItemId, string sLanguageName, int iItemVersionNumber, string sDatabaseName);

        [OperationContract]
        Dictionary<string, List<FieldContract>> GetItemsFields(List<string> oItemsIds, string sDatabaseName);

		[OperationContract]
		Dictionary<Guid, List<Guid>> GetItemsChildIDs(List<string> oItemIds, string sDatabaseName);

		[OperationContract]
		List<VersionUriContract> GetItemVersions(string sItemId, string sDatabaseName);

		[OperationContract]
		List<Guid> SelectIDs(string query, string sDatabaseName);

		[OperationContract]
		Guid SelectSingleID(string query, string sDatabaseName);

		[OperationContract]
		List<Guid> GetChildIDs(string sItemId, string sDatabaseName);

		[OperationContract]
		Guid GetParentID(string sItemId, string sDatabaseName);

		[OperationContract]
		Guid GetRootID(string sDatabaseName);

		[OperationContract]
		List<Guid> GetTemplateIDs(string sDatabaseName);

		[OperationContract]
		Guid ResolvePath(string sItemPath, string sDatabaseName);

		[OperationContract]
		List<LanguageContract> GetLanguages(string sDatabaseName);

		[OperationContract]
		System.IO.MemoryStream GetBlobStream(Guid oBlobId, string sDatabaseName);

		[OperationContract]
		bool BlobStreamExists(Guid oBlobId, string sDatabaseName);

	}
}
