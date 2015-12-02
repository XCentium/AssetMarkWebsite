<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
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
		var CssClassParam = this.GetParameter("CssClassName");
		var CssClasses = CssClassParam.Split(';');
		var rows = new[] { r1, r2 };
		for (var c = 0; c < rows.Length; c++)
		{
			var row = rows[c];
			var sClass = CssClasses.Skip(c).Take(1).FirstOrDefault();
			if (!string.IsNullOrWhiteSpace(sClass))
			{
				row.Attributes["class"] += " " + sClass;
			}
		}
	}
</script>

<div class="rowContainer">
    <div class="rowContent r1" runat="server" id="r1">
        <sc:Placeholder runat="server" Key="r1" />
		<div class="clear"></div> 
    </div>
    <div class="rowContent r2" runat="server" id="r2">
        <sc:Placeholder runat="server" Key="r2" />
		<div class="clear"></div> 
    </div>
    <div class="clear"></div>
</div>