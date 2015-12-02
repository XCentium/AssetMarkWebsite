<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
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
<div id="guidedComparison">
    <div class="container">
        <sc:Placeholder ID="Placeholder2" runat="server" Key="brochure" />
        <sc:Placeholder ID="Placeholder1" runat="server" Key="title" />
        <sc:Placeholder ID="Placeholder3" runat="server" Key="description" />
        <div class="clear"></div>
    </div>

	<sc:Placeholder ID="Placeholder4" runat="server" Key="graph" />
</div>
