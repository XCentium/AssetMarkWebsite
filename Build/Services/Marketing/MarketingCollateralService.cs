using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ServiceModel.Web;

using Sitecore;
using Sitecore.Links;
using Sitecore.Data.Items;
using Sitecore.Security.Accounts;

using Genworth.SitecoreExt.Helpers;
using Genworth.SitecoreExt.Marketing;
using Genworth.SitecoreExt.MarketingCollateral;
using Genworth.SitecoreExt.MarketingCollateral.Entities;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.MailSender;

namespace AssetMark.SitecoreExt.Services.Marketing
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MarketingCollateralService : IMarketingCollateralService
    {
        private const string DEFAULT_DATABASE = "master";

        public MarketingCollateralRootDataItem GetDocumentList(string templateIds)
        {
            return GetDocumentListWithReferences(templateIds, DEFAULT_DATABASE);
        }

        public MarketingCollateralRootDataItem GetDocumentEntry(string itemId)
        {
            return GetDocumentEntryWithReferences(itemId, DEFAULT_DATABASE);
        }

        public MarketingCollateralRootDataItem GetDocumentListFromDatabase(string templateIds, string database)
        {
            return GetDocumentListWithReferences(templateIds, database);
        }

        public MarketingCollateralRootDataItem GetDocumentEntryFromDatabase(string itemId, string database)
        {
            return GetDocumentEntryWithReferences(itemId, database);
        }

        public bool UpdateRepositoryState(string itemId, string repositoryName, string value)
        {
            return UpdateDocumentRepositoryState(itemId, repositoryName, value, DEFAULT_DATABASE);
        }

        public bool UpdateRepositoryStateFromDatabase(string itemId, string repositoryName, string value, string database)
        {
            return UpdateDocumentRepositoryState(itemId, repositoryName, value, database);
        }

        private bool UpdateDocumentRepositoryState(string itemId, string repositoryName, string value, string database)
        {
            try
            {
                using (new Sitecore.SecurityModel.SecurityDisabler())
                {
                    var item = ItemHelper.GetItemById(itemId, database);

                    item.Editing.BeginEdit();

                    var documentStateField = item.GetField("Marketing Collateral Distribution", "Repository Document State");
                    if (documentStateField != null)
                    {
                        var collection = Sitecore.Web.WebUtil.ParseUrlParameters(documentStateField.Value);
                        collection[repositoryName] = value;
                        var stateValues = new List<string>();
                        collection.AllKeys.ToList().ForEach(key => stateValues.Add(string.Format("{0}={1}", Sitecore.Web.WebUtil.UrlEncode(key), Sitecore.Web.WebUtil.UrlEncode(string.IsNullOrWhiteSpace(collection[key]) ? "N" : collection[key]))));
                        documentStateField.Value = string.Join("&", stateValues.ToArray());
                    }

                    item.Editing.EndEdit();

                    MailQProvider mailq = new MailQProvider();

                    var toAddress = item.GetText("Submitter Name");
                    var emailDic = item.GetNameValueDictionary("Repository Owner Emails");
                    var RepDic = item.GetNameValueDictionary("Download Name");
                    string ccAdress = null;
                    string FileName = null;
                    emailDic.TryGetValue(repositoryName, out ccAdress);
                    RepDic.TryGetValue(repositoryName, out FileName);

                    mailq.SendEmailWithOutTemplate(
                        toAddress,
                        null,
                        ccAdress,
                        "Marketing Collateral Distribution - Status Updated",
                        string.Format("New state for '{0}' file. '{1}' Repository State changed to {2}", FileName, repositoryName.Replace("_", " "), value)
                   );
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(string.Format("Error while updating document's repository state value. ItemId: {0}, repository: {1}, value: {2}", itemId ?? "null", repositoryName ?? "null", value ?? "null"), ex, this);
            }
            return true;
        }

        private MarketingCollateralRootDataItem GetDocumentListWithReferences(string templateIds, string database)
        {
            List<MarketingCollateralDocumentItem> list = new List<MarketingCollateralDocumentItem>();

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                string[] templates = templateIds.Split(',');
                foreach (string template in templates)
                {
                    var childrenList = GetItemsData(template, database);
                    list.AddRange(childrenList);
                }
            }

            return new MarketingCollateralRootDataItem() { Items = list.ToArray() };
        }

        private MarketingCollateralRootDataItem GetDocumentEntryWithReferences(string itemId, string database)
        {
            var singleItem = new MarketingCollateralDocumentItem();

            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                singleItem = GetItemData(itemId, database);
            }

            return new MarketingCollateralRootDataItem() { Items = new MarketingCollateralDocumentItem[] { singleItem } };
        }

        private List<MarketingCollateralDocumentItem> GetItemsData(string childrenTemplateId, string database)
        {
            List<MarketingCollateralDocumentItem> itemList = new List<MarketingCollateralDocumentItem>();

            var items = ItemHelper.GetChildrenContentItems(null, null, childrenTemplateId, database);

            foreach (var item in items)
            {
                var mcdItem = BuildMCDItem(item);
                if (mcdItem != null)
                {
                    itemList.Add(mcdItem);
                }
            }
            return itemList;
        }

        private MarketingCollateralDocumentItem GetItemData(string itemId, string database)
        {
            var item = ItemHelper.GetItemById(itemId, database);
            var mcdItem = BuildMCDItem(item);

            // if null, return default empty MCD item
            return mcdItem ?? new MarketingCollateralDocumentItem();
        }

        private MarketingCollateralDocumentItem BuildMCDItem(Item item)
        {
            if (item.Name == "__Standard Values")
            {
                // We filter out standard values content items
                return null;
            }

            var dataItem = new MarketingCollateralDocumentItem()
            {
                Id = item.ID.ToGuid().ToString(),
                FileName = item.Name,
                DisplayName = item.Fields["Display Download Name"].Value,
                FormNumber = item.Fields["Form Number"].Value,
                ComplianceNumber = item.Fields["Compliance Number"].Value
            };

            string documentStateValues = item.Fields["Repository Document State"].Value;
            var collection = Sitecore.Web.WebUtil.ParseUrlParameters(documentStateValues, '&');
            var dict = new Dictionary<string, string>();
            collection.AllKeys.ToList().ForEach(key => dict[key] = collection[key]);
            dataItem.RepositoryParameters = dict;

            return dataItem;
        }
    }
}
