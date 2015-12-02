using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genworth.SitecoreExt.Security;
using Genworth.SitecoreExt.Utilities;
using Genworth.SitecoreExt.Marketing.Request;
using System.Collections.Specialized;
using GFWM.Common.AUM.Entities.Data;
using GFWM.Common.UserProfile.Entities.Data;
using GFWM.Common.Preference.Entities.Data;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.Helpers;
using Sitecore.Data.Items;

namespace Genworth.SitecoreExt.Marketing
{
    public class Controller
    {
        public static SSOMessage BuildMarcomCentralAuthenticationRequest(string aplId, string pageName, string productId)
        {
            SSOMessage ssoMessage = null;
            NameValueCollection settings = null;
            NameValueCollection impersonatingSettings = null;
            AgentDetails agentData = null;
            IEnumerable<SupportTeamDetails> supportTeamData = null;
            MyProfileInformation profileData = null;
            MyProfileInformation impersonatingProfileData = null;
            SalesforceUser impersonatingSalesforceUser = null;
            SalesforceUser salesforceInternalUser = null;

            string settingsItemPath = "/sitecore/content/Home/Marketing/Portal";
            string settingsTemplate = "Marcom Central Setting";
            string settingsField = "Settings";
            string userGroupNameField = "User Group Name";
            string userGroupName = string.Empty;
            string impersonatingUserGroupName = string.Empty;
            string parameters = string.Empty;

            try
            {
                var client = new ServiceClient();
                Authorization currentAuthorization = Authorization.CurrentAuthorization;
                var agents = currentAuthorization.Claim.Roles.Where(r => r.RoleId == 6).Select((a) => a.APLId).Distinct().ToArray();
                var ssoDataAttributes = client.GetUserAttributes(currentAuthorization.Claim.LoggedInSSOGuid, null);

                bool isExternalUser = agents.Length > 0;
                string currentUserId = aplId;
                string marcomCentralUserId = string.Empty;
                string marcomCentralImpersonatingUserId = string.Empty;
                
                if (ssoDataAttributes != null && ssoDataAttributes.UserData != null)
                {
                    var attributes = ssoDataAttributes.UserData.FirstOrDefault();
                    currentUserId = attributes.AplId;
                }
                bool isImpersonating = !string.IsNullOrWhiteSpace(currentUserId) && currentAuthorization.Claim.SSOGuid != currentAuthorization.Claim.LoggedInSSOGuid;
                var roleClaim = currentAuthorization.Claim.Roles.Where(r => r.APLId == aplId).FirstOrDefault();

                var item = ContextExtension.CurrentDatabase.GetItem(settingsItemPath);
                if (item != null)
                {
                    var itemList = item.GetChildrenOfTemplate(settingsTemplate).Where(i => HasChannel(i, roleClaim.ChannelType) && HasPCStatus(i, roleClaim.PC_Status));
                    var settingsItem = itemList.FirstOrDefault();
                    if (settingsItem != null)
                    {
                        parameters = settingsItem[settingsField];
                        userGroupName = settingsItem[userGroupNameField];
                        settings = Sitecore.Web.WebUtil.ParseUrlParameters(parameters);
                    }
                }

                if (isImpersonating)
                {
                    var impersonatingSettingsItem = ContextExtension.CurrentDatabase.GetItem(MarketingLogic.MarcomCentralImpersonatingSettingsPath);
                    if (impersonatingSettingsItem != null)
                    {
                        parameters = impersonatingSettingsItem[settingsField];
                        impersonatingUserGroupName = impersonatingSettingsItem[userGroupNameField];
                        impersonatingSettings = Sitecore.Web.WebUtil.ParseUrlParameters(parameters);
                    }
                }

                if (isExternalUser)
                {
                    var agentDetailsResponse = client.GetAgentDetails(aplId);
                    var supportTeamResponse = client.GetSupportTeamDetails(aplId);

                    agentData = agentDetailsResponse.AgentDetails != null ? agentDetailsResponse.AgentDetails.FirstOrDefault() : null;
                    supportTeamData = supportTeamResponse.SupportTeamDetails;

                    profileData = GetUserInfo(currentUserId);
                    marcomCentralUserId = aplId;
                    
                    if (isImpersonating)
                    {
                        impersonatingProfileData = GetUserInfo(currentUserId);
                        impersonatingSalesforceUser = GetSalesforceUser(currentUserId);
                        marcomCentralImpersonatingUserId = marcomCentralUserId;
                        marcomCentralUserId = TruncateSalesforceId(impersonatingSalesforceUser, currentUserId);
                    }

                    var regionalConsultant = supportTeamData != null ? supportTeamData.Where(t => t.JobTitle == "Regional Consultant").FirstOrDefault() : null;
                    var internalConsultant = supportTeamData != null ? supportTeamData.Where(t => t.JobTitle == "Internal Consultant").FirstOrDefault() : null;

                    string rcName = regionalConsultant != null ? regionalConsultant.FirstName + " " + regionalConsultant.LastName : string.Empty;
                    string icName = internalConsultant != null ? internalConsultant.FirstName + " " + internalConsultant.LastName : string.Empty;

                    string userName = string.Empty;
                    if (isImpersonating)
                    {
                        userName = impersonatingProfileData != null ? impersonatingProfileData.Firstname + " " + impersonatingProfileData.Lastname : string.Empty;
                    }
                    else
                    {
                        userName = profileData != null ? profileData.Firstname + " " + profileData.Lastname : string.Empty;
                    }

                    settings.Add("User.GenericField1", userName.Trim());
                    settings.Add("User.GenericField2", agentData.PrimaryAdvisorID);
                    settings.Add("User.GenericField3", rcName.Trim());
                    settings.Add("User.GenericField4", icName.Trim());
                }
                else
                {
                    // it's an internal user or employee
                    profileData = GetUserInfo(aplId);
                    salesforceInternalUser = GetSalesforceUser(aplId);
                    marcomCentralUserId = TruncateSalesforceId(salesforceInternalUser, aplId);

                    if (isImpersonating)
                    {
                        impersonatingProfileData = GetUserInfo(currentUserId);
                        impersonatingSalesforceUser = GetSalesforceUser(currentUserId);
                        marcomCentralImpersonatingUserId = marcomCentralUserId;
                        marcomCentralUserId = TruncateSalesforceId(impersonatingSalesforceUser, currentUserId);
                    }
                }

                var mapper = new MarcomCentralDataMapper()
                {
                    MarcomCentralUserId = marcomCentralUserId,
                    MarcomCentralImpersonatingUserId = marcomCentralImpersonatingUserId,
                    SalesforceInternalUser = salesforceInternalUser,
                    ImpersonatingSalesforceUser = impersonatingSalesforceUser,
                    PageName = pageName,
                    ProductId = productId,
                    IsExternalUser = isExternalUser,
                    IsImpersonating = isImpersonating,
                    AgentData = agentData,
                    SupportTeamDetails = supportTeamData,
                    UserProfile = profileData,
                    ImpersonatingUserProfile = impersonatingProfileData,
                    Settings = settings,
                    ImpersonatingSettings = impersonatingSettings,
                    UserGroupName = userGroupName,
                    ImpersonatingUserGroupName = impersonatingUserGroupName
                };

                ssoMessage = mapper.MapToMarcomCentralData();

            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to build Marcom Central authentication request for user " + aplId, ex, typeof(Controller));
            }

            return ssoMessage;
        }

