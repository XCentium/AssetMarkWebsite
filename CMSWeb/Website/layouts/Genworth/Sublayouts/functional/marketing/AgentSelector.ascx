<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt.Marketing" %>
<%@ Import Namespace="GFWM.Common.Preference.Entities.Response" %>
<%@ Import Namespace="GFWM.Common.Preference.Entities.Data" %>

<script runat="server">
    private const int AgentUserRole = 6;
    	
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
		ListItem liAgent;
		Item oAdvisorPage = ContextExtension.CurrentItem;
		lTitle.Text = oAdvisorPage.GetText("Page", "Title");
		lSummary.Text = oAdvisorPage.GetText("Page", "Summary");

        string qsUrl = this.Request.QueryString["url"] != null ? this.Request.QueryString["url"] : string.Empty;

		string sURLRedirect = string.Empty;

        var agents = MarketingLogic.GetUserRoles(AgentUserRole);
        if (agents.Count() > 0)
        {
            dAgents.Items.Add(new ListItem("Choose an Advisor ID||", string.Empty));
            foreach (var agent in agents)
            {
                liAgent = new ListItem(agent.Value, agent.Key);
                dAgents.Items.Add(liAgent);
            }
        }
        dAgents.DataBind();
         
    }

    private string BuildDropDownListText(IEnumerable<MyProfileInformation> response)
    {
        var profile = response.FirstOrDefault();
        if (profile != null)
        {
            return string.Format("{0}|{1}|{2}", profile.UserID.PadRight(15), (profile.Firstname + " " + profile.Lastname).Trim().PadRight(30), "".PadLeft(14));
        }
        return string.Empty;
    }
    
    
</script>

<div id="MyUniqeId" class="dialog-wrapper">
    <div class="dialog">
        <h2><asp:Literal ID="lTitle" runat="server" /></h2>
        <div class="dialog-content">
			<asp:Literal ID="lSummary" runat="server" />
            <div class="clear"></div>
            <div class="submit">
            <form id="mainform" method="post" runat="server">
                <asp:DropDownList runat="server" CssClass="selChooseProvider" ID="dAgents" >
				</asp:DropDownList>
                <span class="button">
					<span style="display:none !important;">
						<asp:Button runat="server" ID="btnContinue" Text="Continue" CausesValidation="true"/>
					</span>
					<a href="#" id="aLaunchUnBlockedPopUp">Continue</a>
                </span>
                <div id="loadingMessage" title="Loading Data"><img src="/Shared/Images/gridLoader.gif" alt="Loading Data" /></div>
            </form>
            </div>            
        </div>
    </div>
</div>
<script language="javascript" type="text/javascript">
    $(function () {
        Genworth.Marketing.AgentSelectorLoad();
    });

    agentSelected = function () {
        var agentId = $('#<%= dAgents.ClientID %>').val();
        window.parent.AgentIdSelected(agentId);
    }
</script>
