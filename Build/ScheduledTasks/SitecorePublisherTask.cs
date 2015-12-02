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
    public class SitecorePublisherTask : DailyTimeRangeTaskBase
    {

        private const string className = "Genworth.SitecoreExt.ScheduledTasks.SitecorePublisherTask";

        public SitecorePublisherTask()
            : base(className)
        {
        }

        public void Execute()
        {
            if (!IsValidDateRange)
            {
                Log.Debug(string.Format("Genworth.SitecoreExt.ScheduledTasks.SitecorePublisherTask: Out of Time Range; Start: {0}, End: {1}; Now: {2}", StartUpWindow, EndingWindow, DateTime.Now), this);
                return;
            }

            try
            {
                MailQProvider mailq = new MailQProvider();
                string assetmarkNode = Sitecore.Configuration.Settings.GetSetting("AssetMarkNode");
                string sourceDatabase = Sitecore.Configuration.Settings.GetSetting("SourceDatabase");
                string targetDatabase = Sitecore.Configuration.Settings.GetSetting("TargetDatabase");
                string subject = Sitecore.Configuration.Settings.GetSetting("Publish_Status_Subject");
                string toAddress = Sitecore.Configuration.Settings.GetSetting("Publish_Status_To_Address");
                bool success = false;

                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.SitecorePublisherTask:Execute - start", this);

                success = SitecorePublisher.PublishItem(Sitecore.Configuration.Factory.GetDatabase(sourceDatabase).GetItem(assetmarkNode), true, Sitecore.Configuration.Factory.GetDatabase(sourceDatabase), Sitecore.Configuration.Factory.GetDatabase(targetDatabase), false);

                if (success)
                {
                    string body = ContentEditorHelper.GetPublishStatusEmailBody(success);
                    if (body != string.Empty)
                    {
                        mailq.SendEmailWithOutTemplate(toAddress, string.Empty, string.Empty, subject, body);
                    }
                }

                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.SitecorePublisherTask:Execute - end", this);
            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.SitecorePublisherTask:Execute failed due to {0}", ex.Message), this);
            }
        }

    }
}
