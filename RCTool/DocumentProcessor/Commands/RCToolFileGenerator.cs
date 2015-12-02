using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
//using Genworth.SitecoreExt;
using System.Net;
using Sitecore.Resources.Media;
using Sitecore.SecurityModel;
using System.IO;
using DocumentEntities;
using DocumentProcessor.Helpers;
//using Sitecore.Data.Items;
using Sitecore.Tasks;
using ServerLogic.SitecoreExt;
using System;
using System.Web;

namespace DocumentProcessor.Commands
{
    public class RCToolFileGenerator
    {
        private string libraryPath = "/sitecore/content/Regional Consultant Tools/Material Library";
        private string viewsPath = "/sitecore/content/Regional Consultant Tools/Views";

        private string checkFilePath;
        private string contentFilePath;
        private string serverRelativePath;
        private string serverPhysicalPath;

        public void Execute(Item[] itemArray, CommandItem commandItem, ScheduleItem scheduleItem)
        {
            // first check if it is due to run
            if (!IsDue(scheduleItem))
                return;

            LoadConfigurationSettings();
            ProcessMaterialLibraryItems();
        }

        /// <summary>
        /// Determines whether the specified schedule item is due to run.
        /// </summary>
        /// <remarks>
        /// The scheduled item will only run between defined hours (usually at night) to ensure that the
        /// task is run once a day
        /// Make sure you configure the task to run at least double so often than the time span.
        /// </remarks>
        private bool IsDue(ScheduleItem scheduleItem)
        {
            DateTime startTime;
            DateTime endTime;

            string timeRange = Sitecore.Configuration.Settings.GetSetting("Genworth.RcTools.ExportTimeRange");

            // check for setting in config file
            if (!string.IsNullOrWhiteSpace(timeRange))
            {
                // time range setting should be in the following format 00:00:00|00:00:00, the pipe "|" splits start time and end time, respectively
                string[] time = timeRange.Split('|');

                if (time.Length == 2 && DateTime.TryParse(time[0], out startTime) &&
                    DateTime.TryParse(time[1], out endTime))
                {
                    return (CheckTime(DateTime.Now, startTime, endTime) && !CheckTime(scheduleItem.LastRun, startTime, endTime));
                }
                else
                {
                    Sitecore.Diagnostics.Log.Info("RCToolFileGenerator's Genworth.RcTools.ExportTimeRange setting must be in the following format 00:00:00|00:00:00.", this);
                }
            }
            else
            {
                Sitecore.Diagnostics.Log.Info("RCToolFileGenerator's Genworth.RcTools.ExportTimeRange setting is not set in config file.", this);
            }

            return false;
        }

        private bool CheckTime(DateTime time, DateTime after, DateTime before)
        {
            return ((time >= after) && (time <= before));
        }

        private void LoadConfigurationSettings()
        {
            checkFilePath = Sitecore.Configuration.Settings.GetSetting("Genworth.RcTools.CheckFilePath"); 
            contentFilePath = Sitecore.Configuration.Settings.GetSetting("Genworth.RcTools.ContentFilePath");
            
            serverRelativePath = Sitecore.Configuration.Settings.GetSetting("Genworth.RcTools.ServerRelativePath");
            serverPhysicalPath = Sitecore.Configuration.Settings.GetSetting("Genworth.RcTools.ServerPhysicalPath");

            if (string.IsNullOrWhiteSpace(checkFilePath))
            {
                Sitecore.Diagnostics.Log.Info("RCToolFileGenerator's Genworth.RcTools.CheckFilePath setting is not set in config file.", this);
            }

            if (string.IsNullOrWhiteSpace(contentFilePath))
            {
                Sitecore.Diagnostics.Log.Info("RCToolFileGenerator's Genworth.RcTools.ContentFilePath setting is not set in config file.", this);
            }

            if (string.IsNullOrWhiteSpace(serverRelativePath))
            {
                Sitecore.Diagnostics.Log.Info("RCToolFileGenerator's Genworth.RcTools.ServerRelativePath setting is not set in config file.", this);
            }

            if (string.IsNullOrWhiteSpace(serverPhysicalPath))
            {
                Sitecore.Diagnostics.Log.Info("RCToolFileGenerator's Genworth.RcTools.ServerPhysicalPath setting is not set in config file.", this);
            }
        }

