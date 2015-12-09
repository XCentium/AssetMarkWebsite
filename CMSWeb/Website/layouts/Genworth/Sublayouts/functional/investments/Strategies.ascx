<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
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
		// bind product link
		var oProductLinkItem = Genworth.SitecoreExt.Constants.Investments.Items.ProductLink;
		if (oProductLinkItem != null)
		{
			oProductLinkItem.ConfigureHyperlink(hProductLink);
			hProductLink.Text = oProductLinkItem.DisplayName;
			hProductLink.Visible = true;

			//Set Omniture tags
			oProductLinkItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hProductLink);
		}
	}

	private List<Tuple<string,string>> filterFields;

	private JArray BuildFilterGroups(Item item)
	{
		var filterGroups = new JArray();
		foreach (Item groupItem in item.Children)
		{
			dynamic group = new JObject();
			group.name = groupItem["Name"];

			if (groupItem.TemplateName == "Filter Rollout")
			{
				group.groups = BuildFilterGroups(groupItem);
			}
			else
			{
				var controls = new JArray();
				foreach (Item controlItem in groupItem.Children)
				{
					dynamic control = new JObject();
					control.type = controlItem.TemplateName;
					if (controlItem.TemplateName == "Checkbox Control")
					{
						InternalLinkField fieldLink = controlItem.GetField("Field");
						var target = fieldLink.TargetItem;
						if (target == null)
							continue;

						var field = new Tuple<string,string>(target.Parent.Name, target.Name);
						filterFields.Add(field);

						control.name = controlItem["Name"];
						control.field = field.Item1 + "/" + field.Item2;
					}
					controls.Add(control);
				}

				group.controls = controls;
			}
			filterGroups.Add(group);
		}
		return filterGroups;
	}

	private string ServerData()
	{
		// To do: move this to: Genworth.SitecoreExt.Constants.Investments.Items.StrategyDetailItem
		var detailItem = Sitecore.Context.Database.GetItem("/sitecore/content/Home/Investments/Strategies New/Strategy");
		var strategistsItem = Sitecore.Context.Database.GetItem("/sitecore/content/Shared Content/Investments/Strategists");

		HyperLink olink = new HyperLink();
		detailItem.ConfigureHyperlink(olink);
		var detailUrl = olink.NavigateUrl;

		filterFields = new List<Tuple<string,string>>();

		var filterGroups = BuildFilterGroups(ContextExtension.CurrentItem.Axes.GetChild("Filters"));

		var strategies = new JArray();
		foreach (Item item in strategistsItem.Axes.SelectItems("descendant::*[@@TemplateName='Solution']"))
		{
			dynamic strategy = new JObject();
			strategy.id = item.ID.ToString();
			strategy.name = item.DisplayName;
			strategy.min = 50000;
			strategy.riskProfile = "P2, P3, P4";
			strategy.fee = 0.67;
			strategy.investmentVehicle = "ETF";
			strategy.investmentProvider = "Strategist";

			foreach (var field in filterFields)
			{
				strategy[field.Item1 + "/" + field.Item2] = item.GetField(field.Item1, field.Item2).Value;
			}

			strategies.Add(strategy);
		}

		dynamic serverData = new JObject();
		serverData.detailUrl = detailUrl;
		serverData.strategies = strategies;
		serverData.filterGroups = filterGroups;
		return ((JObject)serverData).ToString();
	}

</script>
<style type="text/css">

.strategySection {
	position: relative;
}

.strategySection .template {
	display: none !important;
}

.strategySection .limitations {
	position: absolute;
	right: 10px;
	top: 0;
	font-size: 11px;
}

.strategySection .filterToolbar {
	margin: 0 10px;
	padding: 5px 0;
}

