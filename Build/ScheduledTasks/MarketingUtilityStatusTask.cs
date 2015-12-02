using Genworth.SitecoreExt.Helpers;
using Genworth.SitecoreExt.MailSender;
using Genworth.SitecoreExt.MarketingCollateral;
using Sitecore.Diagnostics;
using System;
using System.Text;
using ServerLogic.SitecoreExt;
using System.Linq;
using System.Collections.Generic;
using Genworth.SitecoreExt.MarketingCollateral.Configuration;
using Sitecore.Data.Items;
using System.Collections.Specialized;

namespace Genworth.SitecoreExt.ScheduledTasks
{
    public class MarketingUtilityStatusTask : DailyTimeRangeTaskBase
    {

        private const string DEFAULT_DATABASE = "master";

        private const string className = "Genworth.SitecoreExt.ScheduledTasks.MarketingUtilityStatusTask";

        public MarketingUtilityStatusTask()
            : base(className)
        {
        }

        public void Execute()
        {

            if (!IsValidDateRange)
            {
                Log.Debug(string.Format("Genworth.SitecoreExt.ScheduledTasks.MarketingUtilityStatusTask: Out of Time Range; Start: {0}, End: {1}; Now: {2}", StartUpWindow, EndingWindow, DateTime.Now), this);
                return;
            }
            List<KeyValuePair<string, string>> summary = new List<KeyValuePair<string, string>>();

            try
            {
                Log.Debug(string.Format("Genworth.SitecoreExt.ScheduledTasks.MarketingUtilityStatusTask:Execute - start; Time Range; Start: {0}, End: {1}; Now: {2}", StartUpWindow, EndingWindow, DateTime.Now), this);

                var templates = ImporterHelper.GetExtendedDocumentTemplates();

                foreach (var template in templates)
                {
                    var items = GetItemsByTemplate(template);

                    Log.Debug(string.Format("Genworth.SitecoreExt.ScheduledTasks.MarketingUtilityStatusTask: Found {1} in {0}", template.TemplateFullName, items.Count()), this);

                    foreach (var item in items)
                    {
                        var createdDate = item.GetField("Statistics", "__created").GetDate();

                        var ownerEmailsDictionary = item.GetNameValueDictionary("Repository Owner Emails");
                        var documentStateDictionary = item.GetNameValueDictionary("Repository Document State").Where(f => (ownerEmailsDictionary.ContainsKey(f.Key)) && (f.Value == "P" || f.Value == "R"));

                        if (documentStateDictionary.Count() < 1)
                        {
                            continue;
                        }

                        var repositoriesDictionary = GetRepositoryDictionary(item.GetMultilistItems("Repositories"));

                        // Add different emails.
                        var senderEmails = GetDifferentEmails(documentStateDictionary, ownerEmailsDictionary);

                        // Add submiter in case it is not included
                        AddDifferentString(senderEmails, item.GetText("Submitter Name"));

                        // Greate mail body
                        StringBuilder body = new StringBuilder();
                        body.AppendFormat("<h2>'{0}' file has repository status pending for approval</h2>", item.DisplayName);
                        body.AppendFormat("<p><b>Date: </b>{0}<br />", item.GetField("Statistics", "__created").GetDate());
                        body.AppendFormat("<b>Path: </b>{0}</p>", item.Paths.FullPath);

                        foreach (var ds in documentStateDictionary)
                        {
                            string key = null;
                            repositoriesDictionary.TryGetValue(ds.Key, out key);
                            body.AppendFormat("<b>Repository</b> '{0}' <b>status</b> '{1}'<br />", key ?? ds.Key, ds.Value);
                        }

                        string sBody = body.ToString();
                        foreach (var email in senderEmails)
                        {
                            // Add to summary
                            summary.Add(new KeyValuePair<string, string>(email.ToLower(), sBody));
                        }


                    }
                }

                var groups = summary.ToLookup(f => f.Key);
                Log.Debug(string.Format("Genworth.SitecoreExt.ScheduledTasks.MarketingUtilityStatusTask: Sending {0} emails", groups.Count()), this);

                MailQProvider mailq = new MailQProvider();
                foreach (var item in groups)
                {
                    var toAddress = item.Key;
                    var bodys = item.Select(f => f.Value);

                    mailq.SendEmailWithOutTemplate(
                        "armando.gonzalez@assetmark.com",
                        null,
                        null,
                        "Marketing Collateral Distribution Utility - Repository status report " + toAddress,
                        string.Concat(bodys) + "<br/><br/>" + DateTime.Now
                   );

                }

            }
            catch (Exception ex)
            {
                Log.Error(String.Format("Genworth.SitecoreExt.ScheduledTasks.MarketingUtilityStatusTask:Execute failed due to {0}", ex.Message), this);
            }
            finally
            {
                Log.Debug("Genworth.SitecoreExt.ScheduledTasks.MarketingUtilityStatusTask: Ends", this);

            }
        }

        private Dictionary<string, string> GetRepositoryDictionary(List<Item> items)
        {
            var repositoriesDictionary = new Dictionary<string, string>();

            foreach (var repItem in items)
            {
                var value = repItem.DisplayName;
                var key = value.Replace('.', '_').Replace(' ', '_');
                repositoriesDictionary.Add(key, value);
            }
            return repositoriesDictionary;
        }

        private List<string> GetDifferentEmails(IEnumerable<KeyValuePair<string, string>> documentStateDictionary, Dictionary<string, string> ownerEmailsDictionary)
        {
            List<string> senderEmails = new List<string>();
            foreach (var rep in documentStateDictionary.Select(f => f.Key))
            {
                var email = ownerEmailsDictionary[rep];
                AddDifferentString(senderEmails, email);
            }
            return senderEmails;
        }

        private void AddDifferentString(List<string> list, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && !list.Contains(value))
            {
                list.Add(value);
            }
        }

        private IEnumerable<Item> GetItemsByTemplate(ExtendedMediaTemplate template)
        {
            var limitDate = DateTime.Now.AddDays(-3);
            var items = ItemHelper.GetChildrenContentItems(null, null, template.TemplateId, DEFAULT_DATABASE).Where(f =>
                f.Name != "__Standard Values" &&
                f.GetField("Statistics", "__created").GetDate() < limitDate
            );
            return items;
        }
    }
}