        private void ProcessMaterialLibraryItems()
        {
            try
            {
                Sitecore.Diagnostics.Log.Info("RC Tools data/files export started.", this);

                Sitecore.Data.Database database = Sitecore.Data.Database.GetDatabase("web");
                Item libraryItem = database.GetItem(libraryPath);
                Item viewsItem = database.GetItem(viewsPath);

                Category baseCategory = new Category();
                baseCategory.SubCategories = new List<Category>();
                var items = libraryItem.Axes.GetDescendants().Where(item => item.InstanceOfTemplate("Regional Consultant Asset"));
                BuildCategoryAssetStructure(items, baseCategory);

                DocumentEntities.View baseView = new DocumentEntities.View();
                baseView.SubViews = new List<DocumentEntities.View>();
                var viewItems = viewsItem.Axes.GetDescendants().Where(item => item.InstanceOfTemplate("Regional Consultant View"));
                BuildViewStructure(viewItems, baseView);

                RcToolsData data = new DocumentEntities.ContentData()
                {
                    Category = baseCategory,
                    View = baseView
                };

                WriteDataToFileSystem("rcToolContent.xml", contentFilePath, data);
                WriteCheckDataToFileSystem("rcToolCheck.xml", checkFilePath, data);

                string xml = DocumentProcessor.Helpers.RcToolsHelper.ConvertToStringWithFormat<RcToolsData>(data, "xml");
                Sitecore.Diagnostics.Log.Info("RC Tools data/files export finished.", this);                
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("RCToolFileGenerator.ProcessMaterialLibraryItems: error message: " + ex.Message, ex, this);
            }
        }

        private void BuildCategoryAssetStructure(IEnumerable<Item> items, Category baseCategory)
        {
            foreach (var item in items)
            {
                var parents = item.Axes.GetAncestors().Where(pItem => pItem.InstanceOfTemplate("Regional Consultant Category"));
                Category categoryReference = baseCategory;
                Category category = null;
                List<string> categoryNames = new List<string>();
                foreach (var parent in parents)
                {
                    category = BuildCategory(parent);
                    if (!categoryReference.SubCategories.Any(c => c.Id == category.Id))
                    {
                        categoryReference.SubCategories.Add(category);
                        category.SubCategories = new List<Category>();
                        categoryReference = category;
                    }
                    else
                    {
                        Category existingCategory = categoryReference.SubCategories.FirstOrDefault(c => c.Id == category.Id);
                        categoryReference = existingCategory;
                    }
                    categoryNames.Add(parent.Name);
                }
                if (categoryReference.Assets == null)
                    categoryReference.Assets = new List<Asset>();

                Asset asset = BuildAsset(item, categoryNames);
                categoryReference.Assets.Add(asset);
            }
        }

        private void BuildViewStructure(IEnumerable<Item> items, DocumentEntities.View baseView)
        {
            DocumentEntities.View view = null;
            foreach (var item in items)
            {
                view = BuildView(item);
                baseView.SubViews.Add(view);
            }
        }

        private Asset BuildAsset(Item item, List<string> categoryNames)
        {
            Asset asset = new Asset();

            CheckboxField canDistributeField = item.GetField("Security", "Can Distribute");
            CheckboxField clientApprovedField = item.GetField("Security", "Client Approved");
            LookupField defaultOrientationField = item.GetField("External Appearance", "Default Orientation");
            CheckboxField isRotateableField = item.GetField("External Appearance", "Is Rotateable");
            FileField fileField = item.GetField("Document", "File");
            MediaItem mediaItem = fileField.MediaItem;

            asset.CanDistribute = canDistributeField != null ? canDistributeField.Checked : false;
            asset.ClientApproved = clientApprovedField != null ? clientApprovedField.Checked : false;

            asset.AvailableOrientation = new AvailableOrientation();
            if (isRotateableField.Checked)
            {
                asset.AvailableOrientation.Landscape = true;
                asset.AvailableOrientation.Portrait = true;
            }
            else if (defaultOrientationField != null && defaultOrientationField.InnerField != null)
            {
                asset.AvailableOrientation.Landscape = defaultOrientationField.InnerField.Value == "Landscape";
                asset.AvailableOrientation.Portrait = defaultOrientationField.InnerField.Value == "Portrait";
            }

            string assetDescription = item.GetText("Asset", "Description");
            asset.Description = string.IsNullOrWhiteSpace(assetDescription) ? null : assetDescription;
            asset.Files = new List<RCFile>();

            RCFile file = BuildRCToolFile(mediaItem, AssetType.Document, item.Paths.ContentPath);
            if (file != null)
            {
                asset.Files.Add(file);
            }

            asset.Id = item.ID.Guid.ToString();
            asset.Title = item.GetText("Asset", "Title");

            string assetURL = item.GetImageURL("Document", "File");
            if (string.IsNullOrWhiteSpace(assetURL))
            {
                asset.URL = item.GetText("Document", "URL");
            }

            return asset;
        }

        private Category BuildCategory(Item item)
        {
            Category category = new Category();
            category.Id = item.ID.Guid.ToString();
            category.Title = item.GetText("Category", "Title");

            string description = item.GetText("Category", "Description");
            category.Description = string.IsNullOrWhiteSpace(description) ? null : description;

            return category;
        }

