using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genworth.SitecoreExt.MarketingCollateral.Configuration;
using Genworth.SitecoreExt.MarketingCollateral.Entities;
using ServerLogic.SitecoreExt;
using System.Reflection;
using System.Globalization;
using System.Web.Configuration;
using Sitecore.Data.Items;
using System.IO;
using Sitecore.SecurityModel;
using Genworth.SitecoreExt.Pipelines.Upload;
using Genworth.SitecoreExt.Helpers;
using AssetMark.SitecoreExt;
using Sitecore.Data;
using Sitecore.Globalization;
using Sitecore.Data.Managers;
using Genworth.SitecoreExt.MailSender;
using Genworth.SitecoreExt.Utilities;
using System.Web;
using Sitecore.Data.Fields;
using System.Net;
using Salesforce.Common;
using Salesforce.Chatter;
using Salesforce.Chatter.Models;
using System.Net.Http;
using Salesforce.Common.Content;

namespace Genworth.SitecoreExt.MarketingCollateral
{
    public class ImporterHelper
    {
        private const string MediaFileValueFormat = "<file mediaid=\"{0}\" src=\"~/{1}.ashx\" />";
        private static string MarketingUtilityUsersFolderId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingUtilityUsersFolderId);
        private static string MarketingUtilityUsersTemplateId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingUtilityUsersTemplateId);

        public static MarketingCollateralResponse UploadDocument(MarketingCollateralItem mcdItem)
        {
            MarketingCollateralResponse response = new MarketingCollateralResponse();

            Item uploadedItem = null;
            try
            {
                using (new SecurityDisabler())
                {
                    uploadedItem = AddFile(mcdItem, response);
                    if (uploadedItem != null)
                    {
                        var settings = ImporterHelper.LoadRepositorySettings(HttpContext.Current.Request);
                        ChangeTemplate(uploadedItem, response);
                        UploadToMarcomCentralFTP(mcdItem, response, settings);
                        UploadToSalesforce(mcdItem, response, settings);
                        UpdateExtendedDocumentFields(uploadedItem, mcdItem, response);
                        PublishItem(uploadedItem, response);
                        SendEmailNotification(mcdItem, response);
                        ClosePendingFileStream(mcdItem);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Add("Unable to process upload correctly. Please see Marketing Collateral Sitecore log for details.", ResponseStatus.Error);
                MarketingCollateralLog.Log.Error(string.Format("Marketing Collateral ImporterHelper :: Unable to process upload correctly, filename: {0}, uploadPath: {1}", mcdItem.FileName, mcdItem.UploadPath), ex);
            }

            return response;
        }

        private static void ClosePendingFileStream(MarketingCollateralItem mcdItem)
        {
            mcdItem.File.Close();
        }

        private static void UploadToSalesforce(MarketingCollateralItem mcdItem, MarketingCollateralResponse response, List<BaseRepository> settings)
        {
            MemoryStream stream = null;
            try
            {
                var repositoryData = mcdItem.RepositoryData[Genworth.SitecoreExt.Constants.Marketing.RepositorySalesforce];
                if (repositoryData.IsSelected)
                {
                    SalesforceRepository repository = settings.Find(s => s.Name == Genworth.SitecoreExt.Constants.Marketing.RepositorySalesforce) as SalesforceRepository;
                    if (repository != null)
                    {
                        var _auth = new AuthenticationClient();
                        _auth.UsernamePasswordAsync(repository.ConsumerKey, repository.ConsumerSecret, repository.Username, repository.Password, repository.TokenRequestEndpointUrl).Wait();

                        const string apiVersion = "v34.0";
                        var _serviceClient = new ChatterClient(_auth.InstanceUrl, _auth.AccessToken, apiVersion).ServiceClient;

                        if (_serviceClient != null)
                        {

                            if (mcdItem.IsReplacing && !string.IsNullOrWhiteSpace(mcdItem.SalesforceDocumentId))
                            {
                                try
                                {
                                    var deleteDocumentResponse = _serviceClient.HttpDeleteAsync("chatter/files/" + mcdItem.SalesforceDocumentId);
                                    var documentDeleted = deleteDocumentResponse.Result;
                                    if (!documentDeleted)
                                    {
                                        MarketingCollateralLog.Log.Warn("Marketing Collateral ImporterHelper :: Could not delete previous document version from Salesforce. File Id: " + mcdItem.SalesforceDocumentId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Salesforce error while deleting document from Chatter Files. File Id: " + mcdItem.SalesforceDocumentId, ex);
                                }
                            }

                            // copy the file stream
                            stream = new MemoryStream();
                            mcdItem.File.Position = 0;
                            mcdItem.File.CopyTo(stream);
                            stream.Position = 0;

                            var messageSegment = new MessageSegmentInput()
                            {
                                Text = mcdItem.ChatterMessage,
                                Type = "Text"
                            };

                            var body = new MessageBodyInput()
                            {
                                MessageSegments = new List<MessageSegmentInput> { messageSegment }
                            };

                            // post to a group subject Id
                            var feedElementGroup = new FeedElement()
                            {
                                Body = body,
                                SubjectId = repository.GroupSubjectId,
                                FeedElementType = "FeedItem"
                            };

                            var chatterDocumentItem = new ChatterDocument()
                            {
                                Title = repositoryData.DownloadName
                            };

                            var metaDataObject = new Salesforce.Common.Models.MultipartFormDataObject()
                            {
                                InputObject = chatterDocumentItem,
                                MimeType = "application/json",
                                Name = "file"
                            };
                            metaDataObject.ContentBuilder = new StringContentBuilder(metaDataObject);

                            var fileObject = new Salesforce.Common.Models.MultipartFormDataObject()
                            {
                                InputObject = stream,
                                MimeType = "application/octet-stream",
                                Name = "fileData",
                                FileName = mcdItem.FileName
                            };
                            fileObject.ContentBuilder = new StreamContentBuilder(fileObject);

                            var objectList = new List<Salesforce.Common.Models.MultipartFormDataObject>();
                            objectList.Add(metaDataObject);
                            objectList.Add(fileObject);

                            // Upload multipart object to Salesforce, into context user's Files section
                            var documentItemResponse = _serviceClient.HttpPostMultiPartFormDataAsync<ChatterDocument>(objectList, "chatter/users/me/files", null);
                            var documentItem = documentItemResponse.Result;
                            var documentItemId = documentItem.Id;
                            stream.Close();

                            // proceed to share document
                            //if (!string.IsNullOrWhiteSpace(repository.LibrarySubjectId))
                            //{
                            //    FileShareList fileShareList = new FileShareList();
                            //    fileShareList.Shares = new List<FileShareItem>();
                            //    fileShareList.Shares.Add(new FileShareItem() { Id = repository.LibrarySubjectId, SharingType = "V" });
                            //    var sharedFile = _serviceClient.HttpPostAsync<object>(fileShareList, string.Format("chatter/files/{0}/file-shares", documentItemId));
                            //    var sharedResultId = sharedFile.Result;
                            //}

                            // post to group subject Id
                            feedElementGroup.Capabilities = new Capabilities() { Content = new CapabilitiesContent() { ContentDocumentId = documentItemId } };
                            var feedItem = _serviceClient.HttpPostAsync<FeedItem>(feedElementGroup, "chatter/feed-elements");
                            var feedId = feedItem.Result.Id;
                            mcdItem.SalesforceDocumentId = documentItemId;
                            mcdItem.SalesforceFeedItemId = feedId;

                            response.Add("Chatter post message sent and file uploaded successfully to Salesforce.", ResponseStatus.Success);
                            MarketingCollateralLog.Log.Info("Marketing Collateral ImporterHelper :: Chatter post was sent and file uploaded successfully to Salesforce. File Url: " + _auth.InstanceUrl + documentItem.DownloadUrl);
                        }
                    }
                    else
                    {
                        response.Add("Unable to find settings for Salesforce repository.", ResponseStatus.Warning);
                        MarketingCollateralLog.Log.Warn("Marketing Collateral ImporterHelper :: Unable to find settings for Salesforce repository.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (stream != null)
                {
                    stream.Close();
                }

                response.Add("Unable to upload file to Salesforce. Please see Marketing Collateral Sitecore log for details.", ResponseStatus.Error);
                MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Unable to upload file to Salesforce.", ex);
            }        
        }

        private static void UploadToMarcomCentralFTP(MarketingCollateralItem mcdItem, MarketingCollateralResponse response, List<BaseRepository> settings)
        {
            MemoryStream stream = null;
            var repositoryData = mcdItem.RepositoryData[Genworth.SitecoreExt.Constants.Marketing.RepositoryMktPortal];
            if (repositoryData.IsSelected)
            {
                try
                {
                    MarketingPortalRepository repository = settings.Find(s => s.Name == Genworth.SitecoreExt.Constants.Marketing.RepositoryMktPortal) as MarketingPortalRepository;
                    if (repository != null)
                    {
                        FtpClient ftpClient = new FtpClient();
                        var fullUri = new Uri(new Uri(repository.FTPHost, UriKind.Absolute), new Uri(repository.InitialFolder, UriKind.Relative)).ToString();
                        var fileName = string.Format("{0}{1}", repositoryData.DownloadName, Path.GetExtension(mcdItem.FileName));
                        var remoteAddress = string.Format("{0}/{1}", fullUri, fileName);
                        var request = ftpClient.CreateRequest(remoteAddress, repository.FTPUsername, repository.FTPPassword, repository.EnableSsl,
                            usePassive: false, useBinary: true, methodName: WebRequestMethods.Ftp.UploadFile);

                        stream = new MemoryStream();
                        mcdItem.File.Position = 0;
                        mcdItem.File.CopyTo(stream);
                        stream.Position = 0;

                        ftpClient.UploadFile(request, stream);
                        stream.Close();
                        response.Add("File was uploaded successfully to Marcom Central FTP.", ResponseStatus.Success);
                        MarketingCollateralLog.Log.Info("Marketing Collateral ImporterHelper :: File was uploaded successfully to Marcom Central FTP. Request: " + request.RequestUri.ToString());
                    }
                    else
                    {
                        response.Add("Unable to find settings for Marcom Central repository.", ResponseStatus.Warning);
                        MarketingCollateralLog.Log.Warn("Marketing Collateral ImporterHelper :: Unable to find settings for Marcom Central repository.");
                    }
                }
                catch (Exception ex)
                {
                    response.Add("Unable to upload file to Marcom Central through FTP. Please see Marketing Collateral Sitecore log for details.", ResponseStatus.Error);
                    MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Unable to upload file to Marcom Central through FTP.", ex);
                    if (stream != null)
                    {
                        stream.Close();
                    }
                }
            }
        }

        private static void SendEmailNotification(MarketingCollateralItem mcdItem, MarketingCollateralResponse response)
        {
            if (mcdItem.SendNotification)
            {
                try
                {
                    MailQProvider provider = new MailQProvider();
                    string[] emailAddresses = mcdItem.Emails.Split(new char[] {',', ';', '\n', ' '}, StringSplitOptions.RemoveEmptyEntries);
                    string subject = "Marketing Collateral Distribution - " + (mcdItem.IsReplacing ? "REPLACE" : "UPLOAD");
                    string fromAddress = Sitecore.Configuration.Settings.GetSetting("From_Address_KeyName");
                    string fromAddressName = "Sitecore-Notification";
                    provider.SendEmailWithOutTemplate(fromAddress, fromAddressName, string.Join(",", emailAddresses), null, null, subject, EmailBodyTextToHtml(mcdItem.EmailBody));
                    response.Add("Email notification has been queued successfully.", ResponseStatus.Success);
                }
                catch (Exception ex)
                {
                    response.Add("Unable to send email notification. Please see Marketing Collateral Sitecore log for details.", ResponseStatus.Error);
                    MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Unable to push email notification to MailQ system.", ex);
                }
            }
        }

        private static string EmailBodyTextToHtml(string bodyText)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<font face='arial' size='2'>");
            var textLines = bodyText.Split('\n');
            foreach (var textline in textLines)
            {
                if (string.IsNullOrWhiteSpace(textline))
                {
                    sb.AppendLine("<br />");
                }
                else
                {
                    sb.Append("<p>").Append(textline).AppendLine("</p>");
                }
            }
            sb.AppendLine("</font>");

            return sb.ToString();
        }

        private static void PublishItem(Sitecore.Data.Items.Item item, MarketingCollateralResponse response)
        {
            try
            {
                if (item != null)
                {
                    // The publishOptions determine the source and target database,
                    // the publish mode and language, and the publish date
                    Sitecore.Publishing.PublishOptions publishOptions =
                      new Sitecore.Publishing.PublishOptions(item.Database,
                                                             Database.GetDatabase("web"),
                                                             Sitecore.Publishing.PublishMode.SingleItem,
                                                             item.Language,
                                                             System.DateTime.Now);  // Create a publisher with the publishoptions

                    Sitecore.Publishing.Publisher publisher = new Sitecore.Publishing.Publisher(publishOptions);

                    // Choose where to publish from
                    publisher.Options.RootItem = item;

                    // Publish children as well?
                    publisher.Options.Deep = true;

                    // Do the publish!
                    publisher.Publish();

                    string msg = string.Format("Media item {0} has been succesfully published in Sitecore.", item.Name);
                    response.Add(msg, ResponseStatus.Success);
                    MarketingCollateralLog.Log.Info(msg);
                }
                else
                {
                    string msg = "Media item has NOT been published in Sitecore.";
                    response.Add(msg, ResponseStatus.Warning);
                    MarketingCollateralLog.Log.Info(string.Format("{0}. Item is null.", msg));
                }
            }
            catch (Exception ex)
            {
                response.Add("Unable to publish file, please see error details in Marketing Collateral Sitecore log.", ResponseStatus.Error);
                MarketingCollateralLog.Log.Error(string.Format("Unable to publish file to web database. Item: {0}", item.Paths.FullPath), ex);

            }
        }

        private static void UpdateContentItemDocumentReference(Item uploadedItem, MarketingCollateralItem item, MarketingCollateralResponse response)
        {
            var documentTemplateList = GetValidDocumentBaseTemplates();
            string key = string.Empty;
            string contentLocation = string.Empty;
            if (!string.IsNullOrWhiteSpace(contentLocation))
            {
                var contentItem = ItemHelper.GetItemByPath(contentLocation, uploadedItem.Database);
                if (contentItem != null)
                {
                    var itemBaseTemplates = contentItem.Template.BaseTemplates.Select(t => t.Name).ToList();
                    itemBaseTemplates.Add(contentItem.Template.Name);
                    var documentTemplate = documentTemplateList.Where(t => itemBaseTemplates.Contains(t.TemplateName)).FirstOrDefault();
                    if (documentTemplate != null)
                    {
                        contentItem.Editing.BeginEdit();
                        var documentFileField = contentItem.GetField(documentTemplate.SectionName, documentTemplate.FieldName);
                        if (documentFileField != null)
                        {
                            documentFileField.Value = string.Format(MediaFileValueFormat, uploadedItem.ID, uploadedItem.GetMediaURL());
                        }
                        else
                        {
                            var msg = "The selected content item does inherit from a valid template [" + documentTemplate + "], but doesn't have the field [" + documentTemplate.FieldName + "] under the section [" + documentTemplate.SectionName + "].";
                            //response.Add(msg, ResponseStatus.Warning);
                            MarketingCollateralLog.Log.Warn(msg);
                        }
                        contentItem.Editing.EndEdit();
                        //response.Add("Updated content item with new media item reference for " + key + " repository, path " + contentItem.Paths.FullPath, ResponseStatus.Success);
                        MarketingCollateralLog.Log.Info(string.Format("Marketing Collateral ImporterHelper :: Updated content item to reference new mediaItem: item path: {0}, mediaItem path {1}.", contentItem.Paths.FullPath, uploadedItem.Paths.FullPath));
                    }
                    else
                    {
                        string validTemplates = string.Join(",", documentTemplateList.Select(t => t.TemplateName).ToArray());
                        //response.Add("The selected content item does not inherit from any valid template, content item's template: " + contentItem.TemplateName, ResponseStatus.Warning);
                        var msg = "The only valid templates are: " + validTemplates;
                        //response.Add(msg, ResponseStatus.Warning);
                        MarketingCollateralLog.Log.Warn(msg);
                        MarketingCollateralLog.Log.Warn(string.Format("Marketing Collateral ImporterHelper :: The selected content item does not inherit from any valid template: item template: {0}, item path {1}, for repository {2}.", contentItem.TemplateName, contentLocation, key));
                    }
                }
                else
                {
                    response.Add("Unable to find content item for document reference update, item path " + contentLocation, ResponseStatus.Warning);
                    MarketingCollateralLog.Log.Warn(string.Format("Marketing Collateral ImporterHelper :: Unable to find content item for document reference update: item path {0}, for repository {1}.", contentLocation, key));
                }
            }
        }

        public static Dictionary<string, RepositoryData> BuildRepositoryCollection()
        {
            var dict = new Dictionary<string, RepositoryData>();
            dict.Add(Genworth.SitecoreExt.Constants.Marketing.RepositoryAMA, new RepositoryData()
            {
                Name = Genworth.SitecoreExt.Constants.Marketing.RepositoryAMA,
                KeyName = Constants.Marketing.RepositoryAMA.Replace(" ", "_")
            });
            dict.Add(Genworth.SitecoreExt.Constants.Marketing.RepositoryAssetMark, new RepositoryData()
            {
                Name = Genworth.SitecoreExt.Constants.Marketing.RepositoryAssetMark,
                KeyName = Constants.Marketing.RepositoryAssetMark.Replace(".", "_")
            });
            dict.Add(Genworth.SitecoreExt.Constants.Marketing.RepositoryEWM, new RepositoryData()
            {
                Name = Genworth.SitecoreExt.Constants.Marketing.RepositoryEWM,
                KeyName = Constants.Marketing.RepositoryEWM.Replace(".", "_")
            });
            dict.Add(Genworth.SitecoreExt.Constants.Marketing.RepositoryRCTools, new RepositoryData()
            {
                Name = Genworth.SitecoreExt.Constants.Marketing.RepositoryRCTools,
                KeyName = Constants.Marketing.RepositoryRCTools.Replace(" ", "_")
            });
            dict.Add(Genworth.SitecoreExt.Constants.Marketing.RepositoryMktPortal, new RepositoryData()
            {
                Name = Genworth.SitecoreExt.Constants.Marketing.RepositoryMktPortal,
                KeyName = Constants.Marketing.RepositoryMktPortal.Replace(" ", "_")
            });
            dict.Add(Genworth.SitecoreExt.Constants.Marketing.RepositorySalesforce, new RepositoryData()
            {
                Name = Genworth.SitecoreExt.Constants.Marketing.RepositorySalesforce,
                KeyName = Constants.Marketing.RepositorySalesforce.Replace(" ", "_")
            });
            return dict;
        }

        private static void UpdateExtendedDocumentFields(Item uploadedItem, MarketingCollateralItem item, MarketingCollateralResponse response)
        {
            bool updated = false;
            uploadedItem.Editing.BeginEdit();

            var formNumberField = uploadedItem.GetField("Marketing Collateral Distribution", "Form Number");
            if (formNumberField != null)
            {
                formNumberField.Value = item.FormNumber;
                updated = true;
            }

            var complianceNumberField = uploadedItem.GetField("Marketing Collateral Distribution", "Compliance Number");
            if (complianceNumberField != null)
            {
                complianceNumberField.Value = item.ComplianceNumber;
                updated = true;
            }

            var submitterNameField = uploadedItem.GetField("Marketing Collateral Distribution", "Submitter Name");
            if (submitterNameField != null)
            {
                submitterNameField.Value = item.SubmitterName;
            }

            var displayDownloadNameField = uploadedItem.GetField("Marketing Collateral Distribution", "Display Download Name");
            if (displayDownloadNameField != null)
            {
                displayDownloadNameField.Value = item.DownloadName;
            }

            var repCollection = BuildRepositoryCollection();
            foreach (var repositoryName in item.RepositoryData.Where(kv => kv.Value.IsSelected).Select(kv => kv.Key))
            {
                switch (repositoryName)
                {
                    case Constants.Marketing.RepositoryAMA:
                        repCollection[Constants.Marketing.RepositoryAMA].IsSelected = true;
                        repCollection[Constants.Marketing.RepositoryAMA].RepositoryLookupId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralAMALookupId);
                        repCollection[Constants.Marketing.RepositoryAMA].DocumentState = "P";
                        repCollection[Constants.Marketing.RepositoryAMA].DownloadName = item.RepositoryData[Constants.Marketing.RepositoryAMA].DownloadName ?? item.DownloadName;
                        repCollection[Constants.Marketing.RepositoryAMA].OwnerEmail = item.RepositoryData[Constants.Marketing.RepositoryAMA].OwnerEmail;
                        break;
                    case Constants.Marketing.RepositoryAssetMark:
                        repCollection[Constants.Marketing.RepositoryAssetMark].IsSelected = true;
                        repCollection[Constants.Marketing.RepositoryAssetMark].RepositoryLookupId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralAssetMarkLookupId);
                        repCollection[Constants.Marketing.RepositoryAssetMark].DocumentState = "P";
                        repCollection[Constants.Marketing.RepositoryAssetMark].DownloadName = item.RepositoryData[Constants.Marketing.RepositoryAssetMark].DownloadName ?? item.DownloadName;
                        repCollection[Constants.Marketing.RepositoryAssetMark].OwnerEmail = item.RepositoryData[Constants.Marketing.RepositoryAssetMark].OwnerEmail;
                        break;
                    case Constants.Marketing.RepositoryEWM:
                        repCollection[Constants.Marketing.RepositoryEWM].IsSelected = true;
                        repCollection[Constants.Marketing.RepositoryEWM].RepositoryLookupId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralEWMLookupId);
                        repCollection[Constants.Marketing.RepositoryEWM].DocumentState = "P";
                        repCollection[Constants.Marketing.RepositoryEWM].DownloadName = item.RepositoryData[Constants.Marketing.RepositoryEWM].DownloadName ?? item.DownloadName;
                        repCollection[Constants.Marketing.RepositoryEWM].OwnerEmail = item.RepositoryData[Constants.Marketing.RepositoryEWM].OwnerEmail;
                        break;
                    case Constants.Marketing.RepositoryRCTools:
                        repCollection[Constants.Marketing.RepositoryRCTools].IsSelected = true;
                        repCollection[Constants.Marketing.RepositoryRCTools].RepositoryLookupId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralRCToolsLookupId);
                        repCollection[Constants.Marketing.RepositoryRCTools].DocumentState = "P";
                        repCollection[Constants.Marketing.RepositoryRCTools].DownloadName = item.RepositoryData[Constants.Marketing.RepositoryRCTools].DownloadName ?? item.DownloadName;
                        repCollection[Constants.Marketing.RepositoryRCTools].OwnerEmail = item.RepositoryData[Constants.Marketing.RepositoryRCTools].OwnerEmail;
                        break;
                    case Constants.Marketing.RepositoryMktPortal:
                        repCollection[Constants.Marketing.RepositoryMktPortal].IsSelected = true;
                        repCollection[Constants.Marketing.RepositoryMktPortal].RepositoryLookupId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralMktPortalLookupId);
                        repCollection[Constants.Marketing.RepositoryMktPortal].DocumentState = "R";
                        repCollection[Constants.Marketing.RepositoryMktPortal].DownloadName = item.RepositoryData[Constants.Marketing.RepositoryMktPortal].DownloadName ?? item.DownloadName;
                        repCollection[Constants.Marketing.RepositoryMktPortal].OwnerEmail = item.RepositoryData[Constants.Marketing.RepositoryMktPortal].OwnerEmail;
                        break;
                    case Constants.Marketing.RepositorySalesforce:
                        repCollection[Constants.Marketing.RepositorySalesforce].IsSelected = true;
                        repCollection[Constants.Marketing.RepositorySalesforce].RepositoryLookupId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralSalesforceLookupId);
                        repCollection[Constants.Marketing.RepositorySalesforce].DocumentState = "P";
                        repCollection[Constants.Marketing.RepositorySalesforce].DownloadName = item.RepositoryData[Constants.Marketing.RepositorySalesforce].DownloadName ?? item.DownloadName;
                        repCollection[Constants.Marketing.RepositorySalesforce].OwnerEmail = item.RepositoryData[Constants.Marketing.RepositorySalesforce].OwnerEmail;
                        break;
                }
            }

            var selectedRepositoriesField = uploadedItem.GetField("Marketing Collateral Distribution", "Repositories");
            if (selectedRepositoriesField != null)
            {
                selectedRepositoriesField.Value = string.Join("|", repCollection
                    .Where(kv => kv.Value.IsSelected && !string.IsNullOrWhiteSpace(kv.Value.RepositoryLookupId))
                    .Select(kv => kv.Value.RepositoryLookupId).ToArray());
            }

            var documentStateField = uploadedItem.GetField("Marketing Collateral Distribution", "Repository Document State");
            if (documentStateField != null)
            {
                var collection = Sitecore.Web.WebUtil.ParseUrlParameters(documentStateField.Value);
                var stateValues = new List<string>();
                repCollection.Keys.ToList().ForEach(key => collection[repCollection[key].KeyName] = repCollection[key].DocumentState);
                collection.AllKeys.ToList().ForEach(key => stateValues.Add(string.Format("{0}={1}", Sitecore.Web.WebUtil.UrlEncode(key), Sitecore.Web.WebUtil.UrlEncode(string.IsNullOrWhiteSpace(collection[key]) ? "N" : collection[key]))));
                documentStateField.Value = string.Join("&", stateValues.ToArray());
            }

            var downloadNameField = uploadedItem.GetField("Marketing Collateral Distribution", "Download Name");
            if (downloadNameField != null)
            {
                var collection = Sitecore.Web.WebUtil.ParseUrlParameters(downloadNameField.Value);
                var downloadNamesValues = new List<string>();
                repCollection.Keys.ToList().ForEach(key => collection[repCollection[key].KeyName] = repCollection[key].DownloadName);
                collection.AllKeys.ToList().ForEach(key => downloadNamesValues.Add(string.Format("{0}={1}", Sitecore.Web.WebUtil.UrlEncode(key), Sitecore.Web.WebUtil.UrlEncode(collection[key] ?? string.Empty))));
                downloadNameField.Value = string.Join("&", downloadNamesValues.ToArray());
            }

            var ownerEmailField = uploadedItem.GetField("Marketing Collateral Distribution", "Repository Owner Emails");
            if (ownerEmailField != null)
            {
                var collection = Sitecore.Web.WebUtil.ParseUrlParameters(ownerEmailField.Value);
                var ownerEmailValues = new List<string>();
                repCollection.Keys.ToList().ForEach(key => collection[repCollection[key].KeyName] = repCollection[key].OwnerEmail);
                collection.AllKeys.ToList().ForEach(key => ownerEmailValues.Add(string.Format("{0}={1}", Sitecore.Web.WebUtil.UrlEncode(key), Sitecore.Web.WebUtil.UrlEncode(collection[key] ?? string.Empty))));
                ownerEmailField.Value = string.Join("&", ownerEmailValues.ToArray());
            }

            var commentField = uploadedItem.GetField("Marketing Collateral Distribution", "Comment");
            if (commentField != null)
            {
                commentField.Value = item.Comment;
            }

            var ewmAdditionalInfoField = uploadedItem.GetField("Marketing Collateral Distribution", "eWM Additional Info");
            if (ewmAdditionalInfoField != null)
            {
                ewmAdditionalInfoField.Value = item.EWMRepositoryAdditionalInfo;
            }

            var assetMarkAdditionalInfoField = uploadedItem.GetField("Marketing Collateral Distribution", "AssetMark Additional Info");
            if (assetMarkAdditionalInfoField != null)
            {
                assetMarkAdditionalInfoField.Value = item.AssetMarkRepositoryAdditionalInfo;
            }

            var marketingPortalAdditionalInfoField = uploadedItem.GetField("Marketing Collateral Distribution", "Marketing Portal Additional Info");
            if (marketingPortalAdditionalInfoField != null)
            {
                marketingPortalAdditionalInfoField.Value = item.MarketingPortalRepositoryAdditionalInfo;
            }

            var salesforceFeedItemIdField = uploadedItem.GetField("Marketing Collateral Distribution", "SFDC FeedItem Id");
            if (salesforceFeedItemIdField != null)
            {
                salesforceFeedItemIdField.Value = item.SalesforceFeedItemId;
            }

            var salesforceDocumentIdField = uploadedItem.GetField("Marketing Collateral Distribution", "SFDC Document Id");
            if (salesforceDocumentIdField != null)
            {
                salesforceDocumentIdField.Value = item.SalesforceDocumentId;
            }

            uploadedItem.Editing.EndEdit();

            if (updated)
            {
                //response.Add("Media item " + uploadedItem.Name + " has been correctly updated its extended fields with form values.", ResponseStatus.Success);
                MarketingCollateralLog.Log.Info(string.Format("Media item from path {0} was correctly updated its extended document fields for Marketing Collateral Distribution.", uploadedItem.Paths.FullPath));
            }
            else
            {
                //response.Add("Media item " + uploadedItem.Name + " has NOT been updated its extended fields with form values.", ResponseStatus.Warning);
                MarketingCollateralLog.Log.Warn(string.Format("Media item from path {0} was NOT updated its extended document fields for Marketing Collateral Distribution.", uploadedItem.Paths.FullPath));
            }
        }

        private static void ChangeTemplate(Item item, MarketingCollateralResponse response)
        {
            var Templates = GetChangeToExtendedDocumentTemplateDataList();
            var originalTemplateFullname = item.Template.FullName;
            MarketingCollateralLog.Log.Info(string.Format("UPLOAD Pipeline :: Item for Upload, Name: {0}, Original Template: {1}, Item Path: {2}", item.Name, item.Template.FullName, item.Paths.FullPath));
            // Need to change template for this item?
            Sitecore.Resources.Media.Media media = Sitecore.Resources.Media.MediaManager.GetMedia(item);
            string mimeType = string.Empty;
            if (media != null)
            {
                MarketingCollateralLog.Log.Info(string.Format("UPLOAD Pipeline :: Item has a mediaItem, extension: {0}, MIME type: {1}", media.Extension, media.MimeType));
                mimeType = media.MimeType;

                var changeTemplateEntry = Templates.Where(t => t.OldTemplate.Equals(item.Template.FullName) && t.MimeType.ToLower() == mimeType.ToLower()).FirstOrDefault();

                if (changeTemplateEntry != null)
                {
                    var db = item.Database;
                    var newTemplate = db.Templates[changeTemplateEntry.NewTemplate];
                    try
                    {
                        item.ChangeTemplate(newTemplate);
                        //response.Add("Media item " + item.Name + " has changed its template from [" + originalTemplateFullname + "] to [" + newTemplate.FullName + "] to include Document Extended fields in Sitecore.", ResponseStatus.Success);
                        MarketingCollateralLog.Log.Info(string.Format("UPLOAD Pipeline :: Changed Template applied to item {0}: from [{1}] to [{2}].", item.Name, originalTemplateFullname, newTemplate.FullName));
                    }
                    catch (Exception ex)
                    {
                        //response.Add("Unable to change the Media item template to an Extended Document template for item " + item.Name + ". From [" + originalTemplateFullname + "] to [" + newTemplate.FullName + "].", ResponseStatus.Error);
                        response.Add("There was a problem updating document's data. Please see error details in Marketing Collateral Sitecore log.", ResponseStatus.Error);
                        MarketingCollateralLog.Log.Error(string.Format("UPLOAD Pipeline :: Configured ChangeTemplate was unable to be applied on item {0}: original template [{1}], new template [{2}].", item.Name, originalTemplateFullname, newTemplate.FullName), ex);
                    }
                }
            }
        }

        private static MediaItem AddFile(MarketingCollateralItem mcdItem, MarketingCollateralResponse response)
        {
            Item mediaItem = null;
            try
            {
                var database = Sitecore.Configuration.Factory.GetDatabase("master");
                // Create the options
                Sitecore.Resources.Media.MediaCreatorOptions options = new Sitecore.Resources.Media.MediaCreatorOptions();
                // Store the file in the database, not as a file
                options.FileBased = false;
                // Remove file extension from item name
                options.IncludeExtensionInItemName = true;
                // Overwrite any existing file with the same name
                options.KeepExisting = false;
                // Do not make a versioned template
                options.Versioned = true;
                // set the path
                options.Destination = mcdItem.UploadPath + "/" + System.IO.Path.GetFileNameWithoutExtension(mcdItem.FileName);
                // Set the database
                options.Database = database;

                MemoryStream ms = new MemoryStream();
                mcdItem.File.CopyTo(ms);
                ms.Position = 0;

                mediaItem = (MediaItem)database.GetItem(options.Destination);
                if (mediaItem == null && !mcdItem.IsReplacing)
                {
                    // Now create the file in Sitecore.
                    // We create a second stream to update the media item with the correct file extension, because the CreatefromStream doesn't allow a fileName with extension
                    // when creating the media item under a bucket content item
                    MemoryStream ms2 = new MemoryStream();
                    ms.CopyTo(ms2);
                    ms.Position = 0;
                    ms2.Position = 0;

                    // its is not possible to upload document with an extension, it throws an Exception: Sitecore.Exceptions.InvalidItemNameException
                    // with Message: An item name cannot contain any of the following characters: \/:?"<>|[] (controlled by the setting InvalidItemNameChars)
                    // therefore, we remove the extension from the filename
                    mediaItem = Sitecore.Resources.Media.MediaManager.Creator.CreateFromStream(ms, System.IO.Path.GetFileNameWithoutExtension(mcdItem.FileName), options);
                    Sitecore.Resources.Media.Media media = Sitecore.Resources.Media.MediaManager.GetMedia(mediaItem);

                    // We update the media Item's file extension
                    media.SetStream(ms2, Path.GetExtension(mcdItem.FileName).Replace(".", ""));
                    ms2.Dispose();
                    ms.Dispose();
                    mediaItem = (MediaItem)database.GetItem(options.Destination);
                }
                else if (mediaItem != null && mcdItem.IsReplacing)
                {
                    // Attach Version specific media item
                    mediaItem = mediaItem.Versions.AddVersion();
                    Sitecore.Resources.Media.Media media = Sitecore.Resources.Media.MediaManager.GetMedia(mediaItem);
                    media.SetStream(ms, Path.GetExtension(mcdItem.FileName).Replace(".", ""));
                    ms.Dispose();

                    mcdItem.SalesforceFeedItemId = mediaItem.Fields["SFDC FeedItem Id"].Value;
                    mcdItem.SalesforceDocumentId = mediaItem.Fields["SFDC Document Id"].Value;
                }
                else
                {
                    // Tried to upload file to Sitecore, but conditions are not met
                    // 1. The media item exists but it is not intended to replace the file
                    // 2. The media item wasn't found and it was intended to replace it

                    var errorMsg = string.Empty;
                    if (!mcdItem.IsReplacing)
                    {
                        errorMsg = "Unable to upload file into Sitecore: file already exists for the selected group.";
                    }
                    else
                    {
                        // an unlikely scenario, however if the media item is being deleted in another session, by another user, this will get triggered.
                        errorMsg = "Unable to upload file into Sitecore: specified filename was not found in current group for replacement.";
                    }
                    response.Add(errorMsg, ResponseStatus.Error);
                    MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: " + errorMsg + ", file: " + mcdItem.FileName + ", upload location: " + options.Destination);
                    return null;
                }

                var msg = "File " + mcdItem.FileName + " with Sitecore version " + mediaItem.Version.Number + ", has been uploaded to " + options.Destination;
                //response.Add(msg, ResponseStatus.Success);
                MarketingCollateralLog.Log.Info("UPLOAD Pipeline :: " + msg);
                var downloadUrl = mediaItem.GetMediaURL();
                var eWMUrl = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralEWMUrl);
                var fullUrl = new Uri(new Uri(eWMUrl, UriKind.Absolute), new Uri(downloadUrl, UriKind.Relative));
                response.DownloadFileUrl = fullUrl.ToString();
                msg = "File Url: " + fullUrl.ToString();
                //response.Add(msg, ResponseStatus.Success);
                MarketingCollateralLog.Log.Info("UPLOAD Pipeline :: " + msg);
                msg = "File " + mcdItem.FileName + " has been succesfully " + (!mcdItem.IsReplacing ? "uploaded to" : "replaced in") + " Sitecore";
                response.Add(msg, ResponseStatus.Success);
            }
            catch (Exception ex)
            {
                response.Add("Unable to save file in Sitecore, location: " + mcdItem.UploadPath + "/" + System.IO.Path.GetFileNameWithoutExtension(mcdItem.FileName), ResponseStatus.Error);
                response.Add("Please see error details in Marketing Collateral Sitecore log.", ResponseStatus.Error);
                MarketingCollateralLog.Log.Error(string.Format("UPLOAD Pipeline :: Error while creating media file in Media Library and saving into Sitecore: File {0}, Save location: {1}.", mcdItem.DownloadName, mcdItem.UploadPath + "/" + System.IO.Path.GetFileNameWithoutExtension(mcdItem.FileName)), ex);
            }

            return mediaItem;
        }

        public static List<KeyValuePair<string, string>> GetSitecoreUsers(string roleName, string domain)
        {
            var userNames = new List<KeyValuePair<string, string>>();
            try
            {
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    var users = Sitecore.Security.Accounts.UserManager.GetUsers();
                    userNames = users.Where(u => !string.IsNullOrWhiteSpace(u.Profile.FullName)).Select(u => new KeyValuePair<string, string>(u.Name, u.Profile.FullName)).ToList();
                }
                else
                {
                    string fullRoleName = !string.IsNullOrWhiteSpace(domain) ? string.Format("{0}\\{1}", domain.Trim(), roleName.Trim()) : roleName.Trim();
                    var usersInRole = Sitecore.Security.Accounts.RolesInRolesManager.GetUsersInRole(Sitecore.Security.Accounts.Role.FromName(fullRoleName), true);
                    userNames = usersInRole.Where(u => !string.IsNullOrWhiteSpace(u.Profile.FullName)).Select(u => new KeyValuePair<string, string>(u.Name, u.Profile.FullName)).ToList();
                }
                userNames.Sort();
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while getting Sitecore users.", ex, typeof(ImporterHelper));
            }

            return userNames;
        }

        public static Item[] GetMarketingUtilityUserItems(string databaseName)
        {
            Item[] marketingUtilityUsers = null;
            try
            {
                Sitecore.Data.Database database = Sitecore.Configuration.Factory.GetDatabase(databaseName);

                if (string.IsNullOrWhiteSpace(MarketingUtilityUsersFolderId))
                    throw new ApplicationException("AssetMark.SitecoreExt.MarketingCollateral.SubmittersFolderId Sitecore setting has not been set up.");
                if (string.IsNullOrWhiteSpace(MarketingUtilityUsersTemplateId))
                    throw new ApplicationException("AssetMark.SitecoreExt.MarketingCollateral.SubmittersTemplateId Sitecore setting has not been set up.");

                marketingUtilityUsers = ItemHelper.GetChildrenContentItems(MarketingUtilityUsersFolderId, null, MarketingUtilityUsersTemplateId, database);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while getting Marketing Utility users as content items.", ex, typeof(ImporterHelper));
            }

            return marketingUtilityUsers;
        }

        public static List<KeyValuePair<string, string>> GetMarketingUtilityUserList(string databaseName)
        {
            var userNames = new List<KeyValuePair<string, string>>();
            try
            {
                var users = GetMarketingUtilityUserItems(databaseName);
                if (users != null)
                {
                    userNames = users
                        .Where(userItem => ((CheckboxField)userItem.Fields["Allowed To Submit"]).Checked)
                        .Select(userItem =>
                        new KeyValuePair<string, string>(ItemHelper.GetFieldValue(userItem, "User Data", "Email"), ItemHelper.GetFieldValue(userItem, "User Data", "List Display Name"))
                    ).ToList();
                }
                userNames.Sort((kv1, kv2) => { return kv1.Value.CompareTo(kv2.Value); });
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Error while getting Marketing Utility user list.", ex, typeof(ImporterHelper));
            }

            return userNames;
        }

        public static Dictionary<string, string> GetFolderItemGroups()
        {
            var dictionary = new Dictionary<string, string>();

            try
            {
                using (new SecurityDisabler())
                {
                    var database = Sitecore.Configuration.Factory.GetDatabase("master");
                    string itemGroupId = Sitecore.Configuration.Settings.GetSetting(Constants.Marketing.MarketingCollateralItemGroupRootId);
                    if (string.IsNullOrWhiteSpace(itemGroupId))
                    {
                        throw new ApplicationException("Marketing Collateral Distribution: Item Group root Id setting was not found.");
                    }

                    Sitecore.Data.ID rootId = new Sitecore.Data.ID(itemGroupId);
                    var rootItem = database.GetItem(rootId);
                    foreach (Item item in rootItem.GetChildren())
                    {
                        dictionary.Add(item.Name, item.Paths.FullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Unable to get folder item group names to store media files", ex);
            }

            return dictionary;
        }

        public static List<BaseRepository> LoadRepositorySettings(System.Web.HttpRequest request)
        {
            List<BaseRepository> list = new List<BaseRepository>();
            ConfigurationSectionGroup marketingCollateralGroup = WebConfigurationManager.OpenWebConfiguration(request.ApplicationPath).GetSectionGroup("MarketingCollateral");
            if (marketingCollateralGroup != null)
            {
                foreach (var section in marketingCollateralGroup.Sections)
                {
                    RepositorySection repositorySection = section as RepositorySection;
                    if (repositorySection != null)
                    {
                        string repositoryName = repositorySection.Name;
                        int displayOrder = repositorySection.DisplayOrder;
                        foreach (var importer in repositorySection.ImporterAdaptors)
                        {
                            RepositoryType repositoryType;
                            if (Enum.TryParse<RepositoryType>(importer.Name, out repositoryType))
                            {
                                try
                                {
                                    BindRepository(displayOrder, repositoryName, repositoryType, importer, list);
                                }
                                catch (Exception ex)
                                {
                                    MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Exception thrown on binding configuration to entity objects", ex);
                                }
                            }
                            else
                            {
                                MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Importer Adaptor name from configuration is invalid");
                            }
                        }
                    }
                }
            }
            else
            {
                MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Marketing Collateral configuration section is not found.");
            }
            return list;
        }

        private static void BindRepository(int displayOrder, string repositoryName, RepositoryType repositoryType, ImporterAdaptor importerAdaptor, List<BaseRepository> list)
        {
            BaseRepository baseRepository = null;
            Type type = Type.GetType(importerAdaptor.Type, false, true);

            if (type != null)
            {
                baseRepository = (BaseRepository)Activator.CreateInstance(type);

                if (baseRepository != null)
                {
                    baseRepository.DisplayOrder = displayOrder;
                    baseRepository.Name = repositoryName;
                    baseRepository.RepositoryType = repositoryType;
                    BindRepositoryProperties(type, importerAdaptor, baseRepository);
                    list.Add(baseRepository);
                }
            }
        }

        private static void BindRepositoryProperties(Type type, ImporterAdaptor importer, BaseRepository repository)
        {
            if (importer != null & importer.SitecoreProperties != null)
            {
                bool loaded = false;
                foreach (var property in importer.SitecoreProperties)
                {
                    SetPropertyValue(type, repository, property.Name, property.Value, property.Format);
                    loaded = true;
                }

                if (!loaded)
                {
                    MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Repository properties were not found inside collection for importer " + importer.Name);
                }
            }
            else
            {
                MarketingCollateralLog.Log.Error("Marketing Collateral ImporterHelper :: Repository properties collection not found for importer " + importer.Name);
            }
        }

        private static void SetPropertyValue(Type type, BaseRepository repository, string propertyName, string propertyValue, string propertyFormat)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                PropertyInfo property = type.GetProperty(propertyName);

                if (property != null)
                {
                    object value = GetPropertyValue(propertyValue, property, propertyFormat);
                    property.SetValue(repository, value, null);
                }
            }
        }

        private static object GetPropertyValue(string propertyValue, PropertyInfo property, string format)
        {
            object value = null;

            try
            {
                if (property.PropertyType == typeof(string))
                {
                    value = propertyValue;
                }
                else if (property.PropertyType == typeof(bool))
                {
                    value = false;
                    bool boolValue;
                    if (Boolean.TryParse(propertyValue, out boolValue))
                    {
                        value = boolValue;
                    }
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    value = DateTime.ParseExact(propertyValue, format, new DateTimeFormatInfo());
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    value = new Guid(propertyValue);
                }
                else
                {
                    string strValue = propertyValue;
                    value = Convert.ChangeType(strValue, property.PropertyType, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                value = null;
            }

            return value;
        }

        public static List<ChangeToExtendedMediaTemplateData> GetChangeToExtendedDocumentTemplateDataList()
        {
            var list = new List<ChangeToExtendedMediaTemplateData>();

            var section = WebConfigurationManager.GetSection("ChangeToExtendedMediaTemplates") as ChangeToExtendedMediaTemplateSection;
            if (section != null)
            {
                list.AddRange(section.ChangeToExtendedMediaTemplateDataEntries.ToList());
            }

            return list;
        }

        public static List<ExtendedMediaTemplate> GetExtendedDocumentTemplates()
        {
            var list = new List<ExtendedMediaTemplate>();

            var section = WebConfigurationManager.GetSection("ChangeToExtendedMediaTemplates") as ChangeToExtendedMediaTemplateSection;
            if (section != null)
            {
                list.AddRange(section.ExtendedMediaTemplateEntries.ToList());
            }

            return list;
        }

        public static List<DocumentContentItemTemplate> GetValidDocumentBaseTemplates()
        {
            var list = new List<DocumentContentItemTemplate>();

            var section = WebConfigurationManager.GetSection("ContentItemTemplatesWithFiles") as DocumentContentItemTemplateSection;
            if (section != null)
            {
                list.AddRange(section.DocumentContentItemTemplates.ToList());
            }

            return list;
        }
    }
}
