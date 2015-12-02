using Genworth.SitecoreExt.ScheduledTasks;
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
using Genworth.SitecoreExt.Utilities;

namespace Genworth.SitecoreExt.Importers
{
    public class ExpenseRatioImporter : IImportable, IPublishable
    {
        private const string EXPENSE_RATIO_SHEET_FILE_PREFIX = "\\AM_3260_ExpRatios_";
        private const int EXPENSE_RATIO_SHEET_FIELD_COUNT = 7;
        private const int EXPENSE_RATIO_FIELD_ALLOCAPPROACH = 3;
        private const int EXPENSE_RATIO_FIELD_YEAR = 4;
        private const int EXPENSE_RATIO_FIELD_MONTH = 5;

        protected Dictionary<string, string> dictStrategists = new Dictionary<string, string>();
        protected Dictionary<string, Item> dictApproaches = new Dictionary<string, Item>();
        protected Dictionary<string, Sitecore.Data.ID> dictCategories = new Dictionary<string, Sitecore.Data.ID>();
        protected Dictionary<string, Sitecore.Data.ID> dictSources = new Dictionary<string, Sitecore.Data.ID>();

        public ExpenseRatioImporter()
        {
            Log.Debug("Genworth.SitecoreExt.ScheduledTasks.ExpenseRatioImporter:DocImporter", this);

            Database db = Factory.GetDatabase("master");

            dictStrategists = ContentEditorHelper.GetStrategist(db, ContentEditorHelper.STRATEGIST_PATH);
            dictApproaches = ContentEditorHelper.GetAllocationApproches(db, ContentEditorHelper.ALLOCATION_PATH);
            dictCategories = ContentEditorHelper.GetLookupIds(db, ContentEditorHelper.CATEGORY_PATH);
            dictSources = ContentEditorHelper.GetLookupIds(db, ContentEditorHelper.SOURCE_PATH);
        }

        public bool GetFile(string path, out string fileName)
        {
            bool bOk = false;
            fileName = string.Empty;

            try
            {
                Log.Debug("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:GetFile", this);

                fileName = ContentEditorHelper.GetFile(path, EXPENSE_RATIO_SHEET_FILE_PREFIX);
                if (fileName != string.Empty)
                    bOk = true;

            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:GetFile, path {0}, exception: {1}", path, ex.ToString()), this);
                bOk = false;
            }

            return bOk;
        }

