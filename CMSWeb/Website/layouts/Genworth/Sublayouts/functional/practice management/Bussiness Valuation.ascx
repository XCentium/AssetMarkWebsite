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
        IEnumerable<Item> oItems = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Generic Information"));
        if (oItems.Count() > 0)
        {
            rData.DataSource = oItems;
            rData.ItemDataBound += new RepeaterItemEventHandler(rData_ItemDataBound);
            rData.DataBind();
        }

    }

    void rData_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Literal lTitle;
        Literal lSummary;
        Literal lBody;
        HtmlGenericControl spanToggler;
        Item oData = e.Item.DataItem as Item;
        
        
        if (oData != null)
        {
            lTitle = (Literal)e.Item.FindControl("lTitle");
            lSummary = (Literal)e.Item.FindControl("lSummary");
            lBody = (Literal)e.Item.FindControl("lBody");
            
            lTitle.Text = oData.GetText("Title");
            lSummary.Text = oData.GetText("Summary");
            lBody.Text = oData.GetText("Body");
        }
    }
</script>
<asp:Repeater runat="server" ID="rData">
    <ItemTemplate>
        <div>
            <h6>
                <asp:Literal ID="lTitle" runat="server" /></h6>
            <asp:Literal ID="lSummary" runat="server"></asp:Literal>
        </div>
        <div>
            <asp:Literal ID="lBody" runat="server" />
        </div>
    </ItemTemplate>
</asp:Repeater>
