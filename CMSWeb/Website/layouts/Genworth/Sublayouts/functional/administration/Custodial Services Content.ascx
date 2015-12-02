<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/administration/Generic Control.ascx"  TagName="GenericControl" %>
<script runat="server">
    bool bIsSecondLevel = false;
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
        bool bIsLeft = false;
		
		string sParameter =this.GetParameter("Render in Left Column");
		if (!string.IsNullOrWhiteSpace(sParameter) && sParameter.Equals("1"))
			bIsLeft = true;
      
        Item oLinkGroup;
        oLinkGroup = bIsLeft ? ContextExtension.CurrentItem.GetChildrenOfTemplate("Left Link Group").FirstOrDefault() : ContextExtension.CurrentItem.GetChildrenOfTemplate("Right Link Group").FirstOrDefault();
        if (oLinkGroup != null)
        {
            rFirstLevel.DataSource = oLinkGroup.GetChildren();
            rFirstLevel.ItemDataBound += new RepeaterItemEventHandler(rFirstLevel_ItemDataBound);
            rFirstLevel.DataBind();
        }

    }

    void rFirstLevel_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        GenericControl oGeneric;
        oGeneric = (GenericControl)e.Item.FindControl("oGeneric");
        oGeneric.BindingData((Item)e.Item.DataItem, 1);
    }
</script>
<asp:Repeater runat="server" ID="rFirstLevel">
    <ItemTemplate>
        <div class="admin_block">
           <Gen:GenericControl runat="server" ID="oGeneric"></Gen:GenericControl>
        </div>
    </ItemTemplate>
</asp:Repeater>
