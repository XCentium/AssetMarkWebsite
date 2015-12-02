using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Configuration; 
using Sitecore.Data.Items; 
using Sitecore.Security.AccessControl; 
using Sitecore.Security.Accounts;
using ServerLogic.SitecoreExt;


namespace Genworth.SitecoreExt.Security
{
	class GenAuthorizationHelper : ItemAuthorizationHelper 
	{
        private const string TemplatesPath = "/sitecore/templates";
        private const string LayoutPath = "/sitecore/layout";
        private const string MedialibraryPath = "/sitecore/media library";
        private const string ContentPath = "/sitecore/content";
        private const string AssetMarkMediaLibraryPath = "/sitecore/media library/Assetmark/";

		protected override AccessResult GetItemAccess(Item item, Account account, AccessRight accessRight, PropagationType propagationType)
		{
			AccessResult oItemAccessResult;						

			oItemAccessResult = CheckAccess(item, accessRight);

			return oItemAccessResult;
		}

        private const string ClientApprovedenabled = "1";

		private AccessResult CheckAccess(Item oItem, AccessRight right)         
		{
			Authorization oAuthorization;
			bool bPass;
			AccessResult oItemAccessResult;
			AccessExplanation oItemAccessExplanation;
			string sAccessExplanationText;
			bool bExplanationTextSet;
            bool bIsClient;
            string[] oClientSecuredSections;
            string sItemPath;

			bPass = false;
			oAuthorization = Authorization.CurrentAuthorization;
			oItemAccessResult = null;
			oItemAccessExplanation = null;
			bExplanationTextSet = false;
			sAccessExplanationText = string.Empty;
            oClientSecuredSections = oAuthorization.ClientSecuredSections;
            sItemPath = oItem.Paths.ParentPath;

			if (oItem != null)
			{
				//We will only apply the authorization process if the current Item inherits the Security Base template
				//if (oItem.InstanceOfTemplate(Constants.Security.Templates.SecurityBase.Name))
                
                if (sItemPath.Contains(ContentPath) && oItem.InstanceOfTemplate(Constants.Security.Templates.SecurityBase.Name))
				{
					if (oAuthorization != null)
					{
						//Checking by User Levels
						bPass = CheckLevels(
                                            Constants.Security.Templates.SecurityBase.Sections.Security.Fields.UserLevelsFieldName,
                                            Constants.Security.Templates.UserLevel.Sections.UserLevel.Name,
                                            Constants.Security.Templates.UserLevel.Sections.UserLevel.Fields.CodeFieldName, 
                                            oItem, 
                                            oAuthorization.UserLevels, 
                                            oAuthorization,
                                            Constants.Security.Templates.SecurityBase.Sections.Security.Name
                                            );

						//If any level check fails then there is no need to continue the validation has failed
						if (bPass)
						{
							//Checking by Channels
    					    bPass = CheckLevels(
                                                    Constants.Security.Templates.SecurityBase.Sections.Security.Fields.ChannelsFieldName,
                                                    Constants.Security.Templates.Channel.Sections.Channel.Name,
                                                    Constants.Security.Templates.Channel.Sections.Channel.Fields.CodeFieldName, 
                                                    oItem, 
                                                    oAuthorization.Channels, 
                                                    oAuthorization,
                                                    Constants.Security.Templates.SecurityBase.Sections.Security.Name
                                                    );
						}
						else
						{
							sAccessExplanationText = string.Format("User failed user levels check for item {0}.", oItem.ID);
							bExplanationTextSet = true;
						}

						if (bPass)
						{
							//Checking by Custodians
							bPass = CheckLevels(
                                                Constants.Security.Templates.SecurityBase.Sections.Security.Fields.CustodiansFieldName, 
                                                Constants.Security.Templates.Custodian.Sections.Custodian.Name,
                                                Constants.Security.Templates.Custodian.Sections.Custodian.Fields.CodeFieldName, 
                                                oItem, 
                                                oAuthorization.Custodians, 
                                                oAuthorization,
                                                Constants.Security.Templates.SecurityBase.Sections.Security.Name
                                                );
						}
						else if(!bExplanationTextSet)
						{
							sAccessExplanationText = string.Format("User failed channels check for item {0}.", oItem.ID);
						}

                        bIsClient = oAuthorization.IsClient;
                        if (bPass)
                        {
                            //TODO: We need a more flexible authorization resolution. for now we are hardcoding the mapping of the rules for clients
                            //Checking by Manager Strategist privileges
                            bPass = !bIsClient || (bIsClient && CheckLevels(
                                                                                                        Constants.Security.Templates.SecurityBase.Sections.Security.Fields.ManagerStrategistPrivilegesFieldName,
                                                                                                        Constants.Security.Templates.ManagerStrategistPrivilege.Sections.ManagerStrategistPrivilege.Name,
                                                                                                        Constants.Security.Templates.ManagerStrategistPrivilege.Sections.ManagerStrategistPrivilege.Fields.CodeFieldName,
                                                                                                        oItem,
                                                                                                        oAuthorization.ManagerStrategistPrivileges,
                                                                                                        oAuthorization,
                                                                                                        Constants.Security.Templates.SecurityBase.Sections.Security.Name
                                                                                                        )
                                                                 );
                        }
                        else if (!bExplanationTextSet)
                        {
                            sAccessExplanationText = string.Format("User failed custodians check for item {0}.", oItem.ID);
                        }

						if (bPass)
						{
                            //TODO: We need a more flexible authorization resolution. for now we are hardcoding the mapping of the rules for clients
							//Checking by Products
                            bPass = !bIsClient || (bIsClient && CheckLevels(
                                                                                                            Constants.Security.Templates.SecurityBase.Sections.Security.Fields.ProductsFieldName, 
                                                                                                            Constants.Security.Templates.Product.Sections.Product.Name,
                                                                                                            Constants.Security.Templates.Product.Sections.Product.Fields.CodeFieldName, 
                                                                                                            oItem, 
                                                                                                            oAuthorization.Products, 
                                                                                                            oAuthorization,
                                                                                                            Constants.Security.Templates.SecurityBase.Sections.Security.Name
                                                                                                            )
                                                                 );
						}
						else if (!bExplanationTextSet)
						{
                            sAccessExplanationText = string.Format("User failed Manager Strategist privileges check for item {0}.", oItem.ID);
						}

                        if (bPass)
                        {
                            //TODO: We need a more flexible authorization resolution. for now we are hardcoding the mapping of the rules for clients
                            //Checking by client approved
                            if( (bIsClient && oClientSecuredSections != null && oClientSecuredSections.Any(sSection => oItem.Paths.FullPath.Contains(sSection))))
                            {
                                bPass = string.Equals(oItem.GetText(Constants.Security.Templates.SecurityBase.Sections.Security.Name, Constants.Security.Templates.SecurityBase.Sections.Security.Fields.ClientApprovedFieldName, string.Empty), ClientApprovedenabled);
                            }
                        }
                        else if (!bExplanationTextSet)
                        {
                            sAccessExplanationText = string.Format("User failed products check for item {0}.", oItem.ID);
                        }						

                        //Validate PC_Status Security
                        if (bPass && oItem.InstanceOfTemplate(Constants.Security.Templates.PC_StatusSecurity.Name))
                        {
                            if (oAuthorization.IsAgent)
                            {
                                //Checking by PC_Status
                                bPass = CheckLevels(
                                                    Constants.Security.Templates.PC_StatusSecurity.Sections.Security.Fields.PC_StatusFieldName,
                                                    Constants.Security.Templates.PC_Status.Sections.PC_Status.Name,
                                                    Constants.Security.Templates.PC_Status.Sections.PC_Status.Fields.CodeFieldName,
                                                    oItem,
                                                    oAuthorization.PC_Status,
                                                    oAuthorization,
                                                    Constants.Security.Templates.PC_StatusSecurity.Sections.Security.Name
                                                    );

                                //If any level check fails then there is no need to continue the validation has failed
                                if (!bPass)
                                {
                                    sAccessExplanationText = string.Format("User failed PC Status check for item {0}.", oItem.ID);
                                    bExplanationTextSet = true;
                                }
                            }
                            else
                            {
                                bExplanationTextSet = true;
                                sAccessExplanationText = string.Format("The user is not an agent.", oItem.ID);
                            }
                        }
                        
                        if (bPass)
                        {
                            sAccessExplanationText = string.Format("User authorized to access item {0}.", oItem.ID);
                        }
                        else if (!bExplanationTextSet)
                        {
                            sAccessExplanationText = string.Format("User failed alient approved check for item {0}.", oItem.ID);
                        }

					}
					else
					{
						if (System.Web.HttpContext.Current == null)
						{
							sAccessExplanationText = string.Format("{0} Item is being requested without an HTTP Context.", oItem.ID);
							bPass = true; //Since the current item is being requested without an http context, it is an internal request and should be permitted.
						}
						else
						{
							sAccessExplanationText = "Unable to get authorization object";
							oItemAccessExplanation = new AccessExplanation(sAccessExplanationText, new object[0]);
							oItemAccessResult = new AccessResult(AccessPermission.Deny, oItemAccessExplanation);
							Sitecore.Diagnostics.Log.Error(sAccessExplanationText, this);
						}
					}
				}
				else
				{
					sAccessExplanationText = string.Format("{0} item does not inherits from security base template", oItem.ID);
                    
                    //check if an item is a media one and validate if user is logged on before providing it                    
                    // ignore if media item is part of AssetMark website
                    if (oItem.Paths.IsMediaItem && !oItem.Paths.FullPath.StartsWith(AssetMarkMediaLibraryPath))
                    {
                        if (oAuthorization != null)
                        {
                            if (oAuthorization.IsTestMode)
                            {
                                bPass = true;
                            }
                            else
                            {
                                if (oAuthorization.Claim != null)
                                {
                                    bPass = true;
                                }
                                else
                                {
                                    bPass = false;
                                }
                            }
                        }
                        else
                        {
                            bPass = false;
                        }
                    }
                    else
                    {
                        bPass = true; //Since the current item does not inherit from security base it should pass the validation
                    }

				}

				

				if (bPass)
				{
					oItemAccessExplanation = new AccessExplanation(string.Format("{0} right has been granted", right.Name), new object[0]);
					oItemAccessResult = new AccessResult(AccessPermission.Allow, oItemAccessExplanation);
				}
				else
				{
					oItemAccessExplanation = new AccessExplanation(string.IsNullOrEmpty(sAccessExplanationText)? string.Format("{0} access denied", right.Name) : sAccessExplanationText, new object[0]);
					oItemAccessResult = new AccessResult(AccessPermission.Deny, oItemAccessExplanation);
				}
			}
			else
			{
				sAccessExplanationText = "No Item to validate";
				oItemAccessExplanation = new AccessExplanation(sAccessExplanationText, new object[0]);
				oItemAccessResult = new AccessResult(AccessPermission.NotSet, oItemAccessExplanation);				
			}


			return oItemAccessResult;
		}

