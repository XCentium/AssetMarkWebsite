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

				iImage.ImageUrl = string.Format("~/{0}?mh=120&mw=180", sImage);

                //Set omniture tag
                oVideo.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
            }
            
        }
    }
</script>

    <ul class="thumbnails">
        <asp:Repeater runat="server" ID="rVideos">
            <ItemTemplate>
            <asp:PlaceHolder runat="server" ID="pImage">
             <li><asp:HyperLink runat="server" ID="hLink">
                <asp:Image runat="server" ID="iImage" height="120" width="180" /> 
            </asp:HyperLink></li>
            </asp:PlaceHolder>
            </ItemTemplate>

        </asp:Repeater>
        
       
    </ul>
    
