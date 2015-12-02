<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
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
		Item oRootItem;
		var oParentItems = ContextExtension.CurrentItem.GetParentItems();
		oParentItems.Reverse();

		if ((oRootItem = oParentItems.Skip(2).FirstOrDefault()) != null)
		{
			headTitle.InnerText = oRootItem.DisplayName;
		}
	}
	
</script>
<div id="account-investments-modal-window">
	<div class="headTitle">
		<h2 runat="server" id="headTitle"></h2>
	</div>
	<div id="container">
		<a href="javascript:window.print()" id="print_option"></a>
		<div id="menuTabs">
			<sc:Placeholder Key="MenuTabs" runat="server" />
			<div class="clear"></div>
		</div>
		<sc:Placeholder Key="Container" runat="server" />
	</div>
	<sc:Placeholder Key="Footer" runat="server" />
</div>