using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

using Genworth.SitecoreExt.ScheduledTasks;
using Genworth.SitecoreExt.Utilities;

namespace Genworth.SitecoreExt.Importers
{
    public class MonthlyPerformanceImporter : IImportable, IPublishable
    {
        private const string PERF_SHEET_FILE_PREFIX = "\\AM_3301_MonthlyPerformance_";
        private const int PERF_SHEET_FIELD_COUNT = 8;
        //private const int PERF_SHEET_FIELD_ORGANIZATION = 0;
        //private const int PERF_SHEET_FIELD_FORM = 1;
        //private const int PERF_SHEET_FIELD_CATEGORY = 2;
        private const int PERF_SHEET_FIELD_ALLOCAPPROACH = 3;
        private const int PERF_SHEET_FIELD_DATERANGE = 4;
        private const int PERF_SHEET_FIELD_YEAR = 5;
        private const int PERF_SHEET_FIELD_MONTH = 6;
        //private const int PERF_SHEET_FIELD_COMPLIANCENUM = 7;

        protected Dictionary<string, string> dictStrategists = new Dictionary<string, string>();
        protected Dictionary<string, Item> dictApproaches = new Dictionary<string, Item>();
        protected Dictionary<string, Sitecore.Data.ID> dictCategories = new Dictionary<string, Sitecore.Data.ID>();
        protected Dictionary<string, Sitecore.Data.ID> dictSources = new Dictionary<string, Sitecore.Data.ID>();

        public MonthlyPerformanceImporter()
        {
            Log.Debug("Genworth.SitecoreExt.ScheduledTasks.MonthlyPerformanceImporter:DocImporter", this);

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
                Log.Debug("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:GetFile", this);

                fileName = ContentEditorHelper.GetFile(path, PERF_SHEET_FILE_PREFIX);
                if (fileName != string.Empty)
                    bOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:GetFile, path {0}, exception: {1}", path, ex.ToString()), this);
                bOk = false;
            }

            return bOk;
        }

        public bool ImportFile(string path, string fileName)
        {
            Log.Info(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:ImportFiles, base path {0}, file name {1}", path, fileName), this);

            bool bStatus = true;    // overall function status
            bool bOk = true;        // individual call status

            try
            {
                Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                MediaItem mediaItem = null;

                if (!File.Exists(Path.Combine(path, fileName + ".pdf")))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:ImportFile, file not found, skipping file. Path: {0}",
                                       path), this);
                    return false;
                }

                string[] fields = fileName.Split('_');

