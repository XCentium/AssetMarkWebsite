<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Services.Investments" %>
<%@ Import Namespace="Lucene.Net.Documents" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Newtonsoft.Json.Linq" %>

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
		// get the allocation approach in case there is one (GPS doesn't have one, for instance)
		//string sAllocationApproach;
		//var sAllocationApproachParameter = !String.IsNullOrWhiteSpace(sAllocationApproach = Request.QueryString[Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach]) ?
		//	String.Format("&{0}={1}", Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach, sAllocationApproach) : String.Empty;

		// Get the document
		var document = ContextExtension.CurrentDatabase.GetItem((Request.QueryString["Document"] ?? string.Empty).Trim());
		if (document == null)
			return;

		lHeaderTitle.Text = Server.HtmlEncode(document.DisplayName);
		lHeaderSubTitle.Text = Server.HtmlEncode("Tactical Strategies - Enhanced Return Focus");

		// Get the manager or strategist associated with this item
		string sManagerOrStrategistParameter = "";
		var owner = document.Axes.GetAncestors().GetItemsOfTemplate(new string[] { "Manager", "Strategist" }).FirstOrDefault();
		if (owner != null)
		{
			//depending on the owner or manager, we need to build out a research object
			if (owner.InstanceOfTemplate("Manager"))
			{
				//create the search
				var oSearch = new Search(new ItemCache(), true);
				oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.ManagerId, new string[] { owner.ID.ToString() });
				sManagerOrStrategistParameter = string.Format("manager={0}", owner.GetText("Manager", "Code"));
				BindResearch(oSearch.ResultDocuments.Select(oTemp => new Result(oTemp)));
				//h2Title.InnerText = "Manager Detail";
			}
			else if (owner.InstanceOfTemplate("Strategist"))
			{
				var oSearch = new Search(new ItemCache(), true);
				oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.StrategistId, new string[] { owner.ID.ToString() });
				sManagerOrStrategistParameter = string.Format("strategist={0}", owner.GetText("Strategist", "Code"));
				BindResearch(oSearch.ResultDocuments.Select(oTemp => new Result(oTemp)));
				//h2Title.InnerText = "Strategist Detail";
			}
		}

		if (document.InstanceOfTemplate("Document Base"))
		{
			//bind the document
			BindPDF(document);
		}
		else
		{
			var tempDocument = document.GetListItem("Documents", "Fact Sheet");
			if (tempDocument != null)
			{
				//bind the fact sheet
				BindPDF(tempDocument);
			}
			else
			{
				tempDocument = document.GetListItem("Documents", "Profile Sheet");
				if (tempDocument != null)
					BindPDF(tempDocument);
			}
		}

		sResearch.Text = "Go to Archived Research";
		dResearch.Attributes.Add("data-url", String.Format("{0}?{1}", Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.GetURL(), sManagerOrStrategistParameter));
	}

	private void BindResearch(IEnumerable<Result> documents)
	{
		var items = new List<Tuple<string, Result>>();

		foreach (Item categoryItem in ContextExtension.CurrentItem.Axes.GetChild("Sidebar").Children)
		{
			DateTime dDate;

			var categoryName = categoryItem["Name"];
			var categoryValue = categoryItem["Category"];
			
			var categoryDocs = documents.Where(doc => doc.Category == categoryValue);
			var categoryDoc = categoryDocs.OrderByDescending(oResult => DateTime.TryParse(oResult.Date, out dDate) ? dDate.ToString("yyyyMMddTHHmmss") : string.Empty).FirstOrDefault();
			if (categoryDoc != null)
				items.Add(new Tuple<string, Result>(categoryName, categoryDoc));
		}

		rCategories.DataSource = items;
		rCategories.ItemDataBound += new RepeaterItemEventHandler(rCategories_ItemDataBound);
		rCategories.DataBind();
	}

	private void rCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var item = (Tuple<string, Result>)e.Item.DataItem;
		string itemName = item.Item1;
		string itemPath = string.IsNullOrWhiteSpace(item.Item2.sUrl) ? item.Item2.Path : item.Item2.sUrl;
		string itemExt = item.Item2.Extension;

		var dCategory = (HtmlGenericControl)e.Item.FindControl("dCategory");
		dCategory.Attributes.Add("data-url", itemPath);
		dCategory.Attributes.Add("data-extension", itemExt);
		if (e.Item.ItemIndex == 0)
			dCategory.Attributes["class"] = dCategory.Attributes["class"] + " selected";

		var sCategory = (Literal)e.Item.FindControl("sCategory");
		sCategory.Text = Server.HtmlEncode(itemName);

		//Set omniture tag
		//var contentItem = ContextExtension.CurrentDatabase.GetItem(ItemHelper.FormatId(item.Item2.Id));
		//contentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, dCategory);
	}

	private void BindPDF(Item oDocument)
	{
		string sPath;

		if (oDocument != null)
		{
			//does the document have a file?
			if (!string.IsNullOrEmpty(sPath = oDocument.GetImageURL("Document", "File")))
			{
				lPDF.Text = string.Format("<object data='{0}' type='application/pdf'> Your browser does not support PDF plugin. You can <a href='{0}'>click here to download the PDF file.</a></object>", sPath);
			}
		}
	}

