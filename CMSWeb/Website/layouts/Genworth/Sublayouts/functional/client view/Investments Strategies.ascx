<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Register TagPrefix="Investments" Src="~/layouts/Genworth/Sublayouts/functional/client view/Strategist.ascx"
	TagName="StrategisCtrl" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">
	
	bool bShowAll = false;
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}

        // go get the proper bundle url from EWM and then inject into the head block on this page.
        string bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/InvestmentsClientView/Strategies", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Scripts);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddJsToPage(this.Page, bundleUrl);
        }

        bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/InvestmentsClientView/Strategies", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Styles);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddCssToPage(this.Page, bundleUrl);
        }
    }

	private void BindData()
	{
		Item oCurrentItem;

		List<InvestmentHelper.Strategist> oStrategists;
		List<Item> oManagers;
		List<InvestmentHelper.Strategist> oAdditionalStrategists;

		Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.GetManagersAndStrategists(out oStrategists, out oManagers, out oAdditionalStrategists);

		//get the current item
		oCurrentItem = ContextExtension.CurrentItem;

		SetContent(oCurrentItem.GetChildrenOfTemplate("Context Sensitive Help"));

		//get the items
		if (oManagers.Count > 0)
		{
			rManagers.DataSource = oManagers;
			rManagers.ItemDataBound += new RepeaterItemEventHandler(rManagers_ItemDataBound);
			rManagers.DataBind();
		}

		if (oStrategists.Count > 0)
		{

			rStrategists.DataSource = Split(oStrategists);
			rStrategists.ItemDataBound += new RepeaterItemEventHandler(rStrategists_ItemDataBound);
			rStrategists.DataBind();
			bShowAll = true;
		}
		/*
		if (oAdditionalStrategists.Count > 0)
		{

			rOtherStrategist.DataSource = Split(oAdditionalStrategists);
			rOtherStrategist.ItemDataBound += new RepeaterItemEventHandler(rStrategists_ItemDataBound);
			rOtherStrategist.DataBind();
		}*/

		rManagers.Visible = rManagers.Items.Count > 0;
		//rOtherStrategist.Visible = rOtherStrategist.Items.Count > 0;
		rStrategists.Visible = rStrategists.Items.Count > 0;
	}

	/// <summary>
	/// Gets the Title and Text form the items and sets the text to their related controls.
	/// </summary>
	/// <param name="oContentItems"></param>
	private void SetContent(List<Item> oContentItems)
	{
		string sTitle;
		string sSummary;
		string sControlName;
		Literal lTitle;
		Literal lSummary;
		HtmlControl dSummaryHolder;

		foreach (var oContentItem in oContentItems)
		{
			if (String.IsNullOrWhiteSpace(sControlName = oContentItem.GetText("Field Selector")))
				continue;

			if (!String.IsNullOrWhiteSpace(sTitle = oContentItem.GetText("Title"))
				&& (lTitle = this.FindControl(String.Format("l{0}Title", sControlName)) as Literal) != null)
			{
				lTitle.Text = sTitle;
			}

			if ((dSummaryHolder = this.FindControl(String.Format("d{0}Summary", sControlName)) as HtmlControl) != null)
			{
				if (!String.IsNullOrWhiteSpace(sSummary = oContentItem.GetText("Text"))
					&& (lSummary = this.FindControl(String.Format("l{0}Summary", sControlName)) as Literal) != null)
				{
					lSummary.Text = sSummary;
				}
				else
				{
					// avoid adding the div with the html class in case there is not content for
					// the text field in the CMS
					dSummaryHolder.Visible = false;
				}
			}
		}
	}

	IEnumerable<List<InvestmentHelper.Strategist>> Split(IEnumerable<InvestmentHelper.Strategist> oItems)
	{
		int i = 0;
		List<InvestmentHelper.Strategist> oPartItem = null;
		foreach (InvestmentHelper.Strategist oitem in oItems)
		{
			if (i % 3 == 0)
			{
				if (i != 0)
					yield return oPartItem;
				oPartItem = new List<InvestmentHelper.Strategist>(3);
			}
			oPartItem.Add(oitem);
			i++;
		}
		if (oPartItem != null)
			yield return oPartItem;
	}

	void rStrategists_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		List<InvestmentHelper.Strategist> oData = e.Item.DataItem as List<InvestmentHelper.Strategist>;
		Repeater or3colStrategists;
		if (oData != null)
		{
			or3colStrategists = e.Item.FindControl("r3colStrategists") as Repeater;
			if (e.Item.ItemIndex == 0 && oData.Count < 3)
			{
				while (oData.Count < 3)
					oData.Add(null);
			}

			or3colStrategists.DataSource = oData;
			or3colStrategists.ItemDataBound += new RepeaterItemEventHandler(or3colStrategists_ItemDataBound);
			or3colStrategists.DataBind();
		}
	}

	void rManagers_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oManager = e.Item.DataItem as Item;

		Label hMangerTitle;
		if (oManager != null)
		{

			hMangerTitle = (Label)e.Item.FindControl("hMangerTitle");
			hMangerTitle.Text = oManager.GetText("Name");
		}
	}

	void or3colStrategists_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		InvestmentHelper.Strategist oItem = e.Item.DataItem as InvestmentHelper.Strategist;
		StrategisCtrl oStrategist;
		oStrategist = e.Item.FindControl("oStrategist") as StrategisCtrl;
		if (oItem != null)
		{

			oStrategist.BindData(oItem, bShowAll);
		}
		else
		{
			if (oStrategist != null)
				oStrategist.Visible = false;

		}

	}
	
