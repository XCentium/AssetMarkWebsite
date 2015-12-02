<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
        if (Text == null)
        {
            Text = ContextExtension.CurrentItem.GetText("Page", "Title");
            CssClass = this.GetParameter("CssClassName");
        }
        
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

    public string Text { get; set; }
    public string CssClass { get; set; }

	private void BindData()
	{
        lTitle.InnerHtml = Text;
        if (CssClass != null)
        {
            lTitle.Attributes["class"] = CssClass;
        }
	}
</script>
<h1 ID="lTitle" runat="server"></h1>