		/// <summary>
		/// This method will match the permissions associated to an item against the permissions associated to a user
		/// </summary>
		/// <param name="sSecurityLevel">Security level that will be checked ((Custodians, Products, etc.))</param>
		/// <param name="sSecurityFieldToCheck">Field that will be used to check (Code or Name)</param>
		/// <param name="oItem">The user level will be compared against the levels associated to this Sitecore item</param>
		/// <param name="oUserAllowed">Levels associated to the user that will be checked</param>
		/// <returns></returns>
		private bool CheckLevels(string sSecurityLevel, string sSecurityFieldSectionToCheck, string sSecurityFieldToCheck, Item oItem, string[] oUserAllowed, Authorization oAuthorization, string sSecuritySection)
		{
			#region VARIABLES

			List<Item> oItems;
			bool bCheckResult;
			int iMatchedPermissions;

			#endregion

			bCheckResult = false;
			if (oAuthorization != null && oItem != null )
			{
				if ((oItems = oItem.GetMultilistItems(sSecuritySection, sSecurityLevel)).Count > 0 && oUserAllowed != null)
				{
					iMatchedPermissions = oItems.Join(
														(oUserAllowed),
														(oItemSecurity => oItemSecurity.GetText(sSecurityFieldSectionToCheck, sSecurityFieldToCheck)),
														(sUsrSecurity => sUsrSecurity),
														(oItemSecurity, sUsrSecurity) => true
													).Count();

					bCheckResult = iMatchedPermissions > 0;
				}
				else
				{
					//If security level has not been set for the item then is not required to validate it
					bCheckResult = true;
				}
			}

			return bCheckResult;
		}
	}
}
