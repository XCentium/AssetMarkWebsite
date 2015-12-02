using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GFWM.Shared.ServiceRequest;
using GFWM.Common.AUM.Entities.Response;
using GFWM.Common.AUM.Entities.Request;
using GFWM.Shared.ServiceRequestFactory;
using GFWM.Common.AUM.Entities.Data;
using GFWM.Common.Preference.Entities.Request;
using GFWM.Common.Preference.Entities.Response;
using GFWM.Security.STS.Entity;
using System.Web;
//using GFWM.Common.State.Entities.Response;
//using GFWM.Common.State.Entities.Request;
using GFWM.App.Shared.Entity.Registration.Request;
using GFWM.App.Shared.Entity.Registration.Response;
using GFWM.App.Shared.Entity.Registration.Data;
using GFWM.Common.State.Entities.Response;
using GFWM.Common.State.Entities.Request;

namespace Genworth.SitecoreExt.Marketing
{
    public class ServiceClient
    {
        public MyProfileInformationResponse GetUserInfo(string aplId)
        {
            IServiceRequest proxy;
            MyProfileInformationRequest request;
            MyProfileInformationResponse response;

            try
            {

                proxy = ServiceRequestFactory.GetProxy(SERVICES.PREFERENCE_SERVICE);

                request = new MyProfileInformationRequest()
                {
                    UserID = aplId,
                };

                if (null != proxy)
                {
                    response = proxy.Request<MyProfileInformationRequest, MyProfileInformationResponse>(request);
                    if (null != response)
                    {
                        if (response.Fault != null)
                        {
                            // log error
                            Sitecore.Diagnostics.Log.Error(response.Fault, this);
                        }
                        else
                        {
                            return response;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get user " + aplId + "'s data from Preference service", ex, this);
                throw ex;
            }
        }

        public AgentDetailsResponse GetAgentDetails(string agentId)
        {
            IServiceRequest proxy;
            AgentDetailsResponse response;
            AgentDetailsRequest request;

            proxy = null;
            response = null;
            try
            {

                proxy = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

                request = new AgentDetailsRequest();
                request.Filter = string.Format("(APLID == '{0}')", agentId);

                if (null != proxy)
                {
                    response = proxy.Request<AgentDetailsRequest, AgentDetailsResponse>(request);
                    if (null != response)
                    {
                        if (response.Fault != null)
                        {
                            // log error
                            Sitecore.Diagnostics.Log.Error(response.Fault, this);
                        }
                        else
                        {
                            return response;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get Agent " + agentId + "'s data from AUM service", ex, this);
                throw ex;
            }
        }

        public GFWM.Common.AUM.Entities.Response.SupportTeamResponse GetSupportTeamDetails(string agentId)
        {
            
            SupportTeamResponse response = null;
            IServiceRequest proxy;
            try
            {
                SupportTeamRequest request = new SupportTeamRequest();
                proxy = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);
                
                request.AgentId = agentId;
                response = proxy.Request<SupportTeamRequest, SupportTeamResponse>(request);

                if (null != response)
                {
                    if (response.Fault != null)
                    {
                        // log error
                        Sitecore.Diagnostics.Log.Error(response.Fault, this);
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get support team contact details data for Agent " + agentId + " from AUM service", ex, this);
                throw ex;
            }
            return response;
        }

        public UserProfileResponse GetUserSSODetails(string ssoguid)
        {
            IServiceRequest proxy;
            UserProfileResponse response = null;
            UserProfileRequest request = null;

            try
            {
                proxy = ServiceRequestFactory.GetProxy(SERVICES.USERPROFILE_SERVICE);

                request = new UserProfileRequest();
                request.SSOGUID = ssoguid;
                response = proxy.Request<UserProfileRequest, UserProfileResponse>(request);

                if (null != response)
                {
                    if (response.Fault != null)
                    {
                        // log error
                        Sitecore.Diagnostics.Log.Error(response.Fault, this);
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get user details for SSOGUID " + ssoguid + " from User Profile service", ex, this);
                throw ex;
            }
            return response;
        }

        public UserAttributesResponse GetUserAttributes(string ssoguid, string userId)
        {
            IServiceRequest proxy;
            UserAttributesResponse response = null;
            UserAttributesRequest request = null;

            try
            {
                proxy = ServiceRequestFactory.GetProxy(SERVICES.STATE_SERVICE);

                request = new UserAttributesRequest();
                request.SSOGUID = ssoguid;
                request.UserId = userId;
                response = proxy.Request<UserAttributesRequest, UserAttributesResponse>(request);

                if (null != response)
                {
                    if (response.Fault != null)
                    {
                        // log error
                        Sitecore.Diagnostics.Log.Error(response.Fault, this);
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get user attributes for SSOGUID [" + ssoguid + "] and or userId [" + userId + "] from State service", ex, this);
                throw ex;
            }
            return response;

        }

        public SalesforceUserResponse GetSalesforceUser(string userId)
        {
            IServiceRequest proxy;
            SalesforceUserRequest request = null;
            SalesforceUserResponse response = null;

            try
            {
                proxy = ServiceRequestFactory.GetProxy(SERVICES.AUM_SERVICE);

                request = new SalesforceUserRequest();
                request.Filter = string.Format("(APLId == '{0}')", userId); ;
                response = proxy.Request<SalesforceUserRequest, SalesforceUserResponse>(request);

                if (null != response)
                {
                    if (response.Fault != null)
                    {
                        // log error
                        Sitecore.Diagnostics.Log.Error(response.Fault, this);
                    }
                    else
                    {
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get Salesforce user data for userId [" + userId + "] from AUM service", ex, this);
                throw ex;
            }
            return response;

        }
 
    }
}
