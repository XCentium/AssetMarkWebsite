<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
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
		List<Item> oColumns;
		Item oFirstColumn;
		Item oSecondColumn;
		
		//get a reference to the current item
		oCurrentItem = ContextExtension.CurrentItem;
		
		//bind the title
		lTitle.Text = oCurrentItem.GetText("Title");
		lBody.Text = oCurrentItem.GetText("Body");

		//get the all column items
		if ((oColumns = oCurrentItem.GetChildrenOfTemplate("Generic Column")).Count > 0)
		{
			// bind info from first column
			oFirstColumn = oColumns[0];
			lColumnOneTitle.Text = oFirstColumn.GetText("Column Info", "Title");
			lColumnOneBody.Text = oFirstColumn.GetText("Column Info", "Content");
				
			// bind info from second column
			oSecondColumn = (oColumns.Count > 1) ? oColumns[1] : null;
			lColumnTwoTitle.Text = oSecondColumn.GetText("Column Info", "Title");
			lColumnTwoBody.Text = oSecondColumn.GetText("Column Info", "Content");
		}	
	}
</script>
<div class="generic-block">
	<div class="html">
		<h1><asp:Literal ID="lTitle" runat="server" /></h1>
		<asp:Literal ID="lBody" runat="server" />
	</div>
	<hr />
	<div class="gc c6 inside">
		<div class="html">
			<h1><asp:Literal ID="lColumnOneTitle" runat="server" /></h1>
			<asp:Literal ID="lColumnOneBody" runat="server" />
		</div>
		<div class="clear"></div>
	</div>
	<div class="gc c6">
		<div class="html">
			<h1><asp:Literal ID="lColumnTwoTitle" runat="server" /></h1>
			<asp:Literal ID="lColumnTwoBody" runat="server" />
		</div>
		<div class="clear"></div>
	</div>
	<div class="clear"></div>
	<hr />
</div>
