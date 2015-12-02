<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
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
        IEnumerable<Item> oItems = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Quote"));
        if (oItems.Count() > 0)
        {
            rQuote.DataSource = oItems;
            rQuote.ItemDataBound += new RepeaterItemEventHandler(rQuote_ItemDataBound);
            rQuote.DataBind();
        }
        
    }
    void rQuote_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Literal lQuote;
        Literal lCite;
        Item oQuote = e.Item.DataItem as Item;
        if (oQuote != null)
        {
            lQuote = (Literal)e.Item.FindControl("lQuote");
            lCite = (Literal)e.Item.FindControl("lCite");
            lQuote.Text = oQuote.GetText("Quote");
            lCite.Text = oQuote.GetText("Cite");
        }
    }
  </script>
<asp:Repeater runat="server" ID="rQuote">
    <ItemTemplate>
        <blockquote>
            <asp:Literal ID="lQuote" runat="server" />
            <cite>
                <asp:Literal ID="lCite" runat="server" /></cite>
        </blockquote>
    </ItemTemplate>
</asp:Repeater>
