using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Security.AccessControl;
using Sitecore.Security.Accounts;
using Sitecore.Data;
using Sitecore.Data.Items; 

namespace Genworth.SitecoreExt.Security
{
	class GenWebAuthorizationProvider : SqlServerAuthorizationProvider 
	{

		private ItemAuthorizationHelper oItemHelper;

		public GenWebAuthorizationProvider() 
        { 
            oItemHelper = new GenAuthorizationHelper(); 
        } 
  
        
        protected override ItemAuthorizationHelper ItemHelper 
        { 
            get { return oItemHelper; } 
            set { oItemHelper = value; } 
        }

		protected override AccessResult GetAccessCore(ISecurable oEntity, Account oAccount, AccessRight oAccessRight)
		{
			Item oItem;			
			GenAuthorizationHelper oGenAuthorizationHelper;
			AccessResult oItemAccessResult;
			AccessExplanation oItemAccessExplanation;
			string sAccessExplanationText;

			oItemAccessResult = null;
			//Validates that internal tasks have access to sitecore content
			//Specially required for sitecore events Sitecore:Item:Write (Sitecore Cache Update Event)
			//This is needed because we bypass Sitecore Authorization with the Custom Authorization process for GFWM
            if (Sitecore.Context.IsBackgroundThread)
            {
                oItemAccessExplanation = new AccessExplanation(string.Format("{0} access right granted for Internal Task", oAccessRight.Name), new object[0]);

                oItemAccessResult = new AccessResult(AccessPermission.Allow, oItemAccessExplanation);
            }
            else
            {
                switch (oAccessRight.Name)
                {
                    case "item:read":
                        oGenAuthorizationHelper = ItemHelper as GenAuthorizationHelper;
                        if (oGenAuthorizationHelper != null)
                        {
                            oItem = oEntity as Item;
                            oItemAccessResult = oGenAuthorizationHelper.GetAccess(oItem, oAccount, oAccessRight);
                        }

                        break;
                    case "field:read":
                    case "language:read":
                    case "site:enter":
                        oItemAccessExplanation = new AccessExplanation(string.Format("{0} acces right granted", oAccessRight.Name), new object[0]);
                        oItemAccessResult = new AccessResult(AccessPermission.Allow, oItemAccessExplanation);
                        break;
                    default:
                        sAccessExplanationText = string.Format("Access right {0} is unavailable in the web layer", oAccessRight.Name);
                        oItemAccessExplanation = new AccessExplanation(sAccessExplanationText, new object[0]);
                        oItemAccessResult = new AccessResult(AccessPermission.Deny, oItemAccessExplanation);
                        Sitecore.Diagnostics.Log.Debug(sAccessExplanationText, this);
                        break;
                }
            }

			return oItemAccessResult;
		}

        protected override void AddAccessResultToCache(ISecurable entity, Account account, AccessRight accessRight, AccessResult accessResult, PropagationType propagationType) 
        {                                   
           // base.AddAccessResultToCache(entity, account, accessRight, accessResult, propagationType); 
        } 
  
	}
}
