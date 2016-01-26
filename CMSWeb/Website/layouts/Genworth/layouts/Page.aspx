<%@ Page Language="c#" CodePage="65001" AutoEventWireup="true" %>

<%@ Implements Interface="ServerLogic.SitecoreExt.Async.IAsyncLayout" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="ServerLogic.SitecoreExt.Async" %>

<%@ OutputCache Location="None" VaryByParam="none" %>
<script runat="server">
	private string sVersion = "1.0";
	private DateTime dStartTime;
	private List<IAsyncSublayout> oSublayouts = new List<IAsyncSublayout>();
	private string sCssClassName = "";
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!Page.IsPostBack)
		{
			sCssClassName += ContextExtension.CurrentItem.Key + " " + ContextExtension.CurrentItem.GetText("Page", "Body Class");

			JavaScriptModule.Text = ContextExtension.CurrentItem.GetText("Page", "JavaScript Module", null);
			CssModule.Text = ContextExtension.CurrentItem.GetText("Page", "Css Module", null);

			BindData();
		}

        // go get the proper bundle url from EWM and then inject into the head block on this page.
        string bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/Common", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Scripts);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddJsToPage(this.Page, bundleUrl);
        }

        bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/Common", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Styles);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddCssToPage(this.Page, bundleUrl);
        }
    }

	public void RegisterSublayout(IAsyncSublayout oSublayout)
	{
		oSublayouts.Add(oSublayout);
	}

	protected override void OnPreRenderComplete(EventArgs e)
	{
		oSublayouts.ForEach(oItem => oItem.Join());
		base.OnPreRenderComplete(e);
	}

	private void BindData()
	{
		double dVersion;

		//tell the sticky link manager to "stick" so that any sticky links above this page are updated with this page's URL.
		ServerLogic.SitecoreExt.Utilities.StickyLinkManager.Stick();

		// create css class, special case for IE only
		//

		//sCssClassName += " " + Request.Browser.ToLower();
		//sCssClassName += " firefox";

		dVersion = GetInternetExplorerVersion();
		if (dVersion != -1.0)
		{
			string sIeCssClass;
			sIeCssClass = " ie" + Math.Floor(dVersion).ToString();
			sCssClassName += sIeCssClass;
		}
		tBody.Attributes.Add("class", String.Format("{0}", sCssClassName.Trim()));
	}

	protected override void OnInit(EventArgs e)
	{
		if (!string.IsNullOrEmpty(Request.QueryString["debug"]))
		{
			dStartTime = DateTime.Now;
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		if (!string.IsNullOrEmpty(Request.QueryString["debug"]))
		{
			lDebug.Text = string.Format("Load mSec: {0}", (DateTime.Now.Ticks - dStartTime.Ticks) / TimeSpan.TicksPerMillisecond);
		}
	}

	/// <summary>
	/// Returns the version of Internet Explorer or a -1 (indicating the use of another browser).
	/// </summary>
	/// <returns></returns>
	private double GetInternetExplorerVersion()
	{
		double dVersion = -1;
		System.Web.HttpBrowserCapabilities browser = Request.Browser;
		sCssClassName += " " + browser.Browser.ToLower();
		if (browser.Browser.ToUpper() == "IE")
		{
			dVersion = (double)(browser.MajorVersion + browser.MinorVersion);
		}

		return dVersion;
	}
</script>
<!DOCTYPE html>
<html lang="en" xml:lang="en" xmlns="https://www.w3.org/1999/xhtml">
	<head>
		<!-- Page.aspx -->
        <title>AssetMark</title>
		<meta name="description" runat="server" id="MetaDescription" />
		<meta name="keywords" runat="server" id="MetaKeywords" />
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		
		<% 
			string sBaseUrl;
			sBaseUrl = Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ? Page.ResolveClientUrl("~/") : "/";
		%>

		<link href="https://fonts.googleapis.com/css?family=Cabin:600,600italic" rel="stylesheet" type="text/css" />
		<link href="https://fonts.googleapis.com/css?family=Oswald" rel="stylesheet" type="text/css" />
		<link href="https://fonts.googleapis.com/css?family=Cardo&v1" rel="stylesheet" type="text/css" />

		<link rel="icon" href="<%= sBaseUrl %>CMSContent/images/AssetMark_Icon_16.ico" />
        
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-1.10.2.min.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-ui-1.10.3.min.js" type="text/javascript"></script>

		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ellipsis.css" rel="stylesheet" type="text/css" />

		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery-ui.css" rel="stylesheet" type="text/css" /> 

		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.tooltip.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.skinnable.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthscrollbar.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.selectBox.genworth.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthdropdownpanel.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthcheckbox.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthgrid.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthgridpager.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthdialog.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthtooltip.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Scripts/Libraries/ShadowBox/shadowbox.css" rel="stylesheet" type="text/css" />

            <link href="<%= sBaseUrl %>CMSContent/Styles/Master.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/Site.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/SecondaryMenu.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/TertiaryMenu.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/VerticalTabs.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/Content.css?id=1305141238" rel="stylesheet" type="text/css" />
		
		<link href="<%= sBaseUrl %>CMSContent/Styles/MeetingMode.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/GlobalSearch.css" rel="stylesheet" type="text/css" />

        
		    <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-1.10.2.min.js" type="text/javascript"></script>
            <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
            <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-ui-1.10.3.min.js" type="text/javascript"></script>

        <%--<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.dimensions.js" type="text/javascript"></script>--%>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery.glob.min.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.clearfield.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.unobtrusive-ajax.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.validate.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.validate.unobtrusive.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery.hoverIntent.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ellipsis.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/CoreExtensions.js" type="text/javascript"></script>
            
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.cookie.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.tooltip.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/grid.locale-en.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/grid.base.genworth.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/grid.common.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/grid.custom.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/grid.jqueryui.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/grid.subgrid.genworth.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/grid.setcolumns.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.fmatter.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/genworthdatevalidation.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.genworth.countindicator.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.skinnable.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.genworthscrollbar.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.selectBox.genworth.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.genworthdropdownpanel.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.genworthcheckbox.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.genworthgrid.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.genworthgridpager.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.genworthdialog.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.ui.genworthtooltip.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Layout.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Search.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Tabs.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/CollapsiblePanel.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Events.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Marketing.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Omniture.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/PracticeManagement.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Planning.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Toggler.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/MeetingModeAjaxHelper.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/GlobalSearch.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Help.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Investments.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Clients.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.dialogbox.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Articles.js" type="text/javascript"></script>

        <script src="<%= Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ?  string.Concat(sBaseUrl, "CMSContent/Scripts/FeedBack.js") : "/Areas/User/Content/Scripts/FeedBack.js"%>" type="text/javascript"></script>
        <script src="<%= Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ?  string.Concat(sBaseUrl, "CMSContent/Scripts/timeout.js?n=3") : "/DNeWM/Javascripts/timeout.js?n=3"%>" type="text/javascript"></script>

		<link href="<%= sBaseUrl %>CMSContent/Styles/BreadcrumbMenu.css" rel="stylesheet" type="text/css" />

		<link href="<%= sBaseUrl %>CMSContent/Styles/HeaderMessage.css" rel="stylesheet" type="text/css" />
		<script src="<%= sBaseUrl %>CMSContent/Scripts/HeaderMessage.js" type="text/javascript"></script>
		
		<link href="<%= sBaseUrl %>CMSContent/Styles/main.css?v=1.0" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/982_12_10.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/Strategies.css" rel="stylesheet" type="text/css" />

		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/main.js?v=1.0"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridCommon.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridInvestments.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridManagerSearch.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridEvents.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/Carousel.js"></script>
        <script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/Strategies.js"></script>

        <%--Carousel section--%>
		<link href="<%= sBaseUrl %>CMSContent/Styles/Carousel.css" rel="stylesheet" type="text/css" />

        <asp:PlaceHolder ID="pResources" runat="server"></asp:PlaceHolder>

        <%-- feedback: file differences, do not move to EWM before reconciling --%>
        <script src="<%= Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ?  string.Concat(sBaseUrl, "CMSContent/Scripts/FeedBack.js") : "/Areas/User/Content/Scripts/FeedBack.js"%>" type="text/javascript"></script>
	</head>
	<body id="tBody" runat="server">
		<% 
			int iScrollPosition;
			bool bScrolled = Int32.TryParse(Request.QueryString["y"], out iScrollPosition);
			//bScrolled = true;
			string sScrollStyle = "position:absolute;";
			sScrollStyle += bScrolled ? string.Format("top: {0}px;", iScrollPosition) : string.Empty;
		%>
		<a name="scroll" style="<%= sScrollStyle %>"></a>
		<form method="post" runat="server" id="mainform">
			<div id="wrapper">
				<div id="wrapper-inner">
					<sc:Placeholder Key="header" runat="server" />
				
					<div id="sitecore-content">
						<div class="grid-system g982">
							<sc:Placeholder Key="Body" runat="server" />
							<div class="clear"></div>
						</div>
					</div>
					<sc:Placeholder Key="Footer" runat="server" />
				</div>
                <div runat="server" >
                  <img src="<%= Page.ResolveClientUrl("~/") %>CMSContent/Images/container_bg_bottom.png" class="container_bg_bottom" border="0" alt=""/>
                  </div>
			</div>
			<asp:Literal ID="lDebug" runat="server" />
			<asp:Literal ID="JavaScriptModule" runat="server" />
			<asp:Literal ID="CssModule" runat="server" />
			<input id="intScrollPosition" runat="server" type="hidden" value="200" />
		</form>
        <script language="javascript" type="text/javascript">
            //setupTimeout();
        </script>
	</body>
</html>
