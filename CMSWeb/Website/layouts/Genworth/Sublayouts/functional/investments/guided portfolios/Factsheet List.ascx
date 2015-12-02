<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

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
		var Items = ContextExtension.CurrentItem.GetChildrenOfTemplate("Document Library");
		int indexItem = 0;
		if (int.TryParse(this.GetParameter("Control to select"), out indexItem))
		{
			indexItem--;
		}

		Item oCurrentItem = Items.ElementAtOrDefault(indexItem);

		container.Visible = oCurrentItem != null;

		if (container.Visible)
		{
			listTitle.InnerText = oCurrentItem.GetText("Title");
			description.InnerText = oCurrentItem.GetText("Description");

			//Todo: delete this

			rFactSheet.DataSource = oCurrentItem.GetMultilistItems("Items")
				.Where(item => item.InstanceOfTemplate("Portfolio Strategist Sheet")
					|| item.InstanceOfTemplate("Fact Sheets Group"));
			rFactSheet.DataBind();
		}

	}

	private void rFactSheet_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			var sheetTitle = e.Item.FindControl("sheetTitle") as HtmlGenericControl;
			var rLinks = e.Item.FindControl("rLinks") as Repeater;

			if (e.Item.ItemIndex == 0)
			{
				var factsheetItem = e.Item.FindControl("factsheetItem") as HtmlGenericControl;
				factsheetItem.Attributes["class"] += " first";
			}

			sheetTitle.InnerText = oItem.GetText("Title");

			if (oItem.InstanceOfTemplate("Portfolio Strategist Sheet"))
			{
				rLinks.DataSource = new[] { oItem };
				rLinks.DataBind();
			}
			else if (oItem.InstanceOfTemplate("Fact Sheets Group"))
			{
				var items = oItem.GetMultilistItems("Items")
					.Where(item => item.InstanceOfTemplate("Portfolio Strategist Sheet"));
				rLinks.DataSource = items;
				rLinks.DataBind();
			}
		}
	}

	protected void rLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			var sheetLink = e.Item.FindControl("sheetLink") as HtmlAnchor;
			var sheetBody = e.Item.FindControl("sheetBody") as HtmlGenericControl;

			if (e.Item.ItemIndex == 0)
			{
				var documentLinkItem = e.Item.FindControl("documentLinkItem") as HtmlGenericControl;
				documentLinkItem.Attributes["class"] += " first";
			}

			sheetBody.InnerHtml = oItem.GetText("Body");
			string sPath;
			if (!string.IsNullOrEmpty(sPath = oItem.GetImageURL("Document", "File")))
			{
				sheetLink.HRef = "~/" + sPath;
			}

			//Set omniture tag
			oItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, sheetLink);
		}
	}
</script>
<div class="factsheetContainer" id="container" runat="server">
	<div class="titleBlock">
		<h3 id="listTitle" class="listTitle" runat="server"></h3>
		<p class="description" id="description" runat="server"></p>
	</div>
	<div class="bodyBlock">
		<asp:Repeater ID="rFactSheet" runat="server"
			OnItemDataBound="rFactSheet_ItemDataBound">
			<ItemTemplate>
				<div class="factsheetItem" id="factsheetItem" runat="server">
					<h4 id="sheetTitle" runat="server"></h4>
					<asp:Repeater ID="rLinks" runat="server" OnItemDataBound="rLinks_ItemDataBound">
						<ItemTemplate>
							<div class="documentLinkItem" id="documentLinkItem" runat="server">
								<a href="#" id="sheetLink" class="sheetLink" runat="server" target="_blank"></a>
								<div id="sheetBody" runat="server" class="sheetBody"></div>
								<div class="clear"></div>
							</div>
						</ItemTemplate>
					</asp:Repeater>
				</div>
			</ItemTemplate>
		</asp:Repeater>
	</div>
</div>