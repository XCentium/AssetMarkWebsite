<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Register TagPrefix="LeftSidebar" Src="~/layouts/Genworth/Sublayouts/functional/Article Sidebar.ascx"  TagName="LeftSidebar" %>

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
        Item oArticle;

        // just grab the first article
        //
        if ((oArticle = ContextExtension.CurrentItem) != null)
        {
            // get the article details
            //
            lTitle.Text = oArticle.GetText("Page", "Title");

            // get the article's image, if it has one
            //

            lBody.Text = oArticle.GetText("Page", "Body");

            // bind the "return" button to the referring URL or Home if the user landed on this
            // page directly (e.g. via a bookmark)
            //
            hReturn.NavigateUrl = (Request.UrlReferrer == null) ?
                ItemExtension.RootItem.GetURL() : Request.UrlReferrer.ToString();
        }

        var webPageItem = ContextExtension.CurrentParentItems.Skip(2).FirstOrDefault();
        if (webPageItem != null)
        {
            sidebar.CurrentItem = webPageItem;
            webPageItem.ConfigureHyperlink(hReturn);
        }
    }
	
</script>
<div class="article_leftsidebar">
    <div class="gc c3 html">
        <LeftSidebar:LeftSidebar runat="server" ID="sidebar" />
    </div>
    <div class="gc c8 html">
        <div class="genworth-article-detail">
            <h1>
                <asp:Literal ID="lTitle" runat="server" /></h1>
            <!-- START ARTICLE HTML-->
            <div class="article-html">
                <asp:Literal ID="lBody" runat="server" />
            </div>
            <!-- END ARTICLE HTML-->
        </div>
        <asp:HyperLink ID="hReturn" runat="server" CssClass="back-link">Go Back</asp:HyperLink>
    </div>
</div>
