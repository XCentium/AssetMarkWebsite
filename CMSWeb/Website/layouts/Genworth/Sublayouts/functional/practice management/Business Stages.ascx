<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Collections" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
	
	Item oCurrentItem;
    int iTotalGroupItems;
    int iTotalInnerGroupItems;
	
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
        Item oGroupsItems;
        IEnumerable<Item> oBindableGroups;
        int iBindableGroupsCount;
        Item oFirstGroup;
        Item oLastGroup;

        oCurrentItem = ContextExtension.CurrentItem;
        if ((oGroupsItems = oCurrentItem.Children.FirstOrDefault(oItem => oItem.InstanceOfTemplate("Link Group") && oItem.Name.Equals("Resources at Each Stage"))) != null)
        {
            oBindableGroups = oGroupsItems.Children.GetItemsOfTemplate("Link Group");

            iTotalGroupItems = oBindableGroups.Count();
            
            if (oBindableGroups != null && iTotalGroupItems > 0)
            {
                rGroups.DataSource = oBindableGroups;
                rGroups.DataBind();
            }
        }
    }

    void rGroups_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            List<Item> oLinks;
            IEnumerable<Item> oInternalLinks;
            Repeater rGroupLinks;
            HtmlControl hList;
            
            Item groupOfLink = (Item)e.Item.DataItem;

            if (groupOfLink != null)
            {
                oInternalLinks = groupOfLink.GetChildrenOfTemplate("Link");

                iTotalInnerGroupItems = oInternalLinks.Count();

                if (oInternalLinks != null && iTotalInnerGroupItems > 0)
                {
                    // Bind inner links
                    rGroupLinks = e.Item.FindControl("rGroupLinks") as Repeater;

                    if (rGroupLinks != null)
                    {
                        rGroupLinks.DataSource = oInternalLinks;
                        rGroupLinks.DataBind();
                    }
                    
                    // Check style needed
                    hList = e.Item.FindControl("uList") as HtmlControl;
                    SetClass(hList, e.Item.ItemIndex, iTotalGroupItems);
                    
                }
            }
        }		
	}

    private void SetClass(HtmlControl control, int currentIndex, int totalItems)
    {
        // repeater has zero based index, but we want the list index
        int indexOnList = currentIndex + 1;
        
        if (indexOnList == 1)
        {
            control.Attributes.Add("class", "first");
        }
        else if (totalItems > 1 && indexOnList == totalItems)
        {
            control.Attributes.Add("class", "last");
        }
    }
    
    void rGroupLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Item oItemLink;
            HtmlGenericControl oListItemContainingLink;
            HyperLink hLinkBeingConfigured;
            RepeaterItem oRepeaterItem = e.Item;
			Literal lTitle;
            oItemLink = oRepeaterItem.DataItem as Item;

            if (oItemLink != null)
            {
                oListItemContainingLink = oRepeaterItem.FindControl("lListItem") as HtmlGenericControl;

                if (oListItemContainingLink != null)
                {
                    SetClass(oListItemContainingLink, e.Item.ItemIndex, iTotalInnerGroupItems);

                    hLinkBeingConfigured = oListItemContainingLink.FindControl("hLink") as HyperLink;

                    oItemLink.ConfigureHyperlink(hLinkBeingConfigured);
					if (string.IsNullOrWhiteSpace(hLinkBeingConfigured.NavigateUrl))
					{
						lTitle = oRepeaterItem.FindControl("lTitle") as Literal;
						lTitle.Text = oItemLink.DisplayName;
						hLinkBeingConfigured.Visible = false;
					}
					else
                    hLinkBeingConfigured.Text = oItemLink.DisplayName;
                }
            }
        }
	}

	private void ConfigureGroupLink(HtmlGenericControl oLinkContainer, string sGroupListItemId, string sLinkId, Item oItemLink)
	{
		HyperLink hLinkBeingConfigured;
		HtmlGenericControl oListItemContainingLink;
		
		if (oLinkContainer != null && oItemLink != null && !string.IsNullOrEmpty(sGroupListItemId) && !string.IsNullOrEmpty(sLinkId))
		{
			oListItemContainingLink = oLinkContainer.FindControl(sGroupListItemId) as HtmlGenericControl;

			if (oListItemContainingLink != null)
			{
				hLinkBeingConfigured = oListItemContainingLink.FindControl(sLinkId) as HyperLink;
				
				if (hLinkBeingConfigured != null)
				{
					oItemLink.ConfigureHyperlink(hLinkBeingConfigured);
					
						hLinkBeingConfigured.Text = oItemLink.DisplayName;
				}
			}
		}
	}
	
</script>
<!-- START CONTENT-FOOTER LINK LIST -->
<div class="content-footer">
	<label>RESOURCES AT EACH STAGE</label>		
    <asp:Repeater ID="rGroups" runat="server" OnItemDataBound="rGroups_ItemDataBound">
	<ItemTemplate>
	<ul id="uList" runat="server">				
		<asp:Repeater ID="rGroupLinks" runat="server" OnItemDataBound="rGroupLinks_ItemDataBound">				
			<ItemTemplate>				
				<li id="lListItem" runat="server">
				<span>
                    <asp:HyperLink ID="hLink" runat="server"></asp:HyperLink>
					<asp:Literal ID="lTitle" runat="server"></asp:Literal>
					</span>
                </li>				
			</ItemTemplate>
		</asp:Repeater>												
	</ul>
	</ItemTemplate>	
	</asp:Repeater>
	<div class="clear">
	</div>
</div>
<!-- END CONTENT-FOOTER LINK LIST -->
