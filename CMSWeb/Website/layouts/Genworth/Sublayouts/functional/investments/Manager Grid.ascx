<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Configuration" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Services.Investments" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">
	ManagerSearch oManagerSearch;
	int iCount;
	
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
		bool bGroup;
		string sType;
		string sSort;
		List<ManagerSearch.Result> oResults;
		Dictionary<string, List<ManagerSearch.Result>> oGroupedResults;
		Dictionary<string, int> oSorter;
		
		//get the current item
		if ((oCurrentItem = ContextExtension.CurrentItem) != null)
		{
			//lets do a search
			if ((oManagerSearch = Genworth.SitecoreExt.Services.Investments.ManagerSearch.CurrentManagerSearch) != null)
			{
				//set the template
				oManagerSearch.SetTemplate(sType = oCurrentItem.GetText("Manager Grid", "Strategy Type"));

				//get the results
				oResults = oManagerSearch.GetResults().ToList();

				//create the sorts
				oSorter = new Dictionary<string, int>();

				oSorter.Add("us equity", 1);
				oSorter.Add("international/global", 2);
				oSorter.Add("fixed income", 3);
				oSorter.Add("specialty", 4);
				oSorter.Add("value", 1);
				oSorter.Add("blend", 2);
				oSorter.Add("growth", 3);
				oSorter.Add("high", 1);
				oSorter.Add("medium", 2);
				oSorter.Add("low", 3);
				oSorter.Add("taxable", 1);
				oSorter.Add("tax", 2);
				oSorter.Add("core", 1);
				oSorter.Add("opportnistic", 2);
				oSorter.Add("sector", 3);
				oSorter.Add("reit", 1);
				oSorter.Add("balanced", 2);

				//sort the results
				oResults = oResults
					.OrderBy(oItem => oSorter.ContainsKey(sSort = oItem.StylePrefix.ToLower() ?? string.Empty) ? oSorter[sSort] : 0)
					.ThenBy(oItem => oSorter.ContainsKey(sSort = oItem.StyleSuffix.ToLower().Split(' ').FirstOrDefault() ?? string.Empty) ? oSorter[sSort] : 0)
					.ThenBy(oItem => oSorter.ContainsKey(sSort = oItem.StyleSuffix.ToLower().Split(' ').LastOrDefault() ?? string.Empty) ? oSorter[sSort] : 0)
					.ThenBy(oItem => oItem.Manager)
					.ToList();

				//should we group?
				bGroup = oCurrentItem.GetText("Manager Grid", "Enable Grouping", "0").Equals("1");

				//create the grouped results
				oGroupedResults = new Dictionary<string, List<ManagerSearch.Result>>();

				//are we grouping?
				if (bGroup)
				{
					//loop over results
					oResults.ForEach(oResult =>
					{
						//add the result
						if (!oGroupedResults.ContainsKey(oResult.Group))
						{
							oGroupedResults.Add(oResult.Group, new List<ManagerSearch.Result>());
						}
						oGroupedResults[oResult.Group].Add(oResult);
					});
				}
				else
				{
					//we are not grouped
					oGroupedResults.Add(string.Empty, oResults);
				}

				//bind the results
				rGroups.DataSource = oGroupedResults;
				rGroups.ItemDataBound += new RepeaterItemEventHandler(rGroups_ItemDataBound);
				rGroups.DataBind();
			}
		}
	}

	private void rGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		KeyValuePair<string, List<ManagerSearch.Result>> oGroup;
		PlaceHolder pGroup;
		Literal lTitle;
		Repeater rItems;
		
		//get the group
		oGroup = (KeyValuePair<string, List<ManagerSearch.Result>>)e.Item.DataItem;
		
		//get the fields
		pGroup = (PlaceHolder)e.Item.FindControl("pGroup");
		lTitle = (Literal)e.Item.FindControl("lTitle");
		rItems = (Repeater)e.Item.FindControl("rItems");
		
		//should we have a group?
		pGroup.Visible = !string.IsNullOrEmpty(lTitle.Text = oGroup.Key);
		
		//perform the databinding
		rItems.DataSource = oGroup.Value;
		iCount = oGroup.Value.Count;
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		ManagerSearch.Result oResult;
		HyperLink hLink;
		Literal lTitle;
		Literal lStyle;
		Image iIsIMA;
		Image iIsManagerSelect;
		HtmlTableRow hRow;
		List<string> oClasses;
		
		//get the result
		oResult = (ManagerSearch.Result)e.Item.DataItem;
		
		//get the fields
		hLink = (HyperLink)e.Item.FindControl("hLink");
		lTitle = (Literal)e.Item.FindControl("lTitle");
		lStyle = (Literal)e.Item.FindControl("lStyle");
		iIsIMA = (Image)e.Item.FindControl("iIsIMA");
		iIsManagerSelect = (Image)e.Item.FindControl("iIsManagerSelect");
		hRow = (HtmlTableRow)e.Item.FindControl("hRow");
		
		//create a list of styles
		oClasses = new List<string>();
		
		//is this the first row?
		if (e.Item.ItemIndex == 0)
		{
			oClasses.Add("first");
		}
		if (e.Item.ItemIndex == iCount - 1)
		{
			oClasses.Add("last");
		}
		
		//start binding fields
		Genworth.SitecoreExt.Constants.Investments.Items.DocumentViewerItem.ConfigureHyperlink(hLink);
        var contentItem = ContextExtension.CurrentDatabase.GetItem(ItemPointer.Parse(oResult.Id).ItemID);
        var text = contentItem.GetText("Profile Sheet");
        hLink.NavigateUrl = string.Format("{0}?Document={1}", hLink.NavigateUrl, text);
		hLink.Text = oResult.Manager;

		lTitle.Text = oResult.Title;
		lStyle.Text = oResult.Style;
		hRow.Attributes.Add("class", string.Join(" ", oClasses.ToArray()));
		hRow.Attributes.Add("href", oResult.Id);
		iIsIMA.Visible = oResult.IMA.Equals("1");
		iIsManagerSelect.Visible = oResult.ManagerSelect.Equals("1");

        //Set omniture tag
        contentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
	}
