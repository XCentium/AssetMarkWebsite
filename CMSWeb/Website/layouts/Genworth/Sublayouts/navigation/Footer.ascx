<%@ Control Language="c#" %>
<%@ Implements Interface="ServerLogic.SitecoreExt.Async.IAsyncSublayout" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="ServerLogic.SitecoreExt.Async" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>

<script runat="server">
    
    private const string sDefaultEncoding = "default";
	private Thread oThread;
    
    protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			if (Page is IAsyncLayout)
			{
				((IAsyncLayout)Page).RegisterSublayout(this);
				oThread = new Thread(new ParameterizedThreadStart(BindData));
				oThread.Start(HttpContext.Current);
			}
			else
			{
				BindData(null);
			}
		}
	}

	public void Join()
	{
		oThread.Join();
	}

	private void BindData(object oHttpContext)
	{
		if (oHttpContext != null)
			HttpContext.Current = (HttpContext)oHttpContext;
        string sFooterURL;
        string sEncoding;

		//based on the encoding chosen, tell the web client how to download

        sEncoding = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Footer.Encoding, sDefaultEncoding).ToLower().Trim();

        sFooterURL = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Footer.HtmlURL);
        
        
		//get head from remote system
        hFooter.Text = Genworth.SitecoreExt.Helpers.HTMLIntegrationLogic.GetHtmlFromUrlWithCookies(sEncoding, sFooterURL);
        Sitecore.Diagnostics.Log.Debug(string.Format("Remote HTML: {0}", hFooter.Text), this);
	}
</script><asp:Literal ID="hFooter" runat="server" />