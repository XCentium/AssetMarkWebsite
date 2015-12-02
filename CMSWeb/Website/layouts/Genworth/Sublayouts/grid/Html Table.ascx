<%@ Control Language="c#"  ClassName="HtmlTable" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>

<%@ Import Namespace="Genworth.SitecoreExt.Utilities.GridComponent" %>
<script runat="server">
	public GridTable DataSource { get; set; }

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			if (DataSource == null)
			{
				string SectionType = this.GetParameter("Search Type");
				DataSource = Genworth.SitecoreExt.Helpers.HtmlTableHelper.GetProvider(SectionType);
			}
			DataBind();
		}
	}

	private void DataBind()
	{
		container.Visible = DataSource != null;
		if (container.Visible)
		{
			container.Attributes.Add("class", "htmlTable htmlTable-" + DataSource.ID);
			rColumns.DataSource = DataSource.Header;
			rColumns.DataBind();
			rRows.DataSource = DataSource.Rows;
			rRows.DataBind();
		}
	}

	void rColumns_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var data = e.Item.DataItem as GridCell;
		var header = (HtmlTableCell)e.Item.FindControl("header");
		setAttributes(data.Attributes, header);
		header.InnerText = data.Value;
	}

	protected void rRows_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var data = e.Item.DataItem as GridRow;
		var row = (HtmlTableRow)e.Item.FindControl("row");

		setAttributes(data.Attributes, row);

		foreach (var item in data.Cells)
		{
			var cell = new HtmlTableCell();
			cell.InnerText = item.Value;
			setAttributes(item.Attributes, cell);
			row.Controls.Add(cell);
		}
	}

	private void setAttributes(Dictionary<string, string> attributes, HtmlControl element)
	{
		if (attributes == null || element == null)
		{
			return;
		}
		foreach (var item in attributes)
		{
			element.Attributes.Add(item.Key, item.Value);
		}
	}
	
	
</script>
<div runat="server" id="container">
	<table id="table" class="filter-results-table" width="100%" cellpadding="0px" cellspacing="0px">
		<thead>
			<asp:Repeater ID="rColumns" runat="server" OnItemDataBound="rColumns_ItemDataBound">
				<ItemTemplate>
					<th runat="server" id="header"></th>
				</ItemTemplate>
			</asp:Repeater>
		</thead>
		<tbody>
			<asp:Repeater ID="rRows" runat="server" OnItemDataBound="rRows_ItemDataBound">
				<ItemTemplate>
					<tr runat="server" id="row">
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</tbody>
	</table>
</div>
