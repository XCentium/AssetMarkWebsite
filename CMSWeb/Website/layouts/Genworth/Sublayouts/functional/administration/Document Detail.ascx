<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
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
        lTitle.Text = ContextExtension.CurrentItem.GetText("Page","Title");
		lBody.Text = ContextExtension.CurrentItem.GetText("Page", "Body");
	
		hPreviousPage.NavigateUrl = "javascript:history.go(-1)";
    }
</script>
<div class="toptxt">
    <h2><asp:Literal runat="server" ID="lTitle"></asp:Literal></h2>
	<asp:Literal runat="server" ID="lBody" ></asp:Literal>
</div>
<p>
	<asp:HyperLink ID="hPreviousPage" runat="server">BACK TO PREVIOUS PAGE</asp:HyperLink>
</p>