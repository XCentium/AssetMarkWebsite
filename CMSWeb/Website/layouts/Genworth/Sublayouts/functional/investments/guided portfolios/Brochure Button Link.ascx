<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            if (_contextItem == null && !isContextSet)
            {
                _contextItem = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Brochure")).FirstOrDefault();
            }
            BindData();
        }
    }

    private Item _contextItem;
    private bool isContextSet;

    public void SetContextItems(Item contextItems)
    {
        _contextItem = contextItems;
        isContextSet = true;
    }

	private void BindData()
	{
        brochure_button.Visible = _contextItem != null;
        string strNavigateUrl = _contextItem.GetImageURL("Document", "File");

        if (brochure_button.Visible && !String.IsNullOrEmpty(strNavigateUrl))
        {
            SetImage(iPicture, _contextItem.GetField("Document", "Thumbnail"));
            linkImageButton.Attributes["href"] = strNavigateUrl;
                
            _contextItem.ConfigureHyperlink(linkButton);

            //Set omniture tag
            _contextItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, linkButton);
            _contextItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, linkImageButton);
        }
        else
        {
            brochure_button.Visible = false;
        }
	}

    private void SetImage(HtmlImage htmlImage, Sitecore.Data.Fields.Field data)
    {
        var imgItem = data.GetImageItem();
        htmlImage.Visible = imgItem != null;
        if (htmlImage.Visible)
        {
            htmlImage.Src = string.Concat("~/", imgItem.GetMediaURL(string.Empty));
            htmlImage.Attributes["width"] = imgItem.GetText("Image", "Width");
            htmlImage.Attributes["height"] = imgItem.GetText("Image", "Height");
            htmlImage.Alt = imgItem.GetText("Image", "Alt");
        }
    }
</script>
<div class="brochure_button" id="brochure_button" runat="server">
    <asp:HyperLink id="linkImageButton" CssClass="linkImageButton" runat="server" target="_blank"> 
        <img runat="server" ID ="iPicture" />
    </asp:HyperLink>
    <asp:HyperLink id="linkButton" runat="server" target="_blank" CssClass="link_button">Download<br />Brochure</asp:HyperLink>
</div>