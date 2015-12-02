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
  string sPrice;
  lRoomRate.Text = ContextExtension.CurrentItem.GetText("Room Rate");
        if (string.IsNullOrWhiteSpace(sPrice = ContextExtension.CurrentItem.GetText("Pricing")) && string.IsNullOrWhiteSpace(lRoomRate.Text))
        {
            pricingPanel.Visible = false;
        }
  lPrice.Text = sPrice;

    }
</script>
<asp:Panel runat="server" ID="pricingPanel">
<h4>
    Pricing</h4>
<b class="price">
    <asp:Literal runat="server" ID="lPrice"></asp:Literal></b>
<br />
<b class="price">
    <asp:Literal runat="server" ID="lRoomRate"></asp:Literal></b> 
</asp:Panel>