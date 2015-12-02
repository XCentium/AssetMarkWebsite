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
		List<Item> oArticles;

		// get the preview links from the Items field and, if necessary, bind
		//
		oArticles = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").ToList()
			.Where(oItem => oItem.InstanceOfTemplate("Preview Link")).ToList();
		
		if (oArticles != null && oArticles.Count > 0)
		{
			rItems.DataSource = oArticles;
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
			rItems.DataBind();
		}
		else
		{
			rItems.Visible = false;
		}		
	}
	
	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oPreview;
		Literal lTitle;
		Literal lBody;
		Item oIcon;
		Image iIcon;
		HyperLink hIcon;
		//HyperLink hLearnMore;

		// get the preview link
		//
		if ((oPreview = (Item)e.Item.DataItem) != null)
		{
			// get the controls
			//
			lTitle = (Literal)e.Item.FindControl("lTitle");
			lBody = (Literal)e.Item.FindControl("lBody");
			iIcon = (Image)e.Item.FindControl("iIcon");
			hIcon = (HyperLink)e.Item.FindControl("hPreview");
			//hLearnMore = (HyperLink)e.Item.FindControl("hLearnMore");

			// bind 'em
			//
			lTitle.Text = oPreview.GetText("Preview","Title");
			lBody.Text = oPreview.GetText("Preview", "Body");
			oPreview.ConfigureHyperlink(hIcon);
			//oPreview.ConfigureHyperlink(hLearnMore);

			// get the preview image, if it has one
			//
			if ((oIcon = oPreview.GetField("Preview", "Icon").GetImageItem()) != null)
			{
				iIcon.ImageUrl = "~/" + oIcon.GetMediaURL();
			}

            oPreview.ConfigureOmnitureControl(ContextExtension.CurrentItem, hIcon);
		}
	}

</script>
<div class="ad withPadding">
	<asp:Repeater ID="rItems" runat="server">
		<ItemTemplate>
			<asp:HyperLink ID="hPreview" runat="server"><asp:Image ID="iIcon" runat="server" width="124px" height="124px"/></asp:HyperLink>
			<h6>
				<asp:Literal ID="lTitle" runat="server" />
			</h6>
			<div class="promo-note html">
				<asp:Literal ID="lBody" runat="server" />
<%--				<asp:HyperLink ID="hLearnMore" runat="server">Learn more</asp:HyperLink>
--%>			</div>
			<div class="clear"></div>
		</ItemTemplate>
	</asp:Repeater>
</div>