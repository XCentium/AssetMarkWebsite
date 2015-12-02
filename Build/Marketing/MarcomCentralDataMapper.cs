using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genworth.SitecoreExt.Helpers;
using GFWM.Common.UserProfile.Entities.Data;
using GFWM.Common.AUM.Entities.Data;
using GFWM.Common.Preference.Entities.Data;
using System.Collections.Specialized;

namespace Genworth.SitecoreExt.Marketing
{
    public class MarcomCentralDataMapper
    {
        public string PageName { get; set; }
        public string ProductId { get; set; }
        public bool IsImpersonating { get; set; }
        public bool IsExternalUser { get; set; }
        public string MarcomCentralUserId { get; set; }
        public string MarcomCentralImpersonatingUserId { get; set; }
        public SalesforceUser SalesforceInternalUser { get; set; }
        public SalesforceUser ImpersonatingSalesforceUser { get; set; }
        public AgentDetails AgentData { get; set; }
        public IEnumerable<SupportTeamDetails> SupportTeamDetails { get; set; }
        public MyProfileInformation UserProfile { get; set; }
        public MyProfileInformation ImpersonatingUserProfile { get; set; }
        public NameValueCollection Settings { get; set; }
        public NameValueCollection ImpersonatingSettings { get; set; }
        public string UserGroupName { get; set; }
        public string ImpersonatingUserGroupName { get; set; }

        public const string DefaultCountry = "USA";

        public SSOMessage MapToMarcomCentralData()
        {
            SSOMessage ssoMessage = new SSOMessage();
            ssoMessage.PartnerCredentials = new PartnerCredentialsType();
            ssoMessage.PartnerCredentials.Token = MarketingLogic.MarcomCentralToken;
            ssoMessage.SingleSignOnRequest = BuildSSORequestData(MarcomCentralUserId, PageName, ProductId, IsImpersonating, MarcomCentralImpersonatingUserId);

            if (IsExternalUser)
            {
                ssoMessage.EditUserRequest = BuildEditUserRequestData(IsImpersonating ? MarcomCentralImpersonatingUserId : MarcomCentralUserId, AgentData);

                if (IsImpersonating && (ImpersonatingUserProfile != null || ImpersonatingSalesforceUser != null))
                {
                    var impersonatingProfile = MapToProfileInformation(ImpersonatingSalesforceUser, ImpersonatingUserProfile);
                    ssoMessage.EditSecondaryUserRequest = BuildEditUserRequestData(MarcomCentralUserId, impersonatingProfile, AgentData, IsImpersonating);
                }
            }
            else
            {
                var userProfile = MapToProfileInformation(SalesforceInternalUser, UserProfile);
                ssoMessage.EditUserRequest = BuildEditUserRequestData(IsImpersonating ? MarcomCentralImpersonatingUserId : MarcomCentralUserId, userProfile, null, isImpersonatingUser: false);

                if (IsImpersonating && (ImpersonatingUserProfile != null || ImpersonatingSalesforceUser != null))
                {
                    var impersonatingProfile = MapToProfileInformation(ImpersonatingSalesforceUser, ImpersonatingUserProfile);
                    ssoMessage.EditSecondaryUserRequest = BuildEditUserRequestData(MarcomCentralUserId, impersonatingProfile, null, IsImpersonating);
                }
            }

            return ssoMessage;
        }

        private SingleSignOnRequestType BuildSSORequestData(string aplId, string pageName, string productId, bool isImpersonating, string impersonatedUserId)
        {
            return new SingleSignOnRequestType()
            {
                UserCredentials = new UserCredentialsType()
                {
                    ID = BuildIdData(aplId)
                },
                Navigation = new NavigationType()
                {
                    StartPage = new NavigationTypeStartPage()
                    {
                        PageName = GetStartUpPageName(),
                        ProductID = GetProductId()
                    }
                },
                ImpersonateUser = GetImpersonatedUserId(isImpersonating, impersonatedUserId)
            };
        }

