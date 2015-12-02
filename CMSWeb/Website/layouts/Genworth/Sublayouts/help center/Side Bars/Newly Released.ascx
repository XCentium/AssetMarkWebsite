<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>

<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

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
		List<Item> oItems = ContextExtension.CurrentItem.GetMultilistItems("Items").GetItemsOfTemplate(new string[] { "Video" });
		if (oItems.Count > 0)
		{
			rReleases.DataSource = oItems;
			rReleases.ItemDataBound += new RepeaterItemEventHandler(rReleases_ItemDataBound);
			rReleases.DataBind();
		}
	}

	void rReleases_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Image iThumbnail;
		Literal lTopic;
		Literal lDescription;
		Item oItem = e.Item.DataItem as Item;
		if (oItem != null)
		{
			iThumbnail = (Image)e.Item.FindControl("iThumbnail");
			lTopic = (Literal)e.Item.FindControl("lTopic");
			lDescription = (Literal)e.Item.FindControl("lDescription");

			lTopic.Text = oItem.GetText("Video", "Title");
			lDescription.Text = oItem.GetText("Video","Description");
			string sURL = oItem.GetImageURL("Video","Image");
			if (!string.IsNullOrWhiteSpace(sURL))
			{
				iThumbnail.ImageUrl = string.Concat("~/", sURL);
			}
			else
			{
				iThumbnail.Visible = false;
			}

			HyperLink hImage = (HyperLink)e.Item.FindControl("hImage");
			oItem.ConfigureHyperlink(hImage);
		}
	}
</script>
<asp:Repeater runat="server" ID="rReleases">
	<HeaderTemplate>
		<ul class="thumbnails topic-thumbnails">
	</HeaderTemplate>
	<ItemTemplate>
		<li class="first">
			<h6>
				<asp:Literal runat="server" ID="lTopic" />
			</h6>
			
			<asp:HyperLink runat="server" ID="hImage" class="inline-video">
				<asp:Image ID="iThumbnail" runat="server" />
			</asp:HyperLink>
			
			<p>
				<asp:Literal ID="lDescription" runat="server" />
			</p>
		</li>
	</ItemTemplate>
	<FooterTemplate>
		</ul></FooterTemplate>
</asp:Repeater>
