using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genworth.SitecoreExt;
using Sitecore.Pipelines.HttpRequest;
using System.Web;
using Genworth.SitecoreExt.Security;
using Genworth.SitecoreExt.Constants;
using ServerLogic.SitecoreExt;
using Sitecore.Data.Items;
using Sitecore.Web;
using Sitecore.Links;


namespace Genworth.SitecoreExt.Pipelines.HttpRequest
{
    class AuthorizationResolver : HttpRequestProcessor
    {
        #region ATTRIBUTES

        private Security.Authorization oAuthorization;

        #endregion


        #region METHODS       

        /// <summary>
        /// Validates the access to the site at being of http request
        /// </summary>
        /// <param name="args"></param>
        public override void Process(HttpRequestArgs args)
        {
            #region VARIABLES

            bool bAuthenticated;
            string sLoginPageUrl;
            string sLoginPageQueryString;

            Item oItem;

            #endregion

            bAuthenticated = false;

            oAuthorization = Authorization.CurrentAuthorization;

            //get the item currently being processed
            oItem = Sitecore.Context.Item;

            if (oItem != null && oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Security.Templates.SecurityBase.Name))
            {
                bAuthenticated = oAuthorization != null && (oAuthorization.IsTestMode || oAuthorization.Claim != null);

                if (!bAuthenticated)
                {
                    sLoginPageUrl = Authorization.LoginPage;
                    sLoginPageQueryString = Authorization.LoginPage_QueryString;
                    
                    if (!string.IsNullOrEmpty(sLoginPageUrl) )
                    {
                        if (!string.IsNullOrEmpty(sLoginPageQueryString) && args != null && args.Context != null
                          && args.Context.Request != null && !string.IsNullOrEmpty(args.Context.Request.RawUrl)
                            )
                        {
                            sLoginPageUrl += sLoginPageQueryString + System.Web.HttpUtility.UrlEncode(Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(args.Context.Request.RawUrl)));
                        }

                        Sitecore.Diagnostics.Log.Info("AuthorizationResolver.Process redirected to page: " + sLoginPageUrl, this);
                        WebUtil.Redirect(sLoginPageUrl);
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error("Unable to get url for login page. Review setting Genworth.SitecoreExt.Security.LoginPage", this);
                    }

                }
            }
        }

        #endregion
    }
}
