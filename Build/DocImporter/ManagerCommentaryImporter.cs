using Genworth.SitecoreExt.ScheduledTasks;
using Genworth.SitecoreExt.Utilities;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.DocImporter
{
    public class ManagerCommentaryImporter : IImportable, IPublishable
    {
        private const string MGR_COM_SHEET_FILE_PREFIX = "\\AM_20875_MgrComm_";
        private const int MGR_COM_SHEET_FIELD_COUNT = 9;
        private const int MGR_COM_FIELD_MANAGER_CODE = 3;
        private const int MGR_COM_FIELD_DESCRIPTION_CODE = 4;
        private const int MGR_COM_FIELD_QUARTER = 5;
        private const int MGR_COM_FIELD_YEAR = 6;
        private const int MGR_COM_FIELD_MONTH = 7;

        protected Dictionary<string, string> dictStrategists = new Dictionary<string, string>();
        protected Dictionary<string, Item> dictApproaches = new Dictionary<string, Item>();
        protected Dictionary<string, Sitecore.Data.ID> dictCategories = new Dictionary<string, Sitecore.Data.ID>();
        protected Dictionary<string, Sitecore.Data.ID> dictSources = new Dictionary<string, Sitecore.Data.ID>();
        protected Dictionary<string, Item> dictManagers = new Dictionary<string, Item>();
        protected Dictionary<string, Item> dictDescriptions = new Dictionary<string, Item>();

        public ManagerCommentaryImporter()
        {
            Log.Debug("Genworth.SitecoreExt.ScheduledTasks.ManagerCommentaryImporter:DocImporter", this);

            Database db = Factory.GetDatabase("master");

            dictStrategists = ContentEditorHelper.GetStrategist(db, ContentEditorHelper.STRATEGIST_PATH);
            dictApproaches = ContentEditorHelper.GetAllocationApproches(db, ContentEditorHelper.ALLOCATION_PATH);
            dictCategories = ContentEditorHelper.GetLookupIds(db, ContentEditorHelper.CATEGORY_PATH);
            dictSources = ContentEditorHelper.GetLookupIds(db, ContentEditorHelper.SOURCE_PATH);
            dictManagers = ContentEditorHelper.GetManagers(db, ContentEditorHelper.MANAGER_PATH);
            dictDescriptions = ContentEditorHelper.GetDescriptions(db, ContentEditorHelper.DESCRIPTION_PATH);
        }

        public bool GetFile(string path, out string fileName)
        {
            bool bOk = false;
            fileName = string.Empty;

            try
            {
                Log.Debug("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:GetFile", this);

                fileName = ContentEditorHelper.GetFile(path, MGR_COM_SHEET_FILE_PREFIX);
                if (fileName != string.Empty)
                    bOk = true;

            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:GetFile, path {0}, exception: {1}", path, ex.ToString()), this);
                bOk = false;
            }

            return bOk;
        }

        public bool ImportFile(string path, string fileName)
        {
            if (String.IsNullOrEmpty(path))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFile, path is null or empty, skipping file."), this);
                return false;
            }
            else if (String.IsNullOrEmpty(fileName))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFile, file is null or empty, skipping file. Path: {0}",
                    path), this);
                return false;
            }

            Log.Info(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFile, base path {0}, file name {1}", path, fileName), this);

            bool bStatus = true;    // overall function status
            bool bOk = true;        // individual call status

            try
            {
                Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                MediaItem mediaItem = null;

                if (!File.Exists(Path.Combine(path, fileName + ".pdf")))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFile, file not found, skipping file. Path: {0}",
                                       path), this);
                    return false;
                }

                string[] fields = fileName.Split('_');

                if (fields.Length != MGR_COM_SHEET_FIELD_COUNT)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFiles, expecting {0} fields but found {1}, file: {2}",
                        MGR_COM_SHEET_FIELD_COUNT, fields.Length, fileName), this);
                    File.Move(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                string managerCode = fields[MGR_COM_FIELD_MANAGER_CODE].ToUpper();
                if (!dictManagers.ContainsKey(managerCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFiles, unable to lookup manager code {0}", managerCode), this);
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                Item managerItem = dictManagers[managerCode];

                string title = GenerateDocTitle(fileName);
                string contentPath = string.Empty;

                if (!String.IsNullOrEmpty(title))
                {
                    contentPath = "/sitecore/media library/Files/Investments/" + managerItem["Name"] + "/Commentary/" + title;
                    mediaItem = ContentEditorHelper.UploadContent(master, Path.Combine(path, fileName + ".pdf"), contentPath);
                }

                if (mediaItem == null)
                {
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                // set meta-data here
                Sitecore.Data.Items.Item item = master.GetItem(contentPath);
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    item.Editing.BeginEdit();
                    item["Description"] = fileName; // save original file name 
                    //item["DisplayName"] = title;
                    item.Editing.EndEdit();
                }

                // map media item to strategist docs
                bOk = mapMediaItemToSharedContent(master, managerItem, title, mediaItem);
                bStatus = bStatus && bOk;

                // only move file if all parts of the process were successful
                if (bStatus)
                {
                    // move to processed folder
                    if (Directory.Exists(path + "\\Processed"))
                    {
                        string postFix = DateTime.Now.ToString("_MM-dd-yyyy-hh-mm-ss");

                        ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, "Processed", fileName + postFix + ".pdf"));
                    }
                    else
                    {
                        Log.Error("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFiles, processed directory does not exist", this);
                    }
                }
                else
                {
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:ImportFile, file: {0}, exception: {1}", fileName, ex.ToString()), this);
                bStatus = false;
            }

            if (!bStatus)
            {
                ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
            }

            return bOk;
        }

        public string GenerateDocTitle(string fileName)
        {
            Log.Debug("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:GenerateDocTitle", this);
            StringBuilder title = new StringBuilder();

            try
            {
                string[] fields = fileName.Split('_');
                string managerCode = fields[MGR_COM_FIELD_MANAGER_CODE].ToUpper();
                string descriptionCode = fields[MGR_COM_FIELD_DESCRIPTION_CODE].ToUpper().Replace("X", "");

                if (!String.IsNullOrEmpty(managerCode) && !dictManagers.ContainsKey(managerCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:GenerateDocTitle, manager dictionary does not contain the requested manager, Filename: {0}, Allocation code: {1}", fileName, managerCode), this);
                }
                else if (!String.IsNullOrEmpty(descriptionCode) && !dictDescriptions.ContainsKey(descriptionCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:GenerateDocTitle, description dictionary does not contain the requested description, Filename: {0}, Description code: {1}", fileName, descriptionCode), this);
                }
                else
                {
                    Item managerItem = null, descriptionItem = null;

                    if (!String.IsNullOrEmpty(managerCode) && dictManagers.ContainsKey(managerCode))
                    {
                        managerItem = dictManagers[managerCode];
                    }

                    if (!String.IsNullOrEmpty(descriptionCode) && dictDescriptions.ContainsKey(descriptionCode))
                    {
                        descriptionItem = dictDescriptions[descriptionCode];
                    }

                    string year = fields[MGR_COM_FIELD_YEAR];
                    string month = fields[MGR_COM_FIELD_MONTH];
                    string quarter = fields[MGR_COM_FIELD_QUARTER];

                    if (!String.IsNullOrEmpty(managerItem["Name"]))
                    {
                        title.Append(managerItem["Name"].Replace(".", ""));

                        if (descriptionItem != null)
                        {
                            title.Append(" " + descriptionItem.Name.Replace(".", ""));
                        }

                        title.Append(" Manager Commentary " + quarter + " " + year);
                    }
                    else
                    {
                        Log.Error(String.Format("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:GenerateDocTitle, allocation description does not contain value for allocationCode " + managerCode + ", Filename: {0}, Allocation code: {1}", fileName, managerCode), this);
                        title.Append(fileName);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error("Genworth.SitecoreExt.Importers.ManagerCommentaryImporter:GenerateDocTitle, exception:" + ex.ToString(), this);
            }

            return title.ToString();
        }

        public bool PublishSite()
        {
            try
            {
                ContentEditorHelper.PublishSite("/sitecore/content/Shared Content/Investments");
                ContentEditorHelper.PublishSite("/sitecore/media library/Files/Investments");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool mapMediaItemToSharedContent(Sitecore.Data.Database database, Item managerItem, string title, MediaItem mediaItem)
        {
            bool bOk = false;

            try
            {
                Item docs = managerItem.Children["Documents"];
                if (docs == null)
                {
                    Sitecore.Data.Items.TemplateItem docsTemplateItem = database.GetItem("/sitecore/templates/Genworth/Folder Types/Investment Folders/Investment Document Folder");
                    docs = managerItem.Add("Documents", docsTemplateItem);
                }

                Item commentaryItem = docs.Children[title];
                if (commentaryItem == null)
                {
                    Sitecore.Data.Items.TemplateItem commentaryTemplateItem = database.GetItem("/sitecore/templates/Genworth/Document Types/Investments/Product Commentary/Product Commentary");
                    commentaryItem = docs.Add(title, commentaryTemplateItem);
                }

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    commentaryItem.Editing.BeginEdit();

                    Sitecore.Data.Fields.FileField fileField = commentaryItem.Fields["File"];
                    fileField.MediaID = mediaItem.ID;
                    fileField.Src = Sitecore.Resources.Media.MediaManager.GetMediaUrl(mediaItem);

                    if (dictCategories.ContainsKey("Commentary"))
                    {
                        commentaryItem["Category"] = dictCategories["Commentary"].ToString();
                    }

                    if (dictSources.ContainsKey("AssetMark"))
                    {
                        commentaryItem["Source"] = dictSources["AssetMark"].ToString();
                    }

                    commentaryItem["Date"] = Sitecore.DateUtil.ToIsoDate(DateTime.Now);

                    commentaryItem.Editing.EndEdit();
                }

                bOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:mapMediaItemToSharedContent, allocation: {1}, title {2}, exception: {3}", managerItem["Name"], title, ex.ToString()), this);
            }

            return bOk;
        }
    }
}
