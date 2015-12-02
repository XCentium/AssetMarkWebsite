<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<script runat="server">
	
	Paginator<Item> oPaginator;

	
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
				BindData();
		}
	}

    private string _sCategory;
    private string sCategory
    {
        get
        {
            if (_sCategory == null)
            {
                _sCategory = ContextExtension.CurrentItem.GetText("Search", "Category");
            }
            return _sCategory;
        }
    }

	
	private void BindData()
	{
		IEnumerable<Item> oArticles;
        List<string> oNewArticles;
		GenworthQueryString oQueryString;
		GenworthQueryString oQueryStringDateHeader;
		string sSearchKeywords;

		// wrap the current query string
		//
		oQueryString = GenworthQueryString.Current;

		if (!string.IsNullOrEmpty(sSearchKeywords = oQueryString.ArticleKeyword))
		{
			// bind the textbox and initiate the search
			//
			tbSearchNewsArchives.Text = sSearchKeywords;
			oArticles = NewsArchiveHelper.SearchNewsArchive(sSearchKeywords, sCategory);
		}
		else
		{
			// get article from page items and children nodes, then sort by date
			//
			oArticles = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items")
				.Where(oItem => oItem.InstanceOfTemplate("Article"));
			oArticles = oArticles.Concat(ContextExtension.CurrentItem.GetChildrenOfTemplate("Article"));
		}
        
        // removed the articles shown in the home page
        oNewArticles = ItemExtension.RootItem.GetMultilistItems("Page", "Items")
                .Where(oItem => oItem.InstanceOfTemplate("Article")).Select(oNewArticle => oNewArticle.ID.ToString()).ToList();

        oArticles = oArticles.Where(oArticle => !oNewArticles.Contains(oArticle.ID.ToString()));
        
		// create a query string (based on the current query) for the date column hyperlink
		//
		oQueryStringDateHeader = new GenworthQueryString(oQueryString);
		oQueryStringDateHeader.RemovePagination();

		// bind just this page's articles, observing the sort (descending is default)
		//
		if (oQueryString.ArticleSortAscending)
		{
			oArticles = oArticles.OrderBy(oItem => oItem.GetField("Timing", "Date").GetDate());

			oQueryStringDateHeader.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Sort,
				Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.DateDescending, true);
		}
		else
		{
			oArticles = oArticles.OrderByDescending(oItem => oItem.GetField("Timing", "Date").GetDate());

			oQueryStringDateHeader.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Sort,
				Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.DateAscending, true);
		}

		// bind the "date posted" header
		//
		hDatePosted.NavigateUrl = ContextExtension.CurrentItem.GetURL() + oQueryStringDateHeader.ToString();
		liDateHeader.Attributes["class"] = string.Format("article-date sortable {0}",
			GenworthQueryString.Current.ArticleSortAscending ? "ascending" : "descending");		
	
		if (oArticles != null)
		{
			BindArticles(oArticles);
		}
		else
		{
			pArticleList.Visible = false;
		}
		if (rItems.Items.Count == 0)
			hNoResult.Visible = true;
			
		ulListTitles.Visible = !hNoResult.Visible;
		
		// Set Client References
		tbSearchNewsArchives.Attributes.Add("target", lbSearchNewsArchives.ClientID);
		
	}


	private void BindArticles(IEnumerable<Item> oArticles)
	{
		oPaginator = GenworthPaginator.GetPaginator(oArticles,"ArticleList");

		rItems.DataSource = oPaginator.GetDataset();
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();

		// get our page numbers and bind them to the pages list
		//
		rPages.DataSource = oPaginator.GetPageNumbers();
		rPages.ItemDataBound += new RepeaterItemEventHandler(rPages_ItemDataBound);
		rPages.DataBind();
		rPages.Visible = rPages.Items.Count > 1;
		
		// bing the next/prev links
		//
		oPaginator.ConfigureHyperLinkNext(lNext, ContextExtension.CurrentItem.GetURL());
		oPaginator.ConfigureHyperLinkPrevious(lPrevious, ContextExtension.CurrentItem.GetURL());
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oArticle;
		Item oImage;
		HyperLink lArticleThumb;
		Image iArticleImage;
        PlaceHolder pArticleImage;
        HyperLink lArticleTitle;
		Label lArticleDate;
        PlaceHolder phSummary;
        Literal lArticleSummary;
		

		// get the article
		//
		oArticle = (Item)e.Item.DataItem;
		if (oArticle != null)
		{
			// get the controls
			//
			lArticleThumb = (HyperLink)e.Item.FindControl("lArticleThumb");
			iArticleImage = (Image)e.Item.FindControl("iArticleImage");
			pArticleImage = (PlaceHolder)e.Item.FindControl("pArticleImage");
			lArticleTitle = (HyperLink)e.Item.FindControl("lArticleTitle");
			lArticleSummary = (Literal)e.Item.FindControl("lArticleSummary");
			lArticleDate = (Label)e.Item.FindControl("lArticleDate");
            phSummary = (PlaceHolder)e.Item.FindControl("phSummary");

			// bind 'em
			//
			oArticle.ConfigureHyperlink(lArticleTitle);
			oArticle.ConfigureHyperlink(lArticleThumb);
			lArticleTitle.Text = oArticle.GetText("Page", "Title");
			lArticleSummary.Text = oArticle.GetText("Page", "Summary");
            
            if (String.IsNullOrEmpty(lArticleSummary.Text.Trim()))
            {
                phSummary.Visible = false;
            }
            
			lArticleDate.Text = oArticle.GetField("Timing", "Date").GetDateString("MM/dd/yyyy", string.Empty);

            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, lArticleTitle);
            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, lArticleThumb);
						
			// bind the image... or not
			//
			if ((oImage = oArticle.GetField("Page", "Photo").GetImageItem()) != null)
			{
				iArticleImage.ImageUrl = "~/" + oImage.GetMediaURL() + "?mw=200&mh=100";
                pArticleImage.Visible = true;
			}
		}
	}

	private void rPages_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HyperLink hPage;
		Label lSelectedPage;
		int iPageNumber;
		string sPageNumber;

		iPageNumber = (int)e.Item.DataItem;
		sPageNumber = iPageNumber.ToString();

		hPage = (HyperLink)e.Item.FindControl("hPage");
		lSelectedPage = (Label)e.Item.FindControl("lSelectedPage");


		if (oPaginator.PageNumber != iPageNumber)
		{
			oPaginator.ConfigureHyperLinkPage(hPage, ContextExtension.CurrentItem.GetURL(), iPageNumber);
			hPage.Text = sPageNumber;
		}
		else
		{
			lSelectedPage.Text = sPageNumber;
			hPage.Visible = false;
		}
	}

	protected void lbSearchNewsArchives_Click(object sender, EventArgs e)
	{
		string sSearchCriteria;
		GenworthQueryString oQueryString;

		if (Page.IsValid)
		{
			if (!string.IsNullOrEmpty(sSearchCriteria = tbSearchNewsArchives.Text.Trim()))
			{
				// remove pagination and add search criteria from/to query string
				//
				oQueryString = GenworthQueryString.Current;
				oQueryString.RemovePagination();
				oQueryString.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword, sSearchCriteria, true);

				Response.Redirect(ContextExtension.CurrentItem.GetURL() + oQueryString.ToString(), true);
			}
			else if(!String.IsNullOrWhiteSpace(GenworthQueryString.Current.ArticleKeyword))
			{
				// in case the user had previously made a search and wants to clean the search 
				// by clicking the search button, then show the entire list of articles with no filtering
				Response.Redirect(ContextExtension.CurrentItem.GetURL());
			}
		}
	}

