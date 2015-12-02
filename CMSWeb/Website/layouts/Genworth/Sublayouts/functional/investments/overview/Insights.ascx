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
		var oCurrentItem = ContextExtension.CurrentItem;
		hero.InnerHtml = oCurrentItem.GetText("Body");
		yellowsidebar.InnerHtml = oCurrentItem.GetText("Blurb");

	}
</script>
<div id="insights">
	<div class="leftside">
		<div class="gc hero" id="hero" runat="server">
		</div>

		<div class="brochurePlaceholder">
			<sc:Placeholder ID="Placeholder5" Key="brochure" runat="server" />
			<div class="clear"></div>
		</div>

	</div>

	<div class="gc yellowsidebar" runat="server" id="yellowsidebar">
	</div>
</div>
