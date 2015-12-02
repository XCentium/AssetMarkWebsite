using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using Sitecore.SecurityModel;
using Sitecore.Tasks;

using Genworth.SitecoreExt.Importers;
using Genworth.SitecoreExt.Utilities;
using Genworth.SitecoreExt.MailSender;

namespace Genworth.SitecoreExt.ScheduledTasks
{
    public class FactSheetImportTask
    {
        private static string FILE_IMPORT_PATH_KEY = @"FileImportPathFactSheets";
        private static string TO_ADDRESS = @"To_Address";
        private static string FILE_IMPORT_EMAIL_SUBJECT = @"ImporterStatusSubject";

        public void Execute()
        {
            try
            {
                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.FactSheetImportTask:Execute - start", this);

                MailQProvider mailq = new MailQProvider();
                FactSheetImporter importer = new FactSheetImporter();
                List<string> lstProcessedFiles = new List<string>();
                List<string> lstFailedfiles = new List<string>();
                string fileImportPath = Sitecore.Configuration.Settings.GetSetting(FILE_IMPORT_PATH_KEY);
                string subject = Sitecore.Configuration.Settings.GetSetting(FILE_IMPORT_EMAIL_SUBJECT);
                string toAddress = Sitecore.Configuration.Settings.GetSetting(TO_ADDRESS);
                bool bStatus = true;

                if (!String.IsNullOrEmpty(fileImportPath))
                {
                    string fileName;
                    while (importer.GetFile(fileImportPath, out fileName))
                    {
                        bool bFileStatus = importer.ImportFile(fileImportPath, fileName);

                        if (bFileStatus)
                        {
                            lstProcessedFiles.Add(fileName + ".pdf");
                        }
                        else
                        {
                            lstFailedfiles.Add(fileName + ".pdf");
                        }

                        bStatus = bStatus && bFileStatus;
                    }
                }

                if (!bStatus)
                {
                    Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.FactSheetImportTask: import files failed"), this);
                }
                else
                {
                    if (lstProcessedFiles.Count > 0)
                    {
                        bStatus = importer.PublishSite();

                        if (!bStatus)
                        {
                            Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.FactSheetImportTask: publish docs failed"), this);
                        }
                    }
                }

                string body = ContentEditorHelper.GetImporterEmailBody(lstProcessedFiles, lstFailedfiles, "Fact Sheet");
                if (body != string.Empty)
                {
                    mailq.SendEmailWithOutTemplate(toAddress, string.Empty, string.Empty, subject + " " + "Fact Sheet", body);
                }

                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.FactSheetImportTask:Execute - end", this);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.FactSheetImportTask:Execute failed due to {0}", ex.Message), this);
            }
        }
    }
}