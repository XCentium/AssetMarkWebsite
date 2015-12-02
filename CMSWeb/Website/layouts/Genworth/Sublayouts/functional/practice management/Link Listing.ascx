<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
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
		Item oCurrentItem;
		Dictionary<string, IEnumerable<Item>> oData = new Dictionary<string, IEnumerable<Item>>();
		IEnumerable<Item> oLinkFolders;

		oCurrentItem = ContextExtension.CurrentItem;
		title.InnerText = oCurrentItem.GetText("Title");
		
		oLinkFolders = oCurrentItem.GetChildrenOfTemplate("Link Group").Where(oFolder => oFolder.HasChildren);
		if (oLinkFolders.Count() > 0)
		{
			rCategories.DataSource = oLinkFolders;
			rCategories.ItemDataBound += new RepeaterItemEventHandler(rCategories_ItemDataBound);
			rCategories.DataBind();
		}
		
	}

	void rCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Repeater rItems;
		Literal lTitle;
		Item oGroupLink;
		rItems = (Repeater)e.Item.FindControl("rItems");
		lTitle = (Literal)e.Item.FindControl("lTitle");
		oGroupLink = e.Item.DataItem as Item;
		lTitle.Text = oGroupLink.GetText("Title");
		rItems.DataSource = oGroupLink.GetChildrenOfTemplate("Link");
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();

	}

	void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HtmlGenericControl liListItem;
		HyperLink lLink;
		Item oItem;
		Item oLinkedItem;
		oItem = e.Item.DataItem as Item;

		if (oItem != null)
		{

			lLink = (HyperLink)e.Item.FindControl("lLink");
			liListItem = (HtmlGenericControl)e.Item.FindControl("liListItem");
			if ((oLinkedItem = oItem.GetListItem("Item")) != null)
			{
				oItem = oLinkedItem;
				if (oItem.InstanceOfTemplate("Video"))
				{
					liListItem.Attributes.Add("class", "video");
					oItem.ConfigureVideoShadowbox(lLink);
					lLink.Text = oItem.GetText("Title");
					return;
				}
			}
			oItem.ConfigureDocumentHyperlink(lLink, liListItem.Attributes);
			lLink.Text = oItem.DisplayName;
            oItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, lLink);
		}
	}
	

</script>
<div class="PracticeManagement KnowledgeCenter">
	<!-- START PRACTICE MANAGEMENT STYLES -->
	<h5 class="content-title" runat="server" id="title"></h5>
	<asp:Repeater ID="rCategories" runat="server">
		<ItemTemplate>
			<h4>
				<asp:Literal runat="server" ID="lTitle"></asp:Literal></h4>
			<ul class="link-list">
				<asp:Repeater runat="server" ID="rItems">
					<ItemTemplate>
						<li id="liListItem" runat="server">
							<asp:HyperLink runat="server" ID="lLink"></asp:HyperLink></li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
		</ItemTemplate>
	</asp:Repeater>
	<!-- END PRACTICE MANAGEMENT STYLES -->
</div>
