<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>

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

	public void BindData()
	{
		List<Item> lGlossaryItems;
		string sSearchParam;
		GenworthQueryString oQueryString;
        Item oGlossaryTerm = null;
        lGlossaryItems = new List<Item>();

		// wrap the current query string
		//
		oQueryString = GenworthQueryString.Current;

        if (!String.IsNullOrWhiteSpace(sSearchParam = oQueryString.GlossaryTermID) && Sitecore.Data.ID.IsID(sSearchParam))
        {
            // if we received a valid ID display just that item
            //
            if ((oGlossaryTerm = ContextExtension.CurrentDatabase.GetItem(sSearchParam)) != null)
            {
                lGlossaryItems.Add(oGlossaryTerm);
            }
        }
        else
        {
            // if there isn't already a glossary search in the query string, add a default
            //
            if (!oQueryString.ContainsGlossarySearch)
            {
                oQueryString.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.StartsWith,
                    Genworth.SitecoreExt.Constants.HelpCenter.Alphabet.DefaultLetter);
            }

            // are we searching by letter or keyword
            //
            if (!String.IsNullOrWhiteSpace(sSearchParam = oQueryString.GlossaryLetter))
            {
                lGlossaryItems = HelpCenterHelper.SearchGlossaryByPrefix(sSearchParam);

                if (!sSearchParam.Equals(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue))
                {
                    phGlossarySearchTitle.Visible = true;
                    lFirstLetter.Text = sSearchParam;
                }
            }
            else
            {
                lGlossaryItems = HelpCenterHelper.SearchGlossary(
                    string.IsNullOrWhiteSpace(oQueryString.GlossaryKeyword) ? string.Empty : oQueryString.GlossaryKeyword.Replace("*", string.Empty).Trim()
                );
            }
        }

		if (lGlossaryItems.Count() > 0)
		{
			BindGlossaryItems(lGlossaryItems, oQueryString);
		}
		else
		{
			phNoItems.Visible = true;
			phGlossarySearchTitle.Visible = false;
		}
	}

	private void BindGlossaryItems(IEnumerable<Item> iArticles, GenworthQueryString oQueryString)
	{
		oPaginator = GenworthPaginator.GetPaginator(iArticles, "GlossaryList");

		rGlossaryTerms.DataSource = oPaginator.GetDataset();
		rGlossaryTerms.DataBind();

		// get our page numbers and bind them to the pages list
		//
		rPages.DataSource = oPaginator.GetPageNumbers();
		rPages.ItemDataBound += new RepeaterItemEventHandler(rPages_ItemDataBound);
		rPages.DataBind();
		rPages.Visible = rPages.Items.Count > 1;

		oPaginator.ConfigureHyperLinkNext(lNext,ContextExtension.CurrentItem.GetURL(),oQueryString);
		oPaginator.ConfigureHyperLinkPrevious(lPrevious,ContextExtension.CurrentItem.GetURL(),oQueryString);
	}	
	
	void rGlossaryTerms_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			Item oGlossaryTerm;
			RepeaterItem oRepeaterItem = e.Item;
			Literal lGlossaryTitle;
			Literal lGlossaryTerm;

			oGlossaryTerm = oRepeaterItem.DataItem as Item;

			if (oGlossaryTerm != null)
			{
				// set title
				lGlossaryTitle = oRepeaterItem.FindControl("lGlossaryTitle") as Literal;

				if (lGlossaryTitle != null)
				{
					lGlossaryTitle.Text = oGlossaryTerm.GetText("Glossary Term", "Term");
				}

				// set definition
				lGlossaryTerm = oRepeaterItem.FindControl("lGlossaryTerm") as Literal;

				if (lGlossaryTerm != null)
				{
					lGlossaryTerm.Text = oGlossaryTerm.GetText("Glossary Term", "Definition");
				}
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
		
</script>
<div class="glossary">
	<div class="gc c9 inside">
		<asp:PlaceHolder runat="server" ID="phNoItems" Visible="false">
		<h3>
			 No search results found
	    </h3>
		</asp:PlaceHolder>

		<asp:PlaceHolder ID="phGlossarySearchTitle" Visible="false" runat="server">
		<h3>
			Glossary: <asp:Literal ID="lFirstLetter" runat="server" />
	    </h3>
		</asp:PlaceHolder>
		<dl>
			<asp:Repeater ID="rGlossaryTerms" runat="server" OnItemDataBound="rGlossaryTerms_ItemDataBound">
				<ItemTemplate>
					<dt>
						<asp:Literal ID="lGlossaryTitle" runat="server" />
					</dt>
					<dd class="html">
						<p>
							<asp:Literal ID="lGlossaryTerm" runat="server" />
						</p>
					</dd>
				</ItemTemplate>
			</asp:Repeater>
		</dl>

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

		<div class="clear">
		</div>
	</div>

	<div class="gc c3">
		<div class="clear">
		</div>
	</div>
	<div class="clear">
	</div>
</div>
