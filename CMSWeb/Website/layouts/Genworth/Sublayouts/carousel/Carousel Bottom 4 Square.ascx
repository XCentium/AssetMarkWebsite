<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/carousel/Carousel Hero Background with RichText Item.ascx" TagName="CarouselHeroBackgroundWithRichText" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/carousel/Carousel Menu Bottom 4 Square.ascx" TagName="Carousel4SquareMenu" %>
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
            var carouselItems = oCarousel.GetChildrenAndItemsOfTemplate(new string[] { "Carousel Video Item"}).Take(4);
            cMenu.DataSource = carouselItems;
            cMenu.DataBind();
            cHero.DataSource = carouselItems;
            cHero.DataBind();
        }
    }
</script>
<div class="carousel carousel_bottom_4square">
    <div class="carousel_container">
        <Gen:CarouselHeroBackgroundWithRichText runat="server" ID="cHero" />
    </div>
    <div class="carousel_menu">
        <Gen:Carousel4SquareMenu runat="server" ID="cMenu" />
        <div class="clear"></div>
    </div>
</div>
<div class="clear"></div>