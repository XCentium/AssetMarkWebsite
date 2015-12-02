<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">

    Item oCurrentItem;

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
        oCurrentItem = ContextExtension.CurrentItem;
        
        // Set Descriptions of bottom cards
        m1_description.InnerHtml = oCurrentItem.GetText("Market Blend", "Content");
        m2_description.InnerHtml = oCurrentItem.GetText("Market Factor Blend", "Content");

        setLinks("Market Blend", "Factsheet", m1_factsheet);
        setLinks("Market Blend", "Holding", m1_holdings);
        setLinks("Market Factor Blend", "Factsheet",m2_factsheet);
        setLinks("Market Factor Blend", "Holding", m2_holdings);
        
	}

    private void setLinks(string section, string field, HyperLink link)
    {
        Item docItem = oCurrentItem.GetListItem(section, field);
        if (link.Visible = docItem != null)
        {
            docItem.ConfigureHyperlink(link);
            //Set omniture tag
           docItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, link);    
        }
    }
</script>
<div id="Market_Blend_Tab_Container">
<div class="gc c12 inside">
    <div id="Market_Blend_Tab">
        <div class="gc c8 html inside">
            <sc:Placeholder ID="Placeholder2" Key="title" runat="server" />
	        <sc:Placeholder ID="Placeholder4" Key="description" runat="server" />
        </div>
        <div class="gc c3">
	        <sc:Placeholder ID="Placeholder5" Key="video" runat="server" />
        </div>
        <div class="clear"></div>
    </div>

    <div class="gc c6 first">
        <div class="brochure_card logo_factsheet_card">
            <h2>
                <img src="/CMSContent/Images/MarketBlend_Banner.png" />
            </h2>
            <div class="container">
                <div class="html description" id="m1_description" runat="server">
                </div>
                <div class="button_content">
                    <asp:HyperLink id="m1_factsheet" Target="_blank" runat="server"><img src="/CMSContent/Images/ViewFactsheetButton.png" /></asp:HyperLink>
                    <asp:HyperLink id="m1_holdings" Target="_blank" runat="server"><img src="/CMSContent/Images/ViewHoldingsButton.png" /></asp:HyperLink>
                </div>
                <div class="clear"></div>
            </div>
        </div>
    </div>

    <div class="gc c6 last">
        <div class="brochure_card logo_factsheet_card" id="Div1" runat="server">
            <h2>
                <img src="/CMSContent/Images/MarketFactorBlend_Banner.png" />
            </h2>
            <div class="container">
                <div class="html description" id="m2_description" runat="server">
                    <p>GPS Solutions are single-account solutions that invest in proprietary funds reflecting the broad spectrum of viewpoints and strategies available on the AssetMark platform for a low minimum. You can select a comprehensive GPS Solution encompassing a wide range of diverse investment views across&nbsp; the four asset allocation approaches, plus alternative investments. Or you can complement an existing portfolio with a focused, diversification solution. <a href="~/link.aspx?_id=8C9692FB63DB4AE692767CF087CD9BC3&amp;_z=z" shape="rect">Learn More</a>&nbsp;&gt;</p>
                </div>
                <div class="button_content">
                    <asp:HyperLink id="m2_factsheet" Target="_blank" runat="server"><img src="/CMSContent/Images/ViewFactsheetButton.png" /></asp:HyperLink>
                    <asp:HyperLink id="m2_holdings" Target="_blank" runat="server"><img src="/CMSContent/Images/ViewHoldingsButton.png" /></asp:HyperLink>
                </div>
                <div class="clear"></div>
            </div>
        </div>
    </div>

    <div class="clear"></div>
</div>

<div class="clear"></div>
</div>