        private RCFile BuildRCToolFile(MediaItem mediaItem, AssetType type, string itemPath)
        {
            if (mediaItem != null)
            {
                string path = serverRelativePath;
                string location = string.Format("{0}/{1}.{2}", path, mediaItem.ID.Guid.ToString(), mediaItem.Extension);
                string filename = mediaItem.ID.Guid.ToString() + "." + mediaItem.Extension;

                RCFile file = new RCFile()
                {
                    Size = mediaItem.Size.ToString(),
                    Checksum = CreateChecksum(mediaItem),
                    Location = HttpUtility.UrlPathEncode(location),
                };

                switch (type)
                {
                    case AssetType.ViewResource:
                    string customPath = itemPath.Substring(0, itemPath.LastIndexOf("/") + 1);
                    string mediaItemName = mediaItem.Title;

                    if (string.IsNullOrWhiteSpace(mediaItemName))
                    {
                        file.Location = HttpUtility.UrlPathEncode(string.Format("{0}{1}{2}.{3}", path, customPath, mediaItem.Name, mediaItem.Extension));
                        filename = string.Format("{0}{1}.{2}", customPath, mediaItem.Name, mediaItem.Extension);
                    }
                    else
                    {
                        file.Location = HttpUtility.UrlPathEncode(string.Format("{0}{1}{2}", path, customPath, mediaItemName));
                        filename = string.Format("{0}{1}", customPath, mediaItemName);
                    }
                    file.Path = filename.Substring(0, filename.LastIndexOf("/"));
                    break;
                }

                WritefileToFileSystem(path, mediaItem, filename);

                return file;
            }
            return null;
        }

        private DocumentEntities.View BuildView(Item item)
        {
            DocumentEntities.View view = new DocumentEntities.View();
            view.Id = item.ID.Guid.ToString();

            LookupField defaultOrientationField = item.GetField("External Appearance", "Default Orientation");
            CheckboxField isRotateableField = item.GetField("External Appearance", "Is Rotateable");

            view.AvailableOrientation = new AvailableOrientation();
            if (isRotateableField.Checked)
            {
                view.AvailableOrientation.Landscape = true;
                view.AvailableOrientation.Portrait = true;
            }
            else if (defaultOrientationField != null && defaultOrientationField.InnerField != null)
            {
                view.AvailableOrientation.Landscape = defaultOrientationField.InnerField.Value == "Landscape";
                view.AvailableOrientation.Portrait = defaultOrientationField.InnerField.Value == "Portrait";
            }

            Sitecore.Data.Fields.MultilistField resourcesField = item.GetField("View", "Resources");
            Item[] resources = resourcesField.GetItems();

            view.Files = new List<RCFile>();
            foreach (var resourceItem in resources)
            {
                MediaItem mediaItem = ((FileField)resourceItem.GetField("View Resource", "File")).MediaItem;
                RCFile file = BuildRCToolFile(mediaItem, AssetType.ViewResource, resourceItem.Paths.ContentPath);
                if (file != null)
                {
                    view.Files.Add(file);
                }
            }

            return view;
        }

        private string CreateChecksum(MediaItem mediaItem)
        {
            Stream stream = mediaItem.GetMediaStream();
            string checksum = ComputeHash(stream, new System.Security.Cryptography.SHA1CryptoServiceProvider());
            return checksum;
        }

        private string ComputeHash(Stream stream, System.Security.Cryptography.HashAlgorithm algorithm)
        {
            stream.Position = 0;
            byte[] hash = algorithm.ComputeHash(stream);
            string value = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return value;
        }

        private void WritefileToFileSystem(string path, MediaItem mediaItem, string filename)
        {
            string physicalLocation = serverPhysicalPath;
            string filesystemPath = filename.StartsWith("/") ? filename.Replace("/", "\\").Substring(1) : filename.Replace("/", "\\");
            string fullFilename = Path.Combine(physicalLocation, filesystemPath);
            string filenamePath = fullFilename.Substring(0, fullFilename.LastIndexOf("\\") + 1);
            
            CreatePath(filenamePath);

            using (Stream stream = mediaItem.GetMediaStream())
            using (FileStream fileStream = new FileStream(fullFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 512, FileOptions.WriteThrough))
            {
                CopyStream(stream, fileStream);
            }
        }

        private void WriteCheckDataToFileSystem(string filename, string path, RcToolsData data)
        {
            string dataChecksum = string.Empty;
            using (MemoryStream stream = (MemoryStream)RcToolsHelper.ConvertToStreamWithFormat<RcToolsData>(data, "xml"))
            {
                dataChecksum = ComputeHash(stream, new System.Security.Cryptography.SHA1CryptoServiceProvider());
            }

            CheckData checkData = new CheckData();
            checkData.Checksum = dataChecksum;
            RcToolsData dataUpdateCheck = checkData;

            CreatePath(path);

            WriteDataToFileSystem(filename, path, dataUpdateCheck);
        }

        private void WriteDataToFileSystem(string filename, string path, RcToolsData data)
        {
            string fullFilename = Path.Combine(path, filename);

            using (MemoryStream stream = (MemoryStream)RcToolsHelper.ConvertToStreamWithFormat<RcToolsData>(data, "xml"))
            using (FileStream fileStream = new FileStream(fullFilename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 512, FileOptions.WriteThrough))
            {
                CopyStream(stream, fileStream);
            }
        }

        private void CreatePath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private int CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            int total = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                total += len;
                output.Write(buffer, 0, len);
            }
            return total;
        }
    }
}

