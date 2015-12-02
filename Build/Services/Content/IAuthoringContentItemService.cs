using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace Genworth.SitecoreExt.Services.Content
{
    [ServiceContract]
    public interface IAuthoringContentItemService
    {
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTree/{rootItemId}")]
        TreeDataItem[] GetItemSubTree(string rootItemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTree/{rootItemId}/filter/{baseTemplateId}")]
        TreeDataItem[] GetFilteredItemSubTree(string rootItemId, string baseTemplateId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTree/{rootItemId}/filter/{baseTemplateId}/db/{database}")]
        TreeDataItem[] GetFilteredItemSubTreeFromDatabase(string rootItemId, string baseTemplateId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTree/{rootItemId}/db/{database}")]
        TreeDataItem[] GetItemSubTreeFromDatabase(string rootItemId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTreeSelectedNode/{rootItemId}/select/{selectedItemId}")]
        TreeDataItem[] GetItemSubTreeSelectedNode(string rootItemId, string selectedItemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTreeSelectedNode/{rootItemId}/select/{selectedItemId}/filter/{baseTemplateId}")]
        TreeDataItem[] GetFilteredItemSubTreeSelectedNode(string rootItemId, string selectedItemId, string baseTemplateId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTreeSelectedNode/{rootItemId}/select/{selectedItemId}/db/{database}")]
        TreeDataItem[] GetItemSubTreeSelectedNodeFromDatabase(string rootItemId, string selectedItemId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetTreeSelectedNode/{rootItemId}/select/{selectedItemId}/filter/{baseTemplateId}/db/{database}")]
        TreeDataItem[] GetFilteredItemSubTreeSelectedNodeFromDatabase(string rootItemId, string selectedItemId, string baseTemplateId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}")]
        FieldDataItem[] GetContent(string itemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/db/{database}")]
        FieldDataItem[] GetContentFromDatabase(string itemId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent?path={path}")]
        FieldDataItem[] GetContentByPath(string path);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/db/{database}/?path={path}")]
        FieldDataItem[] GetContentByPathFromDatabase(string path, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{parentItemId}")]
        ContentDataItem[] GetChildrenContent(string parentItemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{parentItemId}/db/{database}")]
        ContentDataItem[] GetChildrenContentFromDatabase(string parentItemId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{parentItemId}/{childrenTemplateId}")]
        ContentDataItem[] GetChildrenContentWithTemplateFilter(string parentItemId, string childrenTemplateId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{parentItemId}/{childrenTemplateId}/db/{database}")]
        ContentDataItem[] GetChildrenContentWithTemplateFilterFromDatabase(string parentItemId, string childrenTemplateId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent?path={parentItemPath}")]
        ContentDataItem[] GetChildrenContentByPath(string parentItemPath);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/db/{database}/?path={parentItemPath}")]
        ContentDataItem[] GetChildrenContentByPathFromDatabase(string parentItemPath, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{childrenTemplateId}?path={parentItemPath}")]
        ContentDataItem[] GetChildrenContentByPathWithTemplateFilter(string parentItemPath, string childrenTemplateId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetChildrenContent/{childrenTemplateId}/db/{database}/?path={parentItemPath}")]
        ContentDataItem[] GetChildrenContentByPathWithTemplateFilterFromDatabase(string parentItemPath, string childrenTemplateId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDescendants?templateIds={templateIds}&path={rootItemPath}")]
        ContentDataItem[] GetDescendantsWithTemplateIds(string rootItemPath, string templateIds);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetDescendants/db/{database}/?templateIds={templateIds}&path={rootItemPath}")]
        ContentDataItem[] GetDescendantsWithTemplateIdsFromDatabase(string rootItemPath, string templateIds, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}")]
        FieldDataItem[] GetFieldContent(string itemId, string fieldName);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}/db/{database}")]
        FieldDataItem[] GetFieldContentFromDatabase(string itemId, string fieldName, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}/{sectionName}")]
        FieldDataItem[] GetSpecificContent(string itemId, string fieldName, string sectionName);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}/{sectionName}/db/{database}")]
        FieldDataItem[] GetSpecificContentFromDatabase(string itemId, string fieldName, string sectionName, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}/{sectionName}/{version}")]
        FieldDataItem[] GetSpecificVersionedContent(string itemId, string fieldName, string sectionName, string version);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetContent/{itemId}/{fieldName}/{sectionName}/{version}/db/{database}")]
        FieldDataItem[] GetSpecificVersionedContentFromDatabase(string itemId, string fieldName, string sectionName, string version, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent/{itemId}")]
        Stream GetFileContent(string itemId);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent/{itemId}/db/{database}")]
        Stream GetFileContentFromDatabase(string itemId, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent/{itemId}/{version}")]
        Stream GetVersionedFileContent(string itemId, string version);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent/{itemId}/{version}/db/{database}")]
        Stream GetVersionedFileContentFromDatabase(string itemId, string version, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent?path={path}")]
        Stream GetFileContentByPath(string path);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetFileContent/db/{database}/?path={path}")]
        Stream GetFileContentByPathFromDatabase(string path, string database);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetUsers")]
        SitecoreUserItem[] GetAllSitecoreUsers();

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetUsers/{roleName}")]
        SitecoreUserItem[] GetUsersInRole(string roleName);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetUsers/{roleName}/{domain}")]
        SitecoreUserItem[] GetUsersInDomainRole(string roleName, string domain);
    }
}