.strategySection .filterToolbarButton {
	float: left;
	height: 26px;
	font-size: 11px;
	line-height: 26px;
	text-align: center;
	margin: 0;
	margin-bottom: 5px;
	border: 1px solid rgb(194, 194, 194);
	border-radius: 2px;
	padding: 0 15px;
	color: black;
	background: linear-gradient(to bottom, #ffffff, rgb(253,253,253) 35%, rgb(250,250,250) 50%, rgb(246,246,246) 75%);
	whitespace: no-wrap;
	margin-right: 4px;
	cursor: pointer;
}

.strategySection .filterToolbarButton:hover {
	background: linear-gradient(to bottom, #ffffff, rgb(250,250,250) 35%, rgb(247,247,247) 50%, rgb(243,243,243) 75%);
}

.strategySection .filterList {
	margin-left: 100px;
}

.strategySection .filterList .filterToolbarButton {
	border: 1px solid rgb(1,123,56);
	background: rgb(1,123,56);
	color: white;
}

.strategySection .filterList .filterToolbarButton:hover {
	background: rgb(1,135,61);
}

.strategySection .filterToolbarButton > svg {
	display: inline-block;
	vertical-align: -2px;
	margin-right: 2px;
}

.strategySection .filterStrategySplit {
	position: relative;
	padding: 10px 0;
	min-height: calc(100vh - 220px);
}

.strategySection .filterPanel {
	margin-left: 10px;
	margin-right: 415px;
	-webkit-transition: opacity 0.5s;
	transition: opacity 0.5s;
}

.strategySection .filterStrategySplit.open .filterPanel {
	opacity: 0.4;
}

.strategySection .strategyPanel {
	position: absolute;
	z-index: 1;
	top: 5px;
	right: -4px;
	width: 387px;
	height: calc(100% - 100px);
	-webkit-transition: width 0.5s;
	transition: width 0.5s;
}

.strategySection .filterStrategySplit.open .strategyPanel {
	width: 850px;
}

.strategySection .strategyPanelContent {
	width: 100%;
	height: 100%;
	box-shadow: -2px 2px 4px rgba(0,0,0,0.3);
}

.strategySection .strategyOpener {
	position: absolute;
	z-index: -1;
	top: calc(50% - 108px);
	left: -20px;
	width: 20px;
	height: 217px;
	cursor: pointer;
}

.strategySection .filterGroupItems {
}

.strategySection .filterGroup {
	padding: 5px 0;
	border-bottom: 1px solid #ddd;
}

.strategySection .filterTitle {
	padding: 3px 10px 3px 0;
	font-size: 8px;
	line-height: 10px;
	width: 82px;
	float: left;
}

.strategySection .filterRollout {
}

.strategySection .filterRolloutTitle {
	font-size: 8px;
	font-weight: bold;
	line-height: 10px;
	padding: 10px 0;
	text-transform: uppercase;
	cursor: pointer;
}

.strategySection .filterRolloutIcon {
	display: inline-block;
	vertical-align: middle;
}

.strategySection .filterRolloutIcon g {
	transform: rotate(180deg);
	transition: transform 0.5s;
}

.strategySection .filterRollout.open .filterRolloutIcon g {
	transform: rotate(0deg);
}

.strategySection .filterRolloutGroupItems {
	display: none;
}

.strategySection .filterRollout.open .filterRolloutGroupItems {
	display: block;
}

.strategySection .filterControls {
	float: left;
	width: calc(100% - 92px);
}

.strategySection .strategyToolbar {
	position: relative;
	padding: 8px 12px 8px 10px;
	background: rgb(68,68,68);
	height: 26px;
	color: white;
}

.strategySection .strategySummary {
	display: inline-block;
	font-size: 11px;
	line-height: 26px;
}

.strategySection .strategySearch {
	position: absolute;
	top: 10px;
	right: 0;
	width: 182px;
	height: 24px;
	float: right;
}

.strategySection .strategySearch input {
	margin: 0;
	padding: 0 6px;
	border: 1px solid rgb(222,222,222);
	width: 168px;
	height: 22px;
	font-size: 11px;
	line-height: 22px;
	color: black;
	background: white;
}

.strategySection .strategyList {
	background: #eee;
	border: 1px solid #ededed;
	height: calc(100% - 44px);
	position: relative;
	overflow-x: hidden;
}

.strategySection .strategyListHeader {
	position: absolute;
	top: 6px;
	width: 848px;
	height: 22px;
	font-weight: bold;
	text-transform: uppercase;
}

.strategySection .strategyListScrollArea {
	position: absolute;
	top: 28px;
	width: 100%;
	height: calc(100% - 28px);
	overflow-x: hidden;
	overflow-y: scroll;
	background: white;
}

.strategySection .strategyListBody {
	width: 848px;
}

.strategySection .strategyListRow {
	border-bottom: 1px solid #ededed;
}

.strategySection .strategyListColumn {
	float: left;
	font-size: 8px;
	line-height: 20px;
	height: 20px;
}

.strategySection .strategyListHeader .strategyListColumn {
	line-height: 8px;
	height: 18px;
}

.strategySection .strategyListColumnColor {
	width: 12px;
}

.strategySection .strategyListColumnStrategy {
	width: 200px;
	padding: 0 5px;
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
	cursor: pointer;
}

.strategySection .strategyListColumnFavorite {
	width: 64px;
	text-align: right;
}

.strategySection .strategyListColumnMin {
	width: 60px;
	padding: 0 5px;
	text-align: right;
	transition: width 0.5s;
}

.strategySection .filterStrategySplit.open .strategyListColumnMin {
	width: 100px;
}

.strategySection .strategyListColumnRiskProfile {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListColumnFee {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListColumnVehicle {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListColumnProvider {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListBody .strategyListColumnStrategy {
	color: rgb(14,117,182);
}

.strategySection .strategyListHeader .strategyListColumnMin {
	text-align: right;
}

.strategySection .strategyListColumnMinAmount {
	display: inline-block;
	min-width: 37px;
}

.strategySection .filterApproachControl {
	float: left;
	width: 460px;
}

.strategySection .approachColumn {
	float: left;
	width: 153px;
}

.strategySection .approachColumnTitle {
	padding: 3px 0 2px 0;
	font-weight: bold;
	font-size: 8px;
	line-height: 10px;
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
	text-transform: uppercase;
}

.strategySection .approachCheckbox {
	margin: 3px 8px 3px 0;
	padding: 0 5px;
	font-size: 8px;
	line-height: 21px;
	height: 21px;
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
}

.strategySection .approachCheckbox input[type=checkbox] {
	vertical-align: -3px;
	margin: 0 3px 0 0;
}

.strategySection .filterCheckboxControl {
	display: inline-block;
	margin: 0 8px 6px 0;
	padding: 4px 5px;
	width: 135px;
	font-size: 8px;
	line-height: 12px;
}

.strategySection .filterCheckboxControl input[type=checkbox] {
	vertical-align: -3px;
	margin: 0 3px 0 0;
}

.strategySection .filterCheckboxLabel {
	display: inline-block;
	white-space: pre-wrap;
	vertical-align: top;
}

.strategySection .filterRiskProfileControl {
	float: left;
	width: 450px;
}

.strategySection .filterFeeControl {
	float: left;
	width: 450px;
}

.strategySection .strategyFavoriteButton {
	display: inline-block;
	background: rgb(206, 206, 206);
	color: white;
	text-transform: uppercase;
	text-align: center;
	width: 36px;
	font-size: 7px;
	line-height: 14px;
	border-radius: 1px;
	font-weight: bold;
	margin: 2px 0;
	cursor: pointer;
}

.strategySection .strategyFavoriteButton.selected {
	background: rgb(2, 123, 54);
}

.strategySection .strategyFavoriteButton .strategySave {
	display: inline;
}

.strategySection .strategyFavoriteButton.selected .strategySave {
	display: none;
}

.strategySection .strategyFavoriteButton .strategySaved {
	display: none;
}

.strategySection .strategyFavoriteButton.selected .strategySaved {
	display: inline;
}

</style>

<div class="strategySection">
	<div class="limitations"><asp:HyperLink ID="hProductLink" runat="server" Visible="false"></asp:HyperLink></div>
	<div class="filterToolbar">
		<div class="filterToolbarButton" id="strategyClearFilters">Clear Filters</div>
		<div class="filterList"></div>
		<div style="clear:both"></div>
		<div class="filterToolbarButton template">
			<svg width="12px" height="12px" viewBox="0 0 12 12">
				<circle cx="6" cy="6" r="6" fill="rgb(115,173,86)" />
				<line x1="3" y1="3" x2="9" y2="9" stroke="rgb(1,123,56)" stroke-width="1" />
				<line x1="3" y1="9" x2="9" y2="3" stroke="rgb(1,123,56)" stroke-width="1" />
			</svg>
			<span class="filterToolbarButtonTitle"></span>
		</div>
	</div>
	<div class="filterStrategySplit">
		<div class="filterPanel">
			<div class="filterGroupItems"></div>
			<div class="filterGroup template">
				<div class="filterTitle"></div>
				<div class="filterControls"></div>
				<div style="clear:both"></div>
			</div>
			<div class="filterRollout template">
				<div class="filterRolloutTitle">
					<span class="filterRolloutTitleText"></span>
					<svg class="filterRolloutIcon" width="16px" height="8px" viewBox="-8 -4 16 8">
						<g><path d="M 0 -2 L 4 2 L -4 2 z" fill="rgb(1,125,187)" /></g>
					</svg>
				</div>
				<div class="filterRolloutGroupItems"></div>
				<div style="clear:both"></div>
			</div>
			<div class="filterApproachControl template">
				<div class="approachColumn">
					<div class="approachColumnTitle">Core Markets</div>
					<div class="approachCheckbox" style="background-color:rgb(177,214,234)"><input type="checkbox"> Core Markets</div>
				</div>
				<div class="approachColumn">
					<div class="approachColumnTitle">Tactical Strategies</div>
					<div class="approachCheckbox" style="background-color:rgb(177,214,234)"><input type="checkbox"> Enhanced Return Focus</div>
					<div class="approachCheckbox" style="background-color:rgb(255,199,112)"><input type="checkbox"> Loss Limit Focus</div>
				</div>
				<div class="approachColumn">
					<div class="approachColumnTitle">Diversifying Strategies</div>
					<div class="approachCheckbox" style="background-color:rgb(255,199,112)"><input type="checkbox"> Equity Alternatives</div>
					<div class="approachCheckbox" style="background-color:rgb(255,199,112)"><input type="checkbox"> Bonds &amp; Bond Alternatives</div>
				</div>
				<div style="clear:both"></div>
			</div>
			<div class="filterCheckboxControl template"><input type="checkbox"> <span class="filterCheckboxLabel"></span></div>
			<div class="filterRiskProfileControl template">
				<svg width="450px" height="52px" viewBox="0 0 450 52">
					<rect x="0" y="22" width="450" height="2" fill="rgb(203,205,199)" />
					<rect x="11" y="22" width="342" height="2" fill="rgb(0,124,56)" />
					<rect x="11" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="96" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="182" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="267" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="353" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="439" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<circle cx="11" cy="23" r="10" fill="white" stroke="rgb(180,180,180)" stroke-width="1" />
					<circle cx="353" cy="23" r="10" fill="white" stroke="rgb(180,180,180)" stroke-width="1" />
					<text x="12" y="49" font-size="8" fill="black" text-anchor="middle">P1</text>
					<text x="97" y="49" font-size="8" fill="black" text-anchor="middle">P2</text>
					<text x="183" y="49" font-size="8" fill="black" text-anchor="middle">P3</text>
					<text x="268" y="49" font-size="8" fill="black" text-anchor="middle">P4</text>
					<text x="354" y="49" font-size="8" fill="black" text-anchor="middle">P5</text>
					<text x="440" y="49" font-size="8" fill="black" text-anchor="middle">P6</text>
				</svg>
			</div>
			<div class="filterFeeControl template">
				<svg width="450px" height="52px" viewBox="0 0 450 52">
					<rect x="0" y="22" width="450" height="2" fill="rgb(203,205,199)" />
					<g class="feeBars">
						<rect x="79" y="20" width="10" height="2" fill="rgb(203,205,199)" />
						<rect x="89" y="18" width="10" height="4" fill="rgb(203,205,199)" />
						<rect x="99" y="20" width="10" height="2" fill="rgb(203,205,199)" />
						<rect x="109" y="20" width="10" height="2" fill="rgb(203,205,199)" />
						<rect x="119" y="21" width="10" height="1" fill="rgb(203,205,199)" />
						<rect x="129" y="16" width="10" height="6" fill="rgb(203,205,199)" />
						<rect x="139" y="12" width="10" height="10" fill="rgb(203,205,199)" />
						<rect x="249" y="14" width="10" height="8" fill="rgb(203,205,199)" />
					</g>
					<rect x="118" y="22" width="214" height="2" fill="rgb(0,124,56)" />
					<rect x="11" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="118" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="225" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="332" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<rect x="439" y="15" width="1" height="17" fill="rgb(101,101,101)" />
					<circle cx="118" cy="23" r="10" fill="white" stroke="rgb(180,180,180)" stroke-width="1" />
					<circle cx="332" cy="23" r="10" fill="white" stroke="rgb(180,180,180)" stroke-width="1" />
					<text x="12" y="49" font-size="8" fill="black" text-anchor="middle">0.00%</text>
					<text x="119" y="49" font-size="8" fill="black" text-anchor="middle">0.25%</text>
					<text x="226" y="49" font-size="8" fill="black" text-anchor="middle">0.50%</text>
					<text x="333" y="49" font-size="8" fill="black" text-anchor="middle">0.75%</text>
					<text x="440" y="49" font-size="8" fill="black" text-anchor="middle">1.00%</text>
				</svg>
			</div>
		</div>
		<div class="strategyPanel">
			<div class="strategyOpener" id="strategyOpener">
				<svg width="20px" height="217px" viewBox="0 0 40 434">
					<path d="M 40 0 L 0 40 L 0 394 L 40 434 z" fill="rgb(188,188,188)" />
					<path d="M 16 127 L 24 119 L 24 135 z" fill="rgb(127,127,127)" />
					<path d="M 16 304 L 24 296 L 24 312 z" fill="rgb(127,127,127)" />
					<text x="26" y="215" text-anchor="middle" transform="rotate(-90 26,215)" font-family="Arial" font-size="15" font-weight="bold" fill="rgb(76,76,76)">MORE DETAILS</text>
				</svg>
			</div>
			<div class="strategyPanelContent">
			<div class="strategyToolbar">
				<div class="strategySummary"><b><span class="strategyCount">0</span></b> Strategies</div>
				<div class="strategySearch"><input id="strategySearchField" type="search" placeholder="Search Strategies" /></div>
			</div>
			<div class="strategyList">
				<div class="strategyListHeader">
					<div class="strategyListColumn strategyListColumnColor"></div>
					<div class="strategyListColumn strategyListColumnStrategy"><br/>Strategy</div>
					<div class="strategyListColumn strategyListColumnFavorite"><br/>Favorites</div>
					<div class="strategyListColumn strategyListColumnMin">Investment<br/>Minimum</div>
					<div class="strategyListColumn strategyListColumnRiskProfile">Risk<br/>Profile</div>
					<div class="strategyListColumn strategyListColumnFee">Platform<br/>Fee</div>
					<div class="strategyListColumn strategyListColumnVehicle">Investment<br/>Vehicle</div>
					<div class="strategyListColumn strategyListColumnProvider">Investment<br/>Provider</div>
					<div style="clear:both"></div>
				</div>
				<div class="strategyListScrollArea">
					<div class="strategyListBody"></div>
				</div>
				<div class="strategyListRow template">
					<div class="strategyListColumn strategyListColumnColor"></div>
					<div class="strategyListColumn strategyListColumnStrategy"><a class="strategyDetailLink"></a></div>
					<div class="strategyListColumn strategyListColumnFavorite">
						<div class="strategyFavoriteButton">
							<span class="strategySave">Save</span>
							<span class="strategySaved">Saved</span>
						</div>
					</div>
					<div class="strategyListColumn strategyListColumnMin">
						$<span class="strategyListColumnMinAmount"></span>
					</div>
					<div class="strategyListColumn strategyListColumnRiskProfile"></div>
					<div class="strategyListColumn strategyListColumnFee"></div>
					<div class="strategyListColumn strategyListColumnVehicle"></div>
					<div class="strategyListColumn strategyListColumnProvider"></div>
					<div style="clear:both"></div>
				</div>
			</div>
			</div>
		</div>
		<div style="clear:both"></div>
	</div>
</div>
<script language="javascript" type="text/javascript">

ServerData = <%= ServerData() %>;

function cloneTemplate(query) {
	var copy = $(query).clone();
	copy.removeClass("template");
	return copy;
}

function toStringWithThousandSep(v) {
	var text = Math.round(v).toString();
	for (var i = 3; i < text.length; i += 4) {
		text = text.substr(0, text.length - i) + "," + text.substr(text.length - i);
	}
	return text;
}

var strategyFilters = [];

function sortAndUpdateStrategyList() {
	var reverse = $(".strategyListHeader .strategyListColumnStrategy").hasClass("reverseOrder");

	if (!reverse) {
		ServerData.strategies.sort(function (a, b) { if (a.name < b.name) return -1; else if (a.name > b.name) return 1; else return 0; });
	}
	else {
		ServerData.strategies.sort(function (a, b) { if (a.name > b.name) return -1; else if (a.name < b.name) return 1; else return 0; });
	}

	updateStategyList();
}

function updateStategyList() {
	var body = $(".strategyListBody");
	body.empty();

	var searchText = $("#strategySearchField").val().toLowerCase();

	var count = 0;
	ServerData.strategies.forEach(function (strategy) {
		if (searchText.length > 1 && strategy.name.toLowerCase().indexOf(searchText) == -1) return;

		var skip = false;
		strategyFilters.forEach(function (filter) { if (!filter(strategy)) skip = true; });
		if (skip) return;

		count++;

		var item = cloneTemplate(".strategyListRow.template");
		$(".strategyListColumnColor", item).attr("style", "background-color:rgb(177,214,234)");
		//$(".strategyListColumnColor", item).attr("style", "background-color:rgb(255,199,112)");
		$(".strategyDetailLink", item).text(strategy.name);
		$(".strategyDetailLink", item).attr('href', ServerData.detailUrl + "?Document=" + strategy.id);
		$(".strategyListColumnMinAmount", item).text(toStringWithThousandSep(strategy.min));
		$(".strategyListColumnRiskProfile", item).text(strategy.riskProfile);
		$(".strategyListColumnFee", item).text(strategy.fee.toFixed(2) + "%");
		$(".strategyListColumnVehicle", item).text(strategy.investmentVehicle);
		$(".strategyListColumnProvider", item).text(strategy.investmentProvider);

		if (strategy.favorite) {
			$(".strategyFavoriteButton", item).addClass("selected");
		}

		$(".strategyFavoriteButton", item).on('click', function() {
			if (strategy.favorite) {
				strategy.favorite = false;
				$(this).removeClass("selected");
			}
			else {
				strategy.favorite = true;
				$(this).addClass("selected");
			}
		});

		body.append(item);
	});

	$(".strategyCount").text(count);
}

function createFilterList() {
	strategyFilters = [];

	function createFilterGroup(filtersDiv, filterGroup) {
		if (filterGroup.controls) {
			var groupItem = cloneTemplate(".filterGroup.template");
			$(".filterTitle", groupItem).text(filterGroup.name);

			filterGroup.controls.forEach(function (control) {
				var controlItem = null;
				switch (control.type) {
				case "Investment Approach Control":
					controlItem = cloneTemplate(".filterApproachControl.template");
					break;
				case "Risk Profile Control":
					controlItem = cloneTemplate(".filterRiskProfileControl.template");
					break;
				case "Fee Control":
					controlItem = cloneTemplate(".filterFeeControl.template");
					break;
				case "Checkbox Control":
					controlItem = cloneTemplate(".filterCheckboxControl.template");
					$(".filterCheckboxLabel", controlItem).text(control.name);

					var checkboxInput = $("input[type='checkbox']", controlItem);
					var checked = checkboxInput.get(0).checked;

					var activeFilter = {
						name: control.name,
						clear: function() {
							checkboxInput.get(0).checked = false;
							checked = false;
						},
					};

					checkboxInput.on('change', function () {
						checked = this.checked;
						if (checked) {
							addActiveFilter(activeFilter);
						}
						else {
							removeActiveFilter(activeFilter);
						}
						updateStategyList();
					});

					strategyFilters.push(function (strategy) {
						return !checked || strategy[control.field] == "1";
					});
					break;
				}

				if (controlItem != null) {
					$(".filterControls", groupItem).append(controlItem);
				}
			});

			filtersDiv.append(groupItem);
		}
		else {
			var groupRollout = cloneTemplate(".filterRollout.template");
			var rolloutFilters = $(".filterRolloutGroupItems", groupRollout);
			$(".filterRolloutTitleText", groupRollout).text(filterGroup.name);
			$(".filterRolloutTitle", groupRollout).on('click', function() {
				$(this).parent().toggleClass('open');
			});
			filterGroup.groups.forEach(function (item) { createFilterGroup(rolloutFilters, item); });
			filtersDiv.append(groupRollout);
		}
	}

	var filters = $(".filterGroupItems");
	filters.empty();

	ServerData.filterGroups.forEach(function (item) { createFilterGroup(filters, item); });
}

var activeFilters = [];

function addActiveFilter(filter) {
	filter.button = cloneTemplate(".filterToolbarButton.template");
	$(".filterToolbarButtonTitle", filter.button).text(filter.name);
	filter.button.on('click', function() { removeActiveFilter(filter); });
	$(".filterList").append(filter.button);
	activeFilters.push(filter);
}

function removeActiveFilter(filter) {
	if (filter.button) {
		filter.button.remove();
	}
	var index = activeFilters.indexOf(filter);
	if (index != -1) activeFilters.splice(index, 1);
	filter.clear();
	updateStategyList();
}

function clearActiveFilters() {
	$(".filterList").empty();
	activeFilters.forEach(function(filter) { filter.clear(); });
	activeFilters = [];
	updateStategyList();
}

function toggleStrategyOpener() {
	$(".strategySection .filterStrategySplit").toggleClass("open");
}

function toggleSortOrder() {
	$(this).toggleClass("reverseOrder");
	sortAndUpdateStrategyList();
}

$("#strategyClearFilters").on('click', clearActiveFilters);
$("#strategySearchField").on('input', updateStategyList);
$("#strategyOpener").on('click', toggleStrategyOpener);
$(".strategyListHeader .strategyListColumnStrategy").on('click', toggleSortOrder);

createFilterList();
sortAndUpdateStrategyList();

</script>