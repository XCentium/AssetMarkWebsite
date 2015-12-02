<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
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
	}
</script>
<asp:Repeater ID="rConsultants" runat="server">
	<ItemTemplate>
		<p>
			<asp:Image ID="iProfileImage" runat="server" />
			<asp:Image ID="iLogoImage" runat="server" />
			<asp:HyperLink ID="hLink" runat="server" />
			<br />
			<asp:Label ID="lSummary" runat="server" />
		</p>
	</ItemTemplate>
</asp:Repeater>
<div class="clear"></div>