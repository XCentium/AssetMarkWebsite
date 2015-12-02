<%@ Control Language="C#" Debug="true" ClassName="CarouselScrollBottomMenu" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">

    public IEnumerable<Item> DataSource { get; set; }

    public override void DataBind()
	{
        if (DataSource != null && DataSource.Count() > 0)
        {
            carousel_groups.DataSource = formatGroups(DataSource);
            carousel_groups.DataBind();
        }
	}
        
    protected void carousel_menuItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item contentItem = e.Item.DataItem as Item;
        var text = e.Item.FindControl("text") as HtmlGenericControl;
        var image = e.Item.FindControl("image") as Image;

        contentItem.ConfigImage("Preview Thumbnail", image);
        text.InnerText = contentItem.GetText("Title");
    }

    private List<IEnumerable<Item>> formatGroups(IEnumerable<Item> ds)
    {
        var groups = new List<IEnumerable<Item>>();
        var index = 4;
        for (int i = 0; i < ds.Count(); i += index)
        {
            groups.Add(ds.Skip(i).Take(index));
        }
        
        return groups;
    }
</script>
<div class="carousel_menuItems">
    <asp:Repeater ID="carousel_groups" runat="server">
        <ItemTemplate>
            <div class="carouselGroupItem">
                <asp:Repeater ID="carousel_menuItems" runat="server"
                    OnItemDataBound="carousel_menuItems_ItemDataBound" DataSource="<%#Container.DataItem %>">
                    <ItemTemplate>
                        <div class="carouselMenuItem">
                            <asp:Image runat="server" ID="image" />
                            <h4 runat="server" id="text"></h4>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <div class="clear"></div>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <a href="#" id="ui-carousel-next"><img src="/CMSContent/Images/right-ball.gif" /></a>
    <a href="#" id="ui-carousel-prev"><img src="/CMSContent/Images/left-ball.gif" /></a>
</div>

