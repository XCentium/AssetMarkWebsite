<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Data.Items" %>

<%@ Import Namespace="ServerLogic.SitecoreExt" %>

<script runat="server">
	
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			// class="help-block" or class="help-block aside-right"
			//
			if (this.GetParameter("AsideRight") == "1")
			{
				divHelpBlock.Attributes.Add("class", "help-block aside-right");
				//divHelpBlock.Attributes.Add("style", "height: 505px;");
			}
			else
			{
				divHelpBlock.Attributes.Add("class", "help-block");
			}
		}
	}

</script>

<div id="divHelpBlock"  runat="server">
	<sc:Placeholder Key="help_content" runat="server" />
</div>

