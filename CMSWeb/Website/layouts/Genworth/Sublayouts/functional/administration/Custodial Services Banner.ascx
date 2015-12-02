<%@ Control Language="c#" AutoEventWireup="true" %>
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
		List<Item> oAnnouncements;
		string sTitle;
		Item oPreviewLink;
		
		// get the preview links from the Items field
        oAnnouncements = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").ToList()
			.Where(oItem => oItem.InstanceOfTemplate("Preview Link")).ToList();

        if (oAnnouncements != null && oAnnouncements.Count > 0)
		{
            foreach (Item item in oAnnouncements)
            {
                oPreviewLink = item;

                Literal lBannerText = new Literal();
                lBannerText.Text += oPreviewLink.GetText("Body");

                if (!String.IsNullOrEmpty(sTitle = (oPreviewLink.GetText("Title"))))
                {
                    PlaceHolder phTitle = new PlaceHolder();
                    phTitle.Visible = true;
                    
                    Literal lTitle = new Literal();
                    lTitle.Text = "<H4>" + sTitle + "</H4>";
                    phTitle.Controls.Add(lTitle);
                    panel.Controls.Add(phTitle);
                }
                panel.Controls.Add(lBannerText);
            }		
		}
	}

</script>

<asp:Panel runat="server" ID="panel" CssClass="admin_block">
</asp:Panel>
