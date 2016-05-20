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
                sManagerOrStrategistParameter = string.Format("manager={0}", owner.GetText("Manager", "Code"));

                var oSearch = new Search(new ItemCache(), true);
                oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.ManagerId, new string[] { owner.ID.ToString() });
                var documents = oSearch.ResultDocuments.Select(oTemp => new Result(oTemp));
                IEnumerable<Result> approachDocuments = null;
                
                InternalLinkField fieldLink = StrategyItem.GetField("Allocation Approach");
                var allocationApproachItem = fieldLink.TargetItem;
                if (allocationApproachItem != null)
                {
                    var oApproachSearch = new Search(new ItemCache(), true);
                    oApproachSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.AllocationApproachId, new string[] { allocationApproachItem.ID.ToString() });
                    approachDocuments = oApproachSearch.ResultDocuments.Select(oTemp => new Result(oTemp));
                }

                BindResearch(documents, approachDocuments);
            }
            else if (owner.InstanceOfTemplate("Strategist"))
            {
                sManagerOrStrategistParameter = string.Format("strategist={0}", owner.GetText("Strategist", "Code"));

                var oSearch = new Search(new ItemCache(), true);
                oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.StrategistId, new string[] { owner.ID.ToString() });
                var documents = oSearch.ResultDocuments.Select(oTemp => new Result(oTemp));
                IEnumerable<Result> approachDocuments = null;

                var strategyGroup = StrategyItem.Axes.GetAncestors().GetItemsOfTemplate(new string[] { "Strategy" }).FirstOrDefault();
                if (strategyGroup != null)
                {
                    InternalLinkField fieldLink = strategyGroup.GetField("Allocation Approach");
                    var allocationApproachItem = fieldLink.TargetItem;
                    if (allocationApproachItem != null)
                    {
                        var oApproachSearch = new Search(new ItemCache(), true);
                        oApproachSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.AllocationApproachId, new string[] { allocationApproachItem.ID.ToString() });
                        approachDocuments = oApproachSearch.ResultDocuments.Select(oTemp => new Result(oTemp));
                    }
                }

                BindResearch(documents, approachDocuments);
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
            double value;
            if (Double.TryParse(lRowValue.Text, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out value))
                lRowValue.Text = String.Format("${0:n0}", value);
            else
                lRowValue.Text = String.Format("${0}", lRowValue.Text);
        }
        else if (item["Visual Appearance"] == "Percentage")
        {
            double value;
            if (Double.TryParse(lRowValue.Text, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, System.Globalization.CultureInfo.InvariantCulture, out value))
                lRowValue.Text = String.Format("{0:n2}%", value);
            else
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

    private void BindResearch(IEnumerable<Result> documents, IEnumerable<Result> approachDocuments)
    {
        var items = new List<SidebarItem>();

        foreach (Item categoryItem in ContextExtension.CurrentItem.Axes.GetChild("Sidebar").Children)
        {
            var documentField = ((InternalLinkField)categoryItem.GetField("Document Field")).TargetItem;
            if (documentField == null)
                continue;

            var documentLink = (InternalLinkField)StrategyItem.GetField(documentField.Parent.Name, documentField.Name);
            var documentItem = documentLink.TargetItem;
            if (documentItem == null)
                continue;
            
            var categoryName = categoryItem["Name"];
            var categoryValue = categoryItem["Document Field"];
            var categoryOmnitureEvent = categoryItem["Omniture Event"];
            string categoryPageNumber = null;

            InternalLinkField fieldLink = categoryItem.GetField("Start Page Field");
            var target = fieldLink.TargetItem;
            if (target != null)
                categoryPageNumber = StrategyItem.GetField(target.Parent.Name, target.Name).Value;

            var url = documentItem.GetImageURL("Document", "File");
            var externalUrl = documentItem["URL"];
            var vimeoCode = documentItem["Vimeo Code"];
            if (!String.IsNullOrWhiteSpace(url))
            {
                if (!String.IsNullOrWhiteSpace(categoryPageNumber))
                    url = String.Format("{0}#page={1}", url, categoryPageNumber);

                items.Add(new SidebarItem { Name = categoryName, Path = url, Ext = "pdf", OmnitureEvent = categoryOmnitureEvent });
            }
            else if (!String.IsNullOrWhiteSpace(externalUrl))
            {
                items.Add(new SidebarItem { Name = categoryName, Path = externalUrl, Ext = "", OmnitureEvent = categoryOmnitureEvent });
            }
            else if (!String.IsNullOrWhiteSpace(vimeoCode))
            {
                items.Add(new SidebarItem { Name = categoryName, Path = String.Format("https://player.vimeo.com/video/{0}", vimeoCode), Ext = "embed", OmnitureEvent = categoryOmnitureEvent });
            }
                        
/*
            if (categoryValue == "Performance" && hidePerformance)
                continue;
            
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
                        if (!String.IsNullOrWhiteSpace(categoryPageNumber))
                            url = String.Format("{0}#page={1}", url, categoryPageNumber);

                        items.Add(new SidebarItem { Name = categoryName, Path = url, Ext = "pdf", OmnitureEvent = categoryOmnitureEvent });
                    }
                }
            }
            else
            {
                DateTime dDate;
                var categoryDocs = documents.Where(doc => doc.Category == categoryValue);
                var categoryDoc = categoryDocs.OrderByDescending(oResult => DateTime.TryParse(oResult.Date, out dDate) ? dDate.ToString("yyyyMMddTHHmmss") : string.Empty).FirstOrDefault();
                if (categoryDoc == null && approachDocuments != null)
                {
                    categoryDocs = approachDocuments.Where(doc => doc.Category == categoryValue);
                    categoryDoc = categoryDocs.OrderByDescending(oResult => DateTime.TryParse(oResult.Date, out dDate) ? dDate.ToString("yyyyMMddTHHmmss") : string.Empty).FirstOrDefault();
                }
                if (categoryDoc != null)
                {
                    var url = string.IsNullOrWhiteSpace(categoryDoc.sUrl) ? categoryDoc.Path : categoryDoc.sUrl;
                    if (!String.IsNullOrWhiteSpace(url) && !String.IsNullOrWhiteSpace(categoryPageNumber))
                        url = String.Format("{0}#page={1}", url, categoryPageNumber);

                    items.Add(new SidebarItem { Name = categoryName, Path = url, Ext = categoryDoc.Extension, OmnitureEvent = categoryOmnitureEvent });
                }
            }
*/
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

            lPDF.Text = string.Format("<iframe src='/CMSContent/pdf.js/web/viewer.html?file={0}'></iframe>", item.Path);
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
        <div class="detailHeaderDescription">
            <asp:Literal ID="lHeaderDescription" runat="server" /></div>
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
                    <div class="sidebarRow" id="dCategory" runat="server">
                        <span class="sidebarRowTitle"><asp:Literal ID="sCategory" runat="server" /></span>
                        <div class="sidebarRowArrow"></div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <div class="sidebarRow" id="dResearch" runat="server">
                <span class="sidebarRowTitle"><asp:Literal ID="sResearch" runat="server" /></span>
                <div class="sidebarRowArrow"></div>
            </div>
        </div>
        <div class="detailDocument" data-viewer-url-prefix='/CMSContent/pdf.js/web/viewer.html?file='>
            <asp:Literal ID="lPDF" runat="server" /></div>
        <div style="clear: both"></div>
    </div>
</div>
<script language="javascript" type="text/javascript">
StrategyDetailData = <%= StrategyDetailData() %>;
</script>
