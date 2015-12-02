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
		lSummary.Text = ContextExtension.CurrentItem.GetText("Page", "Summary");
		pContentSummary.Visible = !string.IsNullOrEmpty(lSummary.Text); 		
	}
</script>
<div id="pContentSummary" class="content-summary" runat="server">
	<asp:Literal ID="lSummary" runat="server" />
</div>