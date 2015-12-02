<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Security" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
    
    private const string RemoteNotificationsEnabled = "1";
    private const string IncludeInNavegationEnabled = "1";
    private const string ShowOnMeetingModeEnabled = "1";
    
    private const int NoNotifications = 0;
    private ID oTopSectionSelectedID;
    private bool bIsHomeSecction = false;
    private int iMaxItems;
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

	private void BindData()
	{
		
        Item oCurrentItem = ContextExtension.CurrentItem;
        List<Item> oParentItems = oCurrentItem.GetParentItems();
        if (oParentItems.Count==1)
        {
            
            if ((oTopSectionSelectedID = oCurrentItem.ID) == ItemExtension.RootItem.ID)
                bIsHomeSecction = true;
           
        }
        else
        {
            oParentItems.Reverse();
			oCurrentItem = oParentItems.Skip(1).FirstOrDefault();
            if (oCurrentItem == null || !oCurrentItem.GetText(
                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Name,
                                                                Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Fields.IncludeInNavigationFieldName,
                                                                string.Empty
                                                                ).Equals(IncludeInNavegationEnabled)
               )
            {
                bIsHomeSecction = true;
            }
            else
            {
                oTopSectionSelectedID = oCurrentItem.ID;
            }
        }
        BindMenu();
	}

	private void BindMenu()
	{
		IEnumerable<Item> oItems;      

        oItems = Genworth.SitecoreExt.Helpers.NavigationHelper.GetPrimaryNavigationContent();
        
		rItems.DataSource = oItems;
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
        iMaxItems = ((IEnumerable<Item>)rItems.DataSource).Count()-1;
		rItems.DataBind();
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		HyperLink hLink;
		PlaceHolder pNotificationIndicator;
		Literal lNotificationIndicator;
		Label lIcon;
		Label lIconLinkTitle;
		Literal lText;
		HtmlGenericControl lListItem;
		string sCssClass =String.Empty;
		bool bIsTopNavIconLink;
		int iNotifications;
        Genworth.SitecoreExt.Security.Authorization oAuthorization;
         
		oItem = (Item)e.Item.DataItem;
		
		//does this repeater contain an item?
		if (oItem != null)
		{
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lIconLinkTitle = (Label)e.Item.FindControl("lIconLinkTitle");
			pNotificationIndicator = (PlaceHolder)e.Item.FindControl("pNotificationIndicator");
			lNotificationIndicator = (Literal)e.Item.FindControl("lNotificationIndicator");
			lIcon = (Label)e.Item.FindControl("lIcon");
			lText = (Literal)e.Item.FindControl("lText");
			lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
			
			//set the link
			oItem.ConfigureHyperlink(hLink);

			if ((bIsTopNavIconLink = oItem.InstanceOfTemplate("Top Nav Icon Link")) == true)
			{
				//set the link text
				lIcon.CssClass = string.Format("{0} {1}", lIcon.CssClass, oItem.GetText("Icon", "CSS Class")).Trim();
				hLink.CssClass = "iconLink";
				lIconLinkTitle.Text = "Home";
			}
			else
			{
				//hide the image
				lIcon.Visible = false;
				lText.Text = oItem.DisplayName;
			}

            iNotifications = NoNotifications;
			//output the notification count (or hide if none)
            if (string.Equals(oItem.GetText(
                                            Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Name,
                                            Genworth.SitecoreExt.Constants.Navigation.Templates.NavigationBase.Sections.Navigation.Fields.ExternalNotificationFieldName,
                                            string.Empty
                                            ), RemoteNotificationsEnabled
                              )
                ) 
            {
                oAuthorization = Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization;

                if (oAuthorization != null)
                {
                    iNotifications = oAuthorization.GetNotifications();
                    
                    lNotificationIndicator.Text = iNotifications.ToString();
                }
            }
            else if( (iNotifications = oItem.GetChildrenOfTemplate("Notification").Count) > 0)
			{
				lNotificationIndicator.Text = iNotifications.ToString();
			}
			
            if(iNotifications < 1)
			{
				pNotificationIndicator.Visible = false;
			}
			
			
            if (bIsHomeSecction || oItem.ID == oTopSectionSelectedID)
            {
                sCssClass = "selected";
                lListItem.Attributes.Add("class", "selected");
                bIsHomeSecction = false;
            }
            
			if (e.Item.ItemIndex == 0 )
			{
                sCssClass = string.Format("{0} {1}", sCssClass, "first").Trim();
			}
            else
                if (e.Item.ItemIndex == iMaxItems)
                {
                    sCssClass = string.Format("{0} {1}", sCssClass, "last").Trim();
                }

            if (!string.IsNullOrEmpty(sCssClass))
            {
                lListItem.Attributes.Add("class", sCssClass);
            }
		}
	}
</script>
<div id="mainMenuWrapper">
	<div id="mainMenuContainer">
		<ul id="mainMenu">
			<asp:Repeater ID="rItems" runat="server">
				<ItemTemplate>
					<li id="lListItem" runat="server"><asp:HyperLink ID="hLink" runat="server">
					<asp:PlaceHolder ID="pNotificationIndicator" runat="server"><span class="count orange"><span class="count-value"><asp:Literal ID="lNotificationIndicator" runat="server" /></span></span></asp:PlaceHolder>
					<asp:Label ID="lIcon" runat="server" CssClass="iconSpan" /><asp:Label ID="lIconLinkTitle" runat="server" CssClass="iconLinkTitle"></asp:Label><asp:Literal ID="lText" runat="server" /></asp:HyperLink></li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>
	</div>
</div>