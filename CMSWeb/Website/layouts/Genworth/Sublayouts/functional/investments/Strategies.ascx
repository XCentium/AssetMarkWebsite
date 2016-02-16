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

	class StrategyColumnField
	{
		public string FieldSection { get; set; }
		public string FieldName { get; set; }
	}

	class StrategyColumn
	{
		public StrategyColumn() { Fields = new List<StrategyColumnField>(); }
		public string Name { get; set; }
		public string FieldType { get; set; }
		public List<StrategyColumnField> Fields { get; set; }
	}

	class FilterField
	{
		public string FieldSection { get; set; }
		public string FieldName { get; set; }
		public string VariableName { get { return FieldSection + "/" + FieldName; } }
		public string OmnitureEvent { get; set; }
	}

	private List<FilterField> filterFields;

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

						var field = new FilterField { FieldSection = target.Parent.Name, FieldName = target.Name, OmnitureEvent = controlItem["Omniture Event"] };
						filterFields.Add(field);

						control.name = controlItem["Name"];
						control.field = field.VariableName;
						control.omnitureEvent = field.OmnitureEvent;
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
								approach.name = approachItem["Short Title"];
								approach.rowing = approachItem["Rowing"] == "1";
								approach.omnitureEvent = approachItem["Omniture Event"];
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

	private List<StrategyColumn> BuildColumns(Item item)
	{
		var list = new List<StrategyColumn>();

		foreach (Item column in item.Children)
		{
			string name = column["Name"];

			InternalLinkField typeLink = column.GetField("Visual Appearance");
			if (typeLink.TargetItem == null)
				continue;
			string fieldType = typeLink.TargetItem.Name;

			var col = new StrategyColumn { Name = name, FieldType = fieldType };

			MultilistField fields = column.GetField("Field");
			foreach (var target in fields.GetItems())
			{
				col.Fields.Add(new StrategyColumnField { FieldSection = target.Parent.Name, FieldName = target.Name });
			}

			list.Add(col);
		}

		return list;
	}

	private JArray BuildColumnHeader(List<StrategyColumn> columns)
	{
		var list = new JArray();
		foreach (var field in columns)
		{
			dynamic headerItem = new JObject();
			headerItem.name = field.Name;
			headerItem.type = field.FieldType;
			list.Add(headerItem);
		}
		return list;
	}

	private JObject BuildStrategy(Item item, List<StrategyColumn> columns, bool manager)
	{
		dynamic strategy = new JObject();
		strategy.id = item.ID.ToString();
		strategy.name = String.IsNullOrWhiteSpace(item["Strategy Title"]) ? item.DisplayName : item["Strategy Title"];
		strategy.modelSetTypeId = item["ModelSetTypeId"];
		strategy.strategistCode = item["StrategistCode"];
		try
		{
			strategy.fee = Convert.ToDouble(item["Platform Fee"]);
		}
		catch
		{
			strategy.fee = null;
		}
		strategy.columns = new JArray();
		strategy.filters = new JObject();

		foreach (var field in columns)
		{
			if (field.FieldType == "Investment Provider")
			{
				strategy.columns.Add(manager ? "Manager" : "Strategist");
			}
			else
			{
				strategy.columns.Add(String.Join(" ", field.Fields.Select(f => {
					var v = item.GetField(f.FieldSection, f.FieldName);
					if (v.Type == "Checkbox") return v.Value == "1" ? f.FieldName : "";
					else return v.Value;
				}).ToArray()));
			}
		}

		foreach (var field in filterFields)
		{
			strategy.filters[field.VariableName] = item.GetField(field.FieldSection, field.FieldName).Value;
		}

		return strategy;
	}

	private void AddStrategists(JArray strategies, Item strategistsItem, List<StrategyColumn> columns)
	{
		foreach (Item strategistItem in strategistsItem.Children)
		{
			if (strategistItem.TemplateName != "Strategist")
				continue;

			string strategistName = strategistItem["Name"];
			string strategistEvent = strategistItem["Omniture Event"];

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
					strategy.strategistEvent = strategistEvent;
					strategy.allocationApproach = allocationApproachItem["Short Title"];
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

	private void AddManagers(JArray strategies, Item managersItem, List<StrategyColumn> columns)
	{
		foreach (Item managerItem in managersItem.Children)
		{
			if (managerItem.TemplateName != "Manager")
				continue;

			string managerName = managerItem["Name"];
			string managerEvent = managerItem["Omniture Event"];
			
			foreach (Item solutionItem in managerItem.Children)
			{
				if (!IsDerivedFromTemplateName(solutionItem.Template, "Manager Strategy"))
					continue;

				InternalLinkField fieldLink = solutionItem.GetField("Allocation Approach");
				var allocationApproachItem = fieldLink.TargetItem;
				if (allocationApproachItem == null || allocationApproachItem["Display on Strategies Page"] != "1")
					continue;

				dynamic strategy = BuildStrategy(solutionItem, columns, true);
				strategy.manager = managerName;
				strategy.managerEvent = managerEvent;
				strategy.allocationApproach = allocationApproachItem["Short Title"];
				strategy.rowing = allocationApproachItem["Rowing"] == "1";
				strategies.Add(strategy);
			}
		}
	}

	private string StrategyData()
	{
		var currentItem = ContextExtension.CurrentItem;
		InternalLinkField strategistsField = currentItem.GetField("Strategists");
		InternalLinkField managersField = currentItem.GetField("Managers");
		InternalLinkField approachesField = currentItem.GetField("Allocation Approaches");
		var strategistsItem = strategistsField.TargetItem;
		var managersItem = managersField.TargetItem;

		var detailItem = ContextExtension.CurrentItem.Axes.GetChild("Strategy");
		HyperLink olink = new HyperLink();
		detailItem.ConfigureHyperlink(olink);
		var detailUrl = olink.NavigateUrl;

		filterFields = new List<FilterField>();
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
<div class="strategySection">
	<div class="limitations"><asp:HyperLink ID="hProductLink" runat="server" Visible="false"></asp:HyperLink></div>
	<div class="filterToolbar">
		<div class="filterToolbarButton" id="strategyClearFilters">Clear Filters</div>
		<div class="filterList"></div>
		<div style="clear:both"></div>
		<!--<div class="filterTip">Search for investments using the filters below</div>-->
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
					<svg class="filterRolloutIcon" width="16px" height="8px" viewBox="-8 -4 16 8">
						<g transform="rotate(90)"><path d="M 0 -2 L 4 2 L -4 2 z" fill="rgb(1,125,187)" /></g>
					</svg>
					<span class="filterRolloutTitleText"></span>
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
					<div class="strategyListColumn strategyListColumnStrategy" data-field="strategy" data-omniture="Strategy"><br/>Strategy</div>
					<div class="strategyListColumn strategyListColumnFavorite" data-field="favorite" data-omniture="Favorites"><br/>Favorites</div>
					<div class="strategyListColumn strategyListColumnCustom0" data-field="custom" data-index="0"></div>
					<div class="strategyListColumn strategyListColumnCustom1" data-field="custom" data-index="1"></div>
					<div class="strategyListColumn strategyListColumnCustom2" data-field="custom" data-index="2"></div>
					<div class="strategyListColumn strategyListColumnCustom3" data-field="custom" data-index="3"></div>
					<div class="strategyListColumn strategyListColumnCustom4" data-field="custom" data-index="4"></div>
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
StrategyData = <%= StrategyData() %>;
</script>
