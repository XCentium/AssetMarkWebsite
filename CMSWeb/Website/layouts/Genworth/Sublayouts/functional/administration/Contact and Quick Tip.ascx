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
        Item CurrentItem = ContextExtension.CurrentItem.GetChildrenOfTemplate("Contact and Tips").FirstOrDefault(); ;

        if (CurrentItem != null)
        {
            contactText.InnerHtml = CurrentItem.GetText("Contact Text");
			contactText.Visible = !string.IsNullOrWhiteSpace(contactText.InnerHtml);
			
            var oArticle = CurrentItem.GetMultilistItems("Page", "Items")
                        .Where(oItem => oItem.InstanceOfTemplate("Article")).FirstOrDefault();
			tip_link.Text = CurrentItem.GetText("Tip Text");
			tip_link.Visible = oArticle != null && !string.IsNullOrWhiteSpace(tip_link.Text);
			if (tip_link.Visible)
            {
                oArticle.ConfigureHyperlink(tip_link);
                oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, tip_link);
			}
        }
        

    }
</script>
<div class="contact_tip">
	<p runat="server" id="contactText" />
	<asp:HyperLink runat="server" ID="tip_link" />
</div>
