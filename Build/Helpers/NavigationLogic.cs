using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using Genworth.SitecoreExt.Security;
using ServerLogic.SitecoreExt;

namespace Genworth.SitecoreExt.Helpers
{
    public static class NavigationHelper
    {

        private const string RemoteNotificationsEnabled = "1";
        private const string IncludeInNavegationEnabled = "1";
        private const string ShowOnMeetingModeEnabled = "1";

        public static IEnumerable<Item> GetPrimaryNavigationContent()
        {
            IEnumerable<Item> oItems;
            Authorization oAuthorization;

            oAuthorization = Authorization.CurrentAuthorization;
            string userStatus = oAuthorization.IsTestMode ? "1" : oAuthorization.Claim.Roles.Where(r => r.IsActive).Count() > 0 ? "1" : "0";

            if (oAuthorization.IsMeetingMode)
            {
                oItems = ItemExtension.RootItem.GetChildrenOfTemplate("Web Base").Where(oItem =>
                                                                                                (oItem.GetText(
                                                                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Name,
                                                                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Fields.IncludeInNavigationFieldName,
                                                                                                                string.Empty
                                                                                                                ).Equals(IncludeInNavegationEnabled)) &&
                                                                                                (oItem.GetText(
                                                                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Name,
                                                                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Fields.ShowOnMeetingModeFieldName,
                                                                                                                string.Empty
                                                                                                              ).Equals(ShowOnMeetingModeEnabled)) &&
                                                                                                (CheckUserStatusVisibility(oItem, new string[] { userStatus }, oAuthorization))
                                                                                       );
            }
            else
            {
                oItems = ItemExtension.RootItem.GetChildrenOfTemplate("Web Base").Where(oItem => (oItem.GetText(
                                                                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Name,
                                                                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Fields.IncludeInNavigationFieldName,
                                                                                                                string.Empty
                                                                                                                ).Equals(IncludeInNavegationEnabled)) &&
                                                                                                                (CheckUserStatusVisibility(oItem, new string[] { userStatus }, oAuthorization))
                                                                                       );
            }


            return oItems;
        }

        private static bool CheckUserStatusVisibility(Item oItem, string[] oUserStatusAllowed, Authorization oAuthorization)
        {
            #region VARIABLES

            List<Item> oItems;
            bool bCheckResult;
            int iMatchedPermissions;

            #endregion

            bCheckResult = false;
            if (oAuthorization != null && oItem != null && oUserStatusAllowed != null)
            {
                if ((oItems = oItem.GetMultilistItems("Navigation", "User Status Visibility")).Count > 0)
                {
                    iMatchedPermissions = oItems.Join(
                                                        (oUserStatusAllowed),
                                                        (oItemNavigation => oItemNavigation.GetText("User Status", "Code")),
                                                        (sUsrStatus => sUsrStatus),
                                                        (oItemNavigation, sUsrStatus) => true
                                                    ).Count();

                    bCheckResult = iMatchedPermissions > 0;
                }
                else
                {
                    //If user status has not been set for the item then is not required to validate it
                    bCheckResult = true;
                }
            }

            return bCheckResult;
        }
    }
}
