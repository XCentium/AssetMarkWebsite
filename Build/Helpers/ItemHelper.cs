using ServerLogic.SitecoreExt;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Resources.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.Helpers
{
    public static class ItemHelper
    {
        private static readonly string dash = "-";

        public static string FormatId(string id)
        {
            string newId = string.Empty;

            if (id.Length > 0 && id.Length >= 32)
            {
                string tempId = id.Substring(0, 32);
                newId = tempId.Insert(8, dash).Insert(13, dash).Insert(18, dash).Insert(23, dash);
            }

            return newId;
        }

        public static Stream GetFileStream(Item oItem)
        {
            Stream oStream;
            MediaStream oMediaStream;

            oStream = null;

            if (oItem != null)
            {
                if ((oMediaStream = MediaManager.GetMedia(oItem).GetStream()) != null)
                {
                    oStream = oMediaStream.Stream;
                }
            }

            return oStream;
        }

        public static byte[] ToByteArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public static Stream GetFileById(string Ciid)
        {
            return GetFileById(Ciid, ContextExtension.CurrentDatabase);
        }

        public static Stream GetFileById(string Ciid, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetFileById(Ciid, database);
        }

        public static Stream GetFileById(string Ciid, Sitecore.Data.Database database)
        {
            return GetFileStream(database.GetItem(Ciid));
        }

        public static Stream GetFileByPath(string Path)
        {
            return GetFileByPath(Path, ContextExtension.CurrentDatabase);
        }

        public static Stream GetFileByPath(string Path, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetFileByPath(Path, database);
        }

        public static Stream GetFileByPath(string Path, Sitecore.Data.Database database)
        {
            return GetFileStream(database.SelectSingleItem(Path));
        }

        public static Item GetItemById(string Ciid)
        {
            return GetItemById(Ciid, ContextExtension.CurrentDatabase);
        }

        public static Item GetItemById(string Ciid, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetItemById(Ciid, database);
        }

        public static Item GetItemById(string Ciid, Sitecore.Data.Database database)
        {
            return database.GetItem(Ciid);
        }

        public static Item GetItemByIdAndVersion(string Ciid, string version)
        {
            return GetItemByIdAndVersion(Ciid, version, ContextExtension.CurrentDatabase);
        }

        public static Item GetItemByIdAndVersion(string Ciid, string version, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetItemByIdAndVersion(Ciid, version, database);
        }

        public static Item GetItemByPath(string Path)
        {
            return GetItemByPath(Path, ContextExtension.CurrentDatabase);
        }

        public static Item GetItemByPath(string Path, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetItemByPath(Path, database);
        }

        public static Item GetItemByPath(string Path, Sitecore.Data.Database database)
        {
            return database.SelectSingleItem(Path);
        }

        public static Item GetItemByIdAndVersion(string Ciid, string version, Sitecore.Data.Database database)
        {
            var itemVersion = new Sitecore.Data.Version(version);
            var language = Language.Parse(ContextExtension.CurrentLanguageCode);
            return database.GetItem(Ciid, language, itemVersion);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesById(string Ciid)
        {
            return GetItemValuesById(Ciid, ContextExtension.CurrentDatabase);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesById(string Ciid, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetItemValuesById(Ciid, database);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesById(string Ciid, Sitecore.Data.Database database)
        {
            return GetItemFieldValuesFromTemplate(database.GetItem(Ciid), database);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesByIdAndVersion(string Ciid, string version)
        {
            return GetItemValuesByIdAndVersion(Ciid, version, ContextExtension.CurrentDatabase);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesByIdAndVersion(string Ciid, string version, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetItemValuesByIdAndVersion(Ciid, version, database);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesByIdAndVersion(string Ciid, string version, Sitecore.Data.Database database)
        {
            var itemVersion = new Sitecore.Data.Version(version);
            var language = Language.Parse(ContextExtension.CurrentLanguageCode);
            return GetItemFieldValuesFromTemplate(database.GetItem(Ciid, language, itemVersion), database);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesByPath(string Path)
        {
            return GetItemValuesByPath(Path, ContextExtension.CurrentDatabase);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesByPath(string Path, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetItemValuesByPath(Path, database);
        }

        public static List<KeyValuePair<string, string>> GetItemValuesByPath(string Path, Sitecore.Data.Database database)
        {
            return GetItemFieldValuesFromTemplate(database.SelectSingleItem(Path), database);
        }

        private static List<KeyValuePair<string, string>> GetItemFieldValues(Item oItem)
        {
            List<KeyValuePair<string, string>> oDocument;

            if (oItem != null)
            {
                //create the document as a new object
                oDocument = new List<KeyValuePair<string, string>>();

                //read all of the fields
                oItem.Fields.ReadAll();

                //now we can find the field we care about
                oItem.Fields.ToList().ForEach(oField =>
                {
                    //add the field info to the document
                    oDocument.Add(new KeyValuePair<string, string>(oField.Name, oField.Value));
                });

                oDocument.AddRange(GetAdditionalItemValues(oItem));
            }
            else
            {
                //intialize the new document as null
                oDocument = null;
            }

            //return the document
            return oDocument;
        }

        private static List<KeyValuePair<string, string>> GetItemFieldValuesFromTemplate(Item oItem)
        {
            return GetItemFieldValuesFromTemplate(oItem, ContextExtension.CurrentDatabase);
        }

        private static List<KeyValuePair<string, string>> GetItemFieldValuesFromTemplate(Item oItem, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetItemFieldValuesFromTemplate(oItem, database);
        }

        private static List<KeyValuePair<string, string>> GetItemFieldValuesFromTemplate(Item oItem, Sitecore.Data.Database database)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            if (oItem != null)
            {
                var templates = Sitecore.Data.Managers.TemplateManager.GetTemplates(database);

                var templateItem = templates.Where(t => t.Value.Name == oItem.TemplateName).Select(t => t.Value).FirstOrDefault();
                if (templateItem != null)
                {
                    string templateItemId = templateItem.ID.ToGuid().ToString();
                    templateItem.GetFields(true).ToList().ForEach((f) =>
                    {
                        if (oItem.Fields[f.Name] != null)
                        {
                            list.Add(new KeyValuePair<string, string>(f.Name, oItem.Fields[f.Name].Value));
                        }
                    });
                }
                list.AddRange(GetAdditionalItemValues(oItem));
                list.Sort((c, d) => { return c.Key.CompareTo(d.Key); });
            }

            return list;
        }

        private static List<KeyValuePair<string, string>> GetAdditionalItemValues(Item oItem)
        {
            List<KeyValuePair<string, string>> oDocument = new List<KeyValuePair<string, string>>();

            if (oItem != null)
            {
                oDocument.Add(new KeyValuePair<string, string>("ItemId", oItem.ID.ToGuid().ToString()));
                oDocument.Add(new KeyValuePair<string, string>("ItemName", oItem.Name));
                oDocument.Add(new KeyValuePair<string, string>("TemplateId", oItem.TemplateID.ToGuid().ToString()));
                oDocument.Add(new KeyValuePair<string, string>("TemplateName", oItem.TemplateName));
                oDocument.Add(new KeyValuePair<string, string>("SitecorePath", oItem.Paths.FullPath));
                oDocument.Add(new KeyValuePair<string, string>("Version", oItem.Version.ToString()));

                if (MediaManager.HasMediaContent(oItem))
                {
                    Media media = MediaManager.GetMedia(oItem);
                    if (media != null)
                    {
                        oDocument.Add(new KeyValuePair<string, string>("MediaExtension", media.Extension));
                        oDocument.Add(new KeyValuePair<string, string>("MediaMimeType", media.MimeType));
                    }
                    oDocument.Add(new KeyValuePair<string, string>("HasMediaContent", true.ToString()));
                    oDocument.Add(new KeyValuePair<string, string>("MediaURL", oItem.GetMediaURL()));
                }
                else
                {
                    oDocument.Add(new KeyValuePair<string, string>("HasMediaContent", false.ToString()));
                }
            }

            //return the document
            return oDocument;
        }

        public static Item[] GetChildrenContentItems(string parentItemId, string parentItempath, string childrenTemplateId)
        {
            return GetChildrenContentItems(parentItemId, parentItempath, childrenTemplateId, ContextExtension.CurrentDatabase);
        }

        public static Item[] GetChildrenContentItems(string parentItemId, string parentItempath, string childrenTemplateId, string databaseName)
        {
            Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase(databaseName);
            return GetChildrenContentItems(parentItemId, parentItempath, childrenTemplateId, database);
        }

        public static Item[] GetChildrenContentItems(string parentItemId, string parentItempath, string childrenTemplateId, Sitecore.Data.Database database)
        {
            StringBuilder oQueryBuilder = new StringBuilder();
            
            oQueryBuilder.Append("fast:");

            if (!string.IsNullOrWhiteSpace(parentItemId))
            {
                oQueryBuilder.Append("//*[@@parentid='{").Append(parentItemId).Append("}'");
                if (!string.IsNullOrWhiteSpace(childrenTemplateId))
                {
                    oQueryBuilder.Append(" and @@templateid = '{").Append(childrenTemplateId).Append("}']");
                }
                else
                {
                    oQueryBuilder.Append("]");
                }
            }
            else if (!string.IsNullOrWhiteSpace(parentItempath))
            {
                string[] terms = parentItempath.Split('/');
                StringBuilder sbTerms = new StringBuilder();
                foreach (var term in terms)
                {
                    sbTerms.Append("/");
                    if (term.Contains(' ') || term.Contains('-'))
                    {
                        sbTerms.Append("#").Append(term).Append("#");
                    }
                    else
                    {
                        sbTerms.Append(term);
                    }
                }
                oQueryBuilder.Append(sbTerms.ToString()).Append("/*");
                if (!string.IsNullOrWhiteSpace(childrenTemplateId))
                {
                    oQueryBuilder.Append("[@@templateid = '{").Append(childrenTemplateId).Append("}']");
                }
            }
            else if (!string.IsNullOrWhiteSpace(childrenTemplateId))
            {
                oQueryBuilder.Append("//*[@@templateid = '{").Append(childrenTemplateId).Append("}']");
            }

            return RunContentItemQuery(oQueryBuilder.ToString(), database);
        }

        public static Item[] RunContentItemQuery(string query, Sitecore.Data.Database database)
        {
            return database.SelectItems(query);
        }

        public static string GetFieldValue(Item item, string sectionName, string fieldName)
        {
            var field = item.GetField(sectionName, fieldName);
            return field.GetText();
        }

        public static List<string> GetSectionsByItemId(string itemId)
        {
            List<string> list = new List<string>();
            var item = GetItemById(itemId);
            item.Fields.GroupBy(field => field.Section).ToList().ForEach(s => list.Add(s.Key));
            return list;
        }

        public static Dictionary<string, object> GetItemProperties(string itemId)
        {
            Dictionary<string, object> d = new Dictionary<string, object>();
            try
            {
                var item = GetItemById(itemId);
                if (item != null)
                {
                    d.Add("item.Appearance.ContextMenu", item.Appearance.ContextMenu);
                    d.Add("item.Appearance.DisplayName", item.Appearance.DisplayName);
                    d.Add("item.Appearance.HelpLink", item.Appearance.HelpLink);
                    d.Add("item.Appearance.Hidden", item.Appearance.Hidden);
                    d.Add("item.Appearance.Icon", item.Appearance.Icon);
                    d.Add("item.Appearance.LongDescription", item.Appearance.LongDescription);
                    d.Add("item.Appearance.ReadOnly", item.Appearance.ReadOnly);
                    d.Add("item.Appearance.Ribbon", item.Appearance.Ribbon);
                    d.Add("item.Appearance.ShortDescription", item.Appearance.ShortDescription);
                    d.Add("item.Appearance.Skin", item.Appearance.Skin);
                    d.Add("item.Appearance.Sortorder", item.Appearance.Sortorder);
                    d.Add("item.Appearance.Style", item.Appearance.Style);
                    d.Add("item.Appearance.Thumbnail", item.Appearance.Thumbnail);
                    d.Add("item.Axes.Level", item.Axes.Level);
                    d.Add("item.Axes.Root.ID", item.Axes.Root.ID.ToString());
                    d.Add("item.Database.Name", item.Database.Name);
                    d.Add("item.DisplayName", item.DisplayName);
                    d.Add("item.Empty", item.Empty);
                    d.Add("item.Fields.Count", item.Fields.Count);
                    d.Add("item.HasChildren", item.HasChildren);
                    d.Add("item.ID.", item.ID.ToString());
                    d.Add("item.IsClone", item.IsClone);
                    d.Add("item.IsItemClone", item.IsItemClone);
                    d.Add("item.Key", item.Key);
                    d.Add("item.Language.Name", item.Language.Name);
                    d.Add("item.Language.DisplayName", item.Language.GetDisplayName());
                    d.Add("item.Modified", item.Modified);
                    d.Add("item.Name", item.Name);
                    d.Add("item.ParentID", item.ParentID.ToString());
                    d.Add("item.Parent.Name", item.Parent.Name);
                    d.Add("item.Parent.DisplayName", item.Parent.DisplayName);
                    d.Add("item.Paths.ContentPath", item.Paths.ContentPath);
                    d.Add("item.Paths.FullPath", item.Paths.FullPath);
                    d.Add("item.Paths.IsContentItem", item.Paths.IsContentItem);
                    d.Add("item.Paths.IsFullyQualified", item.Paths.IsFullyQualified);
                    d.Add("item.Paths.IsMediaItem", item.Paths.IsMediaItem);
                    d.Add("item.Paths.LongID", item.Paths.LongID);
                    d.Add("item.Paths.MediaPath", item.Paths.MediaPath);
                    d.Add("item.Paths.ParentPath", item.Paths.ParentPath);
                    d.Add("item.Paths.Path", item.Paths.Path);
                    d.Add("item.Publishing.HideVersion", item.Publishing.HideVersion);
                    d.Add("item.Publishing.NeverPublish", item.Publishing.NeverPublish);
                    d.Add("item.Publishing.PublishDate", item.Publishing.PublishDate);
                    d.Add("item.Publishing.UnpublishDate", item.Publishing.UnpublishDate);
                    d.Add("item.Publishing.ValidFrom", item.Publishing.ValidFrom);
                    d.Add("item.Publishing.ValidTo", item.Publishing.ValidTo);
                    if (item.RuntimeSettings != null)
                    {
                        d.Add("item.RuntimeSettings.BrowseOnly", item.RuntimeSettings.BrowseOnly);
                        d.Add("item.RuntimeSettings.ForceModified", item.RuntimeSettings.ForceModified);
                        d.Add("item.RuntimeSettings.Invalid", item.RuntimeSettings.Invalid);
                        d.Add("item.RuntimeSettings.IsExternal", item.RuntimeSettings.IsExternal);
                        d.Add("item.RuntimeSettings.IsVirtual", item.RuntimeSettings.IsVirtual);
                        if (item.RuntimeSettings.OwnerDatabase != null)
                        {
                            d.Add("item.RuntimeSettings.OwnerDatabase.Name", item.RuntimeSettings.OwnerDatabase.Name);
                            d.Add("item.RuntimeSettings.OwnerDatabase.Protected", item.RuntimeSettings.OwnerDatabase.Protected);
                            d.Add("item.RuntimeSettings.OwnerDatabase.ProxiesEnabled", item.RuntimeSettings.OwnerDatabase.ProxiesEnabled);
                            d.Add("item.RuntimeSettings.OwnerDatabase.ReadOnly", item.RuntimeSettings.OwnerDatabase.ReadOnly);
                            d.Add("item.RuntimeSettings.OwnerDatabase.SecurityEnabled", item.RuntimeSettings.OwnerDatabase.SecurityEnabled);
                        }
                    }
                    d.Add("item.Template.ID", item.Template.ID.ToString());
                    d.Add("item.Template.Name", item.Template.Name);
                    d.Add("item.Template.DisplayName", item.Template.DisplayName);
                    d.Add("item.Template.FullName", item.Template.FullName);
                    if (item.Uri != null)
                    {
                        d.Add("item.Uri.Path", item.Uri.Path);
                        d.Add("item.Uri.Version.Number", item.Uri.Version.Number);
                    }
                    d.Add("item.Version.Number", item.Version.Number);
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to read Sitecore properties for item: " + itemId, ex, typeof(ItemHelper));
            }

            return d;
        }

        public static string GetIconUrl(Item item)
        {
            var icon = item.Template.Icon;
            var source = Sitecore.Resources.Images.GetThemedImageSource(icon, Sitecore.Web.UI.ImageDimension.id16x16);
            return source;
        }
    }
}
