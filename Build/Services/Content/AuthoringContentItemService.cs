using Genworth.SitecoreExt.Helpers;
using ServerLogic.SitecoreExt;
using Sitecore.Data.Items;
using Sitecore.Security.Accounts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.Services.Content
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AuthoringContentItemService : IAuthoringContentItemService
    {
        private const string DEFAULT_DATABASE = "master";

        #region Service operations

        public TreeDataItem[] GetItemSubTree(string rootItemId)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, DEFAULT_DATABASE, null, null);
        }

        public TreeDataItem[] GetFilteredItemSubTree(string rootItemId, string baseTemplateId)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, DEFAULT_DATABASE, null, baseTemplateId);
        }

        public TreeDataItem[] GetFilteredItemSubTreeFromDatabase(string rootItemId, string baseTemplateId, string database)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, database, null, baseTemplateId);
        }

        public TreeDataItem[] GetItemSubTreeSelectedNode(string rootItemId, string selectedItemId)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, DEFAULT_DATABASE, selectedItemId, null);
        }

        public TreeDataItem[] GetFilteredItemSubTreeSelectedNode(string rootItemId, string selectedItemId, string baseTemplateId)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, DEFAULT_DATABASE, selectedItemId, baseTemplateId);
        }

        public TreeDataItem[] GetFilteredItemSubTreeSelectedNodeFromDatabase(string rootItemId, string selectedItemId, string baseTemplateId, string database)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, database, selectedItemId, baseTemplateId);
        }

        public TreeDataItem[] GetItemSubTreeFromDatabase(string rootItemId, string database)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, database, null, null);
        }

        public TreeDataItem[] GetItemSubTreeSelectedNodeFromDatabase(string rootItemId, string selectedItemId, string database)
        {
            return GetItemSubTreeSelectedNodeFromDB(rootItemId, database, selectedItemId, null);
        }

        public FieldDataItem[] GetContent(string itemId)
        {
            return GetData(itemId, null, null, null, null, DEFAULT_DATABASE).ToArray();
        }

        public FieldDataItem[] GetContentFromDatabase(string itemId, string database)
        {
            return GetData(itemId, null, null, null, null, database).ToArray();
        }

        public FieldDataItem[] GetContentByPath(string path)
        {
            return GetData(null, null, null, null, path, DEFAULT_DATABASE).ToArray();
        }

        public FieldDataItem[] GetContentByPathFromDatabase(string path, string database)
        {
            return GetData(null, null, null, null, path, database).ToArray();
        }

        public FieldDataItem[] GetFieldContent(string itemId, string fieldName)
        {
            return GetData(itemId, fieldName, null, null, null, DEFAULT_DATABASE).ToArray();
        }

        public FieldDataItem[] GetFieldContentFromDatabase(string itemId, string fieldName, string database)
        {
            return GetData(itemId, fieldName, null, null, null, database).ToArray();
        }

        public FieldDataItem[] GetSpecificContent(string itemId, string fieldName, string sectionName)
        {
            return GetData(itemId, fieldName, sectionName, null, null, DEFAULT_DATABASE).ToArray();
        }

        public FieldDataItem[] GetSpecificContentFromDatabase(string itemId, string fieldName, string sectionName, string database)
        {
            return GetData(itemId, fieldName, sectionName, null, null, database).ToArray();
        }

        public FieldDataItem[] GetSpecificVersionedContent(string itemId, string fieldName, string sectionName, string version)
        {
            return GetData(itemId, fieldName, sectionName, version, null, DEFAULT_DATABASE).ToArray();
        }

        public FieldDataItem[] GetSpecificVersionedContentFromDatabase(string itemId, string fieldName, string sectionName, string version, string database)
        {
            return GetData(itemId, fieldName, sectionName, version, null, database).ToArray();
        }

        public Stream GetFileContent(string itemId)
        {
            return GetStreamData(itemId, null, null, DEFAULT_DATABASE);
        }

        public Stream GetFileContentFromDatabase(string itemId, string database)
        {
            return GetStreamData(itemId, null, null, database);
        }

        public Stream GetVersionedFileContent(string itemId, string version)
        {
            return GetStreamData(itemId, version, null, DEFAULT_DATABASE);
        }

        public Stream GetVersionedFileContentFromDatabase(string itemId, string version, string database)
        {
            return GetStreamData(itemId, version, null, database);
        }

        public Stream GetFileContentByPath(string path)
        {
            return GetStreamData(null, null, path, DEFAULT_DATABASE);
        }

        public Stream GetFileContentByPathFromDatabase(string path, string database)
        {
            return GetStreamData(null, null, path, database);
        }

        public ContentDataItem[] GetChildrenContent(string parentItemId)
        {
            return GetChildrenData(parentItemId, null, null, DEFAULT_DATABASE).ToArray();
        }

        public ContentDataItem[] GetChildrenContentFromDatabase(string parentItemId, string database)
        {
            return GetChildrenData(parentItemId, null, null, database).ToArray();
        }

        public ContentDataItem[] GetChildrenContentWithTemplateFilter(string parentItemId, string childrenTemplateId)
        {
            return GetChildrenData(parentItemId, childrenTemplateId, null, DEFAULT_DATABASE).ToArray();
        }

        public ContentDataItem[] GetChildrenContentWithTemplateFilterFromDatabase(string parentItemId, string childrenTemplateId, string database)
        {
            return GetChildrenData(parentItemId, childrenTemplateId, null, database).ToArray();
        }

        public ContentDataItem[] GetChildrenContentByPath(string parentItemPath)
        {
            return GetChildrenData(null, null, parentItemPath, DEFAULT_DATABASE).ToArray();
        }

        public ContentDataItem[] GetChildrenContentByPathFromDatabase(string parentItemPath, string database)
        {
            return GetChildrenData(null, null, parentItemPath, database).ToArray();
        }

        public ContentDataItem[] GetChildrenContentByPathWithTemplateFilter(string parentItemPath, string childrenTemplateId)
        {
            return GetChildrenData(null, childrenTemplateId, parentItemPath, DEFAULT_DATABASE).ToArray();
        }

        public ContentDataItem[] GetChildrenContentByPathWithTemplateFilterFromDatabase(string parentItemPath, string childrenTemplateId, string database)
        {
            return GetChildrenData(null, childrenTemplateId, parentItemPath, database).ToArray();
        }

        public ContentDataItem[] GetDescendantsWithTemplateIds(string rootItemPath, string templateIds)
        {
            return GetDescendantsData(rootItemPath, templateIds, DEFAULT_DATABASE).ToArray();
        }

        public ContentDataItem[] GetDescendantsWithTemplateIdsFromDatabase(string rootItemPath, string templateIds, string database)
        {
            return GetDescendantsData(rootItemPath, templateIds, database).ToArray();
        }

        public SitecoreUserItem[] GetAllSitecoreUsers()
        {
            return GetSitecoreUsers(null, null).ToArray();
        }

        public SitecoreUserItem[] GetUsersInRole(string roleName)
        {
            return GetSitecoreUsers(roleName, null).ToArray();
        }

        public SitecoreUserItem[] GetUsersInDomainRole(string roleName, string domain)
        {
            return GetSitecoreUsers(roleName, domain).ToArray();
        }

        #endregion

        #region Helper methods

        public SitecoreUserItem[] GetSitecoreUsers(string roleName, string domain)
        {
            List<SitecoreUserItem> userNames = new List<SitecoreUserItem>();
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    var users = Sitecore.Security.Accounts.UserManager.GetUsers();
                    userNames = users.Select(u => BuildSitecoreUserDataItem(u)).ToList();
                }
                else
                {
                    string fullRoleName = !string.IsNullOrWhiteSpace(domain) ? string.Format("{0}\\{1}", domain.Trim(), roleName.Trim()) : roleName.Trim();
                    var usersInRole = Sitecore.Security.Accounts.RolesInRolesManager.GetUsersInRole(Role.FromName(fullRoleName), true);
                    userNames = usersInRole.Select(u => BuildSitecoreUserDataItem(u)).ToList();
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while getting Sitecore users.", ex, typeof(AuthoringContentItemService));
            }
            return userNames.ToArray();
        }

        private SitecoreUserItem BuildSitecoreUserDataItem(Sitecore.Security.Accounts.User user)
        {
            var roleList = new List<string>();
            user.Roles.ToList().ForEach(r => roleList.Add(r.DisplayName));
            return new SitecoreUserItem()
            {
                UserName = user.LocalName,
                FullUserName = user.Name,
                Domain = user.Domain.Name,
                FullName = user.Profile.FullName,
                Email = user.Profile.Email,
                Roles = string.Join(",", roleList.ToArray()),
                Comment = user.Profile.Comment
            };
        }

        private TreeDataItem[] GetItemSubTreeSelectedNodeFromDB(string itemId, string dbName, string selectedItemId, string filteredBaseTemplateId)
        {
            TreeDataItem tree = null;
            if (string.IsNullOrWhiteSpace(itemId))
                return null;

            if (string.IsNullOrWhiteSpace(dbName))
            {
                dbName = DEFAULT_DATABASE;
            }

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(dbName);

                var rootItem = database.GetItem(new Sitecore.Data.ID(itemId));
                tree = GetChildren(rootItem, selectedItemId, filteredBaseTemplateId);
            }
            return new TreeDataItem[] { tree };
        }

        private TreeDataItem GetChildren(Item item, string selectedItemId, string filteredBaseTemplateId)
        {
            if (!item.HasChildren)
            {
                return BuildTreeNode(item, selectedItemId, filteredBaseTemplateId, false);
            }
            else
            {
                TreeDataItem root = BuildTreeNode(item, selectedItemId, filteredBaseTemplateId, true);
                foreach (Item child in item.Children)
                {
                    root.Children.Add(GetChildren(child, selectedItemId, filteredBaseTemplateId));
                }
                return root;
            }
        }

        private TreeDataItem BuildTreeNode(Item item, string selectedItemId, string filteredBaseTemplateId, bool initializeChildrenCollection)
        {
            var node = new TreeDataItem()
            {
                Id = item.ID.Guid.ToString(),
                DisplayName = item.DisplayName,
                IconUrl = ItemHelper.GetIconUrl(item),
                State = GetNodeState(item, selectedItemId, filteredBaseTemplateId),
                Path = item.Paths.FullPath
            };

            if (initializeChildrenCollection)
            {
                node.Children = new List<TreeDataItem>();
            }

            return node;
        }

        private State GetNodeState(Item item, string selectedItemId, string filteredBaseTemplateId)
        {
            var isDisabled = IsDisabledItem(item, filteredBaseTemplateId);
            var isSelected = IsSelectedItem(item, selectedItemId);
            return new State(isDisabled, isSelected);
        }

        private bool IsDisabledItem(Item item, string filteredBaseTemplateId)
        {
            if (!string.IsNullOrWhiteSpace(filteredBaseTemplateId))
            {
                var baseTemplateIds = item.Template.BaseTemplates.Select(t => t.ID.Guid.ToString().ToLower()).ToList();
                baseTemplateIds.Add(item.TemplateID.Guid.ToString().ToLower());
                if (baseTemplateIds.Contains(filteredBaseTemplateId.ToLower()))
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        private bool IsSelectedItem(Item item, string selectedItemId)
        {
            if (item.ID.Guid.ToString() == selectedItemId)
            {
                return true;
            }
            return false;
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

        private List<ContentDataItem> GetChildrenData(string parentItemId, string childrenTemplateId, string parentItemPath, string database)
        {
            List<ContentDataItem> itemList = new List<ContentDataItem>();
            List<FieldDataItem> fieldList = null;

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                var items = ItemHelper.GetChildrenContentItems(parentItemId, parentItemPath, childrenTemplateId, database);

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

                    fieldList = GetData(dataItem.ItemId, null, null, null, null, database);
                    dataItem.Fields = fieldList.ToArray();
                    itemList.Add(dataItem);
                }
            }
            return itemList;
        }

        private List<ContentDataItem> GetDescendantsData(string rootItemPath, string templateIds, string database)
        {
            List<ContentDataItem> itemList = new List<ContentDataItem>();

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                string[] templates = templateIds.Split(',');
                foreach (string template in templates)
                {
                    var childrenList = GetChildrenData(null, template, rootItemPath, database);
                    itemList.AddRange(childrenList);
                }
            }

            return itemList;
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

        private Stream GetStreamData(string itemId, string version, string path, string databaseName)
        {
            Item item = null;

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    item = ItemHelper.GetItemByPath(path, databaseName);
                }
                else
                {
                    item = string.IsNullOrWhiteSpace(version) ? ItemHelper.GetItemById(itemId, databaseName) : ItemHelper.GetItemByIdAndVersion(itemId, version, databaseName);
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
            }
            return null;
        }

        #endregion




    }
}
