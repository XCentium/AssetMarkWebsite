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

		//get the all column items
		if ((oColumns = ContextExtension.CurrentItem.GetChildrenOfTemplate("Generic Column")).Count > 0)
		{
			rColumns.ItemDataBound += new RepeaterItemEventHandler(rColumns_ItemDataBound);
			rColumns.DataSource = oColumns.Take(3);
			rColumns.DataBind();
		}
	}

	void rColumns_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		RepeaterItem rItem;
		Item oColumnItem;
		Literal lColumnBody;
		Literal lColumnTitle;
		

		rItem = e.Item;
		oColumnItem = rItem.DataItem as Item;
		if (oColumnItem != null)
		{
			// bind the column title
			lColumnTitle = rItem.FindControl("lColumnTitle") as Literal;
			if (lColumnTitle != null)
			{
				lColumnTitle.Text = oColumnItem.GetText("Column Info", "Title");
			}
			
			// bind the column content
			lColumnBody = rItem.FindControl("lColumnBody") as Literal;
			if (lColumnBody != null)
			{
				lColumnBody.Text = oColumnItem.GetText("Column Info", "Content");
			}

			
		}
	}	
    
	
	

	/// <summary>
	/// Returns the style for the current column
	/// </summary>
	/// <param name="iColumnIndex">Number of column being processed</param>
	/// <returns>If is the first column this method will return the style "inside". 
	/// In case is not, nothing will be returned.</returns>
	private string GetColumnStyle(int iColumnIndex)
	{
		return iColumnIndex == 0 ? " inside" : String.Empty;
	}
	
</script>
<div class="generic-block">
	<asp:Repeater ID="rColumns" runat="server">
	<ItemTemplate>
		<div class="gc c4<%# GetColumnStyle(Container.ItemIndex) %>">
			<div class="html">
				<h1><asp:Literal ID="lColumnTitle" runat="server" /></h1>
				<asp:Literal ID="lColumnBody" runat="server" />
				
			</div>
			<div class="clear">
		    </div>
		</div>
	</ItemTemplate>
	</asp:Repeater>
	<div class="clear">
	</div>
</div>
