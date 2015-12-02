<%@ Control Language="c#" ClassName="LeftSidebar" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<script runat="server">

    public Item CurrentItem { get; set; }

	protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            if (CurrentItem == null)
            {
                CurrentItem = ContextExtension.CurrentItem;
            }
            BindData();
		}
	}

    const string HtmlFormatPattern = "<.*?>|\n|&nbsp;";    

	private void BindData()
	{
        var oArticles = CurrentItem.GetMultilistItems("Page", "Items")
                    .Where(oItem => oItem.InstanceOfTemplate("Article")).ToList();
		// just grab the first article
		//
		if (oArticles.Count > 1)
		{
			rItems.DataSource = oArticles.Skip(1).OrderByDescending(oItem => oItem.GetText("Timing", "Date"));
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
			rItems.DataBind();
		}

        var archiveItem = CurrentItem.GetChildrenOfTemplate("Administration Articles").FirstOrDefault();
        if (archiveItem != null)
        {
            archiveItem.ConfigureHyperlink(archiveNewsLink);
            archiveItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, archiveNewsLink);
        }
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oArticle;
		HyperLink hLinkA;
		HyperLink hTitle;
		Literal lSubTitle;
		Literal lBlurb;

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

            ArticleConfigLinks(new HyperLink[] { hLinkA, hTitle }, oArticle);

			hTitle.Text = oArticle.GetText("Page", "Title");
			lBlurb.Text = oArticle.GetText("Page", "Blurb");

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

<div class="article-sidebar">
    <h3>News and Announcements</h3>    
    <asp:Repeater ID="rItems" runat="server">
		<ItemTemplate>
			<div class="article_item" runat="server">
				<h4><asp:HyperLink ID="hTitle" runat="server" /></h4>
				<div class="html">
					<asp:Literal ID="lBlurb" runat="server" />
				</div>
			    <asp:HyperLink ID="hLinkA" runat="server">Read more ></asp:HyperLink>
			</div>
		</ItemTemplate>
	</asp:Repeater>
    <asp:HyperLink runat="server" ID="archiveNewsLink" CssClass="archive_link">Browse Archived News</asp:HyperLink>
</div>
	