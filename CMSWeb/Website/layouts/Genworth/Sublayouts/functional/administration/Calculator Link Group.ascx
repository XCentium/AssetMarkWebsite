<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

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
        var currentItem = ContextExtension.CurrentItem.GetChildrenOfTemplate("Calculator Link Group").FirstOrDefault();

        if (currentItem != null)
        {
            title.InnerHtml = currentItem.GetText("Page", "Title");

            var calculators = currentItem.GetChildrenOfTemplate("Calculator Link");
            if (calculators.Count > 0)
            {
                rCalculator.DataSource = calculators;
                rCalculator.DataBind();
            }
        }
    }

    protected void rCalculator_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        HyperLink hLink;

        oItem = (Item)e.Item.DataItem;

        //does this repeater contain an item?
        if (oItem != null)
        {
            //get the web controls
            hLink = (HyperLink)e.Item.FindControl("link");
            oItem.ConfigureHyperlink(hLink);
            hLink.Text = oItem.DisplayName;
        }
    }
</script>

<div class="CalculatorGroupContainer">
    <h3 runat="server" id="title"></h3>
    <div class="CalculatorGroupLinks">

        <asp:Repeater runat="server" ID="rCalculator" OnItemDataBound="rCalculator_ItemDataBound">
            <ItemTemplate>
                <asp:HyperLink runat="server" ID="link"></asp:HyperLink>
            </ItemTemplate>
        </asp:Repeater>
        <div class="clear"></div>
    </div>
</div>
