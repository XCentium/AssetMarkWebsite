<%@ Control Language="C#" Debug="true" ClassName="CarouselHero" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/carousel/Carousel Video Item.ascx" TagName="CarouselVideoItem" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/carousel/Carousel Static Item.ascx" TagName="CarouselStaticItem" %>
<script runat="server">
    
    public IEnumerable<Item> DataSource { get; set; }

    public override void DataBind()
	{
        if (DataSource != null && DataSource.Count() > 0)
        {
            carousel_contentItems.DataSource = DataSource;
            carousel_contentItems.DataBind();
        }
	}

    protected void carousel_contentItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item contentItem = e.Item.DataItem as Item;

        switch (contentItem.TemplateName)
        {
            case "Carousel Video Item":
                var videoItem = new CarouselVideoItem();
                videoItem.SetCurrentItem(contentItem);
                e.Item.Controls.Add(videoItem);
                break;
            case "Carousel Static Item":
                var staticItem = new CarouselStaticItem();
                staticItem.SetCurrentItem(contentItem);
                e.Item.Controls.Add(staticItem);
                break;
        }
    }
    
</script>

<asp:Repeater ID="carousel_contentItems" runat="server" 
        onitemdatabound="carousel_contentItems_ItemDataBound">
</asp:Repeater>
