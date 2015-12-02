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
		List<Item> oColumns;
		Item oFirstColumn;
		Item oSecondColumn;
		string sFirstColumImageURL;

		//get the all column items
		if ((oColumns = ContextExtension.CurrentItem.GetChildrenOfTemplate("Generic Column")).Count > 0)
		{
			// bind info from first column
			oFirstColumn = oColumns[0];
			lColumnOneTitle.Text = oFirstColumn.GetText("Column Info", "Title");
			lColumnOneBody.Text = oFirstColumn.GetText("Column Info", "Content");
			sFirstColumImageURL = oFirstColumn.GetField("Column Info", "Image").GetImageURL();
			

			if (!String.IsNullOrEmpty(sFirstColumImageURL))
			{
				iColumnOneImage.ImageUrl = String.Format("~/{0}", sFirstColumImageURL);
			}
			else
			{
				iColumnOneImage.Visible = false;
			}
				
			// bind info from second column
			if ((oSecondColumn = (oColumns.Count > 1) ? oColumns[1] : null) != null)
			{
				lColumnTwoBody.Text = oSecondColumn.GetText("Column Info", "Content");
				
			}
		}	
	}
	
</script>
<div class="generic-block">
	<div class="html">
		<h1>
			<asp:Image ID="iColumnOneImage" Width="80px" Height="50px" runat="server" />
			<asp:Literal ID="lColumnOneTitle" runat="server" /></h1>
	</div>
	<div class="gc c6 inside">
		<div class="html">
			<asp:Literal ID="lColumnOneBody" runat="server" />
			<p>
				<asp:HyperLink ID="hColumnOneLink" runat="server"></asp:HyperLink>
			</p>
		</div>
		<div class="clear">
		</div>
	</div>
	<div class="gc c6">
		<div class="html">
			<asp:Literal ID="lColumnTwoBody" runat="server" />
            <div class="center-info-card-list">
            <sc:Placeholder ID="ColumnTwo_Middle" Key="ColumnTwo_Middle" runat="server" />
			</div>
            <p>
				<asp:HyperLink ID="hColumnTwoLink" runat="server"></asp:HyperLink>
			</p>
		</div>
		<div class="clear">
		</div>
	</div>

	<div class="clear"></div>
	<div class="gc c6 inside">
		<div class="html">
			<sc:Placeholder ID="ColumnOne_Bottom" Key="ColumnOne_Bottom" runat="server" />
		</div>
		<div class="clear">
		</div>
	</div>
	<div class="gc c6">
		<div class="html">
			<sc:Placeholder ID="ColumnTwo_Bottom" Key="ColumnTwo_Bottom" runat="server" />
		</div>
		<div class="clear">
		</div>
	</div>

</div>
