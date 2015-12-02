<%@ Control Language="c#" AutoEventWireup="true" ClassName="CustodialServicesText"%>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
    
    public void BindData(List<Item> oPageItems)
    {
        if (oPageItems.Count() > 0)
        {
            rPageInfo.DataSource = oPageItems;
            rPageInfo.DataBind();
        }
   }

    void rPageInfo_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Item oPageItem;
            Literal lSummary;
            RepeaterItem oRepeaterItem = e.Item;
            
            oPageItem = oRepeaterItem.DataItem as Item;

            if (oPageItem != null)
            {
                lSummary = oRepeaterItem.FindControl("lSummary") as Literal;

                if (lSummary != null)
                {
                    lSummary.Text = oPageItem.GetText("Preview", "Body", String.Empty);
                }
            }
        }
    }
    
</script>

<div class="admin_block">
	<asp:Repeater ID="rPageInfo" runat="server" OnItemDataBound="rPageInfo_ItemDataBound">
	<ItemTemplate>
        <asp:Literal ID="lSummary" runat="server"></asp:Literal>
	</ItemTemplate>
	</asp:Repeater>
</div>

