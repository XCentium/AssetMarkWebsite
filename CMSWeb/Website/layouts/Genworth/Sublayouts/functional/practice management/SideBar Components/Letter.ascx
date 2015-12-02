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
        Item oItem = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Donnelly Publication")).FirstOrDefault();
		if (oItem != null)
		{
			string sThumbnail = oItem.GetImageURL("Preview Thumbnail");
			if (string.IsNullOrWhiteSpace(sThumbnail))
			{
				dThumbnail.Visible = false;

			}
			else
			{
				iThumbnail.ImageUrl = string.Format("~/{0}?mh=150&mw=180", sThumbnail);
			}
			lDescription.Text = oItem.GetText("Description");
			lTitle.Text = oItem.GetText("Title");
		}
		else
		{
			dLetter.Visible = false;
		}
    }
</script>
<div class="KnowledgeCenter" runat="server" id="dLetter">
    <div class="withPadding withBorder blue">
        <center runat="server" id="dThumbnail">
            <asp:Image runat="server" ID="iThumbnail" border="1" />
        </center>
        <br />
        <h6 style="text-align:center;">
			<asp:Literal runat="server" ID="lTitle" />
		</h6>
        <div class="html"><asp:Literal runat="server" ID="lDescription" /></div>
    </div>
</div>
