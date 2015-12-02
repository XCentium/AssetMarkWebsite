using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Configuration;

namespace Genworth.SitecoreExt.Utilities
{
    public static class ContentEditorHelper
    {
        public static string STRATEGIST_PATH = "/sitecore/content/Shared Content/Investments/Strategists/*";
        public static string ALLOCATION_PATH = "/sitecore/content/Shared Content/Investments/Asset Allocation Approaches/*";
        public static string CATEGORY_PATH = "/sitecore/content/meta-data/lookups/document/category";
        public static string SOURCE_PATH = "/sitecore/content/meta-data/lookups/document/source";
        public static string MANAGER_PATH = "/sitecore/content/Shared Content/Investments/Managers/*";
        public static string DESCRIPTION_PATH = "/sitecore/content/Shared Content/Investments/Description/*";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Dictionary<string, ID> GetLookupIds(Database db, string path)
        {
            Dictionary<string, Sitecore.Data.ID> dictResults = new Dictionary<string, ID>();
            Item rootItem = db.GetItem(path);

            if (rootItem != null)
            {
                foreach (Item lookupItem in rootItem.GetChildren())
                {
                    dictResults.Add(lookupItem.Name, lookupItem.ID); // note that this node type is not localizable
                }
            }

            return dictResults;
        }