</script>
<div class="client-content-block" id="ChildElementId">
	<h3>
		<asp:Literal ID="lPortfolioTitle" runat="server"></asp:Literal></h3>
	<div class="html" runat="server" id="dPortfolioSummary">
		<p>
			<asp:Literal ID="lPortfolioSummary" runat="server" /></p>
	</div>
	<h6>
		<asp:Literal ID="lPortfolioStrategistsTitle" runat="server"></asp:Literal></h6>
	<div class="html" runat="server" id="dPortfolioStrategistsSummary">
		<p>
			<asp:Literal ID="lPortfolioStrategistsSummary" runat="server" /></p>
	</div>
	<asp:Repeater ID="rStrategists" runat="server">
		<HeaderTemplate>
			<table width="100%" cellpadding="0px" cellspacing="0px" class="info-card-table">
		</HeaderTemplate>
		<ItemTemplate>
			<asp:Repeater runat="server" ID="r3colStrategists">
				<HeaderTemplate>
					<tr>
				</HeaderTemplate>
				<ItemTemplate>
					<td width="25%">
						<Investments:StrategisCtrl runat="Server" ID="oStrategist"></Investments:StrategisCtrl>
					</td>
				</ItemTemplate>
				<FooterTemplate>
					</tr></FooterTemplate>
			</asp:Repeater>
		</ItemTemplate>
		<FooterTemplate>
			</table></FooterTemplate>
	</asp:Repeater>
	<h6>
		<asp:Literal ID="lPortfolioManagersTitle" runat="server"></asp:Literal></h6>
	<div class="html" runat="server" id="dPortfolioManagersSummary">
		<p>
			<asp:Literal ID="lPortfolioManagersSummary" runat="server" /></p>
	</div>
	<asp:Repeater runat="server" ID="rManagers">
		<HeaderTemplate>
			<ul class="manager-list">
		</HeaderTemplate>
		
		<ItemTemplate>
			<li>
			<asp:Label runat="server" ID="hMangerTitle" style="font-weight:bold;"></asp:Label></li>
		</ItemTemplate>
		<FooterTemplate>
			</ul>
		</FooterTemplate>
	</asp:Repeater>
	<asp:PlaceHolder ID="PlaceHolder1" runat ="server" Visible ="false">
	<hr />
	<h3>
		<asp:Literal ID="lAdditionalOptionsTitle" runat="server"></asp:Literal></h3>
	<div class="html" runat="server" id="dAdditionalOptionsSummary">
		<p>
			<asp:Literal ID="lAdditionalOptionsSummary" runat="server" /></p>
	</div>
	<h6>
		<asp:Literal ID="lAdditionalOptionsStrategistsTitle" runat="server"></asp:Literal></h6>
	<div class="html" runat="server" id="dAdditionalOptionsStrategistsSummary">
		<p>
			<asp:Literal ID="lAdditionalOptionsStrategistsSummary" runat="server" /></p>
	</div>
	<asp:Repeater runat="server" ID="rOtherStrategist">
		<HeaderTemplate>
			<table width="100%" cellpadding="0px" cellspacing="0px" class="info-card-table">
		</HeaderTemplate>
		<ItemTemplate>
			<asp:Repeater runat="server" ID="r3colStrategists">
				<HeaderTemplate>
					<tr>
				</HeaderTemplate>
				<ItemTemplate>
					<td width="25%">
						<Investments:StrategisCtrl runat="Server" ID="oStrategist"></Investments:StrategisCtrl>
					</td>
				</ItemTemplate>
				<FooterTemplate>
					</tr></FooterTemplate>
			</asp:Repeater>
		</ItemTemplate>
		<FooterTemplate>
			</table>
		</FooterTemplate>
	</asp:Repeater>
	</asp:PlaceHolder>
</div>
