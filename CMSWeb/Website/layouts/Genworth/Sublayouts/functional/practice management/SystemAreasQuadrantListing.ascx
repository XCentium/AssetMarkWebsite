<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
    
	private const string sIconControlKey = "iIcon";
	private const string sSummaryControlKey = "lSummary";
	private const string sLearnMoreControlKey = "hLearnMore";
	
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
        oCurrentItem = ContextExtension.CurrentItem;
		rQuadrants.ItemDataBound += new RepeaterItemEventHandler(rQuadrants_ItemDataBound);
        rQuadrants.DataSource = oCurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Nav Page Sidebar Resource Listing");
		rQuadrants.DataBind();
    }

	void rQuadrants_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		string sIconURL;
		string sSummary;
		RepeaterItem rItem;
		Item oItemToBind;
		Image iIcon;
		Literal lSummary;
		HyperLink hLearnMore;
		
		rItem = e.Item;
		oItemToBind = rItem.DataItem as Item;
		if (oItemToBind != null)
		{
			
			sIconURL = oItemToBind.GetField("Page", "Icon").GetImageURL();
			sSummary = oItemToBind.GetText("Page", "Blurb", string.Empty);

			iIcon = rItem.FindControl(sIconControlKey) as Image;
			if (iIcon != null)
			{
				iIcon.ImageUrl = String.Format("~/{0}", sIconURL);
			}

			lSummary = rItem.FindControl(sSummaryControlKey) as Literal;
			if (lSummary != null)
			{
				lSummary.Text = sSummary;
			}

			hLearnMore = rItem.FindControl(sLearnMoreControlKey) as HyperLink;
			if (hLearnMore != null)
			{
				oItemToBind.ConfigureHyperlink(hLearnMore);				
			}
			
		}
	}	
    
</script>
<!-- START QUADRANT SYSTEM -->
<div id="divPracticeManagementQuadrantSystem" class="quadrantSystem">
	<asp:Repeater ID="rQuadrants" runat="server">
	<ItemTemplate>
		<div class="quadrant quadrant<%# Container.ItemIndex + 1 %> complete">			
			<asp:Image ID="iIcon" runat="server" />
			<asp:Literal ID="lSummary" runat="server"></asp:Literal>
		</div>	
	</ItemTemplate>
	</asp:Repeater>
	<div class="clear">
	</div>
    <asp:Literal ID="lBody" runat="server" />
</div>
<!-- END QUADRANT SYSTEM -->
