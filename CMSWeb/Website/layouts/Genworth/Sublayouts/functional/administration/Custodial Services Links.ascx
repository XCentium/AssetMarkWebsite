<%@ Control Language="c#" AutoEventWireup="true"  ClassName="CustodialServicesLink"%>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">
    
    public void BindData(List<Item> lLinkItems)
    {
		if (lLinkItems.Count() > 0)
        {
			rDocumentLinks.DataSource = lLinkItems;
            rDocumentLinks.DataBind();
        }
   }

    void rDocumentLinks_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Item oLinkItem;
            HtmlGenericControl oListItemContainingLink;
            HyperLink hLinkBeingConfigured;
            RepeaterItem oRepeaterItem = e.Item;
            
            oLinkItem = oRepeaterItem.DataItem as Item;

			if (oLinkItem != null)
            {
                oListItemContainingLink = oRepeaterItem.FindControl("lListItem") as HtmlGenericControl;
                hLinkBeingConfigured = oListItemContainingLink.FindControl("hDocumentLink") as HyperLink;

				if (oListItemContainingLink != null)
                {
					Item oLinkedItem = null;
                    if(oLinkItem.Fields["Item"] != null){
                        oLinkedItem = Sitecore.Context.Site.Database.GetItem(oLinkItem.Fields["Item"].Value);
                    }
                    
                    Item currentItem = ContextExtension.CurrentItem;
					if (oLinkedItem != null)
					{
						oLinkedItem.ConfigureDocumentHyperlink(hLinkBeingConfigured, oListItemContainingLink.Attributes);
                        oLinkedItem.ConfigureOmnitureControl(currentItem, hLinkBeingConfigured);
					}
					else
					{
						oLinkItem.ConfigureDocumentHyperlink(hLinkBeingConfigured, oListItemContainingLink.Attributes);
                        oLinkItem.ConfigureOmnitureControl(currentItem, hLinkBeingConfigured);
                       
					}
                    
                    hLinkBeingConfigured.Text = oLinkItem.DisplayName;
                }
            }
        }
    }

</script>
<ul class="link-list">
	<asp:Repeater ID="rDocumentLinks" runat="server" OnItemDataBound="rDocumentLinks_ItemDataBound">
	<ItemTemplate>
        <li id="lListItem" runat="server">
            <asp:HyperLink ID="hDocumentLink" runat="server"></asp:HyperLink>
        </li>
	</ItemTemplate>
	</asp:Repeater>
</ul>
