<%@ Control Language="c#" AutoEventWireup="true" ClassName="StrategisCtrl" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Sitecore.Resources.Media" %>

<script runat="server">
	Item oDocumentView;   
	
	public void BindData(InvestmentHelper.Strategist oStrategist, bool ubShowAll)
	{
        string sLogoUrl;
        
		//bind the controls
		Item oItem = oStrategist.Item;
        sLogoUrl = Sitecore.StringUtil.EnsurePrefix('/', oItem.GetImageURL("Strategist", "Logo"));
		oDocumentView = Genworth.SitecoreExt.Constants.Investments.Items.DocumentViewerClientViewItem;
        iLogo.Attributes.Add("src", string.Format("{0}?mw={1}&mh={2}", sLogoUrl, 120, 60));
		Genworth.SitecoreExt.Constants.Investments.Items.ResearchClientViewItem.ConfigureHyperlink(hResearch);
		hResearch.NavigateUrl = string.Format("{0}?strategist={1}", hResearch.NavigateUrl, oItem.GetText("Strategist", "Code"));
		rSolutions.DataSource =  oStrategist.Solutions ;
		rSolutions.ItemDataBound += new RepeaterItemEventHandler(rSolutions_ItemDataBound);
		rSolutions.DataBind();
	}
	private void rSolutions_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{

		InvestmentHelper.Solution oItem;
		HyperLink hLink;

		//does this repeater contain an item?
		if ((oItem = (InvestmentHelper.Solution)e.Item.DataItem) != null)
		{
			
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");

			//bind the controls
			//oDocumentView.ConfigureHyperlink(hLink);
			hLink.NavigateUrl = string.Concat("~/",oItem.Item.GetField("Fact Sheet").GetItem().GetImageURL("Document", "File"));//string.Format("{0}?Document={1}", hLink.NavigateUrl, oItem.Item.ID);
			
			hLink.Text = oItem.SolutionName;
		}
	}
</script>
<ul class="info-card-list">
	<li class="first last blue">
		<div class="info-card">

		<asp:Image ID="iLogo" runat="server" />
			<asp:Repeater runat="server" ID="rSolutions">
				<ItemTemplate>
					<asp:HyperLink runat="server" ID="hLink"  Target="_blank"/>
				</ItemTemplate>
			</asp:Repeater>
			
			<asp:HyperLink runat="server" ID="hResearch" CssClass="foot-link" Text="View Strategist Research"></asp:HyperLink>
		</div>
	</li>
</ul>
