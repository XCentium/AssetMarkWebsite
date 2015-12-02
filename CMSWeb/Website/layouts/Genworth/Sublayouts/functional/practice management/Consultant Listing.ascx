<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
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
		IEnumerable<Item> oItems = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Consultant"));
		if (oItems.Count() > 0)
		{
			rConsultants.DataSource = oItems;
			rConsultants.ItemDataBound += new RepeaterItemEventHandler(rConsultants_ItemDataBound);
			rConsultants.DataBind();
			rSections.DataSource = oItems;
			rSections.ItemDataBound += new RepeaterItemEventHandler(rSections_ItemDataBound);
			rSections.DataBind();
		}
		else
			dConsultants.Visible = false;
	}

	void rSections_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oConsultant = e.Item.DataItem as Item;
		PlaceHolder pSecction;
		HtmlGenericControl aSecction;
		if (oConsultant != null)
		{
			pSecction = (PlaceHolder)e.Item.FindControl("pSecction");
			aSecction = new HtmlGenericControl("a");
			aSecction.Attributes.Add("href",string.Concat("#", oConsultant.ID.Guid.ToString("N")));
			pSecction.Controls.Add(aSecction);
			aSecction.InnerText = oConsultant.GetText("Name");
		}
	}

	void rConsultants_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Image iPhoto;		
		Literal lName;
		Literal lBio;
		Literal lTitle;
		Literal lSeparator;
		HtmlAnchor aSecction;
		Item oConsultant = e.Item.DataItem as Item;
		if (oConsultant != null)
		{
			//find controls
			iPhoto = (Image)e.Item.FindControl("iPhoto");			
			lName = (Literal)e.Item.FindControl("lName");
			lTitle = (Literal)e.Item.FindControl("lTitle");
			lBio = (Literal)e.Item.FindControl("lBio");
			aSecction = (HtmlAnchor)e.Item.FindControl("aSecction");
			lSeparator = (Literal)e.Item.FindControl("lSeparator");
			//setting values
			aSecction.Attributes.Add("name", oConsultant.ID.Guid.ToString("N"));
			string sImage = oConsultant.GetImageURL("Photo");
			if (!string.IsNullOrWhiteSpace(sImage))
			{
				iPhoto.ImageUrl = string.Format("~/{0}?mh=120&mw=180", sImage);
			}
			else
			{
				iPhoto.Visible = false;
			}
			
			lName.Text = oConsultant.GetText("Name");
            lTitle.Text = oConsultant.GetText("Title");

            lSeparator.Text = "<br/>";
			
			lBio.Text = oConsultant.GetText("Bio");
		}
	}
</script>
<div>
<asp:Repeater runat="server" ID="rSections">
	<ItemTemplate>
	<asp:PlaceHolder runat="server" ID="pSecction"></asp:PlaceHolder>
		<br />
	</ItemTemplate>
</asp:Repeater>
</div>
<div class="consultantListing" runat="server" id="dConsultants">
	<ul>
		<asp:Repeater runat="server" ID="rConsultants">
			<ItemTemplate>
				<a runat="server" id="aSecction" style="visibility: hidden;"></a>
				<li><span class="photo">
					<asp:Image runat="server" ID="iPhoto" />
				</span>					
					<span class="name">
						<asp:Literal runat="server" ID="lName" /><asp:Literal runat="server" Text=", "  ID="lSeparator"></asp:Literal><asp:Literal ID="lTitle" runat="server" /></span>
					<p>
						<asp:Literal ID="lBio" runat="server" />
					</p>
					<hr />
				</li>
			</ItemTemplate>
		</asp:Repeater>
	</ul>
</div>
