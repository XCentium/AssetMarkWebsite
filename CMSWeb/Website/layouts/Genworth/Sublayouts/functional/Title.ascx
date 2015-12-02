<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
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
		lTitle.Text = ContextExtension.CurrentItem.GetText("Page", "Title");
        dTitle.Visible = !string.IsNullOrEmpty(lTitle.Text); 
	}
</script>
<div id="dTitle" runat="server">
<h5 class="content-title"><asp:Literal ID="lTitle" runat="server" /></h5>
</div>