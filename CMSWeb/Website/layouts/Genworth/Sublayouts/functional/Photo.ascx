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
		Item oItem;
		Item oImage;
		Sitecore.Data.Fields.Field oField;
		Sitecore.Data.Fields.ImageField oImageField;
		int iWidth;

		//get the item
		oItem = ContextExtension.CurrentItem;
		
		//get the field
		oField = oItem.GetField("Page", "Photo");
		oImageField = (Sitecore.Data.Fields.ImageField)oField;
		
		//if there is an image, use it
		if ((oImage = oField.GetImageItem()) != null)
		{
			iImage.ImageUrl = "~/" + oImage.GetMediaURL();
			lImageCaption.Text = oImage.GetText("Information", "Title");
			if (int.TryParse(oImageField.Width, out iWidth))
			{
				pImage.Width = iWidth;
			}
		}
		else
		{
			//hide the image if there is no image to be shown
			pImage.Visible = false;
		}
	}
	
</script>
<asp:Panel ID="pImage" runat="server" CssClass="photo">
	<p class="promo">
		<asp:Image ID="iImage" runat="server" />
	</p>
	<p class="promo-note">
		<asp:Literal ID="lImageCaption" runat="server" />
	</p>
</asp:Panel>