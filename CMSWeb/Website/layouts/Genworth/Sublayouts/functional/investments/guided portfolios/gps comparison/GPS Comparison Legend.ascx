<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<script runat="server">
    
    private Item contextItem { get; set; }
    List<Item> columnsDefinition { get; set; }
    private bool isContextSet;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            if (contextItem == null && !isContextSet)
            {
                contextItem = ContextExtension.CurrentItem.GetChildrenOfTemplate("GPS Comparison Table").FirstOrDefault();
            }
            BindData();
        }
    }

    public void SetContextItems(Item contextItems)
    {
        contextItem = contextItems;
        isContextSet = true;
    }
    
    private void BindData()
    {
        if (rLeyend.Visible = contextItem != null)
        {
            rLeyend.DataSource = contextItem.GetChildrenOfTemplate("GPS Comparison Subtable").Where(f => f.GetText("Design Data", "Is Header") != "1");
            rLeyend.DataBind();
        }   
    }

    private void rTables_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        if ((oItem = (Item)e.Item.DataItem) != null)
        {
            var table = e.Item.FindControl("table") as HtmlTable;
            
            //IsJoined
            if (oItem.GetText("Design Data", "Is Joined With Previous") == "1")
            {
                table.Attributes["class"] = "joined";
            }
            
            //HasBackgroundColor
            string bgcolor;
            if (!string.IsNullOrWhiteSpace(bgcolor = oItem.GetText("Design Data", "Background Color")))
            {
                table.Style.Add(HtmlTextWriterStyle.BackgroundColor, bgcolor);
            }
        }
    }
    
</script>

<asp:Repeater ID="rLeyend" runat="server">
<HeaderTemplate>
    <ul class="legend">
</HeaderTemplate>
<ItemTemplate>
    <li>
        <div style="background-color:<%# ((Item)Container.DataItem).GetText("Design Data", "Background Color")%>"></div> 
        <%# ((Item)Container.DataItem).GetText("Data", "Legend")%>
    </li>
</ItemTemplate>
<FooterTemplate>
    </ul>
</FooterTemplate>
</asp:Repeater>
   
<div class="clear"></div>