        private EditUserRequestType BuildEditUserRequestData(string aplId, AgentDetails agentData)
        {
            return new EditUserRequestType()
            {
                Action = ActionType.Auto,
                User = BuildUserData(aplId, agentData)
            };
        }

        private EditUserRequestType BuildEditUserRequestData(string aplId, MyProfileInformation profile, AgentDetails agentData, bool isImpersonatingUser)
        {
            return new EditUserRequestType()
            {
                Action = ActionType.Auto,
                User = BuildUserData(aplId, profile, agentData, isImpersonatingUser)
            };
        }

        private UserType BuildUserData(string aplId, AgentDetails agentData)
        {
            return new UserType()
            {
                ID = BuildIdData(aplId),
                Properties = BuildUserPropertiesData(agentData.FirstName, agentData.LastName, agentData.Email, agentData.APLId, MarketingLogic.SHA1Hash(aplId)),
                Settings = BuildSettingsData(Settings),
                UserGroups = BuildUserGroups(agentData),
                Addresses = BuildAddressesData(agentData)
            };
        }

        private UserType BuildUserData(string aplId, MyProfileInformation profile, AgentDetails agentData, bool isImpersonatingUser)
        {
            var settingsParam = isImpersonatingUser ? ImpersonatingSettings : Settings;
            return new UserType()
            {
                ID = BuildIdData(aplId),
                Properties = BuildUserPropertiesData(profile.Firstname, profile.Lastname, profile.Email, aplId, MarketingLogic.SHA1Hash(aplId)),
                Settings = BuildSettingsData(settingsParam),
                UserGroups = BuildUserGroups(profile, isImpersonatingUser),
                Addresses = BuildAddressesData(aplId, profile),
                Impersonation = BuildImpersonationData(profile, isImpersonatingUser)
            };
        }

        private UserPropertiesType BuildUserPropertiesData(string firstName, string lastName, string email, string login, string password)
        {
            return new UserPropertiesType()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Login = login,
                Password = password
            };
        }

        private SettingType[] BuildSettingsData(NameValueCollection settings)
        {
            if (settings == null)
                return null;

            List<SettingType> settingsList = new List<SettingType>();
            foreach (var key in settings.AllKeys)
            {
                settingsList.Add(new SettingType() { Name = key.Replace("_", "."), Value = settings[key] });
            }
            return settingsList.ToArray();
        }

        private UserSelectionType BuildImpersonationData(MyProfileInformation profile, bool isImpersonatingUser)
        {
            if (!isImpersonatingUser)
                return null;

            return new UserSelectionType()
            {
                UserGroups = BuildImpersonatingUserGroups(profile, IsImpersonating)
            };
        }

        private AddressesType BuildAddressesData(AgentDetails agentData)
        {
            var regionalConsultant = SupportTeamDetails != null ? SupportTeamDetails.Where(t => t.JobTitle == "Regional Consultant").FirstOrDefault() : null;
            var internalConsultant = SupportTeamDetails != null ? SupportTeamDetails.Where(t => t.JobTitle == "Internal Consultant").FirstOrDefault() : null;

            string rcName = regionalConsultant != null ? regionalConsultant.FirstName + " " + regionalConsultant.LastName : string.Empty;
            string icName = internalConsultant != null ? internalConsultant.FirstName + " " + internalConsultant.LastName : string.Empty;

            string userName = string.Empty;
            if (IsImpersonating)
            {
                userName = ImpersonatingUserProfile != null ? ImpersonatingUserProfile.Firstname + " " + ImpersonatingUserProfile.Lastname : string.Empty;
            }
            else
            {
                userName = UserProfile != null ? UserProfile.Firstname + " " + UserProfile.Lastname : string.Empty;
            }

            string billingAddress1 = userName.Trim();
            string billingAddress2 = rcName.Trim();
            string billingAddress3 = icName.Trim();
            string billingAddress4 = agentData.PrimaryAdvisorID;            

            return new AddressesType()
            {
                SyncMode = SyncModeType.Auto,
                SyncModeSpecified = true,
                Address = new AddressType[] 
                { 
                    BuildAddressData(string.Format("{0}_ShippingAddress1", agentData.APLId), agentData.Company, agentData.Address1, null, null, null, null, agentData.Phone, agentData.City, agentData.State, agentData.Zip, AddressTypes.Shipping),
                }
            };
        }

