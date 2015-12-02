<%@ Control Language="C#" Debug="true" ClassName="Carousel4SquareMenu" %>
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
            carousel_menuItems.DataSource = DataSource;
            carousel_menuItems.DataBind();
        }
    }

    protected void carousel_menuItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item contentItem = e.Item.DataItem as Item;
        var link = e.Item.FindControl("link") as HyperLink;
        link.ToolTip = contentItem.GetText("Title");
        link.Attributes["href"] = "#";
    }
</script>
<div class="carousel_menuItems">
    <asp:Repeater ID="carousel_menuItems" runat="server"
        OnItemDataBound="carousel_menuItems_ItemDataBound">
        <ItemTemplate>
            <asp:HyperLink runat="server" ID="link"></asp:HyperLink>
        </ItemTemplate>
    </asp:Repeater>
</div>
