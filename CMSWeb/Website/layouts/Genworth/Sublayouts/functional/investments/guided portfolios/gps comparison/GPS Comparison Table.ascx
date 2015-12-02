<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/investments/guided portfolios/gps comparison/GPS Comparison Legend.ascx" TagName="LegendCtrl" %>
<script runat="server">
    
    private List<Item> columnsDefinition { get; set; }

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
        rMultipleTables.DataSource = ContextExtension.CurrentItem.GetChildrenOfTemplate("GPS Comparison Table");
        rMultipleTables.DataBind();
    }
    
    private void rMain_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        if ((oItem = (Item)e.Item.DataItem) != null)
        {
            var guidedComparisonChart = e.Item.FindControl("guidedComparisonChart") as HtmlGenericControl;
            var note = e.Item.FindControl("note") as HtmlGenericControl;
            var rTables = e.Item.FindControl("rTables") as Repeater;
            var legendCtrl = ((ASP.layouts_genworth_sublayouts_functional_investments_guided_portfolios_gps_comparison_gps_comparison_legend_ascx)(e.Item.FindControl("legendCtrl")));
            
            if (guidedComparisonChart.Visible = oItem != null)
            {
                string noteText = oItem.GetText("Data", "Footer Notes");
                if (note.Visible = noteText != null)
                {
                    note.InnerText = noteText;
                }

                columnsDefinition = oItem.GetChildrenOfTemplate("GPS Comparison Column Definition");
                List<Item> subtable = oItem.GetChildrenOfTemplate("GPS Comparison Subtable");

                rTables.DataSource = subtable;
                rTables.DataBind();

                legendCtrl.SetContextItems(oItem);
            }
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
            
            //IsJoined
            if (oItem.GetText("Design Data", "Has Top Bar") == "1")
            {
                table.Attributes["style"] = String.Format("border-top: 6px solid {0};",
                    oItem.GetText("Design Data", "Top Bar Color"));
            }
            
            //IsHeader
            if (oItem.GetText("Design Data", "Is Header") == "1")
            {
                table.Attributes["class"] += "header";
            }
            
            //HasBackgroundColor
            string bgcolor;
            if (!string.IsNullOrWhiteSpace(bgcolor = oItem.GetText("Design Data", "Background Color")))
            {
                table.Style.Add(HtmlTextWriterStyle.BackgroundColor, bgcolor);
            }

            List<Item> rowItems = oItem.GetChildrenOfTemplate("GPS Comparison Row");
            foreach (Item rowItem in rowItems)
            {
                var row = geneteRow(rowItem.GetChildrenOfTemplate("GPS Comparison Value"));
                table.Controls.Add(row);
            }
        }
    }

    private HtmlTableRow geneteRow(List<Item> values)
    {
        var row = new HtmlTableRow();
        //foreach (var item in values)
        for (int c = 0; c < values.Count; c++)
        {
            var item = values[c];
            var cDefinition = columnsDefinition.ElementAtOrDefault(c);
            var cell = new HtmlTableCell();
            cell.InnerText = item.GetText("Data", "Value");

            if (cDefinition != null)
            {
                var defWidth = cDefinition.GetText("Design Data", "Width");
                if(!string.IsNullOrWhiteSpace(defWidth))
                {
                    cell.Width = defWidth;
                }
            }

            row.Controls.Add(cell);
        }
        return row;
    }
    
</script>

<asp:Repeater ID="rMultipleTables" runat="server" OnItemDataBound="rMain_ItemDataBound">
<ItemTemplate>

<div class="guidedComparisonChart" id="guidedComparisonChart" runat="server">

    <asp:Repeater ID="rTables" runat="server" 
            onitemdatabound="rTables_ItemDataBound">
	<ItemTemplate>

        <table id="table" runat="server"></table>

    </ItemTemplate>
    </asp:Repeater>
    <div class="clear"></div>

    <Gen:LegendCtrl ID="legendCtrl" runat="server" />

    <p class="note" id="note" runat="server">
        * Profile 1 Mandate is Focused Absolute Return, Profile 5 Mandate is Focused Unconstrained Return
    </p>

</div>

</ItemTemplate>
</asp:Repeater>
