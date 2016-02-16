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

	Item StrategyItem = null;

	private void BindData()
	{
		// get the allocation approach in case there is one (GPS doesn't have one, for instance)
		//string sAllocationApproach;
		//var sAllocationApproachParameter = !String.IsNullOrWhiteSpace(sAllocationApproach = Request.QueryString[Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach]) ?
		//	String.Format("&{0}={1}", Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach, sAllocationApproach) : String.Empty;

		// Get the document
		StrategyItem = ContextExtension.CurrentDatabase.GetItem((Request.QueryString["Document"] ?? string.Empty).Trim());
		if (StrategyItem == null)
			return;

		lHeaderTitle.Text = Server.HtmlEncode(!String.IsNullOrWhiteSpace(StrategyItem["Strategy Title"]) ? StrategyItem["Strategy Title"] : StrategyItem.DisplayName);
		lHeaderSubTitle.Text = Server.HtmlEncode(StrategyItem["Detail Title"]);
		lHeaderDescription.Text = StrategyItem["Detail Description"];

		// Get the manager or strategist associated with this item
		string sManagerOrStrategistParameter = "";
		var owner = StrategyItem.Axes.GetAncestors().GetItemsOfTemplate(new string[] { "Manager", "Strategist" }).FirstOrDefault();
		if (owner != null)
		{
			//depending on the owner or manager, we need to build out a research object
			if (owner.InstanceOfTemplate("Manager"))
			{
				var oSearch = new Search(new ItemCache(), true);
				oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.ManagerId, new string[] { owner.ID.ToString() });
				sManagerOrStrategistParameter = string.Format("manager={0}", owner.GetText("Manager", "Code"));
				BindResearch(oSearch.ResultDocuments.Select(oTemp => new Result(oTemp)));
			}
			else if (owner.InstanceOfTemplate("Strategist"))
			{
				var oSearch = new Search(new ItemCache(), true);
				oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.StrategistId, new string[] { owner.ID.ToString() });
				sManagerOrStrategistParameter = string.Format("strategist={0}", owner.GetText("Strategist", "Code"));
				BindResearch(oSearch.ResultDocuments.Select(oTemp => new Result(oTemp)));
			}
		}

		sResearch.Text = "Go to Archived Research";
		dResearch.Attributes.Add("data-url", String.Format("{0}?{1}", Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.GetURL(), sManagerOrStrategistParameter));
        dResearch.Attributes.Add("data-omniture-event", "GoToArchive");

		BindTable();
	}

	private void BindTable()
	{
		rTable.DataSource = ContextExtension.CurrentItem.Axes.GetChild("Header").Children;
		rTable.ItemDataBound += new RepeaterItemEventHandler(rTable_ItemDataBound);
		rTable.DataBind();
	}

	private void rTable_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var item = (Item)e.Item.DataItem;
		var rTableRow = (Repeater)e.Item.FindControl("rTableRow");

		rTableRow.DataSource = item.Children;
		rTableRow.ItemDataBound += new RepeaterItemEventHandler(rTableRow_ItemDataBound);
		rTableRow.DataBind();
	}

	private void rTableRow_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var item = (Item)e.Item.DataItem;
		var lRowName = (Literal)e.Item.FindControl("lRowName");
		var lRowValue = (Literal)e.Item.FindControl("lRowValue");
		MultilistField fields = item.GetField("Fields");

		lRowName.Text = item["Name"];

		foreach (Item f in fields.GetItems())
		{
			var v = StrategyItem.GetField(f.Parent.Name, f.Name);
			if (v == null) continue;

			string text = "";
			if (v.Type == "Checkbox") text = v.Value == "1" ? f.Name : "";
			else text = v.Value;

			if (text.Length > 0)
			{
				if (lRowValue.Text.Length > 0) lRowValue.Text += "<br/>";
				lRowValue.Text += Server.HtmlEncode(text);
			}
		}

		if (item["Visual Appearance"] == "USD")
		{
			lRowValue.Text = String.Format("${0}", lRowValue.Text);
		}
		else if (item["Visual Appearance"] == "Percentage")
		{
			lRowValue.Text = String.Format("{0}%", lRowValue.Text);
		}
		else if (item["Visual Appearance"] == "Yes No")
		{
			lRowValue.Text = !String.IsNullOrWhiteSpace(lRowValue.Text) ? "Yes" : "No";
		}
	}

	class SidebarItem
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public string Ext { get; set; }
        public string OmnitureEvent { get; set; }
	}

	private void BindResearch(IEnumerable<Result> documents)
	{
		var items = new List<SidebarItem>();

		foreach (Item categoryItem in ContextExtension.CurrentItem.Axes.GetChild("Sidebar").Children)
		{
			var categoryName = categoryItem["Name"];
			var categoryValue = categoryItem["Category"];
            var categoryOmnitureEvent = categoryItem["Omniture Event"];

			if (categoryValue == "Fact Sheets")
			{
				Item factSheet = StrategyItem.GetListItem("Documents", "Fact Sheet");
				if (factSheet == null)
				{
					factSheet = StrategyItem.GetListItem("Documents", "Profile Sheet");
					if (factSheet == null && StrategyItem.InstanceOfTemplate("Document Base"))
					{
						factSheet = StrategyItem;
					}
				}

				if (factSheet != null)
				{
					var url = factSheet.GetImageURL("Document", "File");
					if (!String.IsNullOrWhiteSpace(url))
					{
						items.Add(new SidebarItem { Name = categoryName, Path = factSheet.GetImageURL("Document", "File"), Ext = "pdf", OmnitureEvent = categoryOmnitureEvent });
					}
				}
			}
			else
			{
				DateTime dDate;
				var categoryDocs = documents.Where(doc => doc.Category == categoryValue);
				var categoryDoc = categoryDocs.OrderByDescending(oResult => DateTime.TryParse(oResult.Date, out dDate) ? dDate.ToString("yyyyMMddTHHmmss") : string.Empty).FirstOrDefault();
				if (categoryDoc != null)
                    items.Add(new SidebarItem { Name = categoryName, Path = string.IsNullOrWhiteSpace(categoryDoc.sUrl) ? categoryDoc.Path : categoryDoc.sUrl, Ext = categoryDoc.Extension, OmnitureEvent = categoryOmnitureEvent });
			}
		}

		rCategories.DataSource = items;
		rCategories.ItemDataBound += new RepeaterItemEventHandler(rCategories_ItemDataBound);
		rCategories.DataBind();
	}

	private void rCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		var item = (SidebarItem)e.Item.DataItem;

		var dCategory = (HtmlGenericControl)e.Item.FindControl("dCategory");
        dCategory.Attributes.Add("data-url", item.Path);
        dCategory.Attributes.Add("data-extension", item.Ext);
        dCategory.Attributes.Add("data-omniture-event", item.OmnitureEvent);
		if (e.Item.ItemIndex == 0)
		{
			dCategory.Attributes["class"] = dCategory.Attributes["class"] + " selected";

            lPDF.Text = string.Format("<iframe data-viewer-url-prefix='/CMSContent/pdf.js/web/viewer.html?file=' src='/CMSContent/pdf.js/web/viewer.html?file={0}'></iframe>", item.Path);
		}

		var sCategory = (Literal)e.Item.FindControl("sCategory");
        sCategory.Text = Server.HtmlEncode(item.Name);

		//Set omniture tag
		//var contentItem = ContextExtension.CurrentDatabase.GetItem(ItemHelper.FormatId(item.Item2.Id));
		//contentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, dCategory);
	}

	private string StrategyDetailData()
	{
		dynamic StrategyDetailData = new JObject();
		StrategyDetailData.modelSetTypeId = StrategyItem["ModelSetTypeId"];
		StrategyDetailData.strategistCode = StrategyItem["StrategistCode"];
		StrategyDetailData.title = !String.IsNullOrWhiteSpace(StrategyItem["Strategy Title"]) ? StrategyItem["Strategy Title"] : StrategyItem.DisplayName;
		return ((JObject)StrategyDetailData).ToString();
	}

