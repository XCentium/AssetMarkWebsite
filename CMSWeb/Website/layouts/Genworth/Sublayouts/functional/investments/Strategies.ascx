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
			group.tip = groupItem["Tooltip"];

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
					else if (controlItem.TemplateName == "Investment Approach Control")
					{
						var groups = new JArray();
						for (int i = 0; i < 3; i++)
						{
							string groupTitle = controlItem[String.Format("Title {0}", i + 1)];
							MultilistField groupList = controlItem.GetField(String.Format("Approach {0}", i + 1));

							var approaches = new JArray();
							foreach (Item approachItem in groupList.GetItems())
							{
								dynamic approach = new JObject();
								approach.name = approachItem["Title"];
								approach.rowing = approachItem["Rowing"] == "1";
								approaches.Add(approach);
							}

							dynamic approachGroup = new JObject();
							approachGroup.name = groupTitle;
							approachGroup.approaches = approaches;
							groups.Add(approachGroup);
						}
						control.groups = groups;
					}
					controls.Add(control);
				}

				group.controls = controls;
			}
			filterGroups.Add(group);
		}
		return filterGroups;
	}

	private List<Tuple<string,string,string,string>> BuildColumns(Item item)
	{
		var list = new List<Tuple<string,string,string,string>>();

		foreach (Item column in item.Children)
		{
			string name = column["Name"];

			/*InternalLinkField typeLink = column.GetField("Visual Appearance");
			if (typeLink.TargetItem == null)
				continue;
			string fieldType = typeLink.TargetItem.Name;

			if (fieldType == "Investment Provider")
			{
				list.Add(new Tuple<string,string,string,string>(name, null, null, fieldType));
			}
			else*/ string fieldType = "Text";
			{
				InternalLinkField fieldLink = column.GetField("Field");
				var target = fieldLink.TargetItem;
				if (target == null)
					continue;

				list.Add(new Tuple<string,string,string,string>(name, target.Parent.Name, target.Name, fieldType));
			}
		}

		return list;
	}

	private JArray BuildColumnHeader(List<Tuple<string,string,string,string>> columns)
	{
		var list = new JArray();
		foreach (var field in columns)
		{
			dynamic headerItem = new JObject();
			headerItem.name = field.Item1;
			headerItem.type = field.Item4;
			list.Add(headerItem);
		}
		return list;
	}

	private JObject BuildStrategy(Item item, List<Tuple<string,string,string,string>> columns, bool manager)
	{
		dynamic strategy = new JObject();
		strategy.id = item.ID.ToString();
		strategy.name = String.IsNullOrWhiteSpace(item["Strategy Title"]) ? item.DisplayName : item["Strategy Title"];
		strategy.modelSetTypeId = item["ModelSetTypeId"];
		strategy.strategistCode = item["StrategistCode"];
		strategy.columns = new JArray();
		strategy.filters = new JObject();

		foreach (var field in columns)
		{
			if (field.Item4 == "Investment Provider")
				strategy.columns.Add(manager ? "Manager" : "Strategist");
			else
				strategy.columns.Add(item.GetField(field.Item2, field.Item3).Value);
		}

		foreach (var field in filterFields)
		{
			strategy.filters[field.Item1 + "/" + field.Item2] = item.GetField(field.Item1, field.Item2).Value;
		}

		return strategy;
	}

	private void AddStrategists(JArray strategies, Item strategistsItem, List<Tuple<string,string,string,string>> columns)
	{
		foreach (Item strategistItem in strategistsItem.Children)
		{
			if (strategistItem.TemplateName != "Strategist")
				continue;

			string strategistName = strategistItem["Name"];

			foreach (Item strategyTypeItem in strategistItem.Children)
			{
				if (strategyTypeItem.TemplateName != "Strategy")
					continue;

				string allocationApproach = strategyTypeItem["Allocation Approach"];

				InternalLinkField fieldLink = strategyTypeItem.GetField("Allocation Approach");
				var allocationApproachItem = fieldLink.TargetItem;
				if (allocationApproachItem == null || allocationApproachItem["Display on Strategies Page"] != "1")
					continue;

				foreach (Item solutionItem in strategyTypeItem.Children)
				{
					if (solutionItem.TemplateName != "Solution")
						continue;

					dynamic strategy = BuildStrategy(solutionItem, columns, false);
					strategy.strategist = strategistName;
					strategy.allocationApproach = allocationApproachItem["Title"];
					strategy.rowing = allocationApproachItem["Rowing"] == "1";
					strategies.Add(strategy);
				}
			}
		}
	}

	private bool IsDerivedFromTemplateName(TemplateItem item, string templateName)
	{
		if (item.Name == templateName)
			return true;

		foreach (TemplateItem baseTemplate in item.BaseTemplates)
		{
			if (IsDerivedFromTemplateName(baseTemplate, templateName))
				return true;
		}

		return false;
	}

	private void AddManagers(JArray strategies, Item managersItem, List<Tuple<string,string,string,string>> columns)
	{
		foreach (Item managerItem in managersItem.Children)
		{
			if (managerItem.TemplateName != "Manager")
				continue;

			string managerName = managerItem["Name"];
			
			foreach (Item solutionItem in managerItem.Children)
			{
				if (!IsDerivedFromTemplateName(solutionItem.Template, "Manager Strategy"))
					continue;

				dynamic strategy = BuildStrategy(solutionItem, columns, true);
				strategy.manager = managerName;
				strategy.rowing = solutionItem["Rowing"] == "1";
				strategies.Add(strategy);
			}
		}
	}

	private string ServerData()
	{
		var currentItem = ContextExtension.CurrentItem;
		InternalLinkField strategistsField = currentItem.GetField("Strategists");
		InternalLinkField managersField = currentItem.GetField("Managers");
		InternalLinkField approachesField = currentItem.GetField("Allocation Approaches");
		var strategistsItem = strategistsField.TargetItem;
		var managersItem = managersField.TargetItem;

		// To do: move this to: Genworth.SitecoreExt.Constants.Investments.Items.StrategyDetailItem
		var detailItem = Sitecore.Context.Database.GetItem("/sitecore/content/Home/Investments/Strategies New/Strategy");
		HyperLink olink = new HyperLink();
		detailItem.ConfigureHyperlink(olink);
		var detailUrl = olink.NavigateUrl;

		filterFields = new List<Tuple<string,string>>();
		var columns = BuildColumns(ContextExtension.CurrentItem.Axes.GetChild("Columns"));

		var filterGroups = BuildFilterGroups(ContextExtension.CurrentItem.Axes.GetChild("Filters"));
		var columnHeader = BuildColumnHeader(columns);

		var strategies = new JArray();
		AddStrategists(strategies, strategistsItem, columns);
		AddManagers(strategies, managersItem, columns);

		dynamic serverData = new JObject();
		serverData.detailUrl = detailUrl;
		serverData.strategies = strategies;
		serverData.filterGroups = filterGroups;
		serverData.header = columnHeader;
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
	min-height: calc(100vh - 300px);
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
	width: 397px;
	height: calc(100% - 30px);
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
	font-size: 10px;
	line-height: 15px;
	width: 82px;
	float: left;
}

