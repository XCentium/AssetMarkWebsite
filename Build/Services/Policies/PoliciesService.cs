using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using Sitecore.Data;
using Version = Sitecore.Data.Version;
using Sitecore.Globalization;
using System.Web;

namespace Genworth.SitecoreExt.Services.Policies
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class PoliciesService : IPoliciesService
    {
        #region CONSTANTS
        


        #endregion

        #region HELPERS

        private Item GetPolicyItem(string sPolicyId, string sVersion)
        {
            #region VARIABLES

            ID oPolicyId;
            Item oPolicyItem;
            Version oPolicyVersion;
            Language oCurrentLanguage;
            int iVersion;

            #endregion

            oPolicyItem = null;

            if (ID.TryParse(sPolicyId, out oPolicyId) && int.TryParse(sVersion, out iVersion))
            {
                if (iVersion > 0)
                {
                    oPolicyVersion = new Version(iVersion);
                    oCurrentLanguage = Language.Parse(ContextExtension.CurrentLanguageCode);
                    oPolicyItem = ContextExtension.CurrentDatabase.GetItem(oPolicyId, oCurrentLanguage, oPolicyVersion);
                }
                else
                {
                    oPolicyItem = ContextExtension.CurrentDatabase.GetItem(oPolicyId);
                }
            }

            return oPolicyItem;
        }

        #endregion

        #region SERVICE OPERATIONS



        public Policy GetVersionedPolicy(string sPolicyId, string sVersion)
        {
            #region VARIABLES 

            Policy oPolicy;            
            Item oPolicyItem;
            

            #endregion

            oPolicy = null;

            oPolicyItem = GetPolicyItem(sPolicyId, sVersion);

            if (oPolicyItem != null && oPolicyItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Policies.Templates.Policy.Name))
            {
                oPolicy = new Policy()
                {
                    Body = oPolicyItem.GetText(Genworth.SitecoreExt.Constants.Policies.Templates.Policy.Sections.Policy.Name, Genworth.SitecoreExt.Constants.Policies.Templates.Policy.Sections.Policy.Fields.BodyFieldName, string.Empty),
                    Version = oPolicyItem.Version.Number
                };

                if (!string.IsNullOrEmpty(oPolicy.Body))
                {
                    oPolicy.Body = HttpUtility.HtmlEncode(oPolicy.Body);
                }
            }

            return oPolicy;
        }

        public Policy GetPolicy(string sPolicyId)
        {
            #region VARIABLES

           

            #endregion

            
            return GetVersionedPolicy(sPolicyId, Version.Latest.Number.ToString());
        }

        public int[] GetPolicyVersions(string sPolicyId)
        {
            #region VARIABLES
            
            Item oPolicyItem;
            int[] oVersions;

            #endregion


            oPolicyItem  = GetPolicyItem(sPolicyId, Version.Latest.Number.ToString());
            oVersions = null;

            if (oPolicyItem != null)
            {                
                oVersions = oPolicyItem.Versions.GetVersionNumbers().Select(v => v.Number).ToArray();                
            }

            return oVersions;
        }

        #endregion
    }
}
