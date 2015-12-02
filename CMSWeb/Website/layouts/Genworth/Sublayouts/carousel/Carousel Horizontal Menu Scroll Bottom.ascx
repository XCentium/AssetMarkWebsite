<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/carousel/Carousel Hero with Title.ascx" TagName="CarouselHeroWithTitle" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/carousel/Carousel Scroll Bottom Menu.ascx" TagName="CarouselScrollBottomMenu" %>
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
        var oCarousel = oCurrentItem.GetChildrenAndItemsOfTemplate("Carousel").FirstOrDefault();

        if (oCarousel != null)
        {
            var carouselItems = oCarousel.GetChildrenAndItemsOfTemplate(new string[] { "Carousel Video Item", "Carousel Static Item" });
            cMenu.DataSource = carouselItems;
            cMenu.DataBind();
            cHero.DataSource = carouselItems;
            cHero.DataBind();
        }
    }
</script>
<div class="carousel carousel_horizontal_menu_scroll_bottom">
    <div class="carousel_container">
        <Gen:CarouselHeroWithTitle runat="server" ID="cHero" />
        <div class="clear"></div>
    </div>
    <div class="clear"></div>
    <div class="carousel_menu">
        <Gen:CarouselScrollBottomMenu runat="server" ID="cMenu" />
        <div class="clear"></div>
    </div>
</div>
