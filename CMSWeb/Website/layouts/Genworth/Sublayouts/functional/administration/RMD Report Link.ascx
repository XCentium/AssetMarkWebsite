<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
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
        var currentItem =  ContextExtension.CurrentItem.GetMultilistItems("Page", "Items")
                .Where(oItem => oItem.InstanceOfTemplate("RMD Report")).FirstOrDefault();

        if (currentItem != null)
        {
            title.InnerHtml = currentItem.GetText("Summary", "Title");
            content.Text = currentItem.GetText("Summary", "Body");
            currentItem.ConfigureHyperlink(link);
            currentItem.ConfigureHyperlink(linkH);
            currentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, link);
            currentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, linkH);
        }
    }

</script>

<div class="RmdReportLinkContainer">
	<asp:HyperLink runat="server" ID="link" CssClass="linkImage"><img src="/CMSContent/Images/RMDreport_icon.jpg" /></asp:HyperLink>
    <div class="RmdReportLinkContent">
        <asp:HyperLink runat="server" ID="linkH" CssClass="linkImage"><h4 runat="server" id="title"></h4></asp:HyperLink>
        <div class="RmdReportLinkText"><asp:Literal runat="server" ID="content"></asp:Literal></div>
    </div>
    <div class="clear"></div>
</div>