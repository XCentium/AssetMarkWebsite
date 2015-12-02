<%@ Control Language="c#" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Services.Investments" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<script runat="server">
	private bool bShowCalendar;
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		bShowCalendar = this.GetParameter("Show Date Filter") == "1";
		if (!Page.IsPostBack)
		{
			BindData();
		}
		else
		{
			ScriptManager.RegisterStartupScript(this.Page, typeof(Page), "wireDatePickers", "wireDatePickers();", true);
		}
		SetUpCalendars();

	}
	private void SetUpCalendars()
	{

		if (bShowCalendar)
		{
			cCalendarFrom.Attributes.Add("target", "txtMyClientId2");
			cCalendarFrom.Attributes.Add("style", "border-width: 0px; border-collapse: collapse;");
			cCalendarTo.Attributes.Add("target", "txtMyClientId3");
			cCalendarTo.Attributes.Add("style", "border-width: 0px; border-collapse: collapse;");
			cCalendarFrom.DayRender += new DayRenderEventHandler(cCalendarFrom_DayRender);
			cCalendarTo.DayRender += new DayRenderEventHandler(cCalendarFrom_DayRender);
		}
		else
			pDate.Visible = false;
	}
	private void BindData()
	{
		IInvestmentsSearch oResearch;
		IEnumerable<Genworth.SitecoreExt.Services.Investments.Filter> oFilters;
		//get the user's current research
		oResearch = Genworth.SitecoreExt.Helpers.InvestmentHelper.GetProvider(this.GetParameter("Search Type"));
		oFilters = oResearch.Filters.Where(oFilter => oFilter.Group == FilterGroup.General && !oFilter.Hide);
		if (oFilters.Count() > 0)
		{
			rFilters.DataSource = oFilters;
			rFilters.ItemDataBound += new RepeaterItemEventHandler(rFilters_ItemDataBound);
			rFilters.DataBind();

			SetDefaultDateValue(oResearch.Months);
		}
		else
		{
			if (!bShowCalendar)
				//dFilterBar.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
				dFilterBar.Style.Add(HtmlTextWriterStyle.Display, "none");
		}
	}

	void cCalendarFrom_DayRender(object sender, DayRenderEventArgs e)
	{
		e.Cell.Controls.Clear();
		HtmlAnchor oAcnhor = new HtmlAnchor();
		oAcnhor.Title = e.Day.Date.ToShortDateString();
		oAcnhor.InnerText = e.Day.DayNumberText;
		e.Cell.Controls.Add(oAcnhor);
	}

	private void rFilters_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
        Genworth.SitecoreExt.Services.Investments.Filter oFilter;
		Repeater rOptions;
		Literal lTitle;
		HtmlGenericControl lLi;
		PlaceHolder pCheckAll;
		Literal lLiteral;
        oFilter = (Genworth.SitecoreExt.Services.Investments.Filter)e.Item.DataItem;
		rOptions = (Repeater)e.Item.FindControl("rOptions");
		lTitle = (Literal)e.Item.FindControl("lTitle");
		
			lLiteral = (Literal)e.Item.FindControl("lCheckAll");
            lLiteral.Text = string.Format("<input type=\"checkbox\" class=\"check-all\" id=\"{0}-{1}\" checked='checked' disabled='disabled'  data-code='{1}' data-filter='{0}' /><label for=\"{0}-{1}\">{1}</label>", oFilter.DataField, "All");

        lTitle.Text = oFilter.Name;
		rOptions.DataSource = oFilter.OptionArray.OrderBy(f=>f.Name);
		rOptions.ItemDataBound += new RepeaterItemEventHandler(rFilterOptions_ItemDataBound);
		rOptions.DataBind();

		if (e.Item.ItemIndex == 0)
		{
			lLi = (HtmlGenericControl)e.Item.FindControl("lLi");
			lLi.Attributes.Add("class", "first");
		}
	}

	private void rFilterOptions_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
        Genworth.SitecoreExt.Services.Investments.Filter.Option oOption;
		Literal lLiteral;
		string sId;
		HtmlGenericControl lLi;
        if ((oOption = (Genworth.SitecoreExt.Services.Investments.Filter.Option)e.Item.DataItem) != null)
		{
			lLiteral = (Literal)e.Item.FindControl("lLiteral");
			sId = string.Format("{0}-{1}", oOption.Filter.Code, oOption.Code);

			//we are outputting the literal to avoid creating HTML
            lLiteral.Text = string.Format("<input type=\"checkbox\"  id=\"{0}\" value='{3}' data-code='{1}' data-filter='{2}' data-option='{4}' /><label for=\"{0}\">{1}</label>", sId, oOption.Name, oOption.Filter.DataField, oOption.Id, oOption.Code);
            lLi = (HtmlGenericControl)e.Item.FindControl("lLi");

            if (!oOption.Available)
            {
                lLi.Style.Add(HtmlTextWriterStyle.Display, "none");
            }

			if (e.Item.ItemIndex == 0)
			{
				lLi.Attributes.Add("class", "first");
			}
		}
	}

	/// <summary>
	/// Sets the default value in the Date dropdown.
	/// This method just handles the graphic part, the filter is done in the ResearchService
	/// Invest
	/// </summary>
	private void SetDefaultDateValue(int iDefaultDate)
	{
		string sListOptionId;

		switch (iDefaultDate)
		{
			case 3:
			case 6:
			case 12:
				sListOptionId = String.Format("Last_{0}_Months", iDefaultDate);
				break;
			case -1:
			default:
				sListOptionId = String.Format("All_Months", iDefaultDate);
				break;
		}

		var listItem = FindControl(sListOptionId) as HtmlContainerControl;
		if (listItem != null)
		{
			listItem.Attributes.Add("class", "selected");

			// set the default text
			txtMyClientId4.Value = listItem.InnerText;
		}
	}
	
