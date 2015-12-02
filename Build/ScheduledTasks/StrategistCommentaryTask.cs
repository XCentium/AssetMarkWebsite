using Genworth.SitecoreExt.DocImporter;
using Genworth.SitecoreExt.MailSender;
using Genworth.SitecoreExt.Utilities;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.ScheduledTasks
{
    public class StrategistCommentaryTask
    {
        private static string FILE_IMPORT_PATH_KEY = @"FileImportPathStrategistCommentary";
        private static string TO_ADDRESS = @"To_Address";
        private static string FILE_IMPORT_EMAIL_SUBJECT = @"ImporterStatusSubject";

        public void Execute()
        {
            try
            {
                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.StrategistCommentaryTask:Execute - start", this);

                MailQProvider mailq = new MailQProvider();
                StrategistCommentaryImporter importer = new StrategistCommentaryImporter();
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
                    Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.StrategistCommentaryTask: import files failed"), this);
                }
                else
                {
                    if (lstProcessedFiles.Count > 0)
                    {
                        bStatus = importer.PublishSite();
                        if (!bStatus)
                        {
                            Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.StrategistCommentaryTask: publish docs failed"), this);
                        }
                    }
                }

                string body = ContentEditorHelper.GetImporterEmailBody(lstProcessedFiles, lstFailedfiles, "Strategist Commentary");
                if (body != string.Empty)
                {
                    mailq.SendEmailWithOutTemplate(toAddress, string.Empty, string.Empty, subject + " " + "Strategist Commentary", body);
                }

                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.StrategistCommentaryTask:Execute - end", this);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.StrategistCommentaryTask:Execute failed due to {0}", ex.Message), this);
            }
        }

    }
}
