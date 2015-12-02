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
        IEnumerable<Item> oItems = ContextExtension.CurrentItem.GetChildren();
        if (oItems.Count() > 0)
        {
            rChild.DataSource = oItems;
            rChild.ItemDataBound += new RepeaterItemEventHandler(rChild_ItemDataBound);
            rChild.DataBind();
        }

    }
    void rChild_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        HyperLink lTitle;
        Literal lSummary;
      
        Item oData = e.Item.DataItem as Item;
        if (oData != null)
        {
            lTitle = (HyperLink)e.Item.FindControl("hTitle");
            lSummary = (Literal)e.Item.FindControl("lSummary");
            lTitle.Text = oData.GetText("Page", "Title");
            lTitle.NavigateUrl = oData.GetURL();
            lSummary.Text = oData.GetText("Page", "Summary");
          
        }
    }
</script>
<asp:Repeater runat="server" ID="rChild">
    <ItemTemplate>
     <div class="section">
                    <h6><asp:HyperLink runat="server" ID="hTitle" style="color:Black"></asp:HyperLink> </h6>
                   <p> <asp:Literal runat="server" ID="lSummary"></asp:Literal></p>
                </div>
    </ItemTemplate>
</asp:Repeater>