        private AddressesType BuildAddressesData(string aplId, MyProfileInformation profile)
        {
            return new AddressesType()
            {
                SyncMode = SyncModeType.Auto,
                SyncModeSpecified = true,
                Address = new AddressType[] 
                { 
                    BuildAddressData(string.Format("{0}_ShippingAddress1", aplId), null, profile.Address1, profile.Address2, null, null, profile.Email, profile.Phone, profile.city, profile.State, profile.zipcode, AddressTypes.Shipping)
                }
            };
        }

        private AddressType BuildAddressData(string aplId, string companyId, string address1, string address2, string address3, string address4, string email, string phoneNumber, string city, string state, string zip, AddressTypes addressType)
        {
            return new AddressType()
            {
                ID = BuildIdData(aplId),
                CompanyName = companyId,
                Address1 = address1,
                Address2 = address2,
                Address3 = address3,
                Address4 = address4,
                Email = email,
                City = city,
                State = state,
                Zip = zip,
                Country = DefaultCountry,
                Type = addressType,
                TypeSpecified = true,
                Default = true,
                DefaultSpecified = true
            };
        }

        private UserGroupsType BuildUserGroups(AgentDetails agentData)
        {
            return new UserGroupsType()
            {
                SyncMode = SyncModeType.Auto,
                SyncModeSpecified = true,
                UserGroup = new IDType[] { BuildIdData(UserGroupName) }
            };
        }

        private UserGroupsType BuildUserGroups(MyProfileInformation profile, bool isImpersonating)
        {
            var userGroupIdType = isImpersonating ?
                BuildIdData(ImpersonatingUserGroupName) :
                BuildIdData(UserGroupName);

            return new UserGroupsType()
            {
                SyncMode = SyncModeType.Auto,
                SyncModeSpecified = true,
                UserGroup = new IDType[] { userGroupIdType }
            };
        }

        private UserGroupsType BuildImpersonatingUserGroups(MyProfileInformation profile, bool isImpersonating)
        {
            return new UserGroupsType()
            {
                SyncMode = SyncModeType.Auto,
                SyncModeSpecified = true,
                AllOrNone = IDListAllNoneType.All,
                AllOrNoneSpecified = true
            };
        }

        private ImpersonateUserType GetImpersonatedUserId(bool isImpersonating, string userId)
        {
            if (isImpersonating)
            {
                return new ImpersonateUserType()
                {
                    ID = BuildIdData(userId)
                };
            }
            return null;
        }

        private IDType BuildIdData(string id)
        {
            return new IDType()
            {
                type = BusinessEntityType.NonPrintable,
                typeSpecified = true,
                Value = id
            };
        }

        private PageNameType GetStartUpPageName()
        {
            PageNameType pageName;
            if (System.Enum.TryParse<PageNameType>(PageName, out pageName))
            {
                return pageName;
            }
            return PageNameType.Home;
        }

        private IDType GetProductId()
        {
            if (string.IsNullOrWhiteSpace(ProductId))
                return null;

            return BuildIdData(ProductId);
        }

        private MyProfileInformation MapToProfileInformation(SalesforceUser userData, MyProfileInformation defaultProfile)
        {
            if (userData == null)
            {
                return defaultProfile;
            }

            return new MyProfileInformation()
            {
                Address1 = userData.Street,
                city = userData.City,
                Email = userData.Email,
                Firstname = userData.FirstName,
                Lastname = userData.LastName,
                zipcode = userData.PostalCode,
                State = userData.State,
                UserID = userData.SalesforceId
            };
        }


    }
}
