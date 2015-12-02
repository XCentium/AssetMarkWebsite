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

	private void BindData()
	{
		Item oArticle;
		Item oImage;
		string sSubTitle;
		string sImageURL;
		
		// just grab the first article
		//
		if ((oArticle = ContextExtension.CurrentItem) != null)
		{
			// get the article details
			//
			lTitle.Text = oArticle.GetText("Page", "Title");

			// build the subtitle
			//
			lSubTitle.Text = oArticle.GetField("Timing", "Date").GetDateString("MMMM dd, yyyy", string.Empty);
			if (lSubTitle.Text.Length > 0 && !string.IsNullOrEmpty(sSubTitle = oArticle.GetText("Page", "Sub Title"))) 
			{
				lSubTitle.Text += " | ";
			}
			
			lSubTitle.Text += oArticle.GetText("Page", "Sub Title");
			
			// get the article's image, if it has one
			//
			if ((oImage = oArticle.GetField("Page", "Photo").GetImageItem()) != null && !string.IsNullOrWhiteSpace((sImageURL = oImage.GetMediaURL())))
			{
				iImage.ImageUrl = "~/" + sImageURL;
			}
			else
				iImage.Visible = false;
			
			// get the article's body
			//
			lBody.Text = oArticle.GetText("Page", "Body");
			
			// bind the "return" button to the referring URL or Home if the user landed on this
			// page directly (e.g. via a bookmark)
			//
			hReturn.NavigateUrl = (Request.UrlReferrer == null) ?
				ItemExtension.RootItem.GetURL() : Request.UrlReferrer.ToString();
		}
	}
	
</script>
<div class="genworth-article-detail">
	<h1><asp:Literal ID="lTitle" runat="server" /></h1>
	<p class="header-note"><asp:Literal ID="lSubTitle" runat="server" /></p>

	<!-- START ARTICLE HTML-->
	<div class="article-html">
		<asp:Image ID="iImage" runat="server" height="150px" width="200px" class="float-left" />
		<asp:Literal ID="lBody" runat="server" />
		<hr/>
	</div>
	<!-- END ARTICLE HTML-->
</div>
<asp:HyperLink ID="hReturn" runat="server" CssClass="return-link">RETURN</asp:HyperLink>
