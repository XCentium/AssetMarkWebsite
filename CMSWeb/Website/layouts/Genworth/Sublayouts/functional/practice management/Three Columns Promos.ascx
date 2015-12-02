<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">

    Item oCurrentItem;

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
        //get hte current item
        oCurrentItem = ContextExtension.CurrentItem;
        
		List<Item> oColumns = new List<Item>();
        oColumns.AddRange(oCurrentItem.GetChildrenOfTemplate("Static Promo"));
        oColumns.AddRange(oCurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Static Promo"));

		//get the all column items
		if (oColumns.Count > 0)
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

		rItem = e.Item;
		oColumnItem = rItem.DataItem as Item;
		if (oColumnItem != null)
        {
            // bind the column content
            var lColumnTitle = rItem.FindControl("lColumnTitle") as HyperLink;
            if (lColumnTitle != null)
            {
                lColumnTitle.Text = oColumnItem.GetText("Data", "Title");
                oColumnItem.ConfigureHyperlink(lColumnTitle);
                oColumnItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, lColumnTitle);
            }
            
            // bind the column content
            var lColumnBody = rItem.FindControl("lColumnBody") as HtmlGenericControl;
            if (lColumnBody != null)
            {
                lColumnBody.InnerText = oColumnItem.GetText("Data", "Description");
            }

            // bind link
            var lColumnLink = rItem.FindControl("lColumnLink") as HyperLink;
            if (lColumnLink != null)
            {
                oColumnItem.ConfigureHyperlink(lColumnLink);
                oColumnItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, lColumnLink);
            }

            // bind img
            var lColumnImage = rItem.FindControl("lColumnImage") as Image;
            if (lColumnImage != null)
            {
                oColumnItem.ConfigImage("Data", "Preview Thumbnail", lColumnImage);
            }

            var lImageLink = rItem.FindControl("lImageLink") as HyperLink;
            if (lColumnLink != null)
            {
                oColumnItem.ConfigureHyperlink(lImageLink);
                oColumnItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, lImageLink);
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
		return iColumnIndex == 0 ? " first" : String.Empty;
	}
	
</script>

<asp:Repeater ID="rColumns" runat="server">
<ItemTemplate>
	<div class="gc c4<%# GetColumnStyle(Container.ItemIndex) %>">
		<div class="html">
            <h3><asp:HyperLink ID="lColumnTitle" runat="server">Learn More</asp:HyperLink></h3>
			<asp:HyperLink ID="lImageLink" runat="server"><asp:Image id="lColumnImage" runat="server" /></asp:HyperLink>
            <div class="clear"></div>
			<p ID="lColumnBody" runat="server" />
			<asp:HyperLink ID="lColumnLink" runat="server">Learn More</asp:HyperLink>
		</div>
		<div class="clear">
		</div>
	</div>
</ItemTemplate>
</asp:Repeater>
<div class="clear">
</div>