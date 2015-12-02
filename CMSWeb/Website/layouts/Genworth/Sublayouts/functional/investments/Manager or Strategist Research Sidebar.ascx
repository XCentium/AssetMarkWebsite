<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Services.Investments" %>
<%@ Import Namespace="Lucene.Net.Documents" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>


<script runat="server">
	//private bool bMoreResearch;
	private string sManagerOrStrategistParameter;
	private string sAllocationApproachParameter;
	
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
		Item oDocument;
		Item oOwner;
		Search oSearch;
		Item oTempDocument;
		string sAllocationApproach;

		// get the allocation approach in case there is one (GPS doesn't have one, for instance)
		sAllocationApproachParameter = !String.IsNullOrWhiteSpace(sAllocationApproach = Request.QueryString[Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach]) ?
			String.Format("&{0}={1}", Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach, sAllocationApproach) : String.Empty;

		//get the document
		if ((oDocument = ContextExtension.CurrentDatabase.GetItem((Request.QueryString["Document"] ?? string.Empty).Trim())) != null)
		{
			//get the manager or strategist associated with this item
			if ((oOwner = oDocument.Axes.GetAncestors().GetItemsOfTemplate(new string[] { "Manager", "Strategist" }).FirstOrDefault()) != null)
			{
				//assume no more research is needed
				//bMoreResearch = false;
				
				//depending on the owner or manager, we need to build out a research object
				if (oOwner.InstanceOfTemplate("Manager"))
				{
					//create the search
					oSearch = new Search(new ItemCache(), true);
					oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.ManagerId, new string[] { oOwner.ID.ToString() });
					sManagerOrStrategistParameter = string.Format("manager={0}", oOwner.GetText("Manager", "Code"));
					BindResearch(oSearch.ResultDocuments.Select(oTemp => new Result(oTemp)));
					h2Title.InnerText = "Manager Detail";
				}
				else if (oOwner.InstanceOfTemplate("Strategist"))
				{
					oSearch = new Search(new ItemCache(), true);
					oSearch.SetFilter(Genworth.SitecoreExt.Constants.Investments.Indexes.Fields.StrategistId, new string[] { oOwner.ID.ToString() });
					sManagerOrStrategistParameter = string.Format("strategist={0}", oOwner.GetText("Strategist", "Code"));
					BindResearch(oSearch.ResultDocuments.Select(oTemp => new Result(oTemp)));
					h2Title.InnerText = "Strategist Detail";
				}
				
			}
						
			if (oDocument.InstanceOfTemplate("Document Base"))
			{
				//bind the document
				BindPDF(oDocument);
			}
			else
			{
				if ((oTempDocument = oDocument.GetListItem("Documents", "Fact Sheet")) != null)
				{
					//bind the fact sheet
					BindPDF(oTempDocument);
				}
			}
		}
	}

	private void BindResearch(IEnumerable<Result> oResults)
	{
		SortedDictionary<string, List<Result>> oCategorizedResults;
		DateTime dDate;
		
		//create a list to hold categorized results
		oCategorizedResults = new SortedDictionary<string, List<Result>>();
		
		//get the distinct categories
		oResults.Select(oDocument => oDocument.Category).Distinct().ToList().ForEach(sCategory =>
		{
			//create a category in the categorized results list
			oCategorizedResults.Add(sCategory, new List<Result>());
		});

		//loop over the documents
		foreach (Result oResult in oResults.OrderByDescending(oResult => DateTime.TryParse(oResult.Date, out dDate) ? dDate.ToString("yyyyMMddTHHmmss") : string.Empty))
		{
			oCategorizedResults[oResult.Category].Add(oResult);
		}

		//select the documents by type
		rCategories.DataSource = oCategorizedResults;
		rCategories.ItemDataBound += new RepeaterItemEventHandler(rCategories_ItemDataBound);
		rCategories.DataBind();
	}

	private void rCategories_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		KeyValuePair<string, List<Result>> oCategoryResults;
		Literal lTitle;
		Repeater rDocuments;
		HyperLink hSeeAll;
		string sCategoryCleanCode;
		
		oCategoryResults = (KeyValuePair<string, List<Result>>)e.Item.DataItem;
		lTitle = (Literal)e.Item.FindControl("lTitle");
		rDocuments = (Repeater)e.Item.FindControl("rDocuments");
		hSeeAll = (HyperLink)e.Item.FindControl("hSeeAll");
		lTitle.Text = oCategoryResults.Key;
        sCategoryCleanCode = Genworth.SitecoreExt.Services.Investments.Filter.Option.CodeCleaner.Replace(oCategoryResults.Key, string.Empty).ToLower();

		hSeeAll.Visible = oCategoryResults.Value.Count > Genworth.SitecoreExt.Constants.Investments.MaxSideResearchItemsPerCategory;
		if (hSeeAll.Visible)
		{
			hSeeAll.NavigateUrl = String.Format("{0}?{1}&category={2}{3}",
													Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.GetURL(),
													sManagerOrStrategistParameter,
													sCategoryCleanCode,
													sAllocationApproachParameter ?? String.Empty);
			hSeeAll.Target = "_top";
		}
		
		rDocuments.DataSource = oCategoryResults.Value.Take(Genworth.SitecoreExt.Constants.Investments.MaxSideResearchItemsPerCategory);
		rDocuments.ItemDataBound += new RepeaterItemEventHandler(rDocuments_ItemDataBound);
		rDocuments.DataBind();
	}

	private void rDocuments_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Result oResult;
		HyperLink hDocument;
		HtmlGenericControl liItem;
		Item contentItem;

		liItem = (HtmlGenericControl)e.Item.FindControl("liItem");
		liItem.Visible = e.Item.ItemIndex < Genworth.SitecoreExt.Constants.Investments.MaxSideResearchItemsPerCategory;

		if (liItem.Visible)
		{
			oResult = (Result)e.Item.DataItem;
			hDocument = (HyperLink)e.Item.FindControl("hDocument");
			hDocument.Text = oResult.Title;
			string itemPath = string.IsNullOrWhiteSpace(oResult.sUrl) ? oResult.Path : oResult.sUrl;
			hDocument.Attributes.Add("href", itemPath);
			hDocument.CssClass = string.Format("{0}-icon", oResult.Extension);

			hDocument.Target = "_blank";
			
			contentItem = ContextExtension.CurrentDatabase.GetItem(ItemHelper.FormatId(oResult.Id));
			//Set omniture tag
			contentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hDocument);
		}
	}

	private void BindPDF(Item oDocument)
	{
		string sPath;

		if (oDocument != null)
		{
			//does the document have a file?
			if (!string.IsNullOrEmpty(sPath = oDocument.GetImageURL("Document", "File")))
			{
				lPDF.Text = string.Format("<object data='{0}' type='application/pdf' style='height: 545px; width: 750px;'> Your browser does not support PDF plugin. You can <a href='{0}'>click here to download the PDF file.</a></object>", sPath);
			}
		}
	}
