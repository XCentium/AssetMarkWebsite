<%@ Control Language="c#" ClassName="ImageLink" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
        
        if (CurrentItem == null)
        {
            CurrentItem = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(item => item.InstanceOfTemplate("Image Link")).FirstOrDefault();
        }
        
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

    public Item CurrentItem { get; set; }

	private void BindData()
	{
        if (CurrentItem != null)
        {
            CurrentItem.ConfigImage("Link", "Image", iImage);
            CurrentItem.ConfigureHyperlink(hLink);
        }
	}
	
</script>
<asp:Panel ID="pImage" runat="server" CssClass="ImageLinkControl">
	<asp:HyperLink runat="server" ID="hLink">
		<asp:Image ID="iImage" runat="server" />
	</asp:HyperLink>

</asp:Panel>