</script>
<script type="text/javascript">
	$().ready(function () {
		managerSearchInitialize();
	});
</script>
<div class="filter-results-block manager-results-block">
	<table cellpadding="0px" cellspacing="0px" width="100%">
		<tr>
			<td width="*" valign="top" class="manager-list">
				<table class="filter-results-table" width="100%" cellpadding="0px" cellspacing="0px">
					<asp:Repeater ID="rGroups" runat="server">
						<ItemTemplate>
							<thead>
								<asp:PlaceHolder ID="pGroup" runat="server">
									<tr class="sectional">
										<th colspan="5"><asp:Literal ID="lTitle" runat="server" /></th>
									</tr>
								</asp:PlaceHolder>
								<tr>
									<th class="first">Investment Manager</td>
									<th>Strategy Name</td>
									<th>Investment Category</td>
									<th class="center">IMA</td>
									<th class="last center">Manager Select</td>
								</tr>
							</thead>
							<tbody>
								<asp:Repeater ID="rItems" runat="server">
									<ItemTemplate>
										<tr id="hRow" runat="server">
											<td class="first"><asp:HyperLink ID="hLink" runat="server" /></td>
											<td><asp:Literal ID="lTitle" runat="server" /></td>
											<td><asp:Literal ID="lStyle" runat="server" /></td>
											<td class="center"><asp:Image id="iIsIMA" runat="server" ImageUrl="~/CMSContent/Images/checkmark.png" /></td>
											<td class="last center"><asp:Image id="iIsManagerSelect" runat="server" ImageUrl="~/CMSContent/Images/checkmark.png" /></td>
										</tr>
									</ItemTemplate>
								</asp:Repeater>
							</tbody>
						</ItemTemplate>
					</asp:Repeater>
				</table>
			</td>
			<td width="230px" valign="top" class="manager-detail">
				<h4>Investment Category</h4>
				<hr />
				<h3 class="manager-name"></h3>
				<label class="strategy-name"></label>
				<table class="investment-style-guide" width="100%" cellspacing="3px" cellpadding="0px">
				</table>
				<hr />
				<div class="html">
					<h6>Investment Philosophy &amp; Process</h6>
					<div class="strategy-information">
							
					</div>
				</div>
			</td>
		</tr>
	</table>
</div>

<pre><asp:Literal ID="lDebug" runat="server" /></pre>
