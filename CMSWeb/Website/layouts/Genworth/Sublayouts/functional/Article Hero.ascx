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

			// get the article's image, if it has one
			//
			if (string.IsNullOrWhiteSpace((sImageUrl = oArticle.GetField("Page", "Photo").GetImageURL())))
			{
				hLinkA.Visible = false;
			}
			else
			{
				iImage.ImageUrl = string.Format("~/{0}?mw=220", sImageUrl);
			}

            ArticleConfigLinks(new HyperLink[] { hLinkB, hLinkA, hTitle }, oArticle);
 
            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLinkB);
            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLinkA);
            oArticle.ConfigureOmnitureControl(ContextExtension.CurrentItem, hTitle);    
		}
	}

	private void ArticleConfigLinks(HyperLink[] links, Item oArticle)
    {
        string bodyTextNoTags = Regex.Replace(oArticle.GetText("Page", "Body"), HtmlFormatPattern, string.Empty);
        bool hasContent = !string.IsNullOrWhiteSpace(bodyTextNoTags);

        links[0].Visible = hasContent;

        foreach (HyperLink item in links)
        {
            if (hasContent)
            {
                oArticle.ConfigureHyperlink(item);
            }
        }
    }
	
</script>
<div class="article-summary">

	<div class="gc c3 inside">
		<p class="promo">
			<asp:HyperLink ID="hLinkA" runat="server"><asp:Image ID="iImage" runat="server" /></asp:HyperLink>
		</p>
		<div class="promo-note">
			<div class="html">
				<asp:Literal ID="lImageCaption" runat="server"  />
			</div>
		</div>
	</div>
    <div class="gc c6">
        <h3>
            <asp:HyperLink ID="hTitle" runat="server" /></h3>
        <div class="html">
            <asp:Literal ID="lText" runat="server" />
        </div>
        <asp:HyperLink ID="hLinkB" runat="server">READ MORE
        </asp:HyperLink>
    </div>
	<div class="clear">&nbsp;</div>
	
</div>