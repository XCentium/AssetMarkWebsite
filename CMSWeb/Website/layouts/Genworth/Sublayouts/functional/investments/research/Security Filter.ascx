<%@ Control Language="c#" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Services.Investments" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
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
		IInvestmentsSearch oResearch;
		
		//get the user's current research
		oResearch = Genworth.SitecoreExt.Helpers.InvestmentHelper.GetProvider(this.GetParameter("Search Type"));
		

		rFilters.DataSource = oResearch.Filters.Where(oFilter => oFilter.Group == FilterGroup.Security);
		rFilters.ItemDataBound += new RepeaterItemEventHandler(rFilters_ItemDataBound);
		rFilters.DataBind();
	}

	private void rFilters_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
        Genworth.SitecoreExt.Services.Investments.Filter oFilter;
		Repeater rOptions;

        oFilter = (Genworth.SitecoreExt.Services.Investments.Filter)e.Item.DataItem;
		rOptions = (Repeater)e.Item.FindControl("rOptions");
		rOptions.DataSource = oFilter.OptionArray;
		rOptions.ItemDataBound += new RepeaterItemEventHandler(rFilterOptions_ItemDataBound);
		rOptions.DataBind();
	}

	private void rFilterOptions_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
        Genworth.SitecoreExt.Services.Investments.Filter.Option oOption;
		Literal lLiteral;
		string sId;

        if ((oOption = (Genworth.SitecoreExt.Services.Investments.Filter.Option)e.Item.DataItem) != null)
		{
			lLiteral = (Literal)e.Item.FindControl("lLiteral");
			sId = string.Format("{0}-{1}", oOption.Filter.Code, oOption.Code, string.Empty);

			//we are outputting the literal to avoid creating HTML
			lLiteral.Text = string.Format("<input type=\"checkbox\" id=\"{0}\" onclick=\"researchFilterBarCheckedChanged(this, '{2}','{3}');\"{4}/> <label for=\"{0}\">{1}</label>", sId, oOption.Name, oOption.Filter.Code, oOption.Code, oOption.Filtered ? " checked=\"checked\"" : string.Empty);
		}
	}
</script>
<div class="filter-results-pagination InvestmentFilterBar">
	<span class="inline-form-block">
		<span class="float-left">
			<asp:Repeater ID="rFilters" runat="server" EnableViewState="false">
				<ItemTemplate>
					<asp:Repeater ID="rOptions" runat="server" EnableViewState="false">
						<ItemTemplate>
							<asp:Literal ID="lLiteral" runat="server" />
						</ItemTemplate>
					</asp:Repeater>
				</ItemTemplate>
			</asp:Repeater>
		</span>
	</span>
</div>