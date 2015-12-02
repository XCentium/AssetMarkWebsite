<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
	
	int iLast;
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
		IEnumerable<Item> oSnippets;
		rItems.DataSource = oSnippets = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").Where(oItem => oItem.InstanceOfTemplate("Snippet"));
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		iLast = oSnippets.Count() - 1;
		rItems.DataBind();
			
		//hide the items if nothing to show
		pItems.Visible = rItems.Items.Count > 0;
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		HtmlGenericControl lListItem;
		PlaceHolder pHeading;
		Literal lHeading;
		Literal lHtml;
		StringBuilder sClass;
		
		//does this repeater contain an item?
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
			pHeading = (PlaceHolder)e.Item.FindControl("pHeading");
			lHeading = (Literal)e.Item.FindControl("lHeading");
			lHtml = (Literal)e.Item.FindControl("lHtml");

			//set the fields
			lHeading.Text = oItem.GetText("Title").Trim();
			pHeading.Visible = lHeading.Text.Length > 0;
			lHtml.Text = oItem.GetText("Body");
			
			//create a stringbuilder to hold class info
			sClass = new StringBuilder();

			//is this the first item?
			if (e.Item.ItemIndex == 0)
			{
				sClass.Append(" first");
			}
			else if (e.Item.ItemIndex == iLast)
			{
				sClass.Append(" last");
			}
			
			//append the styling
			if (sClass.Length > 0)
			{
				lListItem.Attributes.Add("class", sClass.ToString().Trim());
			}
		}
	}
</script>
<asp:PlaceHolder ID="pItems" runat="server">
	<hr class="below-tabs" />
	<div class="foot-cards">
		<ul>
			<asp:Repeater ID="rItems" runat="server">
				<ItemTemplate>
					<li id="lListItem" runat="server">
						<div class="html center">
							<asp:PlaceHolder ID="pHeading" runat="server"><h6><asp:Literal ID="lHeading" runat="server" /></h6></asp:PlaceHolder>
							<asp:Literal ID="lHtml" runat="server" />
						</div>
					</li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>
		<div class="clear"></div>
	</div>
</asp:PlaceHolder>
