<%@ Control Language="C#" Debug="true" ClassName="CarouselVerticalMenuLogo" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">

	public IEnumerable<Item> DataSource { get; set; }
	private Item oCurrentItem { get; set; }

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			if (oCurrentItem == null)
			{
				oCurrentItem = ContextExtension.CurrentItem;
			}
			
			if (DataSource == null)
			{
				var oCarousel = oCurrentItem.GetChildrenAndItemsOfTemplate("Carousel").FirstOrDefault();
				DataSource = oCarousel.GetChildrenAndItemsOfTemplate(new string[] { "Carousel Video Item", "Carousel Static Item" });
			}
			
			DataBind();
		}
	}
	
    public override void DataBind()
	{
        if (DataSource != null && DataSource.Count() > 0)
        {
            carousel_menuItems.DataSource = DataSource;
            carousel_menuItems.DataBind();

        }
	}
        
    protected void carousel_menuItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item contentItem = e.Item.DataItem as Item;
        var link = e.Item.FindControl("link") as HyperLink;
        link.Text = contentItem.GetText("Title");

        switch (contentItem.TemplateName)
        {
            case "Carousel Video Item":
                link.Text += " <img src='/CMSContent/Images/carousel-video-ico.png'  alt='Video' title='Video' class='videoClicPlay' />";
                break;
            case "Carousel Static Item":

                break;
        }        
    }
</script>
<div class="carousel_menuLogo">
	<sc:Placeholder ID="logoContainer" runat="server" Key="logoContainer" />
</div>
<ul class="carousel_menuItems">
    <asp:Repeater ID="carousel_menuItems" runat="server"
        OnItemDataBound="carousel_menuItems_ItemDataBound">
        <ItemTemplate>
            <li>
                <asp:HyperLink runat="server" ID="link"></asp:HyperLink></li>
        </ItemTemplate>
    </asp:Repeater>
</ul>

