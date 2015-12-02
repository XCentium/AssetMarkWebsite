<%@ Control Language="c#" AutoEventWireup="true"
    ClassName="CustodialServicesTaxReources" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
    
    public void BindData(List<Item> oTaxUpdateItems)
    {
		if (oTaxUpdateItems.Count() > 0)
        {
			rTaxUpdates.DataSource = oTaxUpdateItems;
			rTaxUpdates.DataBind();
        }
   }

	void rTaxUpdates_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
			Item oTaxUpdateItem;
            Literal lTitle;
			Literal lDate;
			Item oTaxCategory;
            RepeaterItem oRepeaterItem = e.Item;

			oTaxUpdateItem = oRepeaterItem.DataItem as Item;

			if (oTaxUpdateItem != null)
            {
				
				if ((lTitle = oRepeaterItem.FindControl("lTaxCategory") as Literal) != null
					&& (oTaxCategory = oTaxUpdateItem.GetField("Tax Season Update", "Tax Season Update Description").GetItem()) != null)
                {
					lTitle.Text = oTaxCategory.GetText("Description");        
				}

				lDate = oRepeaterItem.FindControl("lDate") as Literal;

				if (lDate != null)
                {
					//get the date
					lDate.Text = oTaxUpdateItem.GetField("Tax Season Update", "Date").GetDateString("MM/dd/yy", "TBA");
                }
            }
        }
    }
    
</script>
<table class="greytable" width="100%">
	<asp:Repeater ID="rTaxUpdates" runat="server" OnItemDataBound="rTaxUpdates_ItemDataBound">
		<ItemTemplate>
			<tr id="trTaxUpdate">
				<td>
					<asp:Literal ID="lTaxCategory" runat="server"></asp:Literal>
				</td>
				<td class="date">
					<asp:Literal ID="lDate" runat="server"></asp:Literal>
				</td>
			</tr>
		</ItemTemplate>
	</asp:Repeater>
</table>
<br />