</script>
<asp:ScriptManager runat="server" ID="sScriptManager" />
<div class="filter-bar-wrapper InvestmentFilterBar" id="dFilterBar" runat="server">
	<div class="filter-search float-right">
		<a class="reset" href="#">Restore Default</a>
	</div>
	<label>
		Filter By:</label>
	<div class="filter-search-searchbox">
        <input type="text" id="searchTextBox" class="searchTextBox" placeholder="Search Documents" /><input type="image" class="submitImage" src="/CMSContent/Images/FilterGrid/searchicon.png" />
	</div>
    <ul>
		<asp:Repeater ID="rFilters" runat="server" EnableViewState="false">
            <ItemTemplate>
                <li runat="server" id="lLi">
                    <span>
                        <asp:Literal ID="lTitle" runat="server" />
                        <span class='counter'></span>
                    </span>
					<div class="filter-bar-list-wrapper">
						<div class="filter-bar-list">
							<asp:PlaceHolder runat="server" ID="pCheckAll">
								<div class="filter-bar-list-header">
									<ul>
										<li class="first last">
											<asp:Literal runat="server" ID="lCheckAll" />
										</li>
									</ul>
									<hr />
								</div>
							</asp:PlaceHolder>
							<div class="filter-bar-list-body">
								<ul>
									<asp:Repeater ID="rOptions" runat="server" EnableViewState="false">
										<ItemTemplate>
											<li runat="server" id="lLi">
												<asp:Literal ID="lLiteral" runat="server" /></li>
										</ItemTemplate>
									</asp:Repeater>
								</ul>
							</div>
							<div class="filter-body-list-scrollbar">
								<div class="filter-body-list-scroll-button">
									<div class="filter-body-list-scroll-button-inner">
									</div>
								</div>
							</div>
                            <div class="filter-bar-list-buttons">
                                <a href="#" class="reset-link">Reset</a>
                                <span class="button">
                                    <input type="button" value="Go"></span>
                            </div>
                        </div>
					</div>
				</li>
			</ItemTemplate>
		</asp:Repeater>
		<asp:PlaceHolder runat="server" ID="pDate">
			<li class="last range"><span class="date-option-value">Date <span class="dateLabel">
				<input id="txtMyClientId4" type="text" style="width: 75px;" class="months" runat="server" visible="false" />
			</span></span><span class="date-range-values">Date: <span class="text-input">
				<input id="txtMyClientId2" type="text" style="width: 30px;" class="fromdate" />
			</span>to <span class="text-input">
				<input id="txtMyClientId3" type="text" class="todate" style="width: 30px;" />
			</span></span>
				<div class="filter-bar-list-wrapper">
					<div class="filter-bar-list">
						<div class="filter-bar-list-body">
							<ul class="date-options">
								<li runat="server" id="Last_3_Months" val="3">Last 3 Months</li>
								<li runat="server" id="Last_6_Months" val="6">Last 6 Months</li>
								<li runat="server" id="Last_12_Months" val="12">Last 12 Months&nbsp;</li>
								<li runat="server" id="All_Months" val="-1">All</li>
								<li class="custom-date-range" style="display: none;">Custom Date Range</li>
							</ul>
							<table class="date-range-table" width="100%">
								<tr>
									<td width="*" align="center">
										<asp:UpdatePanel runat="server">
											<ContentTemplate>
												<div class="date-picker-wrapper">
													<span class="date-picker">
														<asp:Calendar runat="server" ID="cCalendarFrom" CellSpacing="1" CellPadding="0" class="date-picker-calendar"
															DayStyle-CssClass="day" ShowDayHeader="false" border="0"></asp:Calendar>
													</span>
												</div>
											</ContentTemplate>
										</asp:UpdatePanel>
									</td>
									<td width="*" align="center">
										<asp:UpdatePanel ID="UpdatePanel1" runat="server">
											<ContentTemplate>
												<div class="date-picker-wrapper">
													<span class="date-picker">
														<asp:Calendar ID="cCalendarTo" runat="server" CellSpacing="1" CellPadding="0" class="date-picker-calendar"
															DayStyle-CssClass="day" ShowDayHeader="false" border="0"></asp:Calendar>
													</span>
												</div>
											</ContentTemplate>
										</asp:UpdatePanel>
									</td>
								</tr>
							</table>
						</div>
					</div>
				</div>
			</li>
		</asp:PlaceHolder>
	</ul>
	<div class="clear">
	</div>
</div>
