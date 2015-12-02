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
    public class StrategistCommentaryImporter : IImportable, IPublishable
    {
        private const string STR_COM_SHEET_FILE_PREFIX = "\\AM_1600_StratComm_";
        private const int STR_COM_SHEET_FIELD_COUNT = 10;
        private const int STR_COM_FIELD_STRATEGIST_CODE = 3;
        private const int STR_COM_FIELD_ALLOCAPPROACH = 4;
        private const int STR_COM_FIELD_QUARTER = 6;
        private const int STR_COM_FIELD_STRATEGY = 5;
        private const int STR_COM_FIELD_YEAR = 7;
        private const int STR_COM_FIELD_MONTH = 8;

        protected Dictionary<string, string> dictStrategists = new Dictionary<string, string>();
        protected Dictionary<string, Item> dictApproaches = new Dictionary<string, Item>();
        protected Dictionary<string, Sitecore.Data.ID> dictCategories = new Dictionary<string, Sitecore.Data.ID>();
        protected Dictionary<string, Sitecore.Data.ID> dictSources = new Dictionary<string, Sitecore.Data.ID>();
        protected Dictionary<string, Item> dictManagers = new Dictionary<string, Item>();
        protected Dictionary<string, Item> dictDescriptions = new Dictionary<string, Item>();

        public StrategistCommentaryImporter()
        {
            Log.Debug("Genworth.SitecoreExt.ScheduledTasks.StrategistCommentaryImporter:DocImporter", this);

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
                Log.Debug("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:GetFile", this);

                fileName = ContentEditorHelper.GetFile(path, STR_COM_SHEET_FILE_PREFIX);
                if (fileName != string.Empty)
                    bOk = true;

            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:GetFile, path {0}, exception: {1}", path, ex.ToString()), this);
                bOk = false;
            }

            return bOk;
        }

        public bool ImportFile(string path, string fileName)
        {
            if (String.IsNullOrEmpty(path))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, path is null or empty, skipping file."), this);
                return false;
            }
            else if (String.IsNullOrEmpty(fileName))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, file is null or empty, skipping file. Path: {0}",
                    path), this);
                return false;
            }

            Log.Info(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, base path {0}, file name {1}", path, fileName), this);

            bool bStatus = true;    // overall function status
            bool bOk = true;        // individual call status

            try
            {
                Sitecore.Data.Database master = Sitecore.Configuration.Factory.GetDatabase("master");
                MediaItem mediaItem = null;
                bool isGPS = false;

                if (!File.Exists(Path.Combine(path, fileName + ".pdf")))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, file not found, skipping file. Path: {0}",
                                       path), this);
                    return false;
                }

                string[] fields = fileName.Split('_');

                if (fields.Length != STR_COM_SHEET_FIELD_COUNT)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, expecting {0} fields but found {1}, file: {2}",
                        STR_COM_SHEET_FIELD_COUNT, fields.Length, fileName), this);
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                string strategistCode = fields[STR_COM_FIELD_STRATEGIST_CODE].ToUpper();
                string allocationCode = fields[STR_COM_FIELD_ALLOCAPPROACH].ToUpper();
                string strategyCode = fields[STR_COM_FIELD_STRATEGY].Replace("X", "").ToUpper(); // swap out placeholder value for empty string

                if (!dictStrategists.ContainsKey(strategistCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, could not look up strategist {0}, skipping file", strategistCode), this);
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                    return false;
                }

                // Need to promote GPS & GPS Select to strategists to handle inconsistencies between EWM and Business logic.
                if (((strategistCode == "GFWM") && (allocationCode == "GFWMC")) ||
                    ((strategistCode == "GFWM") && (allocationCode == "GPS")))
                {
                    strategistCode = allocationCode;
                    allocationCode = string.Empty;
                    isGPS = true;
                }

                string strategist = dictStrategists[strategistCode];
                Item allocationItem = null;

                if (!String.IsNullOrEmpty(allocationCode) && dictApproaches.ContainsKey(allocationCode))
                {
                    allocationItem = dictApproaches[allocationCode];
                }


                string title = GenerateDocTitle(fileName);
                string contentPath = string.Empty;

                if (!String.IsNullOrEmpty(title))
                {
                    contentPath = "/sitecore/media library/Files/Investments/" + ContentEditorHelper.CleanNodeName(strategist) + "/Commentary/" + title;
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
                bOk = mapMediaItemToSharedContent(master, strategist, allocationItem, title, mediaItem, out itemId);
                bStatus = bStatus && bOk;

                //if (!isGPS)
                //{
                //    // update solution to point to latest doc
                //    bOk = mapItemToStrategist(master, strategist, allocationItem, strategyCode, itemId);
                //    bStatus = bStatus && bOk;
                //}

                // only move file if all parts of the process were successful
                if (bStatus)
                {
                    // move to processed folder
                    if (Directory.Exists(Path.Combine(path, "Processed")))
                    {
                        string postFix = DateTime.Now.ToString("_MM-dd-yyyy-hh-mm-ss");

                        ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, "Processed", fileName + postFix + ".pdf"));
                    }
                    else
                    {
                        Log.Error("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, processed directory does not exist", this);
                    }
                }
                else
                {
                    ContentEditorHelper.MoveFile(Path.Combine(path, fileName + ".pdf"), Path.Combine(path, fileName + ".pdf.failed"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:ImportFile, file: {0}, exception: {1}", fileName, ex.ToString()), this);
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
            Log.Debug("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:GenerateDocTitle", this);
            StringBuilder title = new StringBuilder();

            try
            {
                string[] fields = fileName.Split('_');
                string strategistCode = fields[STR_COM_FIELD_STRATEGIST_CODE].ToUpper();
                string allocationCode = fields[STR_COM_FIELD_ALLOCAPPROACH].ToUpper();
                string descriptionCode = fields[STR_COM_FIELD_STRATEGY].ToUpper().Replace("X", "");

                // Need to promote GPS & GPS Select to strategists to handle inconsistencies between EWM and Business logic.
                if (((strategistCode == "GFWM") && (allocationCode == "GFWMC")) ||
                    ((strategistCode == "GFWM") && (allocationCode == "GPS")))
                {
                    strategistCode = allocationCode;
                    allocationCode = string.Empty;
                }

                if (!String.IsNullOrEmpty(strategistCode) && !dictStrategists.ContainsKey(strategistCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:GenerateDocTitle, strategist dictionary does not contain requested strategist, Filename: {0}, Strategist code: {1}", fileName, strategistCode), this);
                    title.Append(fileName);
                }
                else if (!String.IsNullOrEmpty(allocationCode) && !dictApproaches.ContainsKey(allocationCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:GenerateDocTitle, allocation dictionary does not contain requested allocation, Filename: {0}, Allocation code: {1}", fileName, allocationCode), this);
                    title.Append(fileName);
                }
                else if (!String.IsNullOrEmpty(descriptionCode) && !dictDescriptions.ContainsKey(descriptionCode))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:GenerateDocTitle, description dictionary does not contain the requested description, Filename: {0}, Description code: {1}", fileName, descriptionCode), this);
                }
                else
                {
                    string strategist = string.Empty;
                    string allocation = string.Empty;
                    Item allocationItem = null, descriptionItem = null;


                    if (!String.IsNullOrEmpty(allocationCode) && dictApproaches.ContainsKey(allocationCode))
                    {
                        allocationItem = dictApproaches[allocationCode];
                    }

                    if (!String.IsNullOrEmpty(descriptionCode) && dictDescriptions.ContainsKey(descriptionCode))
                    {
                        descriptionItem = dictDescriptions[descriptionCode];
                    }

                    string year = fields[STR_COM_FIELD_YEAR];
                    string quarter = fields[STR_COM_FIELD_QUARTER];

                    if (!String.IsNullOrEmpty(strategistCode) && dictStrategists.ContainsKey(strategistCode))
                    {
                        strategist = dictStrategists[strategistCode];

                        title.Append(strategist.Replace(".", "").Replace(",", ""));

                        if (allocationItem != null)
                        {
                            title.Append(" " + allocation.Replace(".", ""));
                        }

                        if (descriptionItem != null)
                        {
                            title.Append(" " + descriptionItem.Name.Replace(".", ""));
                        }

                        title.Append(" Strategist Commentary " + quarter + " " + year); // Per Review with marketing, removing 'as of' date from name and label // + " as of " + DateTime.Now.ToString("MM-dd-yyyy"));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:GenerateDocTitle, exception:" + ex.ToString(), this);
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

        private bool mapMediaItemToSharedContent(Sitecore.Data.Database database, string strategist, Item allocationItem, string title, MediaItem mediaItem, out ID itemId)
        {
            bool bOk = false;
            itemId = null;
            string allocation = string.Empty;

            if (allocationItem != null)
                allocation = allocationItem["Title"];

            try
            {
                // Sitecore/content/Shared Content/Investments/Strategists/<strategist>/<allocation approach>/Documents/<fact sheet>
                string sharedPath = "/sitecore/content/Shared Content/Investments/Strategists/" + strategist;
                Item strategistItem = database.GetItem(sharedPath);

                if (strategistItem == null)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToSharedContent, shared content folder for strategist {0} does not exist, skipping. Path: {1}", strategist, sharedPath), this);
                    return false;
                }

                if (!String.IsNullOrEmpty(allocation))
                {
                    allocationItem = strategistItem.Children[allocation];
                    if (allocationItem == null)
                    {
                        Sitecore.Data.Items.TemplateItem docsTemplateItem = database.GetItem("/sitecore/templates/Genworth/Data/Strategy");
                        allocationItem = strategistItem.Add(allocation, docsTemplateItem);
                    }
                }
                else // GPS & GPS Select-only scenario (no allocation approach)
                {
                    allocationItem = strategistItem;
                }

                //Item docs = allocationItem.Children["Documents"];
                Item docs = strategistItem.Children["Documents"];
                if (docs == null)
                {
                    Sitecore.Data.Items.TemplateItem docsTemplateItem = database.GetItem("/sitecore/templates/Genworth/Folder Types/Investment Folders/Investment Document Folder");
                    //docs = allocationItem.Add("Documents", docsTemplateItem);
                    docs = strategistItem.Add("Documents", docsTemplateItem);
                }

                Item commentaryItem = docs.Children[title];
                if (commentaryItem == null)
                {
                    Sitecore.Data.Items.TemplateItem commentaryTemplateItem = database.GetItem("/sitecore/templates/Genworth/Document Types/Investments/Product Commentary/Product Commentary");
                    commentaryItem = docs.Add(title, commentaryTemplateItem);
                }

                itemId = commentaryItem.ID;

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
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToSharedContent, strategist: {0}, allocation: {1}, title {2}, exception: {3}", strategist, allocation, title, ex.ToString()), this);
            }

            return bOk;
        }

        private bool mapItemToStrategist(Sitecore.Data.Database database, string strategist, Item allocationItem, string strategy, ID itemId)
        {
            bool bOk = false;

            try
            {
                if (database == null)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToStrategist, database cannot be null or empty"), this);
                    return false;
                }
                else if (String.IsNullOrEmpty(strategist))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToStrategist, strategy cannot be null or empty"), this);
                    return false;
                }
                else if (String.IsNullOrEmpty(allocationItem["Title"]))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToStrategist, allocation cannot be null or empty. Strategist: {0}", strategist), this);
                    return false;
                }
                else if (ID.IsNullOrEmpty(itemId))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToStrategist, mediaItem cannot be null or empty. Strategist: {0}, allocation: {1}", strategist, allocationItem["Title"]), this);
                    return false;
                }

                string sharedPath = "/sitecore/content/Shared Content/Investments/Strategists/#" + strategist + "#/#" + allocationItem.Name + "#/*[@@templatekey = 'solution' and @Code='" + strategy + "']";
                Item strategyItem = database.SelectSingleItem(sharedPath);

                if (strategyItem == null)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToStrategist, solution node for strategy {0}, allocation approach {1}, does not exist, skipping. " +
                        "This generally happens when the solution node is either missing or has a code that doesn't match the document naming convention. Path: {2}",
                        strategist,
                        allocationItem["Title"],
                        sharedPath), this);
                    return false;
                }

                Sitecore.Data.Fields.ReferenceField referenceField = strategyItem.Fields["Fact Sheet"];
                if (referenceField != null)
                {
                    using (new Sitecore.SecurityModel.SecurityDisabler())
                    {
                        strategyItem.Editing.BeginEdit();
                        referenceField.Value = itemId.ToString();
                        strategyItem.Editing.EndEdit();
                    }
                }

                bOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.StrategistCommentaryImporter:mapMediaItemToStrategist, strategist: {0}, allocation: {1}, solution: {2}, exception: {3}", strategist, allocationItem["Title"], strategy, ex.ToString()), this);
                bOk = false;
            }

            return bOk;
        }
    }
}