        public static string SendMarcomCentralAuthenticationRequest(SSOMessage ssoMessage)
        {
            try
            {
                string authenticationResponse = string.Empty;
                using (SingleSignOnSoapClient proxy = new SingleSignOnSoapClient())
                {
                    try
                    {
                        // Logging data to send to Marcom Central
                        string xmlRequestData = ssoMessage != null ? SerializationHelper.Serialize<SSOMessage>(ssoMessage) : null;
                        Sitecore.Diagnostics.Log.Info(string.Format("Xml data for submission to Marcom Central: {0}", xmlRequestData ?? ""), typeof(Controller));

                        authenticationResponse = proxy.Authenticate(ssoMessage);
                    }
                    catch (Exception ex)
                    {
                        Sitecore.Diagnostics.Log.Error("Unable to send Marcom Central authentication request for user " + ssoMessage.SingleSignOnRequest.UserCredentials.ID.Value, ex, typeof(Controller));
                    }
                }
                return authenticationResponse;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Single Sign On Marcom Central request error", ex, typeof(Controller));
            }
            return string.Empty;
        }

        private static bool HasChannel(Item item, string channel)
        {
            bool hasChannel = false;
            var items = item.GetMultilistItems("Security", "Channels");

            if (items != null)
            {
                hasChannel = (items.Count == 0 && string.IsNullOrWhiteSpace(channel)) || items.Where(i=> i.GetText("Code").ToUpper() == channel.ToUpper()).Select(s => true).FirstOrDefault();
            }
            return hasChannel;
        }

