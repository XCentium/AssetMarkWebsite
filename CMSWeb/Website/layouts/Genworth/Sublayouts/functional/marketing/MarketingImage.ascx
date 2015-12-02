<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<script runat="server">
	int iMax;
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
		IEnumerable<Item> oItems = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Preview Link") && !string.IsNullOrWhiteSpace(item.GetText("Body")));
		rSidebar.DataSource = oItems;
		rSidebar.ItemDataBound += new RepeaterItemEventHandler(rSidebar_ItemDataBound);
		iMax = oItems.Count()-1;
		rSidebar.DataBind();
		

	}

	void rSidebar_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem = e.Item.DataItem as Item;
		Literal lBody;
		if (oItem != null)
		{
			lBody =(Literal) e.Item.FindControl("lBody");
			lBody.Text = oItem.GetText("Body");
			if (iMax == e.Item.ItemIndex)
			{
				HtmlGenericControl hr = (HtmlGenericControl)e.Item.FindControl("hr");
				hr.Visible = false;
			}

		}
		
	}
</script>
<asp:Repeater ID="rSidebar" runat="server">
	<ItemTemplate>
		<div class="withBorder withPadding center" >
			<asp:Literal runat="server" ID="lBody"></asp:Literal>
		</div>
		<hr runat="server" id="hr" />
	</ItemTemplate>
</asp:Repeater>
