<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
	
	public Item contextItem { get; set; }

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			if (contextItem == null)
			{
				contextItem = ContextExtension.CurrentItem;
			}
			BindData();
		}
	}

	private void BindData()
	{
		Item oRootItem;
		var oParentItems = ContextExtension.CurrentItem.GetParentItems();
		oParentItems.Reverse();

		if ((oRootItem = oParentItems.Skip(2).FirstOrDefault()) != null)
		{
			rTabs.DataSource = oRootItem.GetChildrenOfTemplate("Account Investment Tab Shadow Page");
			rTabs.DataSource = oRootItem.GetChildrenOfTemplate("Link");
			rTabs.DataBind();
		}

	}

	protected void rTabs_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			var TabHeader = e.Item.FindControl("TabHeader") as HtmlGenericControl;
			var rMenu = e.Item.FindControl("rMenu") as Repeater;
			
			TabHeader.InnerText = oItem.DisplayName;
			rMenu.DataSource = oItem.GetChildrenOfTemplate("Account Investment Tab Shadow Page");
			rMenu.DataBind();
		}
	}

	protected void rMenu_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			var hLink = e.Item.FindControl("hLink") as HyperLink;
			hLink.Text = string.Format("<span class='LinkTextContainer'>{0}</span>", oItem.DisplayName);
			oItem.ConfigureHyperlink(hLink);
			hLink.Attributes["rel"] = string.Empty;
			if (oItem.InSelectedPath())
			{
				hLink.CssClass += " selected";
			}
		}
	}
</script>
<asp:Repeater runat="server" ID="rTabs" OnItemDataBound="rTabs_ItemDataBound">
	<ItemTemplate>
		<asp:Panel runat="server" ID="menuTab" CssClass="menuTab">
			<h4 runat="server" id="TabHeader"></h4>
			<div class="TabSecondaryMenu">
				<asp:Repeater runat="server" ID="rMenu" OnItemDataBound="rMenu_ItemDataBound">
					<ItemTemplate>
						<asp:HyperLink ID="hLink" runat="server" />
					</ItemTemplate>
				</asp:Repeater>
			</div>

		</asp:Panel>
	</ItemTemplate>
</asp:Repeater>