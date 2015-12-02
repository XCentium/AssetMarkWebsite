<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/brochures/Brochure with link.ascx" TagName="BrochureWithLink" %>

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
		var oCurrentItem = ContextExtension.CurrentItem;
		var Items = oCurrentItem.GetChildrenOfTemplate("Brochure with link");
		rBrochure.DataSource = Items;
		rBrochure.DataBind();
	}

	protected void rBrochure_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item contentItem = e.Item.DataItem as Item;
		var brochure = e.Item.FindControl("brochure") as BrochureWithLink;

		brochure.SetContextItems(contentItem);
		brochure.DataBind();
	}
</script>

<asp:Repeater ID="rBrochure" runat="server" OnItemDataBound="rBrochure_ItemDataBound">
	<ItemTemplate>
		<Gen:BrochureWithLink ID="brochure" runat="server" />
	</ItemTemplate>
</asp:Repeater>
