<%@ Page Language="c#" CodePage="65001" AutoEventWireup="true" %>

<%@ Implements Interface="ServerLogic.SitecoreExt.Async.IAsyncLayout" %>
<%@ Import Namespace="ServerLogic.SitecoreExt.Async" %>
<%@ OutputCache Location="None" VaryByParam="none" %>

<script runat="server">
	private string sVersion = "1.0";
	private DateTime dStartTime;
	private List<IAsyncSublayout> oSublayouts = new List<IAsyncSublayout>();
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!Page.IsPostBack)
		{
			BindData();
		}

        // go get the proper bundle url from EWM and then inject into the head block on this page.
        string bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/IFramePage", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Scripts);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddJsToPage(this.Page, bundleUrl);
        }

        bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/IFramePage", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Styles);

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
		dVersion = GetInternetExplorerVersion();
		if (dVersion != -1.0)
		{
			string sIeCssClass;
			sIeCssClass = "iFrame ie ie" + Math.Floor(dVersion).ToString();
			tBody.Attributes.Add("class", String.Format("{0}", sIeCssClass));
		}
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
    <!-- IFramePage.aspx -->
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

    <asp:PlaceHolder ID="pResources" runat="server"></asp:PlaceHolder>

    <%-- feedback: file differences, do not move to EWM before reconciling --%>
    <script src="<%= Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization.IsTestMode ?  string.Concat(sBaseUrl, "CMSContent/Scripts/FeedBack.js") : "/Areas/User/Content/Scripts/FeedBack.js"%>" type="text/javascript"></script>
</head>
<body  id="tBody" runat="server" class="iFrame">
	<% 
		int iScrollPosition;
		bool bScrolled = Int32.TryParse(Request.QueryString["y"], out iScrollPosition);
		//bScrolled = true;
		string sScrollStyle = "position:absolute;";
		sScrollStyle += bScrolled ? string.Format("top: {0}px;", iScrollPosition) : string.Empty;
	%>
	<a name="scroll" style="<%= sScrollStyle %>"></a>
	<form method="post" runat="server" id="mainform">
	<div id="ChildElementId">
		<div class="grid-system g982">
			<sc:Placeholder ID="Placeholder2" Key="Body" runat="server" />
			<div class="clear">
			</div>
		</div>
	</div>
	<asp:Literal ID="lDebug" runat="server" />
	<input id="intScrollPosition" runat="server" type="hidden" value="200" />
	</form>

    <script language="javascript" type="text/javascript">
        setupTimeout();
    </script>
</body>
</html>