        /// <summary>
        /// Used to get strategist list.
        /// </summary>
        /// <returns>Returns collection of strategist.</returns>
        public static Dictionary<string, string> GetStrategist(Database db, string strategistPath)
        {
            Dictionary<string, string> dictStrategists = new Dictionary<string, string>();
            Item[] oStrategists = db.SelectItems(strategistPath);

            foreach (Item strategist in oStrategists)
            {
                if (!dictStrategists.ContainsKey(strategist["Code"]))
                {
                    dictStrategists.Add(strategist["Code"], strategist.Name);
                }
                else
                {
                    Log.Info(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:GetStrategist, strategist key already exists: {0}", strategist["Code"]), typeof(ContentEditorHelper));
                }
            }

            return dictStrategists;
        }

        /// <summary>
        /// Used to get allocation approches.
        /// </summary>
        /// <returns>Returns collection of allocations.</returns>
        public static Dictionary<string, Item> GetAllocationApproches(Database db, string allocationPath)
        {
            Dictionary<string, Item> dictApproaches = new Dictionary<string, Item>();
            Item[] oApproaches = db.SelectItems(allocationPath);

            foreach (Item approach in oApproaches)
            {
                if (!dictApproaches.ContainsKey(approach["Code"]))
                {
                    dictApproaches.Add(approach["Code"], approach);
                }
                else
                {
                    Log.Info(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:GetAllocationApproches, allocation key already exists: {0}", approach["Code"]), typeof(ContentEditorHelper));
                }
            }

            return dictApproaches;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="managerPath"></param>
        /// <returns></returns>
        public static Dictionary<string, Item> GetManagers(Database db, string managerPath)
        {
            Dictionary<string, Item> dictManagers = new Dictionary<string, Item>();
            Item[] oManagers = db.SelectItems(managerPath);

            foreach (Item manager in oManagers)
            {
                if (!dictManagers.ContainsKey(manager["Code"]))
                {
                    dictManagers.Add(manager["Code"], manager);
                }
                else
                {
                    Log.Info(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:GetManagers, manager key already exists: {0}", manager["Code"]), typeof(ContentEditorHelper));
                }
            }

            return dictManagers;
        }


        public static Dictionary<string, Item> GetDescriptions(Database db, string descriptionPath)
        {
            Dictionary<string, Item> dictDescriptions = new Dictionary<string, Item>();
            Item[] oDescription = db.SelectItems(descriptionPath);

            foreach (Item approach in oDescription)
            {
                if (!dictDescriptions.ContainsKey(approach["Code"]))
                {
                    dictDescriptions.Add(approach["Code"], approach);
                }
                else
                {
                    Log.Info(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:GetDescriptions, description key already exists: {0}", approach["Code"]), typeof(ContentEditorHelper));
                }
            }

            return dictDescriptions;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        /// <param name="importPath"></param>
        /// <param name="contentPath"></param>
        /// <returns></returns>
        public static MediaItem UploadContent(Database db, string importPath, string contentPath)
        {
            MediaItem mediaItem = null;

            Sitecore.Resources.Media.MediaCreatorOptions options = new Sitecore.Resources.Media.MediaCreatorOptions();
            options.Database = db;
            options.Language = Sitecore.Globalization.Language.Parse(Sitecore.Configuration.Settings.DefaultLanguage);
            options.Versioned = Sitecore.Configuration.Settings.Media.UploadAsVersionableByDefault;
            options.Destination = contentPath;
            options.KeepExisting = false; // force replacement

            try
            {
                Sitecore.Resources.Media.MediaCreator creator = new Sitecore.Resources.Media.MediaCreator();
                mediaItem = creator.CreateFromFile(importPath, options);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:UploadContent, exception creating media item {0}, skipping file. Exception: {1}", importPath.Substring(importPath.LastIndexOf("/")), ex.ToString()), typeof(ContentEditorHelper));
                SitecoreExt.Utilities.ContentEditorHelper.MoveFile(importPath, Path.Combine(importPath + ".failed"));
            }

            return mediaItem;
        }

        public static string CleanNodeName(string nodeName)
        {
            return nodeName.Replace(".", "").Replace(",", "");
        }

        /// <summary>
        /// This will use the Sitecore API to publish all pending changes in the content tree
        /// </summary>
        /// <returns></returns>
        public static bool PublishSite(string item)
        {
            bool bOk = false;

            try
            {
                string sourceDatabase = Sitecore.Configuration.Settings.GetSetting("SourceDatabase");
                string targetDatabase = Sitecore.Configuration.Settings.GetSetting("TargetDatabase");

                return SitecorePublisher.PublishItem(Sitecore.Configuration.Factory.GetDatabase(sourceDatabase).GetItem(item), true, Sitecore.Configuration.Factory.GetDatabase(sourceDatabase), Sitecore.Configuration.Factory.GetDatabase(targetDatabase), false);

            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:PublishSite, exception: {0}", ex.ToString()), ex, typeof(ContentEditorHelper));
            }

            return bOk;
        }

        public static bool RebuildIndex(string indexName)
        {
            bool bOk = false;

            try
            {
                var index = Sitecore.ContentSearch.ContentSearchManager.GetIndex(indexName);
                index.Rebuild();

                bOk = true;
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Importers.Utilities.ContentEditorHelper:RebuildIndex, index {0}, exception: {1}", indexName, ex.ToString()), ex, typeof(ContentEditorHelper));
            }

            return bOk;
        }

        /// <summary>
        /// This method will try to move the requested file to a new name. If the destination file already exists 
        /// it will append a unique string based on the time and date.
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="destinationName"></param>
        /// <returns></returns>
        public static bool MoveFile(string sourceName, string destinationName)
        {
            bool bStatus = false;

            try
            {
                if (!File.Exists(sourceName))
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:MoveFile, source: {0}, destination: {1}",
                        sourceName, destinationName), typeof(ContentEditorHelper));
                }
                else if (File.Exists(destinationName))
                {
                    string postFix = DateTime.Now.ToString("_MM-dd-yyyy-hh-mm-ss");
                    File.Move(sourceName, destinationName + postFix);
                }
                else
                {
                    File.Move(sourceName, destinationName);
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:MoveFile, source: {0}, destination: {1}, exception: {2}",
                    sourceName, destinationName, ex.ToString()), typeof(ContentEditorHelper));
                bStatus = false;
            }

            return bStatus;
        }

        /// <summary>
        /// Used to get file name from specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GetFile(string path, string prefix)
        {
            string fileName = string.Empty;
            try
            {
                if (Directory.Exists(path))
                {
                    foreach (string file in Directory.GetFiles(path))
                    {
                        if (file.Contains(prefix) && file.EndsWith(".pdf"))
                        {
                            fileName = file.Substring(file.LastIndexOf("\\") + 1).Replace(".pdf", "");
                            break;
                        }
                        else if (!file.EndsWith(".failed"))
                        {
                            Log.Info(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:GetFile, non-expense sheet file name ({0}) or extension (.pdf) found: {1}", prefix, file), typeof(ContentEditorHelper));
                            SitecoreExt.Utilities.ContentEditorHelper.MoveFile(file, file + ".failed");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:GetFile, path {0}, exception: {1}", path, ex.ToString()), typeof(ContentEditorHelper));
            }

            return fileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lstProcessedFiles"></param>
        /// <param name="lstFailedfiles"></param>
        /// <param name="importer"></param>
        /// <returns></returns>
        public static string GetImporterEmailBody(List<string> lstProcessedFiles, List<string> lstFailedfiles, string importer)
        {
            string processedTable = string.Empty;
            string failedTable = string.Empty;
            string finalResult = string.Empty;
            try
            {

                if (lstProcessedFiles.Count == 0 && lstFailedfiles.Count == 0)
                    return string.Empty;

                finalResult = "<hr width='100%' size='6' noshade color='#1eb3dd' />";
                finalResult += "<font face='arial' size='2'>";
                finalResult += "<p>File processing status for <b>" + importer + "</b></p>";

                if (lstProcessedFiles.Count > 0)
                {
                    processedTable = "<ul style='list-style-type: none;font-family:Arial;font-size:12;'>";

                    foreach (var file in lstProcessedFiles)
                    {
                        processedTable += "<li>" + file + "</li>";
                    }

                    processedTable += "</ul>";
                }

                if (lstFailedfiles.Count > 0)
                {
                    failedTable = "<ul style='list-style-type: none;font-family:Arial;font-size:12;'>";

                    foreach (var file in lstFailedfiles)
                    {
                        failedTable += "<li>" + file + "</li>";
                    }

                    failedTable += "</ul>";
                }

                if (processedTable != string.Empty)
                    finalResult += "<b>Processed Files</b>" + "<br/>" + processedTable + "<br/>";

                if (failedTable != string.Empty)
                    finalResult += "<b>Failed Files</b>" + "<br/>" + failedTable;


            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper: GetImporterEmailBody failed due to {0}", ex.Message), typeof(ContentEditorHelper));
            }
            return finalResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public static int GetLastDayOfMonth(int month, int year)
        {
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            return firstDayOfMonth.AddMonths(1).AddDays(-1).Day;
        }

        public static bool CheckSolutionExist(Sitecore.Data.Database database, string strategist, Item allocationItem, string strategy)
        {
            if (database == null)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:CheckSolutionExist, database cannot be null or empty"), typeof(ContentEditorHelper));
                return false;
            }
            else if (String.IsNullOrEmpty(strategist))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:CheckSolutionExist, strategy cannot be null or empty"), typeof(ContentEditorHelper));
                return false;
            }
            else if (String.IsNullOrEmpty(allocationItem["Title"]))
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper:CheckSolutionExist, allocation cannot be null or empty. Strategist: {0}", strategist), typeof(ContentEditorHelper));
                return false;
            }

            string sharedPath = "/sitecore/content/Shared Content/Investments/Strategists/#" + strategist + "#/#" + allocationItem.Name + "#/*[@@templatekey = 'solution' and @Code='" + strategy + "']";
            Item strategyItem = database.SelectSingleItem(sharedPath);

            if (strategyItem == null)
                return false;
            else
                return true;
        }

        public static string GetPublishStatusEmailBody(bool status)
        {
            string message = string.Empty;
            string finalBody = string.Empty;
            try
            {

                finalBody = "<hr width='100%' size='6' noshade color='#1eb3dd' />";
                finalBody += "<font face='arial' size='2'>";

                if (status)
                {
                    message = "<span style='font-family:Arial;font-size:12;'>";

                    message += "Hooray, publish operation completed successfully!!!";

                    message += "</span>";
                }
                else
                {
                    message = "<span style='font-family:Arial;font-size:12;'>";

                    message += "Oops, publish operation failed!!!";

                    message += "</span>";
                }

                if (message != string.Empty)
                    finalBody += "<br/>" + message + "<br/>";
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.Utilities.ContentEditorHelper: GetPublishStatusEmailBody failed due to {0}", ex.Message), typeof(ContentEditorHelper));
            }
            return finalBody;
        }

    }
}