                if (fields.Length != PERF_SHEET_FIELD_COUNT)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:ImportFiles, expecting {0} fields but found {1}, file: {2}",
                        PERF_SHEET_FIELD_COUNT, fields.Length, fileName), this);
                    File.Move(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                string allocationCode = fields[PERF_SHEET_FIELD_ALLOCAPPROACH].ToUpper();
                if (!dictApproaches.ContainsKey(allocationCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:ImportFiles, unable to lookup allocation code {0}", allocationCode), this);
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                Item allocationItem = dictApproaches[allocationCode];

                string title = GenerateDocTitle(fileName);
                string contentPath = string.Empty;

                if (!String.IsNullOrEmpty(title))
                {
                    contentPath = "/sitecore/media library/Files/Investments/Strategist Performance/" + allocationItem["Title"] + "/" + title;
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
                ID itemId;
                bOk = mapMediaItemToSharedContent(master, allocationItem, title, mediaItem, out itemId);
                bStatus = bStatus && bOk;

                // update allocation node with latest doc
                string dateRange = fields[PERF_SHEET_FIELD_DATERANGE];
                bOk = mapItemToAllocation(master, allocationItem, itemId, dateRange);
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
                        Log.Error("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:ImportFiles, processed directory does not exist", this);
                    }
                }
                else
                {
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:MonthlyPerformanceImporter, file: {0}, exception: {1}", fileName, ex.ToString()), this);
                bStatus = false;
            }

            if (!bStatus)
            {
                ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
            }

            return bStatus;
        }

        public string GenerateDocTitle(string fileName)
        {
            Log.Debug("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:GenerateDocTitle", this);
            StringBuilder title = new StringBuilder();

            try
            {
                string[] fields = fileName.Split('_');
                string allocationCode = fields[PERF_SHEET_FIELD_ALLOCAPPROACH].ToUpper();

                if (!String.IsNullOrEmpty(allocationCode) && !dictApproaches.ContainsKey(allocationCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:GenerateDocTitle, allocation dictionary does not contain the requested allocation, Filename: {0}, Allocation code: {1}", fileName, allocationCode), this);
                    title.Append(fileName);
                }
                else
                {
                    Item allocationItem = dictApproaches[allocationCode];

                    string year = fields[PERF_SHEET_FIELD_YEAR];
                    string month = fields[PERF_SHEET_FIELD_MONTH];
                    string dateRange = fields[PERF_SHEET_FIELD_DATERANGE].ToUpper();

                    if (!String.IsNullOrEmpty(allocationItem["Title"]))
                    {
                        title.Append(allocationItem["Title"].Replace(".", ""));
                    }
                    else
                    {
                        Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:GenerateDocTitle, allocation description does not contain value for allocationCode " + allocationCode + ", Filename: {0}, Allocation code: {1}", fileName, allocationCode), this);
                        title.Append(fileName);
                    }

                    if (dateRange == "C")
                    {
                        dateRange = "Yearly";
                    }
                    else if (dateRange == "M")
                    {
                        dateRange = "Monthly";
                    }

                    if (!String.IsNullOrEmpty(dateRange))
                    {
                        title.Append(" " + dateRange);
                    }

                    title.Append(" Performance " + month + "-" + year);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:GenerateDocTitle, exception:" + ex.ToString(), this);
            }

            return title.ToString();
        }

        public bool PublishSite()
        {
            try
            {
                ContentEditorHelper.PublishSite("/sitecore/content/Shared Content/Investments");
                ContentEditorHelper.PublishSite("/sitecore/media library/Files/Investments/Strategist Performance");
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;

        }

        private bool mapMediaItemToSharedContent(Sitecore.Data.Database database, Item allocationItem, string title, MediaItem mediaItem, out ID itemId)
        {
            bool bOk = false;
            itemId = null;

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
                    Sitecore.Data.Items.TemplateItem performanceTemplateItem = database.GetItem("/sitecore/templates/Genworth/Document Types/Investments/Portfolio Information/Monthly Strategist Performance Report");
                    performanceItem = docs.Add(title, performanceTemplateItem);
                }

                itemId = performanceItem.ID;

                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    performanceItem.Editing.BeginEdit();

                    Sitecore.Data.Fields.FileField fileField = performanceItem.Fields["File"];
                    fileField.MediaID = mediaItem.ID;
                    fileField.Src = Sitecore.Resources.Media.MediaManager.GetMediaUrl(mediaItem);

                    if (dictCategories.ContainsKey("Performance"))
                    {
                        performanceItem["Category"] = dictCategories["Performance"].ToString();
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
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:mapMediaItemToSharedContent, allocation: {1}, title {2}, exception: {3}", allocationItem["Title"], title, ex.ToString()), this);
            }

            return bOk;
        }

        private bool mapItemToAllocation(Sitecore.Data.Database database, Item allocationItem, ID itemId, string dateRange)
        {
            bool bOk = false;

            try
            {
                if (database == null)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:mapItemToAllocation, database cannot be null or empty"), this);
                    return false;
                }
                else if (String.IsNullOrEmpty(allocationItem["Title"]))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:mapItemToAllocation, allocation cannot be null or empty. Strategist: {0}", allocationItem["Title"]), this);
                    return false;
                }
                else if (ID.IsNullOrEmpty(itemId))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:mapItemToAllocation, itemId to map cannot be null or empty. Allocation: {0}", allocationItem["Title"]), this);
                    return false;
                }


                if (allocationItem["Title"] == null)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:mapItemToAllocation, allocation approach {0}, does not exist, skipping. Path: {1}",
                        allocationItem["Title"],
                        allocationItem.Paths.Path), this);
                    return false;
                }

                Sitecore.Data.Fields.ReferenceField referenceField = null;

                if (dateRange == "M")
                {
                    referenceField = allocationItem.Fields["Calendar Month Performance"];
                }
                else if (dateRange == "C")
                {
                    referenceField = allocationItem.Fields["Calendar Year Performance"];
                }
                else
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:mapItemToAllocation, unsupported date range -- must be 'C' or 'M'. Allocation: {0}, dateRange {1}",
                        allocationItem["Title"],
                        dateRange), this);
                    bOk = false;
                }

                if (referenceField != null)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        allocationItem.Editing.BeginEdit();
                        referenceField.Value = itemId.ToString();
                        allocationItem.Editing.EndEdit();
                    }
                }

                bOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.MonthlyPerformanceImporter:mapItemToAllocation, allocation: {0}, dateRange: {1}, exception: {2}", allocationItem["Title"], dateRange, ex.ToString()), this);
            }

            return bOk;
        }

    }
}