</script>
<style type="text/css">

.strategyDetail {
}

.strategyDetail .detailHeader {
	position: relative;
	margin-left: 150px;
	padding: 10px 0 30px 0;
}

.strategyDetail .detailHeaderBack {
	position: absolute;
	left: -139px;
	top: 10px;
	min-width: 55px;
	height: 26px;
	font-size: 11px;
	line-height: 26px;
	text-align: center;
	margin: 0;
	border: 1px solid rgb(194, 194, 194);
	border-radius: 2px;
	padding: 0 15px;
	color: black;
	background: linear-gradient(to bottom, #ffffff, rgb(253,253,253) 35%, rgb(250,250,250) 50%, rgb(246,246,246) 75%);
	whitespace: no-wrap;
	margin-right: 4px;
	cursor: pointer;
}

.strategyDetail .detailHeaderBack:hover {
	background: linear-gradient(to bottom, #ffffff, rgb(250,250,250) 35%, rgb(247,247,247) 50%, rgb(243,243,243) 75%);
}


.strategyDetail .detailHeaderTitleArea {
	padding: 0 0 10px 0;
}

.strategyDetail .detailHeaderTitle {
	display: inline-block;
}

.strategyDetail .detailHeaderLine1 {
	color: rgb(67,67,67);
	font-size: 18px;
	font-weight: bold;
	margin-bottom: 2px;
}

.strategyDetail .detailHeaderLine2 {
	color: rgb(144,144,144);
	font-size: 11px;
	font-style: italic;
}

.strategyDetail .detailSavedButton {
	display: inline-block;
	vertical-align: top;
	text-transform: uppercase;
	background: rgb(2,123,53);
	color: white;
	font-size: 10px;
	line-height: 26px;
	padding: 0 16px;
	border-radius: 2px;
	margin-top: 3px;
	margin-left: 22px;
}

.strategyDetail .detailHeaderTableArea {
}

.strategyDetail .detailHeaderTableArea table {
	display: inline-table;
	vertical-align: top;
	border-spacing: 0;
	border-collapse: collapse;
	margin-right: 2px;
}

.strategyDetail .detailHeaderTableArea tr {
}

.strategyDetail .detailHeaderTableArea td:first-child {
	font-size: 9px;
	line-height: 11px;
	text-align: right;
	color: gray;
	vertical-align: top;
	border: 1px solid rgb(238,238,238);
	border-right: none;
	padding: 6px 20px;
	padding-right: 6px;
}

.strategyDetail .detailHeaderTableArea td:last-child {
	font-size: 9px;
	line-height: 11px;
	font-weight: bold;
	vertical-align: top;
	border: 1px solid rgb(238,238,238);
	border-left: none;
	padding: 6px 20px;
	padding-left: 0;
}

.strategyDetail .detailHeaderLinks {
	position: absolute;
	right: 15px;
	bottom: 0px;
}

.strategyDetail .downloadLink {
	display: inline-block;
}

.strategyDetail .printLink {
	display: inline-block;
	padding-left: 10px;
}

.strategyDetail .detailBody {
	margin: 15px -4px;
}

.strategyDetail .detailSidebar {
	float: left;
	width: 134px;
	padding: 2px 0;
}

.strategyDetail .detailDocument {
	position: relative;
	margin-left: 150px;
	margin-right: 20px;
	height: calc(100vh - 350px);
	border: 1px solid rgb(238,238,238);
}

.strategyDetail .detailDocument > iframe,
.strategyDetail .detailDocument > object {
	position: absolute;
	margin: 0;
	padding: 0;
	border: none;
	width: 100%;
	height: 100%;
}

.strategyDetail .sidebarRow {
	padding: 9px 13px;
	font-size: 10px;
	line-height: 11px;
	font-weight: bold;
	border-bottom: 1px solid rgb(238,238,238);
	border-right: 1px solid rgb(238,238,238);
	cursor: pointer;
	position:relative;
}

.strategyDetail .sidebarRow:first-child {
	border-top: 1px solid rgb(238,238,238);
}

.strategyDetail .sidebarRow:hover {
	background: #eee;
}

.strategyDetail .sidebarRow.selected {
	background: rgb(1,101,161);
	border-top-color: rgb(1,101,161);
	border-bottom-color: rgb(1,101,161);
	border-right-color: rgb(1,101,161);
	color: white;
}

.strategyDetail .sidebarRowArrow {
	display: none;
	position: absolute;
	top: -1px;
	right: -9px;
	width: 8px;
	height: calc(100% + 2px);
	background-image: url("data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIj8+PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI4IiBoZWlnaHQ9IjMwIiB2aWV3Qm94PSIwIDAgOCAzMCI+PHBhdGggZmlsbD0icmdiKDEsMTAxLDE2MSkiIGQ9Ik0wIDAgTDggMTUgTCAwIDMwIHoiIC8+PC9zdmc+");
	background-position: top right;
	background-size: 8px 100%;
	background-repeat: no-repeat;
}

.strategyDetail .sidebarRow.selected .sidebarRowArrow {
	display: block;
}

</style>

<div class="strategyDetail">

	<div class="detailHeader">
		<div class="detailHeaderBack" onclick="javascript:window.history.back()">&lt; Back</div>
		<div class="detailHeaderTitleArea">
			<div class="detailHeaderTitle">
				<div class="detailHeaderLine1"><asp:Literal ID="lHeaderTitle" runat="server" /></div>
				<div class="detailHeaderLine2"><asp:Literal ID="lHeaderSubTitle" runat="server" /></div>
			</div>
			<div class="detailSavedButton">Saved</div>
		</div>
		<div class="detailHeaderTableArea">
			<asp:Repeater ID="rTable" runat="server">
				<ItemTemplate>
					<table>
						<asp:Repeater ID="rTableRow" runat="server">
							<ItemTemplate>
								<td><asp:Literal ID="lRowName" runat="server" />:</td>
								<td><asp:Literal ID="lRowValue" runat="server" /></td>
							</ItemTemplate>
						</asp:Repeater>
					</table>
				</ItemTemplate>
			</asp:Repeater>
			<table>
				<tr>
					<td>Investment Objective:</td>
					<td>Growth</td>
				</tr>
				<tr>
					<td>Geographic Focus:</td>
					<td>Global</td>
				</tr>
				<tr>
					<td>Investment Minimum:</td>
					<td>$35,000</td>
				</tr>
			</table>
			<table>
				<tr>
					<td>Investment Vehicle Type:</td>
					<td>Blended ETFs/Mutual Funds<br/>Mutual Funds<br/>Fixed Income</td>
				</tr>
				<tr>
					<td>Platform Fee:</td>
					<td>0.50%</td>
				</tr>
			</table>
			<table>
				<tr>
					<td>Strategy Features:</td>
					<td>Tax Sensitive<br/>Personal Values</td>
				</tr>
				<tr>
					<td>Risk Profile:</td>
					<td>P2, P3, P4</td>
				</tr>
			</table>
		</div>
		<div style="clear: both"></div>
		<div class="detailHeaderLinks">
			<div class="downloadLink">Download</div>
			<div class="printLink">Print</div>
		</div>
	</div>
	<div class="detailBody">
		<div class="detailSidebar">
			<asp:Repeater ID="rCategories" runat="server">
				<ItemTemplate>
					<div class="sidebarRow" ID="dCategory" runat="server">
						<span class="sidebarRowTitle"><asp:Literal ID="sCategory" runat="server" /></span>
						<div class="sidebarRowArrow"></div>
					</div>
				</ItemTemplate>
			</asp:Repeater>
			<div class="sidebarRow" ID="dResearch" runat="server">
				<span class="sidebarRowTitle"><asp:Literal ID="sResearch" runat="server" /></span>
				<div class="sidebarRowArrow"></div>
			</div>
		</div>
		<div class="detailDocument"><asp:Literal ID="lPDF" runat="server" /></div>
		<div style="clear: both"></div>
	</div>
</div>

<script language="javascript" type="text/javascript">
$(".strategyDetail .sidebarRow").on('click', function (e) {
	var url = $(this).attr("data-url");
	var ext = $(this).attr("data-extension");
	if (ext != undefined && ext.toLowerCase() == "pdf") {
		$(".strategyDetail .sidebarRow").removeClass("selected");
		$(this).addClass("selected");
		$(".detailDocument object").attr("data", url);
	}
	else {
		window.location.href = url;
	}
});
</script>
