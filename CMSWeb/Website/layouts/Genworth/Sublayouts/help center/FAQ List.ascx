<%@ Control Language="c#" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>
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

	private void BindData()
	{
		IEnumerable<Item> iFaqs;
		GenworthQueryString oQueryString;

		// wrap the current query string
		//
		oQueryString = GenworthQueryString.Current;

		// if there isn't already a glossary search in the query string, add a default
		//
		if (!oQueryString.ContainsFAQSearch)
		{
			oQueryString.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Category,
				Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue);
		}

		// load the FAQ's
		//
		iFaqs = Genworth.SitecoreExt.Helpers.HelpCenterHelper.SearchFAQ(oQueryString.FAQKeyword, oQueryString.FAQCategory);
		if (iFaqs != null && iFaqs.Count() > 0)
		{
			rItems.DataSource = iFaqs;
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
			rItems.DataBind();

			// initialize the paginator
			//
			oPaginator = GenworthPaginator.GetPaginator(iFaqs, "FAQList");

			// bind  the paginator
			//
			BindFAQs(oPaginator, oQueryString);

			// bind the 'jump list' to only those FAQ's shown on the page
			//
			rJumpList.DataSource = oPaginator.GetDataset();
			rJumpList.ItemDataBound += new RepeaterItemEventHandler(rJumpList_ItemDataBound);
			rJumpList.DataBind();
		}
		else
			pNoResult.Visible = true;
		// Bind current category
		BindCategory(oQueryString.FAQCategory);
	}

	// Show the current category
	private void BindCategory(string sCategory)
	{
		Item oCategory;

		// in the query string is the category id, but we need to show the
		// category title
		if ((oCategory = Sitecore.Context.Database.GetItem(sCategory)) != null && oCategory.TemplateName.Equals("FAQ Category"))
		{
			lCategoryName.Text = oCategory.GetText("FAQ Category", "Title");
		}
		else
		{
			lCategoryName.Text = Genworth.SitecoreExt.Constants.HelpCenter.FAQ.CategoryAll;
		}
	}

	private void BindFAQs(Paginator<Item> oPaginator,GenworthQueryString oQueryString)
	{
		rItems.DataSource = oPaginator.GetDataset();
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();

		// get our page numbers and bind them to the pages list
		//
		rPages.DataSource = oPaginator.GetPageNumbers();
		rPages.ItemDataBound += new RepeaterItemEventHandler(rPages_ItemDataBound);
		rPages.DataBind();
		rPages.Visible = rPages.Items.Count > 1;

		oPaginator.ConfigureHyperLinkNext(lNext, ContextExtension.CurrentItem.GetURL(), oQueryString);
		oPaginator.ConfigureHyperLinkPrevious(lPrevious, ContextExtension.CurrentItem.GetURL(), oQueryString);
	}
	
	private void rItems_ItemDataBound(Object sender, RepeaterItemEventArgs e)
	{
		Item oFaq;
		Literal lQuestion;
		Literal lAnswer;
		HtmlAnchor aBookmark;

		if ((oFaq = (Item)e.Item.DataItem) != null)
		{
			// get the controls
			//
			lQuestion = (Literal)e.Item.FindControl("lQuestion");
			lAnswer = (Literal)e.Item.FindControl("lAnswer");
			aBookmark = (HtmlAnchor)e.Item.FindControl("aBookmark");
			
			// bind 'em
			//
			lAnswer.Text = oFaq.GetText("FAQ", "Answer");
			lQuestion.Text = oFaq.GetText("FAQ", "Question");
			aBookmark.Attributes.Add("name", oFaq.ID.ToString());
		}
	}

	private void rJumpList_ItemDataBound(Object sender, RepeaterItemEventArgs e)
	{
		Item oFaq;
		Literal lQuestion;
		HyperLink hBookmark;

		if ((oFaq = (Item)e.Item.DataItem) != null)
		{
			// get the controls
			//
			lQuestion = (Literal)e.Item.FindControl("lQuestion");
			hBookmark = (HyperLink)e.Item.FindControl("hBookmark");

			// bind 'em
			//
			lQuestion.Text = oFaq.GetText("FAQ", "Question");
			hBookmark.Attributes.Add("href", string.Format("#{0}", oFaq.ID.ToString()));
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
	
</script>
<div class="faq">
	<div class="gc c9 inside">
		<h3>Frequently Asked Questions: <asp:Literal id="lCategoryName" runat="server" /></h3>

		<dl>
			<asp:PlaceHolder runat="server" Visible="false" id="pNoResult" >
					<h3>No search results found</h3>
			</asp:PlaceHolder>
			<asp:Repeater ID="rItems" runat="server">
				<ItemTemplate>
					<dt><a id="aBookmark" runat="server" /><asp:Literal id="lQuestion" runat="server" /></dt>
					<dd class="html">
						<asp:Literal id="lAnswer" runat="server" />
					</dd>
				<div class="clear"></div>
				</ItemTemplate>
			</asp:Repeater>
		</dl>

	<hr />

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
		<div class="clear"></div>
	</div>

	<div class="gc c3">
		<h3>Jump to questions:</h3>
		<asp:Repeater ID="rJumpList" runat="server">
			<ItemTemplate>
				<p>
				<asp:HyperLink ID="hBookmark" runat="server"><asp:Literal ID="lQuestion" runat="server" /></asp:HyperLink>
				</p>
			</ItemTemplate>
		</asp:Repeater>
		
		<div class="clear"></div>
	</div>

	<div class="clear"></div>
</div>
