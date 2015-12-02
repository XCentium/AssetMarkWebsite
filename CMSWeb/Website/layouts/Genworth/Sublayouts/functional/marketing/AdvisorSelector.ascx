<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>

<script runat="server">
			
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            BindData();
			aLaunchUnBlockedPopUp.Visible = true;
        }
		string sNoLinkScript = "JavaScript: var i = 1;";
		aLaunchUnBlockedPopUp.Attributes.Add("href", sNoLinkScript);
		dAdvisors.Attributes.Add("onload", "alert('loaded');");
		dAdvisors.Attributes.Add("onchange", "JavaScript: var s = $('option:selected', this).attr('url'); var o = $('#" + aLaunchUnBlockedPopUp.ClientID + "'); window.CLOSE_MODAL_WINDOW = (s!='NO-LINK'); var h = !window.CLOSE_MODAL_WINDOW ? \"" + sNoLinkScript + "\": s; o.attr('href',h);");
    }

    private void BindData()
    {
		ListItem liAdvisor;
		Item oAdvisorPage = ContextExtension.CurrentItem;
		lTitle.Text = oAdvisorPage.GetText("Page", "Title");
		lSubtitle.Text = oAdvisorPage.GetText("Page", "Sub Title");
		lSummary.Text = oAdvisorPage.GetText("Page", "Summary");

		string sVendor = this.Request.QueryString[Marketing.Vendor] != null ? this.Request.QueryString[Marketing.Vendor] : string.Empty;

		string sURLRedirect = string.Empty;

		foreach (string val in MarketingLogic.GetAdvisors())
		{
			liAdvisor = new ListItem(val);
			sURLRedirect = MarketingLogic.BuildOrderURL(sVendor, val);
			liAdvisor.Attributes.Add("url", sURLRedirect);
			dAdvisors.Items.Add(liAdvisor);
		}
		
		dAdvisors.DataBind();
         
    }      
    
    
</script>
<div id="MyUniqeId" class="dialog-wrapper">
    <div class="dialog">
        <h2><asp:Literal ID="lTitle" runat="server" /></h2>
        <div class="dialog-content">
            <h3><asp:Literal ID="lSubtitle" runat="server" /></h3>
			<asp:Literal ID="lSummary" runat="server" />
            <div class="clear"></div>
            <div class="submit">
                <asp:DropDownList runat="server" CssClass="selChooseProvider" ID="dAdvisors" >
			
				</asp:DropDownList>
                <span class="button">
					<span style="display:none !important;">
						<asp:Button runat="server" ID="bContinue" Text="Continue" CausesValidation="true"/>
					</span>
					<a id="aLaunchUnBlockedPopUp" runat="server" target="_blank" onclick="CloseAdvisorsShadowBox();">Continue</a>
                </span>
            </div>
        </div>
    </div>
</div>
<script language="javascript" type="text/javascript">
	function CloseAdvisorsShadowBox() {
		var bReturn = window.CLOSE_MODAL_WINDOW == true;
		if (bReturn) {
			var t = setTimeout(function () { parent.dialogBox.close(); }, 100);
		}
	}
	$(function () {
		var s = $('.selChooseProvider option:first').attr('url');
		var o = $('#<%= aLaunchUnBlockedPopUp.ClientID %>');
		window.CLOSE_MODAL_WINDOW = (s != 'NO-LINK');
		var h = !window.CLOSE_MODAL_WINDOW ? 'JavaScript: var i;' : s;
		o.attr('href', h);
	});
</script>
