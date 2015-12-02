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
		List<Item> oCollapsibleSnippets;

		//get the articles from the Items field
		oCollapsibleSnippets = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").Where(oItem => oItem.InstanceOfTemplate("Collapsible Snippet")).ToList();

		if (oCollapsibleSnippets != null && oCollapsibleSnippets.Count() > 0)
		{
			//bind the articles
			rItems.DataSource = oCollapsibleSnippets;
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
			rItems.DataBind();
		}			
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oCollapsibleSnippet;
		Literal lPanelName;
		Literal lContent;

		oCollapsibleSnippet = (Item)e.Item.DataItem;

		if (oCollapsibleSnippet != null)
		{
			lPanelName = (Literal)e.Item.FindControl("lPanelName");
			lContent = (Literal)e.Item.FindControl("lContent");

			lPanelName.Text = oCollapsibleSnippet.GetText("Snippet", "Title");
			lContent.Text = oCollapsibleSnippet.GetText("Snippet", "Body");
		}
	}
</script>

<ul class="collapsible">
	<asp:Repeater ID="rItems" runat="server">
		<ItemTemplate>
			<li class="expanded">
				<h6><asp:Literal ID="lPanelName" runat="server"></asp:Literal></h6>
				<div>
					<asp:Literal ID="lContent" runat="server"></asp:Literal>
				</div>
			</li>
		</ItemTemplate>
	</asp:Repeater>
</ul>