        public bool ImportFile(string path, string fileName)
        {
            if (String.IsNullOrEmpty(path))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFile, path is null or empty, skipping file."), this);
                return false;
            }
            else if (String.IsNullOrEmpty(fileName))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFile, file is null or empty, skipping file. Path: {0}",
                    path), this);
                return false;
            }

            Log.Info(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFile, base path {0}, file name {1}", path, fileName), this);

            bool bStatus = true;    // overall function status
            bool bOk = true;        // individual call status

            try
            {
                Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                MediaItem mediaItem = null;

                if (!File.Exists(Path.Combine(path, fileName + ".pdf")))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFile, file not found, skipping file. Path: {0}",
                                       path), this);
                    return false;
                }

                string[] fields = fileName.Split('_');

                if (fields.Length != EXPENSE_RATIO_SHEET_FIELD_COUNT)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFiles, expecting {0} fields but found {1}, file: {2}",
                        EXPENSE_RATIO_SHEET_FIELD_COUNT, fields.Length, fileName), this);
                    File.Move(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                string allocationCode = fields[EXPENSE_RATIO_FIELD_ALLOCAPPROACH].ToUpper();
                if (!dictApproaches.ContainsKey(allocationCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFiles, unable to lookup allocation code {0}", allocationCode), this);
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                Item allocationItem = dictApproaches[allocationCode];

                string title = GenerateDocTitle(fileName);
                string contentPath = "/sitecore/media library/Files/Investments/Expense Ratios/" + allocationItem["Title"] + "/" + title;
                
                mediaItem = ContentEditorHelper.UploadContent(master, Path.Combine(path, fileName + ".pdf"), contentPath);

                if (mediaItem == null)
                    return false;

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
                bOk = mapMediaItemToSharedContent(master, allocationItem, title, mediaItem);
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
                        Log.Error("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFiles, processed directory does not exist", this);
                    }
                }
                else
                {
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:ImportFile, file: {0}, exception: {1}", fileName, ex.ToString()), this);
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
            Log.Debug("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:GenerateDocTitle", this);
            StringBuilder title = new StringBuilder();

            try
            {
                string[] fields = fileName.Split('_');
                string allocationCode = fields[EXPENSE_RATIO_FIELD_ALLOCAPPROACH].ToUpper();

                if (!String.IsNullOrEmpty(allocationCode) && !dictApproaches.ContainsKey(allocationCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:GenerateDocTitle, allocation dictionary does not contain the requested allocation, Filename: {0}, Allocation code: {1}", fileName, allocationCode), this);
                    title.Append(fileName);
                }
                else
                {
                    Item allocationItem = dictApproaches[allocationCode];

                    string year = fields[EXPENSE_RATIO_FIELD_YEAR];
                    string month = fields[EXPENSE_RATIO_FIELD_MONTH];

                    if (!String.IsNullOrEmpty(allocationItem["Title"]))
                    {
                        title.Append("Expense Ratios for " + allocationItem["Title"].Replace(".", "") + " as of " + month + "-" + ContentEditorHelper.GetLastDayOfMonth(int.Parse(month), int.Parse(year)) + "-" + year);
                    }
                    else
                    {
                        Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:GenerateDocTitle, allocation description does not contain value for allocationCode " + allocationCode + ", Filename: {0}, Allocation code: {1}", fileName, allocationCode), this);
                        title.Append(fileName);
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:GenerateDocTitle, exception:" + ex.ToString(), this);
            }

            return title.ToString();
        }

        public bool PublishSite()
        {
            try
            {
                ContentEditorHelper.PublishSite("/sitecore/content/Shared Content/Investments");
                ContentEditorHelper.PublishSite("/sitecore/media library/Files/Investments/Expense Ratios");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool mapMediaItemToSharedContent(Sitecore.Data.Database database, Item allocationItem, string title, MediaItem mediaItem)
        {
            bool bOk = false;

            try
            {
                Item docs = allocationItem.Children["Documents"];
                if (docs == null)
                {
                    Sitecore.Data.Items.TemplateItem docsTemplateItem = database.GetItem("/sitecore/templates/Genworth/Folder Types/Investment Folders/Investment Document Folder");
                    docs = allocationItem.Add("Documents", docsTemplateItem);
                }

                Item performanceItem = docs.Children[title];
                if (performanceItem == null)
                {
                    Sitecore.Data.Items.TemplateItem performanceTemplateItem = database.GetItem("/sitecore/templates/Genworth/Document Types/Investments/Portfolio Information/Portfolio Information");
                    performanceItem = docs.Add(title, performanceTemplateItem);
                }

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    performanceItem.Editing.BeginEdit();

                    Sitecore.Data.Fields.FileField fileField = performanceItem.Fields["File"];
                    fileField.MediaID = mediaItem.ID;
                    fileField.Src = Sitecore.Resources.Media.MediaManager.GetMediaUrl(mediaItem);

                    if (dictCategories.ContainsKey("Portfolio Information"))
                    {
                        performanceItem["Category"] = dictCategories["Portfolio Information"].ToString();
                    }

                    if (dictSources.ContainsKey("AssetMark"))
                    {
                        performanceItem["Source"] = dictSources["AssetMark"].ToString();
                    }

                    performanceItem["Date"] = Sitecore.DateUtil.ToIsoDate(DateTime.Now);

                    performanceItem.Editing.EndEdit();
                }

                bOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.ExpenseRatioImporter:mapMediaItemToSharedContent, allocation: {1}, title {2}, exception: {3}", allocationItem["Title"], title, ex.ToString()), this);
            }

            return bOk;
        }

    }
}
