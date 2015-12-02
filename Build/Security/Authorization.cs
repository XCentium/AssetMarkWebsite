using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

using Sitecore.Diagnostics;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Web;

using ServerLogic.SitecoreExt;

using Genworth.SitecoreExt.Constants;

using GFWM.Shared.ServiceRequest;
using GFWM.Shared.ServiceRequestFactory;
using GFWM.Security.STS.Entity;
using GFWM.Shared.Entity.Data;
using GFWM.Common.State.Entities.Request;
using GFWM.Common.State.Entities.Response;
using GFWM.Common.State.Entities.Data;
using GFWM.Common.AUM.Entities.Request;
using GFWM.Common.AUM.Entities.Response;
using GFWM.Common.AUM.Entities.Data;
using GFWM.Common.Preference.Entities.Request;
using GFWM.Common.Preference.Entities.Response;
using GFWM.Shared.Entity.Response;
using GFWM.Shared.Entity.Request;
using GFWM.Common.PracticeManagement.Entities.Request;
using GFWM.Common.PracticeManagement.Entities.Response;

namespace Genworth.SitecoreExt.Security 
{
	/// <summary>
	/// 
	/// </summary>
	public class Authorization
	{

		#region ATTRIBUTES

		#region CONSTANTS

		/// <summary>
		/// Constant used to mark the relation between an end client and advisor as not set, which means that no value is set in EWM
		/// </summary>
		private const int RoleCodeNotFound = -1;

		/// <summary>
		/// Parameter used to retrieve the claim from the state service (zephyr services)
		/// </summary>
		private const string SWTClaimRequestParameterKey = "SWTCLAIMS";

		private const string MeetingModeRequestParameterKey = "MEETING_MODE";
		#endregion

		/// <summary>
		/// This is the token that will be shared among all the application. This token allows validation of the user and retribal of preferencex
		/// </summary>
		private SWTClaim oSWTClaim;

		/// <summary>
		/// Flag that indicates whether we are working on meeting mode in the current request. Should not be accessed directly used the property instead
		/// </summary>
		private bool bIsMeetingMode;

		/// <summary>
		/// Flag to lazy load meeting mode flag
		/// </summary>
		private bool bIsMeetingModeLoaded;

		/// <summary>
		/// Array of strings that contains the codes for the roles associated to the user. 
		/// </summary>
		private string[] oUserLevels;

		/// <summary>
		/// Array of strings that contains the codes for the channels associated to the current user
		/// </summary>
		private string[] oChannels;

		/// <summary>
		/// Array of strings that contains the codes for the custodians associated to the current user
		/// </summary>
		private string[] oCustodians;

		/// <summary>
		/// Array of strings that contains the codes for the products associated to the current user
		/// </summary>
		private string[] oProducts;

		/// <summary>
		/// Array of strings that contains the codes for the advisors associated to the current user
		/// </summary>
		private string[] oAdvisors;

		/// <summary>
		/// Gets a pipe delimited list containing all the section that need to be secured by client aoproved
		/// </summary>
		private string[] oClientSecuredSections;

		/// <summary>
		/// list that contains objects of type ClientAgent, this objects map the relation between an end client-Advisor-channel
		/// </summary>
		private List<ClientAgent> oClientAgentPreferences;

		/// <summary>
		/// This array od string contains the codes for the preferences that an advisor has set for an end-client (Preferences for the access of Managers and strategists)
		/// </summary>
		private string[] oManagerStrategistPrivileges;

        /// <summary>
        /// This array od string contains the codes for the PC-Status values associated to a agent
        /// </summary>
        private string[] oPC_Status;

		private IServiceRequest oSWTService;

		private IServiceRequest oStateService;

		private IServiceRequest oAUMService;

		/// <summary>
		/// Flag that indicates whether we are running on integrated mode. True = Means no call to zephyr services
		/// </summary>
		private bool bIsTesMode;

		/// <summary>
		/// Flag that contains the testig configuration loaded from the configuration file defined in the include configuration file (i.e. SWTTest.xml)
		/// </summary>
		private XElement oTestAccount;

		/// <summary>
		/// File system path for the file that contains the test configuration to be loaded in oTestAccount
		/// </summary>
		private string oTestAccountConfigurationPath;

		/// <summary>
		/// Used by a client role, contains the current invesments information
		/// </summary>
		private GFWM.Common.Preference.Entities.Data.ClientInvestmentOption[] oCurrentInvestments;
		/// <summary>
		/// Used by a client role, contains the aditional investments information
		/// </summary>
		private GFWM.Common.Preference.Entities.Data.ClientInvestmentOption[] oAditionalInvestments;

		#endregion

		#region PROPERTIES

		private List<ClientAgent> ClientAgentPreferences
		{
			get
			{
				if (oClientAgentPreferences == null)
				{
					LoadClientAgents();
				}

				return oClientAgentPreferences;
			}
		}

		/// <summary>
		/// Loads the test configuration to be used for authorization when working in none integrated mode
		/// </summary>
		private string TestAccountConfiguration
		{
			get
			{
				if (string.IsNullOrEmpty(oTestAccountConfigurationPath))
				{
					oTestAccountConfigurationPath = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.SWTTestModeConfiguration, string.Empty);
				}

				return oTestAccountConfigurationPath;

			}
		}

		/// <summary>
		/// Test configuration to be used when working in none integrated mode
		/// </summary>
		private XElement TestAccount
		{
			get
			{
				if (oTestAccount == null)
				{
					try
					{
						oTestAccount = XElement.Load(TestAccountConfiguration);
					}
					catch
					{
						Sitecore.Diagnostics.Log.Debug("Unable to load SWT test configuration, Check configuration path it maybe invalid or has not been configured yet...");
						throw;
					}
				}

				return oTestAccount;
			}
		}


