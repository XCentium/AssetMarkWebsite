<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<script runat="server">
    
    int iShowAll = 0;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            BindData();
            divShowAll.Visible = iShowAll > 1;
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
            spanToggler = (HtmlGenericControl)e.Item.FindControl("spanToggler");
            
            lTitle.Text = oData.GetText("Title");
            lSummary.Text = oData.GetText("Summary");
            lBody.Text = oData.GetText("Body");
            spanToggler.Visible = lBody.Text.Trim().Length > 0;
            iShowAll += (spanToggler.Visible ? 1 : 0);
        }
    }
</script>
<div id="divShowAll" runat="server" class="section">
    <span class="show-all" showText="+ Show All +" hideText="- Hide All -">+ Show All +</span>
</div>
<asp:Repeater runat="server" ID="rData">
    <ItemTemplate>
        <div class="section">
            <h6>
                <asp:Literal ID="lTitle" runat="server" /></h6>
            <asp:Literal ID="lSummary" runat="server"></asp:Literal>
        </div>
        <div class="section">
            <asp:Literal ID="lBody" runat="server" />
			<span id="spanToggler" runat="server" class="toggler" showtext="+ Show More +" hidetext="Hide Content">+ Show More +</span>
        </div>
    </ItemTemplate>
</asp:Repeater>
