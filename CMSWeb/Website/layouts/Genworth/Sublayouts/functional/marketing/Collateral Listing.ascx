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
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

	private void BindData()
	{
		rItems.DataSource = ItemExtension.GetItemsOfTemplate(new List<Item>(), "Donnelly Publication");
		//rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		//HyperLink hLink;
		Literal lDescription;

		oItem = (Item)e.Item.DataItem;

		//does this repeater contain an item?
		if (oItem != null)
		{
			lDescription = (Literal)e.Item.FindControl("lIntroduction");

			lDescription.Text = oItem.DisplayName;
			
			////get the web controls
			//hLink = (HyperLink)e.Item.FindControl("hLink");

			////set the link text
			//hLink.Text = oItem.DisplayName;

			////set the link
			//oItem.ConfigureHyperlink(hLink);

			////set the mouse over
			//hLink.Attributes.Add("onmouseover", "dropmenuOpen(this, '" + oItem.ID.Guid.ToString() + "', event);");
		}
	}
	
	
</script>

<asp:Repeater ID="rItems" runat="server">
	<ItemTemplate>
		<h2><asp:HyperLink ID="hLink" runat="server" /></h2>
		<p><asp:Literal ID="lIntroduction" runat="server" /></p>
	</ItemTemplate>
</asp:Repeater>