		/// <summary>
		/// Indicates whether we are working on integrated mode or not. integrated mode = false.
		/// </summary>
		public bool IsTestMode
		{
			get
			{
				string sTestMode;

				sTestMode = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.SWTTestMode);

				bool.TryParse(sTestMode, out bIsTesMode);

				return bIsTesMode;
			}
		}

		/// <summary>
		/// Exposes the GFWM State Service (Handles Meeting Mode State)
		/// </summary>
		private IServiceRequest StateService
		{
			get
			{
				if (oStateService == null)
				{
					oStateService = ServiceRequestFactory.GetProxy(GFWM.Shared.ServiceRequestFactory.SERVICES.STATE_SERVICE);
				}

				return oStateService;
			}
		}

		private IServiceRequest SWTService
		{
			get
			{
				if (oSWTService == null)
				{
					oSWTService = ServiceRequestFactory.GetProxy(GFWM.Shared.ServiceRequestFactory.SERVICES.SWT_SERVICE);
				}

				return oSWTService;
			}
		}


		private IServiceRequest AUMService
		{
			get
			{
				if (oAUMService == null)
				{
					oAUMService = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);
				}

				return oAUMService;
			}
		}

		/// <summary>
		/// Lazy loaded property that contains the Token that allows the authentication of the current user through all of the sites. Use this property do not use oSWTClaim directly
		/// </summary>
		public SWTClaim Claim
		{
			get
			{
				if (!IsTestMode && oSWTClaim == null)
				{
					if (PingClaim())
					{
						LoadClaim();
					}
					else
					{
                        if (!string.IsNullOrEmpty(LoginPage))
                        {
                            string loginPage = LoginPage;
                            
                            if (!string.IsNullOrEmpty(LoginPage_QueryString) && HttpContext.Current != null
                                && HttpContext.Current.Request != null
                                && !string.IsNullOrEmpty(HttpContext.Current.Request.RawUrl)
                                )
                            {
                                loginPage += LoginPage_QueryString + System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(HttpContext.Current.Request.RawUrl)));
                            }

                            Sitecore.Diagnostics.Log.Info("Authorization.Claim redirected to page: " + loginPage, this);
                            WebUtil.Redirect(loginPage);
                        }
                        else
                        {
                            Sitecore.Diagnostics.Log.Error("Unable to get url for login page. Review setting Genworth.SitecoreExt.Security.LoginPage", this);
                        }
					}
				}

				return oSWTClaim;
			}

		}

        public string UserName
        {
            get
            {
                IEnumerable<Agent> agents = GetAgent();
                if (agents != null)
                {
                    Agent agent = agents.FirstOrDefault();
                    if (agent != null)
                    {
                        return agent.DisplayName;
                    }
                }
                return string.Empty;
            }
        }

		/// <summary>
		/// Indicates whether the current request is running in meeting mode    
		/// </summary>
		public bool IsMeetingMode
		{
			get
			{
				if (!bIsMeetingModeLoaded)
				{
					LoadMeetingMode();
					bIsMeetingModeLoaded = true;
				}
				return bIsMeetingMode;
			}
		}

		/// <summary>
		/// Gets the codes for the roles/user levels associated to the current user
		/// </summary>
		public string[] UserLevels
		{
			get
			{
				if (oUserLevels == null)
				{
					LoadUserLevels();
				}

				return oUserLevels;
			}
		}

		/// <summary>
		/// Gets the channel types assocaited to the current user as an array of strings.
		/// </summary>
		public string[] Channels
		{
			get
			{
				if (oChannels == null)
				{
					LoadChannels();
				}
				return oChannels;
			}
		}

		/// <summary>
		/// Gets the codes of the custodias assocaited to the current user
		/// </summary>
		public string[] Custodians
		{
			get
			{
				if (oCustodians == null)
				{
					LoadCustodians();
				}
				return oCustodians;
			}
		}


		/// <summary>
		/// Gets the product codes for the products associated to the current user
		/// </summary>
		public string[] Products
		{
			get
			{
				if (oProducts == null)
				{
					LoadProducts();
				}
				return oProducts;
			}
		}

		/// <summary>
		/// Gets the preferences set for the advisor to the end client to access Managers/strategists
		/// </summary>
		public string[] ManagerStrategistPrivileges
		{
			get
			{
				if (oManagerStrategistPrivileges == null)
				{
					LoadClientManagerStrategistPrivileges();
				}
				return oManagerStrategistPrivileges;
			}
		}

        /// <summary>
        /// Gets the codes for the pc status associated to the current user
        /// </summary>
        public string[] PC_Status
        {
            get
            {
                if (oPC_Status == null)
                {
                    LoadPC_Status();
                }

                return oPC_Status;
            }
        }

		/// <summary>
		/// Gets the login page configured for all the sites. 
		/// </summary>
		public static string LoginPage
		{
			get
			{
				return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.LoginPage, string.Empty);
			}
		}

        /// <summary>
        /// Gets the login page configured for all the sites. 
        /// </summary>
        public static string LoginPage_QueryString
        {
            get
            {
                return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.LoginPageQueryString, string.Empty);
            }
        }

		/// <summary>
		/// Gets a pipe delimited list containing all the section that need to be secured by client aoproved
		/// </summary>
		public string[] ClientSecuredSections
		{
			get
			{
				string sSections;

				if (oClientSecuredSections == null)
				{
					if (!string.IsNullOrEmpty(sSections = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.ClientSecuredSectionsThroughClientApprovedField, string.Empty)))
					{
						oClientSecuredSections = sSections.Split('|');
					}
					else
					{
						Log.Error("Client secured sections have not been configured in the settings section", this);
						oClientSecuredSections = new string[0];
					}
				}

				return oClientSecuredSections;
			}
		}

		/// <summary>
		/// Gets the identifier for the client role
		/// </summary>
		int? iClientRoleId;
		public int ClientRoleId
		{
			get
			{
				if (!iClientRoleId.HasValue)
				{
					string sClientRoleId;
					int iClientRoleCode;
					ID oClientRoleItemId;
					Item oClientRoleItem;
					string sClientRoleCode;

					iClientRoleCode = RoleCodeNotFound;
					sClientRoleId = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.ClientRoleItem, string.Empty);

					if (!string.IsNullOrEmpty(sClientRoleId) && ID.TryParse(sClientRoleId, out oClientRoleItemId))
					{
						oClientRoleItem = ContextExtension.CurrentDatabase.GetItem(oClientRoleItemId);

						if (oClientRoleItem != null)
						{
							sClientRoleCode = oClientRoleItem.GetText(Genworth.SitecoreExt.Constants.Security.Templates.UserLevel.Sections.UserLevel.Name, Genworth.SitecoreExt.Constants.Security.Templates.UserLevel.Sections.UserLevel.Fields.CodeFieldName, string.Empty);

							if (!int.TryParse(sClientRoleCode, out iClientRoleCode))
							{
								iClientRoleCode = RoleCodeNotFound;
								Sitecore.Diagnostics.Log.Error("Unable to get the Client Role Code. This is information may not be correct in the meta data item", this);
							}
						}
						else
						{
							Sitecore.Diagnostics.Log.Error("Client Role Id is not set check the include configuration file or the meta data item", this);
						}
					}
					iClientRoleId = iClientRoleCode;
				}

				return iClientRoleId.Value;
			}
		}

		/// <summary>
		/// Gets the code/identifier for the agent role
		/// </summary>
		/// 
		int? iAgentRoleId;
		public int AgentRoleId
		{

			get
			{
				if (!iAgentRoleId.HasValue)
				{
					string sAgentRoleId;
					int iAgentRoleCode;
					ID oAgentRoleItemId;
					Item oAgentRoleItem;
					string sAgentRoleCode;

					iAgentRoleCode = RoleCodeNotFound;
					sAgentRoleId = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.AgentRoleItem, string.Empty);

					if (!string.IsNullOrEmpty(sAgentRoleId) && ID.TryParse(sAgentRoleId, out oAgentRoleItemId))
					{
						oAgentRoleItem = ContextExtension.CurrentDatabase.GetItem(oAgentRoleItemId);

						if (oAgentRoleItem != null)
						{
							sAgentRoleCode = oAgentRoleItem.GetText(Genworth.SitecoreExt.Constants.Security.Templates.UserLevel.Sections.UserLevel.Name, Genworth.SitecoreExt.Constants.Security.Templates.UserLevel.Sections.UserLevel.Fields.CodeFieldName, string.Empty);

							if (!int.TryParse(sAgentRoleCode, out iAgentRoleCode))
							{
								iAgentRoleCode = RoleCodeNotFound;
								Sitecore.Diagnostics.Log.Error("Unable to get the Agent Role Code. This is information may not be correct in the meta data item", this);
							}
						}
						else
						{
							Sitecore.Diagnostics.Log.Error("Agent Role Id is not set check the include configuration file or the meta data item", this);
						}
					}
					iAgentRoleId = iAgentRoleCode;
				}
				return iAgentRoleId.Value;
			}

		}

		/// <summary>
		/// Indicates whether the current user has the Client Role
		/// </summary>
		/// 
		bool? bIsClient;
		public bool IsClient
		{
			get
			{
				if (!bIsClient.HasValue)
				{
					SWTClaim oClaim = this.Claim;
                    bIsClient = !IsTestMode && oClaim.Roles != null ? oClaim.Roles.Any(r => r.RoleId == this.ClientRoleId) : false;
				}
				return bIsClient.Value;
			}
		}

        /// <summary>
        /// Indicates whether the current user has the Agent Role
        /// </summary>
        /// 
        bool? bIsAgent;
        public bool IsAgent
        {
            get
            {
                if (!bIsAgent.HasValue)
                {
                    SWTClaim oClaim = this.Claim;
                    bIsAgent = !IsTestMode && oClaim.Roles != null ? oClaim.Roles.Any(r => r.RoleId == this.AgentRoleId) : false;
                }
                return bIsAgent.Value;
            }
        }

        private string[] agentIds;
        public string[] AgentIds
        {
            get
            {
                if (this.agentIds == null)
                {
                    if (this.IsAgent) 
                    {
                            SWTClaim oClaim = this.Claim;

                            if (oClaim != null && oClaim.Roles != null && oClaim.Roles.Any(r => r.RoleId == this.AgentRoleId))
                            {
                                agentIds = oClaim.Roles.Where(r => r.RoleId == this.AgentRoleId).Select((a) => a.APLId).Distinct().ToArray();
                            }
                    }
                    else
                    {
                        agentIds = new string[0];
                    }
                 }
           
                return agentIds;
            }
        }

		#endregion

		#region STATIC ACCESS

		/// <summary>
		/// Exposes the authorization object for the current request
		/// </summary>
		public static Authorization CurrentAuthorization
		{
			get
			{
				Authorization oAuthorization;
				string sKey;

				sKey = Constants.Settings.Security.AuthorizationObject;
				if (HttpContext.Current != null)
				{
					if ((oAuthorization = (Authorization)HttpContext.Current.Items[sKey]) == null)
					{
						//create and store the authorization
						HttpContext.Current.Items[sKey] = oAuthorization = new Authorization();
					}
				}
				else
				{
					oAuthorization = null;
				}
				return oAuthorization;
			}
		}
		#endregion

		#region CONSTRUCTORS

		private Authorization()
		{

		}


		#endregion




		#region METHODS


		#region HELPER METHODS FOR TEST MODE

		/// <summary>
		/// Loads testing cofniguration from the test configuration file
		/// </summary>
		/// <param name="sLevelType"></param>
		/// <param name="sLevelName"></param>
		/// <returns></returns>
		private string[] LoadLevelFromTestAccount(string sLevelType, string sLevelName)
		{
			string[] oValuesToReturn;

			oValuesToReturn = null;

			if (IsTestMode)
			{
				if (TestAccount != null)
				{
					if (TestAccount.Element(sLevelType) != null)
					{
						oValuesToReturn = (
										from
											tLevel
											in
											TestAccount.Element(sLevelType).Elements(sLevelName)
										select
											tLevel.Attribute("Name") != null ? tLevel.Attribute("Name").Value : string.Empty
										).ToArray();
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error("Unable to get Test Account file", this);
					throw new Exception("Unable to get Test Account file");
				}
			}
			else
			{
				oValuesToReturn = new string[0];
			}

			return oValuesToReturn;
		}
		#endregion



		/// <summary>
		/// GG Use this approach to make a SWT Claim. No need to read SWT Cookie anymore
		/// Every response will return the latest SWTClaim
		/// Make the response call for EACH request so that you get the latest SWTClaim. The token renews periodically.
		/// </summary>
		/// <returns></returns>
		private GFWM.Shared.Entity.Data.SWTClaim GetSWTClaims()
		{
			IServiceRequest stateService;
			GetUserPreferenceRequest oSWTClaimRequest;
			SWTClaim oClaimReceived;
			string[] oSWTClaimRequestParameters;
			UserPreferenceResponse oSWTClaimResponse;

			oClaimReceived = null;
			stateService = this.StateService;
			oSWTClaimRequest = new GetUserPreferenceRequest();
			oSWTClaimRequestParameters = new string[] { SWTClaimRequestParameterKey };
			oSWTClaimRequest.Keys = oSWTClaimRequestParameters;
			oSWTClaimResponse = null;
			// validate access to the site by Bookmark
			try
			{
				oSWTClaimResponse = ServiceRequest_Ext.Request<GetUserPreferenceRequest, UserPreferenceResponse>(stateService, oSWTClaimRequest);
			}
			catch
			{
				oClaimReceived = null;
			}

			if (oSWTClaimResponse != null && oSWTClaimResponse.Preferences != null)
			{
				oClaimReceived = (
									from
										oPreference
									 in
										oSWTClaimResponse.Preferences
									where
										oPreference.Value is SWTClaim &&
										string.Compare(oPreference.Key, SWTClaimRequestParameterKey, true) == 0
									select
										oPreference.Value as SWTClaim
									).FirstOrDefault();

				if (oClaimReceived == null)
				{
					Log.Error("Cast may have failed!!!", typeof(Authorization));
					throw new SessionExpiredException("Cast may have failed!!!");
				}
			}


			return oClaimReceived;
		}

		/// <summary>
		/// Loads the SWT Claim
		/// </summary>
		private void LoadClaim()
		{
			if (!this.IsTestMode)
			{
				this.oSWTClaim = this.GetSWTClaims();
				//Log.Info("Claim Load attempted", this);
			}

		}

		/// <summary>
		/// Verifies the state of the claim
		/// </summary>
		/// <param name="transactionName"></param>
		/// <returns>True if a valid claim, false otherwise</returns>
		public bool PingClaim()
		{
			IServiceRequest oClientPing;
			PingResponse oPingResponse;
			bool bClaimStatus;
			PingRequest oPingRequest;

			oClientPing = null;
			oPingResponse = null;
			bClaimStatus = false;
			try
			{

				oClientPing = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

				oPingRequest = new PingRequest();

				if (null != oClientPing)
				{
					oPingResponse = oClientPing.Request<PingRequest, PingResponse>(oPingRequest);
					if (null != oPingResponse)
					{
						if (oPingResponse.Fault != null)
						{
							// log error
							Sitecore.Diagnostics.Log.Error(oPingResponse.Fault, this);

							bClaimStatus = false;
						}
						else
						{
							// Ping responded correctly
							bClaimStatus = true;
						}
					}
				}

			}
			catch (Exception oPingException)
			{
				Sitecore.Diagnostics.Log.Error("Unable to check claim status", oPingException, this);
			}
			return bClaimStatus;
		}

        public IEnumerable<Agent> GetAgent()
        {
            IServiceRequest proxy;
            AgentResponse response;
            AgentRequest request;

            proxy = null;
            response = null;
            try
            {

                proxy = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

                request = new AgentRequest();
                request.Filter = string.Format("(APLId == '{0}')", this.AgentIds[0]);

                if (null != proxy)
                {
                    response = proxy.Request<AgentRequest, AgentResponse>(request);
                    if (null != response)
                    {
                        if (response.Fault != null)
                        {
                            // log error
                            Sitecore.Diagnostics.Log.Error(response.Fault, this);
                        }
                        else
                        {
                            return response.Agents;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get Agent data from SWT serice", ex, this);
            }
            return new List<Agent>();
        }

        public IEnumerable<Agent> GetAgents()
        {
            IServiceRequest proxy;
            AgentResponse response;
            AgentRequest request;

            proxy = null;
            response = null;

            try
            {
                if (this.AgentIds.Count() > 0)
                {
                    proxy = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

                    request = new AgentRequest();

                    //Build filter
                    StringBuilder agentBuilder = new StringBuilder();
                    for (int i = 0; i < this.AgentIds.Length; i++)
                    {
                        agentBuilder.AppendFormat("(APLId == '{0}')", this.AgentIds[i]);
                        if (i != this.AgentIds.Length - 1)
                            agentBuilder.Append("||");
                    }

                    request.Filter = agentBuilder.ToString();

                    if (null != proxy)
                    {
                        response = proxy.Request<AgentRequest, AgentResponse>(request);
                        if (null != response)
                        {
                            if (response.Fault != null)
                            {
                                // log error
                                Sitecore.Diagnostics.Log.Error(response.Fault, this);
                            }
                            else
                            {
                                return response.Agents;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get Agent data from SWT serice", ex, this);
            }
            return new List<Agent>();
        }

        public IEnumerable<UserRole> GetRoles()
        {
            IServiceRequest proxy = null;
            UserRoleRequest request = null;
            UserRoleResponse response = null;

            SWTClaim claim = this.Claim;
            string ssoguid = claim.SSOGuid;
            int defaultLoginRoleId = claim.DefaultLoginRoleId;

            try
            {
                request = new UserRoleRequest();
                request.SsoGuId = ssoguid;
                request.TokenValues = new GFWM.Shared.Entity.Data.SWTClaim()
                {
                    DefaultLoginRoleId = defaultLoginRoleId
                };

                proxy = ServiceRequestFactory.GetProxy(SERVICES.SWT_SERVICE);
                response = proxy.Request<UserRoleRequest, UserRoleResponse>(request);

                if (null != response)
                {
                    if (response.Fault != null)
                    {
                        // log error
                        Sitecore.Diagnostics.Log.Error(response.Fault, this);
                    }
                    else
                    {
                        return response.UserRole;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get user role data for SSOGUID " + ssoguid + " from SWT service", ex, this);
                throw ex;
            }
            return new List<UserRole>();
        }


		/// <summary>
		/// Sets the state of the meeting mode user preference to the value given as parameter 
		/// </summary>
		/// <param name="bMeetingModeValue"></param>
		/// <returns>True if the value was successfully set false otherwise</returns>
		public bool SaveMeetingModeState(bool bMeetingModeValue)
		{

			#region VARIABLES
			UserPreferenceResponse oResponse;
			SaveUserPrferenceRequest oRequest;
			List<UserPreference> oUserPreferences;
			bool bResponseResult;

			#endregion

			bResponseResult = false;

			if (IsTestMode)
			{
				bIsMeetingMode = bMeetingModeValue;
			}
			else
			{

				oRequest = new SaveUserPrferenceRequest();

				oUserPreferences = new List<UserPreference>();
				oUserPreferences.Add(new UserPreference()
				{
					Key = Genworth.SitecoreExt.Constants.Security.Authorization.MeetingModeKey,
					Value = bMeetingModeValue.ToString()
				}
										);

				oRequest.UserPrefences = oUserPreferences;
				oRequest.Persist = false;

				oResponse = StateService.Request<SaveUserPrferenceRequest, UserPreferenceResponse>(oRequest);

				if (oResponse == null || (oResponse != null && oResponse.Fault != null))
				{
					Sitecore.Diagnostics.Log.Error("SaveUserPreferenceRequest was unable to set the meeting mode check Sate Service", this);
				}
				else
				{

					bResponseResult = true;
				}

			}

			return bResponseResult;
		}

		/// <summary>
		/// Loads the IsMeetingMode status from the Transient data Service
		/// </summary>
		private void LoadMeetingMode()
		{
			Sitecore.Diagnostics.Log.Debug("LoadMeetingMode - Begins", this);

			#region VARIABLES

			GetUserPreferenceRequest oRequest;
			UserPreferenceResponse oResponse;
			string[] oRequestParameters;

			#endregion

			if (IsTestMode)
			{
				bIsMeetingMode = TestAccount != null && TestAccount.Element("MeetingMode") != null && TestAccount.Element("MeetingMode").Attribute("Enabled") != null && bool.TryParse(TestAccount.Element("MeetingMode").Attribute("Enabled").Value, out bIsMeetingMode) ? bIsMeetingMode : false;
			}
			else
			{
				oRequest = new GetUserPreferenceRequest();

				oRequestParameters = new string[] { MeetingModeRequestParameterKey };

				oRequest.Keys = oRequestParameters;

				oResponse = ServiceRequest_Ext.Request<GetUserPreferenceRequest, UserPreferenceResponse>(this.StateService, oRequest);

				if (oResponse != null)
				{
					bool.TryParse(
									(
										from
											oUserPreference
											in 
                                            oResponse.Preferences
										where 
                                            oUserPreference != null && 
                                            object.Equals(oUserPreference.Key, MeetingModeRequestParameterKey) && 
                                            oUserPreference.Value != null
										select 
                                            oUserPreference.Value.ToString()
									).FirstOrDefault<string>(), out this.bIsMeetingMode
								);
					   
				}
				else
				{
					Log.Error("Unable to execute GetUserPreferenceRequest from State Service ", this);
				}


			}
			Sitecore.Diagnostics.Log.Debug("LoadMeetingMode - Ends", this);
		}

		/// <summary>
		/// Loads the advisors associated to the current user/claim
		/// </summary>
		/// <returns></returns>
		public string[] GetAdvisors()
		{
			SWTClaim oClaim;

			Sitecore.Diagnostics.Log.Debug("GetAdvisors - Begins", this);

			if (!IsTestMode)
			{
				oClaim = this.Claim;

				if (null != oClaim && oClaim.Roles != null && oClaim.Roles.Any())
				{
					oAdvisors = oClaim.Roles.Select((a) => a.APLId).Distinct().ToArray();
				}
				else
				{
					if (oClaim == null)
					{
						Sitecore.Diagnostics.Log.Warn("Claim is null", this);
					}
					else
					{
						Sitecore.Diagnostics.Log.Warn("Claim roles are null", this);
					}
					Sitecore.Diagnostics.Log.Error("Unable to Load Advisors", this);
				}
			}
			else
			{
				oAdvisors = LoadLevelFromTestAccount("Advisors", "Advisor");
			}

			Sitecore.Diagnostics.Log.Debug("GetAdvisors - Ends", this);

			return oAdvisors;
		}

		/// <summary>
		/// Loads the codes for the roles associated to the current user
		/// </summary>
		private void LoadUserLevels()
		{
			int iUserLevelsCount;
			int iUserLevelIndex;
			SWTClaim oClaim;

			if (IsTestMode)
			{
				oUserLevels = LoadLevelFromTestAccount("UserLevels", "UserLevel");
			}
			else
			{
				oClaim = Claim;

				if (null != oClaim && oClaim.Roles != null && (iUserLevelsCount = oClaim.Roles.Count()) > 0)
				{
					oUserLevels = new string[iUserLevelsCount];

					for (iUserLevelIndex = 0; iUserLevelIndex < iUserLevelsCount; iUserLevelIndex++)
					{
						oUserLevels[iUserLevelIndex] = oClaim.Roles[iUserLevelIndex].SiteCoreRoleId.ToString();
					}
				}
				else
				{
					if (oClaim == null)
					{
						Sitecore.Diagnostics.Log.Warn("Claim is null", this);
					}
					else
					{
						Sitecore.Diagnostics.Log.Warn("Claim roles are null", this);
					}
					Sitecore.Diagnostics.Log.Error("Unable to Load User Levels", this);
				}
			}
		}

		/// <summary>
		/// Loads the channels associated to the current user/claim
		/// </summary>
		private void LoadChannels()
		{
			Sitecore.Diagnostics.Log.Debug("LoadChannels - Begins", this);
			SWTClaim oClaim;

			if (IsTestMode)
			{
				oChannels = LoadLevelFromTestAccount("Channels", "Channel");
			}
			else
			{
				oClaim = Claim;
                List<string> oChannelList = new List<string>();

                if (oClaim != null && oClaim.Roles != null && oClaim.Roles.Length > 0)
				{
					foreach (var oRole in oClaim.Roles)
					{
                        if (!string.IsNullOrWhiteSpace(oRole.ChannelType))
                        {
                            oChannelList.Add(oRole.ChannelType);
                            Sitecore.Diagnostics.Log.Debug(string.Format("Channel {0} loaded", oRole.ChannelType), this);
                        }
                    }

                    if(oChannelList.Count > 0)
                    { 
                        oChannels = oChannelList.ToArray();
                    }
				}
				else
				{
					if (oClaim == null)
					{
						Sitecore.Diagnostics.Log.Warn("Claim is null", this);
					}
					else
					{
						Sitecore.Diagnostics.Log.Warn("Claim roles are null", this);
					}
					Sitecore.Diagnostics.Log.Error("Unable to pull channels from claim", this);
				}

			}

			Sitecore.Diagnostics.Log.Debug("LoadChannels - Ends", this);
		}

		/// <summary>
		/// Loads the custodians associated to the current user/claim
		/// </summary>
		private void LoadCustodians()
		{

			int iCustodiansCount;
			int iCustodianIndex;
			CustodianCatalogRequest oCustodiansRequest;
			CustodianCatalogResponse oCustodiansResponse;
			int iInitialCusrodianInResponse;

			iInitialCusrodianInResponse = 0;

			if (IsTestMode)
			{
				oCustodians = LoadLevelFromTestAccount("Custodians", "Custodian");
			}
			else
			{

				oCustodiansRequest = new CustodianCatalogRequest();

				oCustodiansResponse = this.AUMService.Request<CustodianCatalogRequest, CustodianCatalogResponse>(oCustodiansRequest);

				if (oCustodiansResponse.Custodians != null)
				{
					iCustodiansCount = oCustodiansResponse.Custodians.Count();

					oCustodians = new string[iCustodiansCount];

					iCustodianIndex = iInitialCusrodianInResponse;

					foreach (CustodianCatalog custodian in oCustodiansResponse.Custodians)
					{
						oCustodians[iCustodianIndex] = custodian.CustodianId;
						iCustodianIndex++;

					}

				}


			}
		}

        /// <summary>
        /// Loads the pc status associated to the current user/claim
        /// </summary>
        private void LoadPC_Status()
        {
            if (IsTestMode)
            {
                oPC_Status = LoadLevelFromTestAccount("PcStatusList", "PcStatus");
            }
            else
            {
                if (this.Claim != null && Claim.Roles != null && Claim.Roles.Any(r => r.RoleId == this.AgentRoleId))
                {
                    //We will get the client Id from the claim (I am assuming that one client can have more than one client Id associated to it)
                    RoleClaim[] oAgentRoleClaims = Claim.Roles.Where(r => r.RoleId == this.AgentRoleId).ToArray();

                    if (oAgentRoleClaims != null && oAgentRoleClaims.Length > 0)
                    {
                         oPC_Status = new string[oAgentRoleClaims.Length];

                         for(int i =0; i < oAgentRoleClaims.Length; i++ )
                         {
                              oPC_Status[i] = oAgentRoleClaims[i].PC_Status;
                         }
                                           
                    }
                }
            }
        }
		#region END-CLIENT AUTHORIZATION



		/// <summary>
		/// Loads the relation client-agent-channel for the current user (Only for users with the client role)
		/// </summary>
		private void LoadClientAgents()
		{
			#region VARIABLES

			IServiceRequest oAUMServiceProxy;
			RoleClaim[] oClientRoleClaims;
			GFWM.Common.AUM.Entities.Request.Client.ClientRequest oAUMServiceRequest;
			GFWM.Common.AUM.Entities.Response.Client.ClientResponse oAUMServiceResponse;
			string sClientId;
			ClientAgent oClientAgent;

			#endregion

			if (IsTestMode)
			{
				oManagerStrategistPrivileges = LoadLevelFromTestAccount("ManagerStrategistPrivileges", "ManagerStrategistPrivilege");
			}
			else
			{
				oClientAgentPreferences = new List<ClientAgent>();

				if (this.Claim != null && Claim.Roles != null && Claim.Roles.Any(r => r.RoleId == this.ClientRoleId))
				{
					//We will get the client Id from the claim (I am assuming that one client can have more than one client Id associated to it)
					oClientRoleClaims = Claim.Roles;

					if (oClientRoleClaims != null && oClientRoleClaims.Length > 0)
					{
						try
						{
							oAUMServiceProxy = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

							foreach (RoleClaim oClientRoleClaim in oClientRoleClaims)
							{
								sClientId = oClientRoleClaim.APLId;

								//Request that gets the agents associated to the current client id
								oAUMServiceRequest = new GFWM.Common.AUM.Entities.Request.Client.ClientRequest()
								{
									Filter = string.Format("(APLId == '{0}')", sClientId)
								};

								oAUMServiceResponse = oAUMServiceProxy.Request<GFWM.Common.AUM.Entities.Request.Client.ClientRequest, GFWM.Common.AUM.Entities.Response.Client.ClientResponse>(oAUMServiceRequest);

								if (oAUMServiceResponse != null)
								{

									foreach (GFWM.Common.AUM.Entities.Data.Client clientData in oAUMServiceResponse.Clients)
									{
										oClientAgent = new ClientAgent()
										{
											ClientId = sClientId,
											AgentId = clientData.AgentId,
											ChannelType = oClientRoleClaim.ChannelType
										};

										oClientAgentPreferences.Add(oClientAgent);
									}
                                    Log.Info(string.Format("LoadClientAgents oClientAgentPreferences[{0}]", oClientAgentPreferences.Count()), this);
								}

							}
						}
						catch (Exception LoadingAgentsException)
						{
							Sitecore.Diagnostics.Log.Error("LoadManagerStrategistPrivileges Unable to load client identifiers (APLId) is null", LoadingAgentsException, this);
							throw;
						}
					}
					else
					{
						Sitecore.Diagnostics.Log.Error("LoadManagerStrategistPrivileges Unable to load client identifiers (APLId) is null", this);
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error("LoadManagerStrategistPrivileges Claim or Roles is null or the current user is not an end client", this);
				}

			}
		}

		/// <summary>
		/// Loads Agents preferences for clients against strategists-managers (Applies only for end clients)
		/// </summary>
		private void LoadClientManagerStrategistPrivileges()
		{
			#region VARIABLES

			List<ClientAgent> oClientAgentList;
			IServiceRequest oPreferenceServiceProxy;
			ClientInvestmentOptionRequest oPreferenceRequest;
			ClientInvestmentOptionResponse oPreferenceResponse;
			int iCurrentPreferenceTypeCD;
			List<GFWM.Common.Preference.Entities.Data.ClientInvestmentOption> oTmpCurrentOptions = new List<GFWM.Common.Preference.Entities.Data.ClientInvestmentOption>();
			List<GFWM.Common.Preference.Entities.Data.ClientInvestmentOption> oTmpAditionalOptions = new List<GFWM.Common.Preference.Entities.Data.ClientInvestmentOption>();
			
            List<string> oPreferencesList;
            UserPageSettingRequest oUserPageSettingRequest;
            UserPageSettingResponse oUserPageSettingResponse;
            

            string NoPreferenceSet = "0";
            SWTClaim oClaim;
            List<string> oPreferencesForCurrentMapping;

            string filter = "false";
            int[] PreferenceTypeCDList = { 1806 };

			#endregion

			if (IsTestMode)
			{
				if (TestAccount != null)
				{
					if (TestAccount.Element("ClientInvestmentOptions") != null)
					{
						oTmpCurrentOptions = (from
													oClientInvestmentOption
													in
												   TestAccount.Element("ClientInvestmentOptions").Element("CurrentOptions").Elements("ClientInvestmentOption")
											   select new GFWM.Common.Preference.Entities.Data.ClientInvestmentOption
											   {
												   HasInvestmentManagerFee = oClientInvestmentOption.Attribute("HasInvestmentManagerFee").Value.Equals("1") ? true : false,
												   StrategistCode = oClientInvestmentOption.Attribute("StrategistCode").Value,
												   AssetAllocationName = oClientInvestmentOption.Attribute("AssetAllocationId").Value,
												   ProgramId = oClientInvestmentOption.Attribute("SolutionTypeId").Value
											   }).ToList();


						oTmpAditionalOptions = (from
													oClientInvestmentOption
													in
													 TestAccount.Element("ClientInvestmentOptions").Element("AdditionalOptions").Elements("ClientInvestmentOption")
												 select new GFWM.Common.Preference.Entities.Data.ClientInvestmentOption
												 {
													 HasInvestmentManagerFee = oClientInvestmentOption.Attribute("HasInvestmentManagerFee").Value.Equals("1") ? true : false,
													 StrategistCode = oClientInvestmentOption.Attribute("StrategistCode").Value,
													 AssetAllocationName = oClientInvestmentOption.Attribute("AssetAllocationId").Value,
													 ProgramId = oClientInvestmentOption.Attribute("SolutionTypeId").Value
												 }).ToList();

					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error("Unable to get Test Account file", this);
					throw new Exception("Unable to get Test Account file");
				}

			}
			else
			{

				oPreferencesList = new List<string>();
				oClaim = this.Claim;

                if (oClaim != null && oClaim.Roles != null && oClaim.Roles.Any(r => r.RoleId == this.ClientRoleId))
                {
                    oClientAgentList = ClientAgentPreferences;

                    if (oClientAgentList != null)
                    {
                        oPreferenceServiceProxy = ServiceRequestFactory.GetProxy(SERVICES.PREFERENCE_SERVICE);
                        Log.Debug(string.Format("ClientManagerStrategistPrivileges oClientAgentList[{0}]", oClientAgentList.Count()), this);
                        foreach (var oClientAgentPref in oClientAgentList)
                        {
                            oPreferencesForCurrentMapping = new List<string>();

                            foreach (var prefType in PreferenceTypeCDList)
                            {
                                filter = "((" + filter + ") || (PreferenceTypeCD == " + prefType + "))";
                            }

                            oUserPageSettingRequest = new UserPageSettingRequest()
                            {
                                AgentID = oClientAgentPref.ClientId,
                                Filter = filter
                            };

                            oUserPageSettingResponse = oPreferenceServiceProxy.Request<UserPageSettingRequest, UserPageSettingResponse>(oUserPageSettingRequest);

                            if (oUserPageSettingResponse != null)
                            {
                                foreach (GFWM.Common.Preference.Entities.Data.UserPageSetting oUserSetting in oUserPageSettingResponse.UserPageSetting)
                                {
                                    if (string.Equals(oUserSetting.APLID, oClientAgentPref.ClientId))
                                    {
                                        //Preference for current client id, agent id and channel type
                                        oPreferencesList.Add(oUserSetting.DisplaySetting.ToString());
                                        oPreferencesForCurrentMapping.Add(oUserSetting.DisplaySetting.ToString());
                                    }
                                }

                                oClientAgentPref.Preferences = oPreferencesForCurrentMapping.ToArray<string>();
                                oClientAgentPref.PrefencesLoaded = true;
                                Log.Debug(string.Format("ClientManagerStrategistPrivileges oClientAgentPref.Preferences[{0}]", oClientAgentPref.Preferences.Count()), this);
                                foreach (string oPreferenceType in oClientAgentPref.Preferences)
                                {
                                    if (int.TryParse(oPreferenceType, out iCurrentPreferenceTypeCD))
                                    {
                                        oPreferenceRequest = new ClientInvestmentOptionRequest()
                                        {
                                            PreferenceTypeCD = iCurrentPreferenceTypeCD,
                                            AgentID = oClientAgentPref.AgentId,
                                            ClientId = oClientAgentPref.ClientId
                                        };

                                        oPreferenceResponse = oPreferenceServiceProxy.Request<ClientInvestmentOptionRequest, ClientInvestmentOptionResponse>(oPreferenceRequest);
                                        if (oPreferenceResponse.CurrentOptions != null)
                                        {
                                            lock (oTmpCurrentOptions)
                                            {

                                                oTmpCurrentOptions.AddRange(oPreferenceResponse.CurrentOptions);
                                            }
                                        }
                                        if (oPreferenceResponse.AdditionalOptions != null)
                                        {
                                            lock (oTmpAditionalOptions)
                                            {
                                                oTmpAditionalOptions.AddRange(oPreferenceResponse.AdditionalOptions);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        Log.Error("Unable to execute LoadManagerStrategistPrivileges because no client-channel-agent information was found ", this);
                    }


                }
            }
            oAditionalInvestments = oTmpAditionalOptions.ToArray();
            oCurrentInvestments = oTmpCurrentOptions.ToArray();
            oManagerStrategistPrivileges = oAditionalInvestments.Select(oAinvestment => oAinvestment.StrategistCode).Concat(oCurrentInvestments.Select(oCinvestment => oCinvestment.StrategistCode)).ToArray();
            this.oProducts = oAditionalInvestments.Select(oAinvestment => oAinvestment.ProgramId).Concat(oCurrentInvestments.Select(oCinvestment => oCinvestment.ProgramId)).Distinct().ToArray();
        }

		#region PRODUCTS LOADING


		private void LoadProducts()
		{
			SWTClaim oClaim;
			int iClientRoleId;

			if (IsTestMode)
			{
				oProducts = LoadLevelFromTestAccount("Products", "Product");
			}
			else
			{

				oClaim = this.Claim;

				if (oClaim != null && oClaim.Roles != null)
				{
					iClientRoleId = this.ClientRoleId;
					if (oClaim.Roles.Any(r => r.RoleId == iClientRoleId))
					{
						//This means that we need to load the products for an end client
						LoadClientProducts();
					}
					else
					{
						//At this point we know that the current user is not an end client so we are going to load the products catalog
						LoadProductsCatalog();
					}
				}
			}
		}

		private void LoadClientProducts()
		{
			//#region VARIABLES

			//List<ClientAgent> oClientAgentList;
			//IServiceRequest oPreferenceServiceProxy;
			//ClientInvestmentOptionRequest oPreferenceRequest;
			//ClientInvestmentOptionResponse oPreferenceResponse;
			//List<string> oClientProducts;
			//int iCurrentPreferenceTypeCD;

			//#endregion

			//oClientAgentList = ClientAgentPreferences;

			//if (oClientAgentList != null)
			//{
			//    oPreferenceServiceProxy = ServiceRequestFactory.GetProxy(SERVICES.PREFERENCE_SERVICE);
			//    oClientProducts = new List<string>();
			//    foreach (var oClientAgentPreference in oClientAgentList)
			//    {

			//        if (!oClientAgentPreference.PrefencesLoaded)
			//        {
			//            LoadClientManagerStrategistPrivileges();
			//        }

			//        foreach (string oPreferenceType in oClientAgentPreference.Preferences)
			//        {
			//            if (int.TryParse(oPreferenceType, out iCurrentPreferenceTypeCD))
			//            {
			//                oPreferenceRequest = new ClientInvestmentOptionRequest()
			//                {
			//                    PreferenceTypeCD = iCurrentPreferenceTypeCD,
			//                    AgentID = oClientAgentPreference.AgentId,
			//                    ClientId = oClientAgentPreference.ClientId
			//                };

			//                oPreferenceResponse = oPreferenceServiceProxy.Request<ClientInvestmentOptionRequest, ClientInvestmentOptionResponse>(oPreferenceRequest);

			//                if (oPreferenceResponse != null)
			//                {
			//                    foreach (GFWM.Common.Preference.Entities.Data.ClientInvestmentOption option in oPreferenceResponse.CurrentOptions)
			//                    {
			//                        //We are assuming that program = product
			//                        oClientProducts.Add(option.ProgramId);
			//                    }
			//                }
			//            }
			//        }
			//    }

			//    this.oProducts = oClientProducts.ToArray<string>();
			//}
			//else
			//{
			//    Sitecore.Diagnostics.Log.Error("LoadClientProducts - Unable to load end-client products", this);

			//}

			if (this.oProducts == null)
				LoadClientManagerStrategistPrivileges();

		}


		/// <summary>
		/// Loads an unfiltered catalog of products
		/// </summary>
		private void LoadProductsCatalog()
		{
			int iProductsCount;
			int iProductIndex;
			ProductCatalogRequest oProductsRequest;
			ProductCatalogResponse oProductsResponse;

			oProductsRequest = new ProductCatalogRequest();
			oProductsResponse = this.AUMService.Request<ProductCatalogRequest, ProductCatalogResponse>(oProductsRequest);

			if (oProductsResponse != null)
			{
				if (oProductsResponse.Products != null)
				{
					iProductsCount = oProductsResponse.Products.Count();
					oProducts = new string[iProductsCount];
					iProductIndex = 0;
					foreach (ProductCatalog product in oProductsResponse.Products)
					{
						oProducts[iProductIndex] = product.ProgramId;
						iProductIndex++;
					}
				}
			}
		}


		#endregion


		#endregion

		/// <summary>
		/// Retrieves the open notifications for the current user
		/// </summary>
		/// <returns></returns>
		public int GetNotifications()
		{
			#region VARIABLES

			IServiceRequest oClientAUMService;
			WQItemsCountRequest oNotificationsRequest;
			WQItemsCountResponse oNotificationsResponse;
			int iNoOpenNotificationsFound;
			int iOpenNotifications;

			#endregion

			iOpenNotifications = iNoOpenNotificationsFound = 0;

			if (IsTestMode)
			{
				if (TestAccount != null && TestAccount.Attribute("OpenNotifications") != null)
				{
					if (!int.TryParse(TestAccount.Attribute("OpenNotifications").Value, out iOpenNotifications))
					{
						iOpenNotifications = iNoOpenNotificationsFound;
					}
				}
			}
			else
			{
				oClientAUMService = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

				oNotificationsRequest = new WQItemsCountRequest()
				{
					StartDate = null,
					EndDate = null,
					Length = 0,
					Start = 0
				};

				oNotificationsResponse = oClientAUMService.Request<WQItemsCountRequest, WQItemsCountResponse>(oNotificationsRequest);

				iOpenNotifications = oNotificationsResponse != null ? oNotificationsResponse.AdvisorResponseCount : iNoOpenNotificationsFound;
			}

			return iOpenNotifications;
		}


		#endregion


		public void GetManagersAndStrategists(out List<Genworth.SitecoreExt.Helpers.InvestmentHelper.Strategist> oStrategists, out List<Item> oManagers, out List<Genworth.SitecoreExt.Helpers.InvestmentHelper.Strategist> oAdditionalStrategists)
		{
			#region VARIABLES
			Item oItem;  // this will hold either a manager or strategist
			Dictionary<string, Genworth.SitecoreExt.Helpers.InvestmentHelper.Strategist> oTmpStrategist = new Dictionary<string, Helpers.InvestmentHelper.Strategist>();
			Dictionary<string, Genworth.SitecoreExt.Helpers.InvestmentHelper.Strategist> oTmpAdditionalStrategists = new Dictionary<string, Helpers.InvestmentHelper.Strategist>();
			// This List will have all the managers followed by all the strategists from sitecore tree
			List<Item> oAllManagersAndStrategists;
			oAllManagersAndStrategists = Investments.Items.ManagersFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Manager);
			oAllManagersAndStrategists.AddRange(Investments.Items.StrategitsFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategist));
			oManagers = new List<Item>();

			#endregion
			if (oManagerStrategistPrivileges == null)
				LoadClientManagerStrategistPrivileges();
			foreach (GFWM.Common.Preference.Entities.Data.ClientInvestmentOption option in oCurrentInvestments)
			{

				oItem = (option.HasInvestmentManagerFee) ? oAllManagersAndStrategists.FirstOrDefault(oManagerOrStrategist => oManagerOrStrategist.GetText("Code") == option.StrategistCode) : oAllManagersAndStrategists.LastOrDefault(oManagerOrStrategist => oManagerOrStrategist.GetText("Code") == option.StrategistCode);

				if (oItem != null)
				{
                    Sitecore.Diagnostics.Log.Debug(string.Format("CurrentOptions ProgramId[{0}] ,HasInvestmentManagerFee[{1}], ProgramName[{2}], StrategistCode[{3}], StrategistName[{4}], AllocationId[{5}], AllocationName[{6}]", option.ProgramId, option.HasInvestmentManagerFee, option.ProgramName, option.StrategistCode, option.StrategistName, option.AssetAllocationShortName, option.AssetAllocationName), this);
						if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategist))
						{
							if (oTmpStrategist.ContainsKey(option.StrategistCode))
								oTmpStrategist[option.StrategistCode].AddSolution(option.AssetAllocationName, option.ProgramId);
							else
							{
								Genworth.SitecoreExt.Helpers.InvestmentHelper.Strategist oStrategis = new Helpers.InvestmentHelper.Strategist(oItem);
								oStrategis.AddSolution(option.AssetAllocationName, option.ProgramId);
								oTmpStrategist.Add(option.StrategistCode, oStrategis);
							}
						}
						else
						{
							oManagers.Add(oItem);
						}
					
				}
				else
                    Sitecore.Diagnostics.Log.Debug(string.Format("CurrentOption Not found ProgramId[{0}] ,HasInvestmentManagerFee[{1}], ProgramName[{2}], StrategistCode[{3}], StrategistName[{4}], AllocationId[{5}], AllocationName[{6}]", option.ProgramId, option.HasInvestmentManagerFee, option.ProgramName, option.StrategistCode, option.StrategistName, option.AssetAllocationShortName, option.AssetAllocationName), this);
			}

            /*foreach (GFWM.Common.Preference.Entities.Data.ClientInvestmentOption option in oAditionalInvestments)
            {
									

                oItem = (option.HasInvestmentManagerFee) ? oAllManagersAndStrategists.FirstOrDefault(oManagerOrStrategist => oManagerOrStrategist.GetText("Code") == option.StrategistCode) : oAllManagersAndStrategists.LastOrDefault(oManagerOrStrategist => oManagerOrStrategist.GetText("Code") == option.StrategistCode);

                if (oItem != null)
                {
                    Sitecore.Diagnostics.Log.Info(string.Format("AdditionalOptions ProgramId[{0}] ,HasInvestmentManagerFee[{1}], ProgramName[{2}], StrategistCode[{3}], StrategistName[{4}], AllocationId[{5}], AllocationName[{6}]", option.ProgramId, option.HasInvestmentManagerFee, option.ProgramName, option.StrategistCode, option.StrategistName, option.AssetAllocationShortName, option.AssetAllocationName), this);
                    if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategist))
                    {
                            if (oTmpAdditionalStrategists.ContainsKey(option.StrategistCode))
                                oTmpAdditionalStrategists[option.StrategistCode].AddSolution(option.AssetAllocationName, option.ProgramId);
                            else
                            {
                                Genworth.SitecoreExt.Helpers.InvestmentHelper.Strategist oStrategis = new Helpers.InvestmentHelper.Strategist(oItem);
                                oStrategis.AddSolution(option.AssetAllocationName, option.ProgramId);
                                oTmpAdditionalStrategists.Add(option.StrategistCode, oStrategis);
                            }
						
                    }
                }
                else
                    Sitecore.Diagnostics.Log.Info(string.Format("AdditionalOptions Not found ProgramId[{0}] ,HasInvestmentManagerFee[{1}], ProgramName[{2}], StrategistCode[{3}], StrategistName[{4}], AllocationId[{5}], AllocationName[{6}]", option.ProgramId, option.HasInvestmentManagerFee, option.ProgramName, option.StrategistCode, option.StrategistName,option.AssetAllocationShortName,option.AssetAllocationName), this);
            }*/

            oAdditionalStrategists = oTmpAdditionalStrategists.Values.ToList();
			oStrategists = oTmpStrategist.Values.ToList();
		}

        /// <summary>
        /// Indicates whether the current user has the Osj Role
        /// </summary>
        /// 
        bool? bIsOsj;
        public bool IsOsj
        {
            get
            {
                if (!bIsOsj.HasValue)
                {
                    SWTClaim oClaim = this.Claim;
                    bIsOsj = oClaim.Roles != null ? oClaim.Roles.Any(r => r.RoleId == this.OsjRoleId) : false;
                 }
                return bIsOsj.Value;
            }
        }

       
        private string[] osjIds;
        public string[] OsjIds
        {
            get
            {
                if (this.osjIds == null)
                {
  
                    if (this.IsOsj)
                    {
                         SWTClaim oClaim = this.Claim;

                         if (oClaim != null && oClaim.Roles != null && oClaim.Roles.Any(r => r.RoleId == this.OsjRoleId))
                         {
                             osjIds = oClaim.Roles.Where(r => r.RoleId == this.OsjRoleId).Select((a) => a.APLId).Distinct().ToArray();
                         }
                    }
                    else
                    {
                         osjIds = new string[0];
                    }  
                
                }
                    
                return osjIds;
             }              
           
        }

        /// <summary>
        /// Gets the code/identifier for the osj role
        /// </summary>
        /// 
        /// <summary>
		/// Gets the code/identifier for the agent role
		/// </summary>
		/// 
		int? iOsjRoleId;
		public int OsjRoleId
		{

			get
			{
				if (!iOsjRoleId.HasValue)
				{
                    string sOsjRoleId;
                    int iOsjRoleCode;
                    ID oOsjRoleItemId;
                    Item oOsjRoleItem;
                    string sOsjRoleCode;

                    iOsjRoleCode = RoleCodeNotFound;
                    sOsjRoleId = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Security.OsjRoleItem, string.Empty);

                    if (!string.IsNullOrEmpty(sOsjRoleId) && ID.TryParse(sOsjRoleId, out oOsjRoleItemId))
                    {
                        oOsjRoleItem = ContextExtension.CurrentDatabase.GetItem(oOsjRoleItemId);

                        if (oOsjRoleItem != null)
                        {
                            sOsjRoleCode = oOsjRoleItem.GetText(Genworth.SitecoreExt.Constants.Security.Templates.UserLevel.Sections.UserLevel.Name, Genworth.SitecoreExt.Constants.Security.Templates.UserLevel.Sections.UserLevel.Fields.CodeFieldName, string.Empty);

                            if (!int.TryParse(sOsjRoleCode, out iOsjRoleCode))
                            {
                                iOsjRoleCode = RoleCodeNotFound;
                                Sitecore.Diagnostics.Log.Error("Unable to get the Osj Role Code. This is information may not be correct in the meta data item", this);
                            }
                        }
                        else
                        {
                            Sitecore.Diagnostics.Log.Error("Osj Role Id is not set check the include configuration file or the meta data item", this);
                        }
                    }
                    iOsjRoleId = iOsjRoleCode;
                   } 
                return iOsjRoleId.Value;
            }
        }

		#region HELPER CLASSES

		/// <summary>
		/// This helper class holds the relation between a client and an agent
		/// </summary>
		[Serializable]
		private class ClientAgent
		{
			public string ClientId;
			public string AgentId;
			public string ChannelType;
			public string[] Preferences;
			public bool PrefencesLoaded;
		}

		#endregion

	}


}
