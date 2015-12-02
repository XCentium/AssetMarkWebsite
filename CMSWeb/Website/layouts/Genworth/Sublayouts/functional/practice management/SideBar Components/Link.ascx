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

        Item oItem = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Preview Link") && !string.IsNullOrWhiteSpace(item.GetText("Body"))).FirstOrDefault();
        if (oItem != null)
        {
            //lTitle.Text = oItem.GetText("Title");
            lBody.Text = oItem.GetText("Body");
          

        }
        else
        {
            dContent.Visible = false;
        }

    }
</script>
<div runat="server" id="dContent">
    <p>
        <asp:Literal ID="lBody" runat="server"></asp:Literal><br />
    </p>
</div>
