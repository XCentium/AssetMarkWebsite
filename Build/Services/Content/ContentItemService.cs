using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.Helpers;
using Sitecore.Data.Items;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Sitecore.Data;
using Sitecore.Globalization;
using System.IO;

namespace Genworth.SitecoreExt.Services.Content
{         
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ContentItemService : IContentItemService
    {
        private const string DEFAULT_DATABASE = "web";

        #region Service operations

        public FieldDataItem[] GetContent(string itemId)
        {
            return GetData(itemId, null, null, null, null).ToArray();
        }

        public FieldDataItem[] GetContentByPath(string path)
        {
            return GetData(null, null, null, null, path).ToArray();
        }

        public FieldDataItem[] GetFieldContent(string itemId, string fieldName)
        {
            return GetData(itemId, fieldName, null, null, null).ToArray();
        }

        public FieldDataItem[] GetSpecificContent(string itemId, string fieldName, string sectionName)
        {
            return GetData(itemId, fieldName, sectionName, null, null).ToArray();
        }

        public FieldDataItem[] GetSpecificVersionedContent(string itemId, string fieldName, string sectionName, string version)
        {
            return GetData(itemId, fieldName, sectionName, version, null).ToArray();
        }

        public Stream GetFileContent(string itemId)
        {
            return GetStreamData(itemId, null, null);
        }

        public Stream GetVersionedFileContent(string itemId, string version)
        {
            return GetStreamData(itemId, version, null);
        }

        public Stream GetFileContentByPath(string path)
        {
            return GetStreamData(null, null, path);
        }

        public ContentDataItem[] GetChildrenContent(string parentItemId)
        {
            return GetChildrenData(parentItemId, null, null).ToArray();
        }

        public ContentDataItem[] GetChildrenContentWithTemplateFilter(string parentItemId, string childrenTemplateId)
        {
            return GetChildrenData(parentItemId, childrenTemplateId, null).ToArray();
        }

        public ContentDataItem[] GetChildrenContentByPath(string parentItemPath)
        {
            return GetChildrenData(null, null, parentItemPath).ToArray();
        }

        public ContentDataItem[] GetChildrenContentByPathWithTemplateFilter(string parentItemPath, string childrenTemplateId)
        {
            return GetChildrenData(null, childrenTemplateId, parentItemPath).ToArray();
        }

        public ContentDataItem[] GetItemRecursive(string rootItemId)
        {
            return GetItemSubTreeFromDatabase(rootItemId, DEFAULT_DATABASE);
        }

        public ContentDataItem[] GetItemSubTreeFromDatabase(string rootItemId, string database)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, database);
        }

        #endregion

        #region Helper methods

        private List<FieldDataItem> GetData(string itemId, string fieldName, string sectionName, string version, string path)
        {
            List<FieldDataItem> list = new List<FieldDataItem>();
            var singleItem = new FieldDataItem()
            {
                FieldName = fieldName,
                Value = null
            };

            if (!string.IsNullOrWhiteSpace(path))
            {
                GetAllValuesByPath(path).ForEach(v => list.Add(
                        new FieldDataItem()
                        {
                            FieldName = v.Key,
                            Value = v.Value,
                        }));
            }
            else if (string.IsNullOrWhiteSpace(fieldName) && string.IsNullOrWhiteSpace(sectionName) && string.IsNullOrWhiteSpace(version))
            {
                GetAllValues(itemId).ForEach(v => list.Add(
                        new FieldDataItem()
                        {
                            FieldName = v.Key,
                            Value = v.Value
                        }));
            } 
            else if (string.IsNullOrWhiteSpace(version) && string.IsNullOrWhiteSpace(sectionName))
            {                
                var pair = GetValueForField(itemId, fieldName);
                singleItem.Value = pair.Value;
                list.Add(singleItem);
            }
            else
            {
                var item = string.IsNullOrWhiteSpace(version) ? ItemHelper.GetItemById(itemId) : ItemHelper.GetItemById(itemId, version);
                var value = ItemHelper.GetFieldValue(item, sectionName, fieldName);
                singleItem.Value = value;
                list.Add(singleItem);
            }

            return list;
        }

        private List<ContentDataItem> GetChildrenData(string parentItemId, string childrenTemplateId, string parentItemPath)
        {
            List<ContentDataItem> itemList = new List<ContentDataItem>();
            List<FieldDataItem> fieldList = null;

            var items = ItemHelper.GetChildrenContentItems(parentItemId, parentItemPath, childrenTemplateId);
            
            foreach (var item in items)
            {
                var dataItem = new ContentDataItem()
                {
                    ItemId = item.ID.ToGuid().ToString(),
                    ItemName = item.Name,
                    TemplateId = item.TemplateID.ToGuid().ToString(),
                    TemplateName = item.TemplateName,
                    Version = item.Version.ToString(),
                    Path = item.Paths.FullPath
                };

                fieldList = GetData(dataItem.ItemId, null, null, null, null);
                dataItem.Fields = fieldList.ToArray();
                itemList.Add(dataItem);
            }
            return itemList;
        }

        private List<KeyValuePair<string, string>> GetAllValues(string id)
        {
            return ItemHelper.GetItemValuesById(id);
        }

        private List<KeyValuePair<string, string>> GetAllValuesByPath(string path)
        {
            return ItemHelper.GetItemValuesByPath(path);
        }

        private KeyValuePair<string, string> GetValueForField(string id, string fieldName)
        {
            return GetAllValues(id).Where(v => v.Key.ToLower() == fieldName.ToLower()).FirstOrDefault();
        }

        private Stream GetStreamData(string itemId, string version, string path)
        {
            Item item = null;

            if (!string.IsNullOrWhiteSpace(path))
            {
                item = ContextExtension.CurrentDatabase.SelectSingleItem(path);
            }
            else
            {
                item = string.IsNullOrWhiteSpace(version) ? ItemHelper.GetItemById(itemId) : ItemHelper.GetItemById(itemId, version);
            }

            if (item != null)
            {
                Sitecore.Resources.Media.Media media = Sitecore.Resources.Media.MediaManager.GetMedia(item);
                if (media != null)
                {
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.ContentType = media.MimeType;
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", string.Format("attachment; filename={0}", item.Name));
                    System.ServiceModel.Web.WebOperationContext.Current.OutgoingResponse.Headers.Add("X-Filename", item.Name);
                    Sitecore.Resources.Media.MediaStream mediaStream = media.GetStream();
                    return mediaStream.Stream;
                }
            }
            return null;
        }

        private ContentDataItem[] GetItemSubTreeSelectedNodeFromDB(string itemId, string dbName)
        {
            ContentDataItem tree = null;
            if (string.IsNullOrWhiteSpace(itemId))
                return null;

            if (!string.IsNullOrWhiteSpace(dbName))
            {
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(dbName);

                    var rootItem = database.GetItem(new Sitecore.Data.ID(itemId));
                    tree = GetChildren(rootItem);
                }
            }

            return new ContentDataItem[] { tree };
        }

        private ContentDataItem GetChildren(Item item)
        {
            if (!item.HasChildren)
            {
                return BuildTreeNode(item, false);
            }
            else
            {
                ContentDataItem root = BuildTreeNode(item, true);
                foreach (Item child in item.Children)
                {
                    root.Children.Add(GetChildren(child));
                }
                return root;
            }
        }

        private ContentDataItem BuildTreeNode(Item item, bool initializeChildrenCollection)
        {
            var dataItem = new ContentDataItem()
            {
                ItemId = item.ID.ToGuid().ToString(),
                ItemName = item.Name,
                TemplateId = item.TemplateID.ToGuid().ToString(),
                TemplateName = item.TemplateName,
                Version = item.Version.ToString(),
                Path = item.Paths.FullPath
            };

            List<FieldDataItem> fieldList = GetData(dataItem.ItemId, null, null, null, null, item.Database.Name);
            dataItem.Fields = fieldList.ToArray();

            if (initializeChildrenCollection)
            {
                dataItem.Children = new List<ContentDataItem>();
            }

            return dataItem;
        }

        private List<FieldDataItem> GetData(string itemId, string fieldName, string sectionName, string version, string path, string databaseName)
        {
            List<FieldDataItem> list = new List<FieldDataItem>();
            var singleItem = new FieldDataItem()
            {
                FieldName = fieldName,
                Value = null
            };
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    GetAllValuesByPath(path, databaseName).ForEach(v => list.Add(
                            new FieldDataItem()
                            {
                                FieldName = v.Key,
                                Value = v.Value,
                            }));
                }
                else if (string.IsNullOrWhiteSpace(fieldName) && string.IsNullOrWhiteSpace(sectionName) && string.IsNullOrWhiteSpace(version))
                {
                    GetAllValues(itemId, databaseName).ForEach(v => list.Add(
                            new FieldDataItem()
                            {
                                FieldName = v.Key,
                                Value = v.Value
                            }));
                }
                else if (string.IsNullOrWhiteSpace(version) && string.IsNullOrWhiteSpace(sectionName))
                {
                    var pair = GetValueForField(itemId, fieldName, databaseName);
                    singleItem.Value = pair.Value;
                    list.Add(singleItem);
                }
                else
                {
                    var item = string.IsNullOrWhiteSpace(version) ? ItemHelper.GetItemById(itemId, databaseName) : ItemHelper.GetItemByIdAndVersion(itemId, version, databaseName);
                    var value = ItemHelper.GetFieldValue(item, sectionName, fieldName);
                    singleItem.Value = value;
                    list.Add(singleItem);
                }
            }

            return list;
        }

        private List<KeyValuePair<string, string>> GetAllValues(string id, string databaseName)
        {
            return ItemHelper.GetItemValuesById(id, databaseName);
        }

        private List<KeyValuePair<string, string>> GetAllValuesByPath(string path, string databaseName)
        {
            return ItemHelper.GetItemValuesByPath(path, databaseName);
        }

        private KeyValuePair<string, string> GetValueForField(string id, string fieldName, string databaseName)
        {
            return GetAllValues(id, databaseName).Where(v => v.Key.ToLower() == fieldName.ToLower()).FirstOrDefault();
        }

        #endregion
    }
}
