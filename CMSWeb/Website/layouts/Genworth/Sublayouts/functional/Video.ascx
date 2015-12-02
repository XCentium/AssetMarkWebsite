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
        IEnumerable<Item> oItems = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Video"));
        if (oItems.Count() > 0)
        {
            rVideos.DataSource = oItems;
            rVideos.ItemDataBound +=new RepeaterItemEventHandler(rVideos_ItemDataBound);
            rVideos.DataBind();
        }
        
    }
    void rVideos_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Image iImage;
        HyperLink hLink;
        PlaceHolder pImage;
        Item oVideo = e.Item.DataItem as Item;
        if (oVideo != null)
        {
            
            string sImage = oVideo.GetImageURL("Image");
            if (string.IsNullOrWhiteSpace(sImage))
            {
                pImage = (PlaceHolder)e.Item.FindControl("pImage");
                pImage.Visible = true;
            }
            else
            {
                iImage = (Image)e.Item.FindControl("iImage");
                hLink = (HyperLink)e.Item.FindControl("hLink");

				oVideo.ConfigureVideoShadowbox(hLink);
                hLink.Attributes["rel"] += "scroll=no;";
                
                //Check alt, width and height
                SetImage(iImage, oVideo.GetField("Image"));
                
				//iImage.ImageUrl = string.Format("~/{0}?mh=mw=180&120", sImage);
            }
            
        }
    }

    private void SetImage(Image htmlImage, Sitecore.Data.Fields.Field data)
    {
        var imgItem = data.GetImageItem();
        htmlImage.Visible = imgItem != null;
        if (htmlImage.Visible)
        {
            var width = imgItem.GetText("Image", "Width");
            var height = imgItem.GetText("Image", "Height");
            htmlImage.ImageUrl = string.Format("~/{0}?mw={1}&mh={2}", imgItem.GetMediaURL(string.Empty), width, height);
            htmlImage.Attributes["width"] = width;
            htmlImage.Attributes["height"] = height;
            htmlImage.AlternateText = imgItem.GetText("Image", "Alt");
            htmlImage.ToolTip = htmlImage.AlternateText;
        }
    }
</script>

    <ul class="thumbnails">
        <asp:Repeater runat="server" ID="rVideos">
            <ItemTemplate>
            <asp:PlaceHolder runat="server" ID="pImage">
             <li><asp:HyperLink runat="server" ID="hLink">
                <asp:Image runat="server" ID="iImage" /> 
            </asp:HyperLink></li>
            </asp:PlaceHolder>
            </ItemTemplate>

        </asp:Repeater>
        
       
    </ul>
    