        private static bool HasPCStatus(Item item, string pcStatus)
        {
            bool hasChannel = false;
            var items = item.GetMultilistItems("Security", "PC-Status");
            if (items != null)
            {
                hasChannel = (items.Count == 0 && string.IsNullOrWhiteSpace(pcStatus)) || items.Where(i => i.GetText("Code").ToUpper() == pcStatus.ToUpper()).Select(s => true).FirstOrDefault();
            }
            return hasChannel;
        }

        private static string GetSalesforceUserId(string userId, string defaultUserId)
        {
            var salesforceUser = GetSalesforceUser(userId);
            if (salesforceUser != null)
            {
                return salesforceUser.SalesforceId.Substring(0, 15);
            }
            return defaultUserId;
        }

        private static string TruncateSalesforceId(SalesforceUser user, string defaultId)
        {
            if (user != null)
            {
                if (user.SalesforceId.Length > 15)
                {
                    return user.SalesforceId.Substring(0, 15);
                }
                else
                {
                    return user.SalesforceId;
                }
            }
            return defaultId;
        }

        private static SalesforceUser GetSalesforceUser(string userId)
        {
            var client = new ServiceClient();
            var salesforceUserResponse = client.GetSalesforceUser(userId);
            var salesforceUser = salesforceUserResponse != null && salesforceUserResponse.SalesforceUsers != null ? salesforceUserResponse.SalesforceUsers.FirstOrDefault() : null;
            return salesforceUser;
        }

        private static MyProfileInformation GetUserInfo(string userId)
        {
            var client = new ServiceClient();
            var userInfo = client.GetUserInfo(userId);
            var profileData = userInfo.MyProfileInformation != null ? userInfo.MyProfileInformation.FirstOrDefault() : null;
            return profileData;
        }

        public static string CreateStandardRegisterHtmlForm(string aplId, string target, bool scriptSubmit = false, string actionUrl = null)
        {
            try
            {
                // Build POST data
                var collection = BuildStandardRegisterHtmlFormData(aplId, scriptSubmit, actionUrl);

                // Create dynamic Html Form
                string htmlForm = HtmlHelper.PreparePOSTForm(aplId, LogonURL, collection, scriptSubmit, target, autoSubmit: false);

                return htmlForm;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to create Standard Register dynamic html form for user " + aplId, ex, typeof(Controller));
            }

            return string.Empty;
        }

