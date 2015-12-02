<%@ Control Language="c#" AutoEventWireup="true" ClassName="CarouselVideoItem" %>
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
        image.Attributes["border"] = "0px";
        videoHTML.Text = oCurrentItem.GetText("Embedding Video HTML Code");
        var parent = oCurrentItem.GetParentItems().Where(p => p.InstanceOfTemplate("Web Page")).FirstOrDefault();
        if (parent != null)
        {
            oCurrentItem.ConfigureOmnitureControl(parent, image);
        }
    }
</script>
<div class="carouselContentVideoItem carouselContentItem">
    <div class="imgSlide">
        <asp:Image runat="server" ID="image" />
    </div>
    <div class="videoSlide" style="display:none">
        <asp:Literal runat="server" ID="videoHTML"></asp:Literal>
    </div>
</div>

    
