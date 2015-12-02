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

using Genworth.SitecoreExt.Importers;
using Genworth.SitecoreExt.MailSender;
using Genworth.SitecoreExt.Utilities;

namespace Genworth.SitecoreExt.ScheduledTasks
{
    public class MonthlyPerformanceImportTask
    {
        private static string FILE_IMPORT_PATH_KEY = @"FileImportPathMonthlyPerformance";
        private static string TO_ADDRESS = @"To_Address";
        private static string FILE_IMPORT_EMAIL_SUBJECT = @"ImporterStatusSubject";

        public void Execute()
        {
            try
            {
                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.MonthlyPerformanceImportTask:Execute - start", this);

                MailQProvider mailq = new MailQProvider();
                MonthlyPerformanceImporter importer = new MonthlyPerformanceImporter();
                List<string> lstProcessedFiles = new List<string>();
                List<string> lstFailedfiles = new List<string>();
                string fileImportPath = Sitecore.Configuration.Settings.GetSetting(FILE_IMPORT_PATH_KEY);
                string toAddress = Sitecore.Configuration.Settings.GetSetting(TO_ADDRESS);
                string subject = Sitecore.Configuration.Settings.GetSetting(FILE_IMPORT_EMAIL_SUBJECT);
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
                    Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.MonthlyPerformanceImportTask: import files failed"), this);
                }
                else
                {
                    if (lstProcessedFiles.Count > 0)
                    {
                        bStatus = importer.PublishSite();
                        if (!bStatus)
                        {
                            Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.MonthlyPerformanceImportTask: publish docs failed"), this);
                        }
                    }
                }

                string body = ContentEditorHelper.GetImporterEmailBody(lstProcessedFiles, lstFailedfiles, "Monthly Performance");
                if (body != string.Empty)
                {
                    mailq.SendEmailWithOutTemplate(toAddress, string.Empty, string.Empty, subject + " " + "Monthly Performance", body);
                }

                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.MonthlyPerformanceImportTask:Execute - end", this);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.MonthlyPerformanceImportTask:Execute failed due to {0}", ex.Message), this);
            }
        }
    }
}
