<%@ Control Language="c#" AutoEventWireup="true" ClassName="CarouselStaticItem" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">

    Item oCurrentItem;

    public void SetCurrentItem(Item currentItem)
    {
        oCurrentItem = currentItem;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            if (oCurrentItem == null)
            {
                oCurrentItem = ContextExtension.CurrentItem;
            }
            BindData();
        }
    }

    private void BindData()
    {
        oCurrentItem.ConfigImage("Preview Thumbnail", image);
        oCurrentItem.ConfigureHyperlink(link);
        var parent = oCurrentItem.GetParentItems().Where(p => p.InstanceOfTemplate("Web Page")).FirstOrDefault();
        if (parent != null)
        {
            oCurrentItem.ConfigureOmnitureControl(parent, link);
        }
    }
</script>
<div class="carouselContentStaticItem carouselContentItem">
    <asp:HyperLink runat="server" ID="link">
        <asp:Image runat="server" ID="image" />
    </asp:HyperLink>
</div>

    