.strategySection .filterTitleInfoIcon {
	display: inline-block;
	width: 10px;
	height: 10px;
	vertical-align: -2px;
	background: url("/Content/Images/infoicon.png") center no-repeat;
	margin-left: 3px;
	position: relative;
}

.strategySection .filterTitleTooltip {
	position: absolute;
	bottom: 15px;
	left: -5px;
	box-shadow: 2px 2px 2px rgba(0,0,0,0.1);
	background: #fafafa;
	border: 1px solid #cdcdcd;
	border-radius: 1px;
	padding: 2px 5px;
	z-index: 1;
	opacity: 0;
	transition: opacity 0.4s;
	pointer-events: none;
	white-space: nowrap;
}

.strategySection .filterTitleTooltip > :first-child {
	margin-top: 0;
}

.strategySection .filterTitleTooltip > :last-child {
	margin-bottom: 0;
}

.strategySection .filterTitleInfoIcon:hover .filterTitleTooltip {
	opacity: 1;
	pointer-events: inherit;
}

.strategySection .filterRollout {
}

.strategySection .filterRolloutTitle {
	font-size: 10px;
	font-weight: bold;
	line-height: 15px;
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
	font-size: 13px;
	line-height: 26px;
}

.strategySection .strategySearch {
	position: absolute;
	top: 9px;
	right: 15px;
	height: 24px;
}

