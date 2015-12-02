<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		BindData();
	}
	
	private void BindData()
	{
		List<Item> oItems;

		oItems = ContextExtension.CurrentItem.GetChildrenOfTemplate("Calculator");

		//bind the events
		rCalculators.DataSource = oItems;
		rCalculators.ItemDataBound += new RepeaterItemEventHandler(rCalculators_ItemDataBound);
		rCalculators.DataBind();
	}

	private void rCalculators_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oCalculator;
		HyperLink hLink;
		Label lSummary;

		oCalculator = (Item)e.Item.DataItem;

		if (oCalculator != null)
		{
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lSummary = (Label)e.Item.FindControl("lSummary");
			//if (ItemExtension.GetMediaURL(oCalculator.GetField("Document","File")) != null)
			//{
			hLink.Text = oCalculator.GetField("Page", "Title").Value;
			oCalculator.ConfigureHyperlink(hLink);
			//}		
			lSummary.Text = oCalculator.GetField("Page", "Summary").Value;
            oCalculator.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
		}	
	}

</script>

<asp:Repeater ID="rCalculators" runat="server">
	<ItemTemplate>
		<p>
			<asp:HyperLink ID="hLink" runat="server" />
			<br />
			<asp:Label ID="lSummary" runat="server" />
		</p>
	</ItemTemplate>
</asp:Repeater>

<div class="clear"></div>