</script>
<h1>
	Archived News<span class="article-search"><asp:TextBox ID="tbSearchNewsArchives"
		runat="server" ValidationGroup="SearchGroup" CssClass="keywords"></asp:TextBox>
		<a href="JavaScript: $('#<%= lbSearchNewsArchives.ClientID %>').click();"></a>
		<asp:Button ID="lbSearchNewsArchives" runat="server" OnClick="lbSearchNewsArchives_Click"
			ValidationGroup="SearchGroup" CssClass="target"></asp:Button></span></h1>

				
<asp:PlaceHolder ID="pArticleList" runat="server">
	<div id="genworth-article-list-wrapper">
	
		<div class="genworth-article-list-head">
			<h3 runat="server" id="hNoResult" visible="false"><br /><br />No search results found<br /><br /></h3>
			<ul id="ulListTitles" runat="server">
				<li class="article-title first">Title</li>
				<li id="liDateHeader" runat="server">
					<asp:HyperLink ID="hDatePosted" runat="server">Date Posted</asp:HyperLink>
				</li>
			</ul>
		</div>
        <div class="clearfix">
			<asp:Repeater ID="rItems" runat="server">
				<ItemTemplate>
                    <div class="article">
                        <div class="article-title-box">
						    <asp:PlaceHolder ID="pArticleImage" runat="server" Visible="false">
							    <asp:HyperLink ID="lArticleThumb" CssClass="article-thumb" runat="server">
								    <asp:Image ID="iArticleImage" runat="server" />
							    </asp:HyperLink>
						    </asp:PlaceHolder>
                            <asp:HyperLink ID="lArticleTitle" CssClass="article-title" runat="server"></asp:HyperLink>
                        </div>
                        <div class="article-date-box">
    						<asp:Label ID="lArticleDate" CssClass="article-date" runat="server"></asp:Label>
                        </div>
						<asp:PlaceHolder ID="phSummary" runat="server">
                            <div class="article-summary">
                                <asp:Literal ID="lArticleSummary" runat="server" />
                            </div>
                        </asp:PlaceHolder>
                    </div>
				</ItemTemplate>
			</asp:Repeater>
        </div>
		<div class="genworth-list-foot">
			<div class="pagination">
				<asp:HyperLink ID="lPrevious" runat="server" CssClass="previous" Visible="false">Previous</asp:HyperLink>
				<ul>
					<asp:Repeater ID="rPages" runat="server">
						<ItemTemplate>
							<li>
								<asp:Label ID="lSelectedPage" runat="server" />
								<asp:HyperLink ID="hPage" runat="server" />
							</li>
						</ItemTemplate>
					</asp:Repeater>
				</ul>
				<asp:HyperLink ID="lNext" runat="server" CssClass="next" Visible="false">Next</asp:HyperLink>
			</div>
		</div>
	</div>
</asp:PlaceHolder>
<asp:Literal ID="lDebug" runat="server" />