.strategySection .strategySearch input {
	margin: 0;
	padding: 0 30px 0 8px;
	border: none;
	width: 180px;
	height: 24px;
	font-size: 12px;
	line-height: 24px;
	color: black;
	background: url("/Content/Images/searchicon.png") center right 7px no-repeat white;
}

.strategySection .strategySearch input::-webkit-input-placeholder {
	font-style: italic;
}
.strategySection .strategySearch input::-moz-placeholder {
	font-style: italic;
}
.strategySection .strategySearch input::-ms-input-placeholder {
	font-style: italic;
}
.strategySection .strategySearch input:placeholder-shown {
	font-style: italic;
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
	font-size: 10px;
	line-height: 26px;
	height: 26px;
}

.strategySection .strategyListHeader .strategyListColumn {
	line-height: 10px;
	height: 22px;
	white-space: pre-wrap;
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

.strategySection .strategyListColumnCustom0 {
	width: 80px;
	padding: 0 5px;
	text-align: right;
	transition: width 0.5s;
}

.strategySection .filterStrategySplit.open .strategyListColumnCustom0 {
	width: 100px;
}

.strategySection .strategyListColumnCustom1 {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListColumnCustom2 {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListColumnCustom3 {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListColumnCustom4 {
	width: 100px;
	text-align: right;
}

.strategySection .strategyListBody .strategyListColumnStrategy {
	color: rgb(14,117,182);
}

.strategySection .strategyListHeader .strategyListColumnCustom0 {
	text-align: right;
}

.strategySection .strategyListColumnMinAmount {
	display: inline-block;
	min-width: 37px;
}

.strategySection .filterApproachControl {
	float: left;
	width: 100%;
}

.strategySection .approachColumn {
	float: left;
	width: 155px;
}

.strategySection .approachColumnTitle {
	padding: 3px 0 2px 0;
	font-weight: bold;
	font-size: 10px;
	line-height: 15px;
	white-space: nowrap;
	overflow: hidden;
	text-overflow: ellipsis;
	text-transform: uppercase;
}

.strategySection .approachCheckbox {
	display: block;
	margin: 3px 4px 3px 0;
	padding: 0 5px;
	font-size: 10px;
	line-height: 26px;
	height: 26px;
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
	margin: 0 10px 6px 0;
	padding: 4px 5px;
	width: 135px;
	font-size: 10px;
	line-height: 17px;
}

.strategySection .filterCheckboxControl input[type=checkbox] {
	margin: 2px 3px 0 0;
	position: absolute;
}

.strategySection .filterCheckboxLabel {
	display: inline-block;
	white-space: pre-wrap;
	vertical-align: top;
	padding-left: 17px;
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

.strategySection .sailing {
	background-color:rgb(177,214,234);
}

.strategySection .rowing {
	background-color:rgb(255,199,112);
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
				<div class="filterTitle">
					<span class="filterTitleText"></span><span class="filterTitleInfoIcon"><div class="filterTitleTooltip"></div></span>
				</div>
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
				<div class="approachColumns"></div>
				<div style="clear:both"></div>
			</div>
			<div class="approachColumn template">
				<div class="approachColumnTitle"></div>
			</div>
			<label class="approachCheckbox template"><input type="checkbox"> <span class="approachCheckboxLabel"></span></label>
			<label class="filterCheckboxControl template"><input type="checkbox"> <span class="filterCheckboxLabel"></span></label>
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
					<div class="strategyListColumn strategyListColumnCustom0"></div>
					<div class="strategyListColumn strategyListColumnCustom1"></div>
					<div class="strategyListColumn strategyListColumnCustom2"></div>
					<div class="strategyListColumn strategyListColumnCustom3"></div>
					<div class="strategyListColumn strategyListColumnCustom4"></div>
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
					<!--<div class="strategyListColumn strategyListColumnCustom0">$<span class="strategyListColumnMinAmount"></span></div>-->
					<div class="strategyListColumn strategyListColumnCustom0"></div>
					<div class="strategyListColumn strategyListColumnCustom1"></div>
					<div class="strategyListColumn strategyListColumnCustom2"></div>
					<div class="strategyListColumn strategyListColumnCustom3"></div>
					<div class="strategyListColumn strategyListColumnCustom4"></div>
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


function sliderControl() {

    var gray1 = "rgb(203,205,199)";
    var gray2 = "rgb(101,101,101)";
    var gray3 = "rgb(180,180,180)";
    var green = "rgb(0,124,56)";
    var white = "white";

    var minPos = 118;
    var maxPos = 332;

    var controlWidth = 464;
    var controlHeight = 62;
    var baselineY = 32;
    var rulerStartY = 25;
    var rulerEndY = 25 + 17;
    var rulerTextY = 59;
    var marginX = 20;

    function createSvgElement(tagName, attributes) {
        var e = document.createElementNS("http://www.w3.org/2000/svg", tagName);
        if (tagName == "svg") {
            e.setAttributeNS("http://www.w3.org/2000/xmlns/", "xmlns:xlink", "http://www.w3.org/1999/xlink");
        }
        if (attributes != undefined) {
            for (var key in attributes) {
                e.setAttribute(key, attributes[key]);
            }
        }
        return e;
    }

    var svg = createSvgElement("svg", { width: controlWidth + "px", height: controlHeight + "px", viewBox: "0 0 " + controlWidth + " " + controlHeight });
    var feeBars = svg.appendChild(createSvgElement("g"));
    for (var i = 0; i < 30; i++) {
        var width = 12;
        var x = 25 + i * width;
        var value = Math.random() * 20;
        var feeBar = feeBars.appendChild(createSvgElement("rect", { x: x, y: baselineY - Math.max(value, 0), width: width + 1, height: Math.abs(value) + 1, fill: gray1 }));
    }
    var baseline = svg.appendChild(createSvgElement("rect", { x: 0, y: baselineY, width: controlWidth, height: 2, fill: gray1 }));
    var rangeBar = svg.appendChild(createSvgElement("rect", { x: minPos, y: baselineY, width: maxPos - minPos, height: 2, fill: green }));

    for (var i = 0; i < 6; i++) {
        var x = marginX + i * 80;
        var crossline = svg.appendChild(createSvgElement("rect", { x: x, y: rulerStartY, width: 1, height: rulerEndY - rulerStartY, fill: gray2 }));
        var text = svg.appendChild(createSvgElement("text", { x: x + 1, y: rulerTextY, "font-size": 10, fill: "black", "text-anchor": "middle", style: "cursor: default" }));
        text.appendChild(document.createTextNode((i * 0.25).toFixed(2) + "%"));
    }

    var minCircle = svg.appendChild(createSvgElement("circle", { cx: minPos, cy: baselineY + 1, r: 10, fill: white, stroke: gray3, "stroke-width": 1 }));
    var maxCircle = svg.appendChild(createSvgElement("circle", { cx: maxPos, cy: baselineY + 1, r: 10, fill: white, stroke: gray3, "stroke-width": 1 }));

    // Block cursor selection if user misclicks
    svg.addEventListener("mousedown", function (e) { e.preventDefault(); });
    svg.addEventListener("mouseup", function (e) { e.preventDefault(); });

    function handleCircleInput(circle, moveBeginHandler, moveHandler) {
        var startValue = 0;
        var delta = 0;

        circle.addEventListener("mousedown", function (e) {
            function mouseMove(e) {
                e.stopPropagation();

                delta += e.movementX;
                moveHandler(startValue + delta);
            }

            function mouseUp(e) {
                e.stopPropagation();
                document.body.removeEventListener("mousemove", mouseMove, true);
                document.body.removeEventListener("mouseup", mouseUp, true);
            }

            e.stopPropagation();
            e.preventDefault();
            document.body.addEventListener("mousemove", mouseMove, true);
            document.body.addEventListener("mouseup", mouseUp, true);

            delta = 0;
            startValue = moveBeginHandler();
        });
    }

    function updateRange() {
        minCircle.setAttribute("cx", minPos);
        maxCircle.setAttribute("cx", maxPos);
        rangeBar.setAttribute("x", minPos);
        rangeBar.setAttribute("width", maxPos - minPos);
    }

    handleCircleInput(minCircle, function () {
        return minPos;
    }, function (pos) {
        minPos = Math.max(Math.min(pos, maxPos - 20), marginX);
        updateRange();
    });

    handleCircleInput(maxCircle, function () {
        return maxPos;
    }, function (pos) {
        maxPos = Math.min(Math.max(pos, minPos + 20), marginX + 5 * 80);
        updateRange();
    });

    return {
        element: svg
    };
}
/*
var simulateFavs = [{ ModelSetTypeId: "CallanTypeId", StrategistCode: "GuideMarkCode" }];
function simulateAjax(args) {
	if (args.url == "/api/v1/FavoriteInvestment") {
		setTimeout(function() {
			args.success(simulateFavs);
		}, 150); // simulate 150 ms ping to server
	}
}
*/
function loadSavedFavorites() {
	function loadSuccess(savedFavorites) {
		// Convert list to dictionary for faster lookup:
		var dictionary = {};
		savedFavorites.forEach(function (favorite) {
			dictionary[favorite.ModelSetTypeId + "::" + favorite.StrategistCode] = true;
		});

		// Update favorite status on strategies:
		ServerData.strategies.forEach(function (strategy) {
			var key = strategy.modelSetTypeId + "::" + strategy.strategistCode;
			strategy.favorite = dictionary[key] ? true : false;
		});

		updateStrategyList();
	}

//	simulateAjax({
	$.ajax({
		url: "/api/v1/FavoriteInvestment",
		dataType: "json",
		success: loadSuccess
	});
}

function addSavedFavorite(strategy) {
	$.ajax({
		url: "/api/v1/FavoriteInvestment",
		type: "POST",
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({
			ModelSetTypeId: strategy.modelSetTypeId,
			StrategistCode: strategy.strategistCode,
			Title: strategy.name
		})
	});
}

function removeSavedFavorite(strategy) {
	$.ajax({
		url: "/api/v1/FavoriteInvestment/delete",
		type: "POST",
		dataType: "json",
		contentType: "application/json; charset=utf-8",
		data: JSON.stringify({
			ModelSetTypeId: strategy.modelSetTypeId,
			StrategistCode: strategy.strategistCode,
			Title: strategy.name
		})
	});
}

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

	updateStrategyList();
}

function updateStrategyHeader() {

	var index = 0;
	ServerData.header.forEach(function (item) {
		$(".strategyListHeader .strategyListColumnCustom" + index).text(item.name);
		index++;
	});

}

function updateStrategyList() {
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
		if (strategy.rowing !== undefined) {
			$(".strategyListColumnColor", item).addClass(strategy.rowing ? "rowing" : "sailing");
		}
		$(".strategyDetailLink", item).text(strategy.name);
		$(".strategyDetailLink", item).attr('href', ServerData.detailUrl + "?Document=" + strategy.id);

		var customIndex = 0;
		strategy.columns.forEach(function (customValue) {
			$(".strategyListColumnCustom" + customIndex, item).text(customValue);
			customIndex++;
		});

		//$(".strategyListColumnMinAmount", item).text(toStringWithThousandSep(strategy.min));
		//$(".strategyListColumnCustom2", item).text(strategy.fee.toFixed(2) + "%");
		//$(".strategyListColumnCustom4", item).text(strategy.investmentProvider);

		if (strategy.favorite) {
			$(".strategyFavoriteButton", item).addClass("selected");
		}

		$(".strategyFavoriteButton", item).on('click', function() {
			if (strategy.favorite) {
				strategy.favorite = false;
				$(this).removeClass("selected");
				removeSavedFavorite(strategy);
			}
			else {
				strategy.favorite = true;
				$(this).addClass("selected");
				addSavedFavorite(strategy);
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
			$(".filterTitleText", groupItem).text(filterGroup.name);
			if (filterGroup.tip && filterGroup.tip.length > 0) {
				$(".filterTitleTooltip", groupItem).html(filterGroup.tip);
			}
			else {
				$(".filterTitleInfoIcon", groupItem).hide();
			}

			filterGroup.controls.forEach(function (control) {
				var controlItem = null;
				switch (control.type) {
				case "Investment Approach Control":
					controlItem = cloneTemplate(".filterApproachControl.template");
					var approachGroupItems = $(".approachColumns", controlItem);
					control.groups.forEach(function (approachGroup) {
						var approachGroupItem = cloneTemplate(".approachColumn.template");
						$(".approachColumnTitle", approachGroupItem).text(approachGroup.name);
						approachGroup.approaches.forEach(function (approach) {
							var approachItem = cloneTemplate(".approachCheckbox.template");
							$(".approachCheckboxLabel", approachItem).text(approach.name);
							approachItem.addClass(approach.rowing ? "rowing" : "sailing");
							approachGroupItem.append(approachItem);

							var checkboxInput = $("input[type='checkbox']", approachItem);
							var checked = checkboxInput.get(0).checked;

							var activeFilter = {
								name: approach.name,
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
								updateStrategyList();
							});

							strategyFilters.push(function (strategy) {
								return !checked || strategy.allocationApproach == approach.name;
							});

						});
						approachGroupItems.append(approachGroupItem);
					});
					break;
				case "Risk Profile Control":
					controlItem = cloneTemplate(".filterRiskProfileControl.template");
					break;
				case "Fee Control":
					controlItem = sliderControl().element;
					break;
				case "Strategist List Control":
					controlItem = $("<div></div>");
					var strategists = {};
					ServerData.strategies.forEach(function (strategy) {
						if (strategy.strategist == undefined) return;
						strategists[strategy.strategist] = true;
					});
					Object.keys(strategists).forEach(function (strategist) {
						var strategistItem = cloneTemplate(".filterCheckboxControl.template");
						$(".filterCheckboxLabel", strategistItem).text(strategist);
						controlItem.append(strategistItem);

						var checkboxInput = $("input[type='checkbox']", strategistItem);
						var checked = checkboxInput.get(0).checked;

						var activeFilter = {
							name: strategist,
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
							updateStrategyList();
						});

						strategyFilters.push(function (strategy) {
							return !checked || strategy.strategist == strategist;
						});
					});
					break;
				case "Manager List Control":
					controlItem = $("<div></div>");
					var managers = {};
					ServerData.strategies.forEach(function (strategy) {
						if (strategy.manager == undefined) return;
						managers[strategy.manager] = true;
					});
					Object.keys(managers).forEach(function (manager) {
						var managerItem = cloneTemplate(".filterCheckboxControl.template");
						$(".filterCheckboxLabel", managerItem).text(manager);
						controlItem.append(managerItem);

						var checkboxInput = $("input[type='checkbox']", managerItem);
						var checked = checkboxInput.get(0).checked;

						var activeFilter = {
							name: manager,
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
							updateStrategyList();
						});

						strategyFilters.push(function (strategy) {
							return !checked || strategy.manager == manager;
						});
					});
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
						updateStrategyList();
					});

					strategyFilters.push(function (strategy) {
						return !checked || strategy.filters[control.field] == "1";
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
				$(this).parent().find('.filterRolloutGroupItems').slideToggle();
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
	updateStrategyList();
}

function clearActiveFilters() {
	$(".filterList").empty();
	activeFilters.forEach(function(filter) { filter.clear(); });
	activeFilters = [];
	updateStrategyList();
}

function toggleStrategyOpener() {
	$(".strategySection .filterStrategySplit").toggleClass("open");
}

function toggleSortOrder() {
	$(this).toggleClass("reverseOrder");
	sortAndUpdateStrategyList();
}

$("#strategyClearFilters").on('click', clearActiveFilters);
$("#strategySearchField").on('input', updateStrategyList);
$("#strategyOpener").on('click', toggleStrategyOpener);
$(".strategyListHeader .strategyListColumnStrategy").on('click', toggleSortOrder);

createFilterList();
updateStrategyHeader();
sortAndUpdateStrategyList();
loadSavedFavorites();

</script>