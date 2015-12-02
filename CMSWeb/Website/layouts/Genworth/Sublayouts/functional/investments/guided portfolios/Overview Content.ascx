<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
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
	}
</script>

<div id="Investments_GuidedPortfolios_Overview">
    <sc:Placeholder ID="Placeholder2" Key="title" runat="server" />
    <div class="clear"></div>
    <div class="gc brochure inside">
	    <sc:Placeholder ID="Placeholder1" Key="brochure" runat="server" />
    </div>
    <div class="gc description">
	    <sc:Placeholder ID="Placeholder4" Key="description" runat="server" />
    </div>
    <div class="gc video">
	    <sc:Placeholder ID="Placeholder5" Key="video" runat="server" />
    </div>
    <div class="clear"></div>
</div>