        public static NameValueCollection BuildStandardRegisterHtmlFormData(string aplId, bool scriptSubmit = false, string actionUrl = null)
        {
            try
            {
                //Retrieve the required data from the service
                var client = new ServiceClient();
                StandardRegisterLoginData standardRegisterData = null;
                Authorization currentAuthorization = Authorization.CurrentAuthorization;
                var agents = currentAuthorization.Claim.Roles.Where(r => r.RoleId == 6).Select((a) => a.APLId).Distinct().ToArray();
                var ssoData = client.GetUserSSODetails(currentAuthorization.Claim.LoggedInSSOGuid);
                bool isImpersonating = currentAuthorization.Claim.SSOGuid != currentAuthorization.Claim.LoggedInSSOGuid ||
                    currentAuthorization.Claim.Roles.Where(r =>
                        r.RoleTypeCD == (int)Constants.Security.RoleType.RC ||
                        (r.RoleTypeCD == (int)Constants.Security.RoleType.SHAREDACCESS && r.RoleId == 6) ||
                        r.RoleTypeCD == (int)Constants.Security.RoleType.RM ||
                        r.RoleTypeCD == (int)Constants.Security.RoleType.ADMINASSISTANT)
                        .Count() > 0;

                if (agents.Length > 0)
                {
                    int i = 0;
                    for (int j = 0; j < agents.Length; j++)
                    {
                        if (agents[j] == aplId)
                            i = j;
                    }
                    var agentData = client.GetAgentDetails(agents[i]);
                    var supportTeamContactDetails = client.GetSupportTeamDetails(agents[i]);

                    if (agentData != null && agentData.AgentDetails != null && agentData.AgentDetails.FirstOrDefault() != null)
                    {
                        // Map the data
                        var mapper = new DataMapper();
                        standardRegisterData = mapper.MapToStandardRegisterData(agentData.AgentDetails.FirstOrDefault(), supportTeamContactDetails.SupportTeamDetails);

                        if (isImpersonating)
                        {
                            mapper.MapImpersonatingUserData(standardRegisterData, ssoData.User);
                        }
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Info(string.Format("Retrieving data for agent " + aplId + " to send to SR is empty."), typeof(Controller));
                    }
                }
                else
                {
                    var userInfo = client.GetUserInfo(aplId);

                    // Map the data
                    var mapper = new DataMapper();
                    standardRegisterData = mapper.MapToStandardRegisterData(aplId, userInfo.MyProfileInformation);
                }

                // Serialize to xml string
                List<Type> list = new List<Type>();
                list.Add(typeof(AuxField));
                list.Add(typeof(ImpersonatingUserData));
                list.Add(typeof(PhysicalAddressData));
                list.Add(typeof(StandardRegisterLoginData));
                string xmlRequestData = SerializationHelper.Serialize<StandardRegisterLoginData>(standardRegisterData, list.ToArray(), XmlNamespace, omitXmlDeclaration: true);
                Sitecore.Diagnostics.Log.Info(string.Format("Xml data for submission to Standard Register: {0}", xmlRequestData ?? ""), typeof(Controller));

                // Build POST data
                var collection = RequestBuilder.BuildFormFieldValues(Issuer, xmlRequestData, token: null, actionUrl: actionUrl);

                return collection;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to build Standard Register data collection for user " + aplId, ex, typeof(Controller));
            }
            return new NameValueCollection();
        }

        private static string sLogonURL;
        public static string LogonURL
        {

            get
            {
                if (string.IsNullOrEmpty(sLogonURL))
                {
                    sLogonURL = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.StandardRegisterLogonURL);
                    if (string.IsNullOrEmpty(sLogonURL))
                    {
                        Sitecore.Diagnostics.Log.Error("Standard Register Logon URL has not been configured in sitecore\\settings", typeof(Controller));
                    }
                }

                return sLogonURL;
            }
        }

        private static string sXmlNamespace;
        public static string XmlNamespace
        {

            get
            {
                if (string.IsNullOrEmpty(sXmlNamespace))
                {
                    sXmlNamespace = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.StandardRegisterXmlNamespace);
                    if (string.IsNullOrEmpty(sXmlNamespace))
                    {
                        Sitecore.Diagnostics.Log.Error("Standard Register Xml Namespace for request field has not been configured in sitecore\\settings", typeof(Controller));
                    }
                }

                return sXmlNamespace;
            }
        }

        private static string sIssuer;
        public static string Issuer
        {

            get
            {
                if (string.IsNullOrEmpty(sIssuer))
                {
                    sIssuer = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.StandardRegisterIssuer);
                    if (string.IsNullOrEmpty(sIssuer))
                    {
                        Sitecore.Diagnostics.Log.Error("Standard Register Issuer has not been configured in sitecore\\settings", typeof(Controller));
                    }
                }

                return sIssuer;
            }
        }
    }
}
