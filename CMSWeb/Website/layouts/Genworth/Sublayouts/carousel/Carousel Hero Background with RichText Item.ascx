<%@ Control Language="C#" Debug="true" ClassName="CarouselHeroBackgroundWithRichText" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/carousel/Carousel Video Item.ascx" TagName="CarouselVideoItem" %>
<script runat="server">
    
    public IEnumerable<Item> DataSource { get; set; }

    public override void DataBind()
    {
        if (DataSource != null && DataSource.Count() > 0)
        {
            carousel_contentItems.DataSource = DataSource;
            carousel_contentItems.DataBind();
            var imgItem = getImageItem(DataSource.FirstOrDefault());
            if (imgItem != null)
            {
                SetSizeStylesToPanel(carouselItemWrapper, imgItem);            
            }
        }
    }

    private Item getImageItem(Item contentItem)
    {
        Item response = null;
        var imageField = contentItem.GetField("Background Image");
        if (imageField != null)
        {
            response = imageField.GetImageItem();
        }
        return response;
    }

    private void SetSizeStylesToPanel(Panel panel, Item imgItem)
    {
        var width = imgItem.GetText("Image", "Width") + "px";
        var height = imgItem.GetText("Image", "Height") + "px";
        panel.Style.Add(HtmlTextWriterStyle.Width, width);
        panel.Style.Add(HtmlTextWriterStyle.Height, height);
    }

    protected void carousel_contentItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item contentItem = e.Item.DataItem as Item;
        var panel = e.Item.FindControl("carouselItemContainer") as Panel;
        var inlineStylesPanel = e.Item.FindControl("inlineStylesPanel") as Panel;
        var videoItem = e.Item.FindControl("videoItem") as CarouselVideoItem;
        var htmlContent = e.Item.FindControl("htmlContent") as HtmlGenericControl;

        var imgItem = getImageItem(contentItem);
        if (imgItem != null)
        {
            SetSizeStylesToPanel(panel, imgItem);
            panel.Style.Add(HtmlTextWriterStyle.BackgroundImage, imgItem.GetMediaURL());

        }
        videoItem.SetCurrentItem(contentItem);
        htmlContent.InnerHtml = contentItem.GetText("Body");
        inlineStylesPanel.Attributes["style"] = contentItem.GetText("Video Inline Styles");
    }
    
</script>
<asp:Panel runat="server" ID="carouselItemWrapper" CssClass="carouselItemWrapper">
    <asp:Repeater ID="carousel_contentItems" runat="server"
        OnItemDataBound="carousel_contentItems_ItemDataBound">
        <ItemTemplate>
            <asp:Panel runat="server" ID="carouselItemContainer" CssClass="carouselItemContainer">
                <asp:Panel runat="server" ID="inlineStylesPanel">
                    <Gen:CarouselVideoItem runat="server" ID="videoItem" />
                </asp:Panel>
                <div class="htmlContent" runat="server" id="htmlContent">
                </div>
            </asp:Panel>
        </ItemTemplate>
    </asp:Repeater>
</asp:Panel>
