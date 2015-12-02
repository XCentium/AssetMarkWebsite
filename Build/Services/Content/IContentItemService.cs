using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace Genworth.SitecoreExt.Services.Content
{
    [ServiceContract]
    public interface IContentItemService
    {
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}")]
        FieldDataItem[] GetContent(string itemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent?path={path}")]
        FieldDataItem[] GetContentByPath(string path);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{parentItemId}")]
        ContentDataItem[] GetChildrenContent(string parentItemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{parentItemId}/{childrenTemplateId}")]
        ContentDataItem[] GetChildrenContentWithTemplateFilter(string parentItemId, string childrenTemplateId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent?path={parentItemPath}")]
        ContentDataItem[] GetChildrenContentByPath(string parentItemPath);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{childrenTemplateId}?path={parentItemPath}")]
        ContentDataItem[] GetChildrenContentByPathWithTemplateFilter(string parentItemPath, string childrenTemplateId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}")]
        FieldDataItem[] GetFieldContent(string itemId, string fieldName);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}/{sectionName}")]
        FieldDataItem[] GetSpecificContent(string itemId, string fieldName, string sectionName);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}/{sectionName}/{version}")]
        FieldDataItem[] GetSpecificVersionedContent(string itemId, string fieldName, string sectionName, string version);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent/{itemId}")]
        Stream GetFileContent(string itemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent/{itemId}/{version}")]
        Stream GetVersionedFileContent(string itemId, string version);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent?path={path}")]
        Stream GetFileContentByPath(string path);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetItemRecursive/{rootItemId}")]
        ContentDataItem[] GetItemRecursive(string rootItemId);
    }
}
