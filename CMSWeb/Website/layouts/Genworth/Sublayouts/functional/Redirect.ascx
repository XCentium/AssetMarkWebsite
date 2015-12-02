<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		Item oItem = ContextExtension.CurrentItem;
		if (oItem.InstanceOfTemplate("Link"))
		{
			Response.Redirect(oItem.GetURL());
		}
	}
	
</script>