using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerLogic.SitecoreExt;
using Genworth.SitecoreExt.Helpers;
using Sitecore.Data.Items;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Genworth.SitecoreExt.Services.Content
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class NavigationContentService : INavigationContentService
    {
        private const string RemoteNotificationsEnabled = "1";
        private const string IncludeInNavegationEnabled = "1";
        private const string ShowOnMeetingModeEnabled = "1";

        public NavigationItem[] GetPrimaryNavigationContent()
        { 
            IEnumerable<Item> oItems;
            IEnumerable<NavigationItem> oNavigationContent;

            oNavigationContent = null;

            try
            {
                oItems = NavigationHelper.GetPrimaryNavigationContent();                
                if (oItems != null)
                {
                    oNavigationContent = (
                                            from
                                                oItem
                                            in
                                                oItems
                                            select
                                                ConfigureNavigationItem(oItem)
                                        );
                }

                if (oNavigationContent == null)
                {
                    oNavigationContent = new NavigationItem[0];
                }
            }
            catch (Exception oServiceException)
            {
                Sitecore.Diagnostics.Log.Error("Unable to get primary navigation meta data", oServiceException, this);
                oNavigationContent = new NavigationItem[0];
            }

            return oNavigationContent.ToArray();

        }


        private static NavigationItem ConfigureNavigationItem(Item oItem)
        {
            NavigationItem oNavigationItem;
            List<Item> oSecurityItems;

            oNavigationItem = new NavigationItem();


            oNavigationItem.Title = oItem.DisplayName;
            oNavigationItem.URL = ConfigureURL(oItem);
            oNavigationItem.RemoteNotificationsEnabled = string.Equals(oItem.GetText(
                                                                                                        Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Name,
                                                                                                        Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Fields.ExternalNotificationFieldName,
                                                                                                        string.Empty
                                                                                                        ), RemoteNotificationsEnabled
                                                                       );

            oNavigationItem.IsShownOnMeetingMode = oItem.GetText(
                                                                    Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Name,
                                                                    Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Fields.ShowOnMeetingModeFieldName,
                                                                    string.Empty
                                                                ).Equals(ShowOnMeetingModeEnabled);


            oSecurityItems = oItem.GetMultilistItems(Constants.Security.Templates.SecurityBase.Sections.Security.Name, Constants.Security.Templates.SecurityBase.Sections.Security.Fields.UserLevelsFieldName);

            if(oSecurityItems != null)
            {
                oNavigationItem.Security = new ContentSecurity();

                oNavigationItem.Security.UserLevels = (
                                                           from
                                                               oSecutiryItem
                                                           in
                                                               oSecurityItems
                                                           select
                                                               oSecutiryItem.GetText(
                                                                                       Constants.Security.Templates.UserLevel.Sections.UserLevel.Name,
                                                                                       Constants.Security.Templates.UserLevel.Sections.UserLevel.Fields.CodeFieldName
                                                                                   )
                                                       ).ToArray();
            }                         

            return oNavigationItem;
            
        }

        private static string ConfigureURL(Item oItem)
        {
            string sURL;

            sURL = string.Empty;
            //is the item non-null?
            if (oItem != null)
            {
                //configure the link to either open in a specified target OR open in a shadow box
                sURL = Sitecore.Links.LinkManager.GetItemUrl(oItem);
            }
            return sURL;
        }
     
        public IEnumerable<NotificationItem> GetAlerts(string itemID)
        {
        
            Item oItem = ContextExtension.CurrentDatabase.GetItem(itemID);
            if(oItem == null) return null;

            string hLink;
            string summary;
            string target= string.Empty;
            Item alert;

            //get the web controls
            List<Item> alerts = oItem.GetChildrenOfTemplate(new string[] { "Notification" });
              
            if(alerts == null) return null;
            return alerts.Select(n=>new NotificationItem { Summary = n.GetText("Summary"), URL = n.GetText("URL"), Target=n.GetText("Target")});
           
        }

        }
    }
