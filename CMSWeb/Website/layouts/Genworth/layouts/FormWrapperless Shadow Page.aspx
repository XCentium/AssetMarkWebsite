<%@ Page Language="c#" CodePage="65001" AutoEventWireup="true" %>
<%@ OutputCache Location="None" VaryByParam="none" %>

<script runat="server">
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // go get the proper bundle url from EWM and then inject into the head block on this page.
        string bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/ShadowPage", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Scripts);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddJsToPage(this.Page, bundleUrl);
        }

        bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/ShadowPage", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Styles);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddCssToPage(this.Page, bundleUrl);
        }
    }
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "https://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html lang="en" xml:lang="en" xmlns="https://www.w3.org/1999/xhtml">
	<head>
        <!-- FormWrapperless Shadow Page.aspx -->
		<title>AssetMark</title>
		<meta name="description" runat="server" id="MetaDescription" />
		<meta name="keywords" runat="server" id="MetaKeywords" />
		<meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
		<meta name="CODE_LANGUAGE" content="C#" />
		<meta name="vs_defaultClientScript" content="JavaScript" />
		
		<% 
			string sBaseUrl;
			sBaseUrl = Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ? Page.ResolveClientUrl("~/") : "/";

            string sCssClassName = " " + Request.Browser.Browser.ToLower();
		%>
		<link href="https://fonts.googleapis.com/css?family=Cabin:600,600italic" rel="stylesheet" type="text/css" />
		<link href="https://fonts.googleapis.com/css?family=Oswald" rel="stylesheet" type="text/css" />
		<link href="https://fonts.googleapis.com/css?family=Cardo&v1" rel="stylesheet" type="text/css" />
        <link rel="icon" href="<%= sBaseUrl %>CMSContent/images/AssetMark_Icon_16.ico" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ellipsis.css" rel="stylesheet" type="text/css" />
		
        <link href="<%= sBaseUrl %>CMSContent/Styles/jquery-ui.css" rel="stylesheet" type="text/css" /> 

		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.tooltip.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/ui.jqgrid.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.skinnable.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthscrollbar.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.selectBox.genworth.css?id=1305151010" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthdropdownpanel.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthcheckbox.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthgrid.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthgridpager.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthdialog.css" rel="stylesheet" type="text/css" />
		<%--<link href="<%= sBaseUrl %>CMSContent/Styles/jquery.ui.genworthtooltip.css" rel="stylesheet" type="text/css" />--%>
		<link href="<%= sBaseUrl %>CMSContent/Scripts/Libraries/ShadowBox/shadowbox.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/Site.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/SecondaryMenu.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/TertiaryMenu.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/VerticalTabs.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/Content.css?id=1305141237" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/Investments.css" rel="stylesheet" type="text/css" />
 
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-1.10.2.min.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
        <script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/Jquery/jquery-ui-1.10.3.min.js" type="text/javascript"></script>

        <asp:PlaceHolder ID="pResources" runat="server"></asp:PlaceHolder>

		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.clearfield.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.unobtrusive-ajax.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.validate.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.validate.unobtrusive.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Libraries/ShadowBox/shadowbox.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/genworthdatevalidation.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/jquery.genworth.countindicator.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Layout.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Search.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/CollapsiblePanel.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/ModalWindow.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Events.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Marketing.js?id=1306141849" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/PracticeManagement.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Toggler.js" type="text/javascript"></script>
		<script src="<%= sBaseUrl %>CMSContent/Scripts/Articles.js" type="text/javascript"></script>

        <%-- feedback: file differences, do not move to EWM before reconciling --%>
        <script src="<%= Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ?  string.Concat(sBaseUrl, "CMSContent/Scripts/FeedBack.js") : "/Areas/User/Content/Scripts/FeedBack.js"%>" type="text/javascript"></script>

		<link href="<%= sBaseUrl %>CMSContent/Styles/BreadcrumbMenu.css" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>Areas/AccountsAnalysis/Content/Styles/HeaderMessage.css" rel="stylesheet" type="text/css" />

		<script src="<%= sBaseUrl %>Areas/AccountsAnalysis/Content/Scripts/HeaderMessage.js" type="text/javascript"></script>
		
		<link href="<%= sBaseUrl %>CMSContent/Styles/main.css?v=1.0" rel="stylesheet" type="text/css" />
		<link href="<%= sBaseUrl %>CMSContent/Styles/982_12_10.css" rel="stylesheet" type="text/css" />
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/main.js?v=1.0"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridCommon.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridInvestments.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridManagerSearch.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/GridEvents.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/jquery.genworth.ajaxSelector.js"></script>
		<script type="text/javascript" src="<%= sBaseUrl %>CMSContent/Scripts/genworth.flyForm.js"></script>
		
	</head>
	<body class="ModalWindowBody<%= sCssClassName %>">
<%--		<form method="post" runat="server" id="mainform">
		</form>--%>
        <sc:Placeholder ID="Placeholder1" Key="Body" runat="server" />

        <script language="javascript" type="text/javascript">
            setupTimeout();
        </script>
	</body>
</html>
