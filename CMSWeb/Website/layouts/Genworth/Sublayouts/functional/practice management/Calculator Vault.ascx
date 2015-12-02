<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
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
        Item oCurrentItem;
        oCurrentItem = ContextExtension.CurrentItem;
        lTitle.Text = oCurrentItem.GetText("Page", "Title");
        lSummary.Text = oCurrentItem.GetText("Page", "Summary");
        string sMediaUrl = oCurrentItem.GetImageURL("Image");
        if (!string.IsNullOrWhiteSpace(sMediaUrl))
        {
            iImage.ImageUrl = string.Format("~/{0}", sMediaUrl);
        }
        else
            iImage.Visible = false;
        rCalculator.DataSource = oCurrentItem.GetChildrenOfTemplate("Internal Calculator");
        rCalculator.ItemDataBound += new RepeaterItemEventHandler(rCalculator_ItemDataBound);
        rCalculator.DataBind();
    }

    void rCalculator_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
       HyperLink hDownload;
       Literal lCalculatorTitle;
       Literal lCSummary;
       Item oItem = e.Item.DataItem as Item;
       if (oItem != null)
       {
           hDownload = (HyperLink)e.Item.FindControl("hDownload");
           lCalculatorTitle = (Literal)e.Item.FindControl("lCalculatorTitle");
           lCSummary = (Literal)e.Item.FindControl("lCSummary");
           oItem.ConfigureDocumentHyperlink(hDownload);
		   hDownload.Visible = hDownload.NavigateUrl != string.Empty;
          
           lCalculatorTitle.Text = oItem.GetText("Page", "Title");
           lCSummary.Text = oItem.GetText("Page", "Summary");
       }
            
    }
</script>
<div class="calculator-block">
    <h5 class="content-title"><asp:Literal runat="server" ID="lTitle"></asp:Literal></h5>
    <asp:Image runat="server" ID="iImage" />
    <asp:Literal runat="server" ID="lSummary"></asp:Literal>
    <div class="clear">
    </div>
    <ul class="calculator-list">
        <asp:Repeater runat="server" ID="rCalculator">
            <ItemTemplate>
                <li >
                    <asp:HyperLink runat="server" ID="hDownload" Text="Download Now"></asp:HyperLink><h6>
                        <asp:Literal runat="server" ID="lCalculatorTitle"></asp:Literal></h6>
                    <asp:Literal runat="server" ID="lCSummary"></asp:Literal>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
</div>
