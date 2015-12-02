<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	
	private const string sDefaultEncoding = "utf-8";
	
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
		string sURL;
		Item oItem;
		bool bUseIframe;
		
		oItem = ContextExtension.CurrentItem;

		if (oItem.InstanceOfTemplate("Iframe"))
		{
			bUseIframe = oItem.GetField("Iframe Info", "Use Iframe").Value.Equals("1");
			if (!string.IsNullOrEmpty(sURL = oItem.GetText("Iframe Info", "Source", string.Empty)))
			{

				if (bUseIframe)
				{
					iframe.Visible = true;
					lHTML.Visible = false;

					//If we are using a iFrame									
					iframe.Attributes.Add("src", sURL);
				}
				else
				{
					iframe.Visible = false;
					lHTML.Visible = true;
					//We will get the HTML using a web request based on the encoding chosen					
					
					lHTML.Text = Genworth.SitecoreExt.Helpers.HTMLIntegrationLogic.GetHtmlFromUrlWithCookies(sDefaultEncoding, sURL);

					Sitecore.Diagnostics.Log.Debug(string.Format("Remote HTML: {0}", lHTML.Text), this);
				}
			}
		}
	}		
	
</script>

<asp:Literal ID="lHTML" runat="server" />
<iframe frameborder="0" marginheight="0" marginwidth="0" scrolling="auto" width="100%" height="700px" runat="server" id="iframe" visible="false">
	<noframes></noframes>
</iframe>
