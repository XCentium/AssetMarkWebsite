<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
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
		lSummary.Text = ContextExtension.CurrentItem.GetText("Summary");
		lTitle.Text = ContextExtension.CurrentItem.Parent.DisplayName;
	}
</script>
<div class="grid-system g982">
	<div class="gc c12">
		<div class="help-header">
			<h1>
				<asp:Literal runat="server" ID="lTitle"></asp:Literal></h1>
			<asp:Literal runat="server" ID="lSummary"></asp:Literal>
		</div>
		<div class="clear">
		</div>
	</div>
	<div class="clear">
	</div>
</div>
