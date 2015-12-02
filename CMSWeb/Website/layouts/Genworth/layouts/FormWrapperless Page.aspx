<%@ Page language="c#" Codepage="65001" AutoEventWireup="true" %>
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
			sCssClassName += ContextExtension.CurrentItem.Key + " ";
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
        <!-- FormWrapperless Page.aspx -->
		<title>AssetMark</title>
		<meta name="description" id="MetaDescription" />
		<meta name="keywords" id="MetaKeywords" />
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

		<link href="<%= sBaseUrl %>CMSContent/Styles/Site.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/Content.css?id=1305141148" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/BreadcrumbMenu.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>Areas/AccountsAnalysis/Content/Styles/HeaderMessage.css" rel="stylesheet" type="text/css" />		
		<link href="<%= sBaseUrl %>CMSContent/Styles/main.css?v=1.0" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/982_12_10.css" rel="stylesheet" type="text/css" />
        <link href="<%= sBaseUrl %>CMSContent/Styles/jquery-ui.css" rel="stylesheet" type="text/css" />             
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthdialog.css" rel="stylesheet" type="text/css" />
        <link href="<%= sBaseUrl %>CMSContent/Styles/jquery.selectBox.genworth.css" rel="stylesheet" type="text/css" />

		<script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-1.10.2.min.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-ui-1.10.3.min.js" type="text/javascript"></script> 

        <asp:PlaceHolder ID="pResources" runat="server"></asp:PlaceHolder>

        <script src="<%= sBaseUrl %>CMSContent/Scripts/genworth.flyForm.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.genworth.ajaxSelector.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.genworth.countindicator.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.clearfield.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Layout.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Search.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/MeetingModeAjaxHelper.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Help.js" type="text/javascript"></script>

        <%-- feedback: file differences, do not move to EWM before reconciling --%>
        <script src="<%= Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ?  string.Concat(sBaseUrl, "CMSContent/Scripts/FeedBack.js") : "/Areas/User/Content/Scripts/FeedBack.js"%>" type="text/javascript"></script>

        <script type="text/javascript" src="<%= sBaseUrl %>Areas/Common/Content/Scripts/SiteCatalyst.js"></script>
	</head>
	<body id="tBody" runat="server">
		<% 
			int iScrollPosition;
			bool bScrolled = Int32.TryParse(Request.QueryString["y"], out iScrollPosition);
			string sScrollStyle = "position:absolute;";
			sScrollStyle += bScrolled ? string.Format("top: {0}px;", iScrollPosition) : string.Empty;
		%>
		<a name="scroll" style="<%= sScrollStyle %>"></a>
		
			<div id="wrapper">
				<div id="wrapper-inner">
					<sc:Placeholder ID="Placeholder1" Key="header" runat="server" />
				
					<div id="sitecore-content">
						<div class="grid-system g982">
							<sc:Placeholder ID="Placeholder2" Key="Body" runat="server" />
							<div class="clear"></div>
						</div>
					</div>
					<sc:Placeholder ID="Placeholder3" Key="Footer" runat="server" />
				</div>
                <div id="Div1" runat="server" >
                  <img src="<%= Page.ResolveClientUrl("~/") %>CMSContent/Images/container_bg_bottom.png" class="container_bg_bottom" border="0" alt=""/>
                  </div>
			</div>
			<asp:Literal ID="lDebug" runat="server" />
            <form method="post" runat="server" id="mainform">
			    <input id="intScrollPosition" runat="server" type="hidden" value="200" />
		    </form>

        <script language="javascript" type="text/javascript">
            setupTimeout();
        </script>
	</body>
</html>
