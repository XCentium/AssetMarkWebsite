<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/global component list/Image Link.ascx"  TagName="ImageLink" %>
<script runat="server">
    bool bIsSecondLevel = false;
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (CurrentItem == null)
        {
            var ContainerID = this.GetParameter("Container ID");
            CurrentItem = ContextExtension.CurrentItem;

            if (!string.IsNullOrWhiteSpace(ContainerID))
            {
                var componentContainer = ContextExtension.CurrentItem.GetChildrenOfTemplate("Global Component Container")
                    .Where(f => {
                        var response = false;
                        var value = f.GetText("Global Component", "Container ID");
                        if(!string.IsNullOrWhiteSpace(value)){
                            response = value.ToLower() == ContainerID.ToLower();
                        }
                        return response;
                    }).FirstOrDefault();
                
                if (componentContainer != null) CurrentItem = componentContainer;
            }
        }
        
        if (!Page.IsPostBack)
        {
            BindData();
        }
    }

    public Item CurrentItem { get; set; }

    private void BindData()
    {
        component_list.DataSource = CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate(
            new string[] { "Image Link" }));
        component_list.DataBind();

    }

    protected void component_list_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var panel = e.Item.FindControl("component_item") as Panel;
        var oItem = e.Item.DataItem as Item;
        var control = new ImageLink()
        {
            CurrentItem = oItem
        };
        panel.Controls.Add(control);
        control.DataBind();
    }
</script>
<div class="component_list">
<asp:Repeater runat="server" ID="component_list" OnItemDataBound="component_list_ItemDataBound">
<ItemTemplate>
    <asp:Panel runat="server" ID="component_item" CssClass="component_item"></asp:Panel>
</ItemTemplate>
</asp:Repeater>
</div>