</script>
<div id="divStrategistDetail" class="dialog-wrapper">
	<div class="dialog">
		<h2  runat="server" id ="h2Title">Detail</h2>
		<div class="dialog-content stretchy-dialog-content strategist-detail-dialog">
			<table width="100%" height="100%" cellpadding="0" cellspacing="0" border="0">
				<tr>
					<td width="*">
						<div class="stretchy withIframe">
							<asp:Literal ID="lPDF" runat="server" />
						</div>
					</td>
					<td width="200px">
						<div class="stretchy dialog-aside">
							<h3>Research <%--<asp:HyperLink ID="hMoreResearch" runat="server" Visible="false" Text="More..." />--%></h3>
							<hr />
							<asp:Repeater ID="rCategories" runat="server">
								<ItemTemplate>
									<h4><asp:Literal ID="lTitle" runat="server" /></h4>
									<asp:HyperLink ID="hSeeAll" runat="server" Visible="false">See All</asp:HyperLink>
									<ul class="document-list">
										<asp:Repeater ID="rDocuments" runat="server">
											<ItemTemplate>
												<li runat="server" id="liItem"><asp:HyperLink ID="hDocument" runat="server" /></li>
											</ItemTemplate>
										</asp:Repeater>
									</ul>
									<hr />
								</ItemTemplate>
							</asp:Repeater>
						</div>
					</td>
				</tr>
			</table>
		</div>
	</div>
</div>
<div class="clear"></div>
<script type="text/javascript" language="javascript">
    jQuery(function ($) {
        var fResize = function () {
            $('#divStrategistDetail').each(function () {
                var eWrapper = this;
                var jqWrapper = $(eWrapper);
                var jqH2 = $('h2', eWrapper);
                var jqContent = $('.stretchy-dialog-content', eWrapper);
                if (jqContent.size() > 0) {
                    var iMaxHeight = jqWrapper.outerHeight();
                    var iH2Height = jqH2.outerHeight();
                    var iDeltaHeight = parseInt(jqContent.css('paddingTop').toString().replace('px', '')) + parseInt(jqContent.css('paddingBottom').toString().replace('px', ''));
                    var iContentHeight = iMaxHeight - iH2Height - iDeltaHeight;
                    jqContent.css('height', iContentHeight);
                    $('.stretchy', eWrapper).each(function () {
                        var e = this;
                        var eParent = this.parentNode;
                        var jq = $(this);
                        var jqParent = $(eParent);
                        var iHeight = jqParent.innerHeight();
                        var iWidth = jqParent.innerWidth();
                        jq.css({ height: iHeight, width: iWidth });
                    });
                }
            });
        };

        $(window).resize(fResize);

        fResize();
    });
</script>
