<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/global component list/Image Link.ascx" TagName="ImageLink" %>

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
        IEnumerable<Item> oItems = ContextExtension.CurrentItem.GetChildrenAndItemsOfTemplate(new string[2] {"Image Link", "Video"});
        if (oItems.Count() > 0)
        {
            rVideos.DataSource = oItems;
            rVideos.DataBind();
        }
    }

    protected void rVideos_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        const int DEFAULT_HEIGHT = 481;
        const int DEFAULT_WIDTH = 878;
        
        var oItem = e.Item.DataItem as Item;
        var caption = e.Item.FindControl("caption") as HtmlGenericControl;

        var hLink = e.Item.FindControl("hLink") as HyperLink;
        hLink.NavigateUrl = oItem["url"];

        oItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
        
        var iThumb = e.Item.FindControl("iThumb") as System.Web.UI.WebControls.Image;
        iThumb.ImageUrl = oItem.GetImageURL("Image");

        if (oItem.TemplateName == "Video")
        {
            int height;
            int.TryParse(oItem["height"], out height);
            if (height <= 0)
            {
                height = DEFAULT_HEIGHT;
            }

            int width;
            int.TryParse(oItem["width"], out width);
            if (width <= 0)
            {
                width = DEFAULT_WIDTH;          
            }

            hLink.Attributes.Add("rel", "shadowbox;height=" + height + ";width=" + width + ";scroll=no;player=iframe");

            StringBuilder sbCaption = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(oItem.GetText("title")))
            {
                sbCaption.Append(oItem.GetText("title"));
            }

            if (!String.IsNullOrWhiteSpace(oItem.GetText("length in minutes")))
            {
                if (sbCaption.Length > 0)
                {
                    sbCaption.Append(" - ");
                }

                sbCaption.Append(oItem.GetText("length in minutes") + " minutes");
            }

            caption.InnerHtml = sbCaption.ToString();
        }
        else
        {
            hLink.Target = oItem.GetText("target");
            caption.InnerHtml = oItem.GetText("caption");
        }
    }
</script>
<div class="video-presentation-container">
    <div class="video-presentation-top">
        <sc:Placeholder runat="server" Key="video-presentation-top" />
    </div>
    <div class="video-presentation-content">
        <asp:Repeater runat="server" ID="rVideos" OnItemDataBound="rVideos_ItemDataBound">
            <ItemTemplate>
                <figure>
                    <asp:HyperLink runat="server" ID="hLink">
                        <asp:Image runat="server" ID="iThumb" />
                    </asp:HyperLink><br />
                    <figcaption runat="server" id="caption"></figcaption>
                </figure>
            </ItemTemplate>
        </asp:Repeater>
        <div class="clear"></div>
    </div>
</div>