</script>
<div class="strategyDetail">
	<div class="detailHeader">
		<div class="detailHeaderBack" onclick="javascript:window.history.back()">&lt; Back</div>
		<div class="detailHeaderTitleArea">
			<div class="detailHeaderTitle">
				<div class="detailHeaderLine1"><asp:Literal ID="lHeaderTitle" runat="server" /></div>
				<div class="detailHeaderLine2"><asp:Literal ID="lHeaderSubTitle" runat="server" /></div>
			</div>
		</div>
		<div class="detailHeaderDescription"><asp:Literal ID="lHeaderDescription" runat="server" /></div>
		<div class="detailHeaderTableArea">
			<asp:Repeater ID="rTable" runat="server">
				<ItemTemplate>
					<table>
						<asp:Repeater ID="rTableRow" runat="server">
							<ItemTemplate>
								<tr>
									<td><asp:Literal ID="lRowName" runat="server" />:</td>
									<td><asp:Literal ID="lRowValue" runat="server" /></td>
								</tr>
							</ItemTemplate>
						</asp:Repeater>
					</table>
				</ItemTemplate>
			</asp:Repeater>
		</div>
		<div style="clear: both"></div>
		<div class="detailHeaderLinks">
			<div class="strategyFavoriteButton">
				<span class="strategySave">Save</span>
				<span class="strategySaved">Saved</span>
			</div>
			<div class="downloadLink"></div>
			<div class="printLink"></div>
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
StrategyDetailData = <%= StrategyDetailData() %>;
</script>
