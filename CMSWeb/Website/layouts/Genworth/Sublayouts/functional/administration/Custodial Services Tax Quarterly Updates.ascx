<%@ Control Language="c#" AutoEventWireup="true"
    ClassName="CustodialServicesTaxQuarterlyUpdates" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
    
	/// <summary>
	/// This control will bind the information with the investments Quarterly Updates that
	/// are selected in the links
	/// </summary>
	/// <param name="oTaxUpdateItems"></param>
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
			Item oTaxQuarterlyUpdate;
			Item oTaxQuarterlyUpdateCategory;
			Literal lTitle;
			Literal lDate;
            RepeaterItem oRepeaterItem = e.Item;

			oTaxQuarterlyUpdate = oRepeaterItem.DataItem as Item;

			if (oTaxQuarterlyUpdate != null)
            {
				// Get the Title of the Category that is the parent of the Quarter selected in the link
				if ((lTitle = oRepeaterItem.FindControl("lTaxCategory") as Literal) != null
					&& (oTaxQuarterlyUpdateCategory = oTaxQuarterlyUpdate.Parent) != null)
                {
					lTitle.Text = oTaxQuarterlyUpdateCategory.GetText("Title");        
				}

				lDate = oRepeaterItem.FindControl("lDate") as Literal;

				if (lDate != null)
                {
					//get the date
					lDate.Text = oTaxQuarterlyUpdate.GetField("Quarter", "Date").GetDateString("MM/dd/yy",
						oTaxQuarterlyUpdate.GetText("Quarter", "Message"));
                }
            }
        }
    }

	private string GetCleanTitle()
	{
		return string.Empty;
	}
    
</script>
<table class="greytable" width="100%">
	<thead><tr><th>Reviews &amp; Summaries</th><th class="date">Date</th></tr></thead>
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