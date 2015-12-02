<%@ Control Language="c#" AutoEventWireup="true" %>
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
		Item oCurrentItem = ContextExtension.CurrentItem;
		string sImage = string.Empty;
		Item oBanner = oCurrentItem.GetMultilistItems("Items").FirstOrDefault(oItem => oItem.InstanceOfTemplate("Banner"));

		if (oBanner != null)
		{
			lBody.Text = oBanner.GetText("Body");
			if (oBanner.GetText("Use Section Title") == "1")
			{
				List<Item> oParents = oCurrentItem.GetParentItems();

				oParents.Reverse();
				if (oParents.Count >= 1)
				{   
					Item oParent = oParents[1];
					h1title.Visible = !string.IsNullOrWhiteSpace(h1title.InnerText = oParent.InstanceOfTemplate("Link") ? oParent.DisplayName : oParent.GetText("Title"));
				}
			}
			else
			{
				h1title.Visible = !string.IsNullOrWhiteSpace(h1title.InnerText = oBanner.GetText("Title"));
			}
		}
		else
		{
			dDiv.Visible = false;
		}
	}

</script>
<div class="grid-system g982" runat="server" id="dDiv">
	<div class="gc c12">
		<div class="message-block investments-message-block">
			<div class="banner">
				<asp:HyperLink runat="server" ID="hLink" Visible="false">
					<asp:Image runat="server" ID="iImage" />
				</asp:HyperLink>
				<h1 runat="server" id="h1title">
					Quarterly Updates</h1>
				<asp:Literal runat="server" ID="lBody"></asp:Literal>
			</div>
			<span class="close">Close</span> <span class="open">Open</span>
			<div class="clear">
			</div>
		</div>
	</div>
	<div class="clear">
	</div>
</div>
