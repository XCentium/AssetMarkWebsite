<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<script runat="server">

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

    const string HtmlFormatPattern = "<.*?>|\n|&nbsp;";

	private void BindData()
	{
		List<Item> oArticles;
		Item oArticle;
		string sImageUrl;
		
		//get the articles from the Items field
		oArticles = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items")
			.Where(oItem => oItem.InstanceOfTemplate("Article")).ToList();

		// just grab the first article
		//
		if ((oArticle = oArticles.FirstOrDefault()) != null)
		{
			// get the article details
			//
			hTitle.Text = oArticle.GetText("Page", "Title");
			lText.Text = oArticle.GetText("Page", "Summary");
			lImageCaption.Text = oArticle.GetText("Page", "Blurb");

			// get the subtitle
			//
			lSubTitle.Text = BuildSubtitle(oArticle,false);
			
			// get the article's image, if it has one
			//
			if (string.IsNullOrWhiteSpace((sImageUrl = oArticle.GetField("Page", "Photo").GetImageURL())))
			{
				hLinkA.Visible = false;
			}
			else
			{
				iImage.ImageUrl = string.Format("~/{0}?mw=320&mh=188", sImageUrl);
			}

            ArticleConfigLinks(new HyperLink[] { hLinkB, hLinkA, hTitle }, oArticle);

            if (!hTitle.Visible)
            {
                Literal litTitle = new Literal();
                litTitle.Text = hTitle.Text;
                Title.Controls.Add(litTitle);
            }
            			
			// bind the link to archived news
			//
            Item archivedNews = ContextExtension.CurrentItem.Children["Archived News"];
            archivedNews.ConfigureHyperlink(hArchivedNews);
			
			// bind the first article and remove from the list
			//
			rItems.DataSource = oArticles.Skip(1).OrderByDescending(oItem => oItem.GetText("Timing", "Date"));
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
			rItems.DataBind();

            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLinkB);
            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLinkA);
            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hTitle);
            archivedNews.ConfigureOmnitureControl(ContextExtension.CurrentItem, hArchivedNews);
		}
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oArticle;
		HyperLink hLinkA;
		HyperLink hTitle;
		Literal lSubTitle;
		Literal lBlurb;
		PlaceHolder pHorizontalLine;

		//get the article
		oArticle = (Item)e.Item.DataItem;
		
		//does this repeater contain an item?
		if (oArticle != null)
		{
			//get the web controls
			hTitle = (HyperLink)e.Item.FindControl("hTitle");
			lSubTitle = (Literal)e.Item.FindControl("lSubTitle");
			lBlurb = (Literal)e.Item.FindControl("lBlurb");
			hLinkA = (HyperLink)e.Item.FindControl("hLinkA");
			pHorizontalLine = (PlaceHolder)e.Item.FindControl("pHorizontalLine");

            ArticleConfigLinks(new HyperLink[] { hLinkA, hTitle }, oArticle);

            hTitle.Text = oArticle.GetText("Page", "Title");
			lSubTitle.Text = BuildSubtitle(oArticle);
			lBlurb.Text = oArticle.GetText("Page", "Blurb");
			if (pHorizontalLine != null)
			{
				pHorizontalLine.Visible = (e.Item.ItemIndex < ((IEnumerable<Item>)rItems.DataSource).Count() - 1);
			}

            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLinkA);
            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hTitle);
		}
	}

    private void ArticleConfigLinks(HyperLink[] links, Item oArticle)
    {
        string bodyTextNoTags = Regex.Replace(oArticle.GetText("Page", "Body"), HtmlFormatPattern, string.Empty);
        bool hasContent = !string.IsNullOrWhiteSpace(bodyTextNoTags);

        foreach (HyperLink item in links)
        {
            item.Visible = hasContent;
            if (hasContent)
            {
                oArticle.ConfigureHyperlink(item);
            }
        }
    }

	private string BuildSubtitle(Item oArticle,bool nIncludeSubtitle = true)
	{
		// build the subtitle
		//
		StringBuilder retVal =
			new StringBuilder(oArticle.GetField("Timing", "Date").GetDateString("MMMM dd, yyyy", string.Empty));

        if (nIncludeSubtitle)
        {
            string sSubTitle = oArticle.GetText("Page", "Sub Title");

            if (!string.IsNullOrWhiteSpace(sSubTitle))
            {
                if (retVal.Length > 0)
                {
                    retVal.Append(" | ");
                }

                retVal.Append(sSubTitle);
            }
        }
        return retVal.ToString();
	}
	
</script>
<div class="article-summary">
	<h1 id="Title" runat="server"><asp:HyperLink ID="hTitle" runat="server" /></h1>
	<p class="header-note"><asp:Literal ID="lSubTitle" runat="server" /></p>

	<!-- START PROMO ARTICLE DISPLAY -->
	<div class="gc c4 inside">
		<p class="promo">
			<asp:HyperLink ID="hLinkA" runat="server"><asp:Image ID="iImage" runat="server" /></asp:HyperLink>
		</p>
		<div class="promo-note">
			<div class="html">
				<asp:Literal ID="lImageCaption" runat="server"  />
			</div>
		</div>
	</div>
	<div class="gc c4">
		<div class="extra-indent">
			<div class="html">
				<asp:Literal ID="lText" runat="server" />
			</div>
			<asp:HyperLink ID="hLinkB" class="home-read-me" runat="server">
			<img alt="" src="<%= Page.ResolveClientUrl("~/") %>CMSContent/Images/Home/ReadMeButton.png" />
			</asp:HyperLink>
		</div>
	</div>
	<div class="clear">&nbsp;</div>
	<!-- END PROMO ARTICLE DISPLAY -->

	<!-- START LATEST NEWS SECTION -->
	<div class="latest-news">
		<div class="gc c8 inside">
			<h2>Latest News</h2>
		</div>

		<asp:Repeater ID="rItems" runat="server">
			<ItemTemplate>
				<div class="clear">&nbsp;</div>
				<div id="Div1" class="gc c4 inside" runat="server">
					<h3><asp:HyperLink ID="hTitle" runat="server" /></h3>
					<p class="header-note"><asp:Literal ID="lSubTitle" runat="server" /></p>
					<div class="html">
						<asp:Literal ID="lBlurb" runat="server" />
					</div>
					<p><asp:HyperLink ID="hLinkA" runat="server">Read full article</asp:HyperLink></p>
				</div>
			</ItemTemplate>
			<AlternatingItemTemplate>
				<div id="Div2" class="gc c4" runat="server">
					<div class="extra-indent">
						<h3>
							<asp:HyperLink ID="hTitle" runat="server" /></h3>
						<p class="header-note">
							<asp:Literal ID="lSubTitle" runat="server" /></p>
						<div class="html">
							<asp:Literal ID="lBlurb" runat="server" />
						</div>
						<p>
							<asp:HyperLink ID="hLinkA" runat="server">Read full article</asp:HyperLink></p>
					</div>
				</div>
				<asp:PlaceHolder id="pHorizontalLine" runat="server"><hr /></asp:PlaceHolder>
			</AlternatingItemTemplate>
		</asp:Repeater>
		<!-- END LATEST NEWS SECTION -->
		<hr />
		<div class="clear">&nbsp;</div>
		<h4>
			<asp:HyperLink ID="hArchivedNews" runat="server" Visible="true">Browse Archived News</asp:HyperLink>
		</h4>
		<br />
	</div>
	
</div>