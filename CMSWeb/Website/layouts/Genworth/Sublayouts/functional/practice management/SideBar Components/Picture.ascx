<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
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
        var previewLink = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Preview Link")).FirstOrDefault();
       if(previewLink != null)
       {
            string imgUrl = string.Empty;
            string lnkUrl = string.Empty;
           
            if (string.IsNullOrWhiteSpace(imgUrl = previewLink.GetImageURL("Icon")))
            {
                dImage.Visible = false;
            }
            else
            {
                lLink.ImageUrl = string.Concat("~/", imgUrl);
            }

            if (!string.IsNullOrWhiteSpace(lnkUrl = previewLink.GetURL()))
            {
                lLink.NavigateUrl = lnkUrl;
            }
        } 
    }
</script>
<div runat="server"  id="dImage">
    <asp:HyperLink runat="server" ID="lLink" />
</div>
