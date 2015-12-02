<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Genworth.SitecoreExt.Marketing" %>
<%@ Import Namespace="Genworth.SitecoreExt.Security" %>

<script runat="server">
    
    private const int SelectTheOnlyAdvisor = 0;
    private const int AgentUserRole = 6;
    private const string DefaultAgentId = "AG1634";
    private string htmlSource = string.Empty;
    
    Item oShadowPage =null;
    private Dictionary<string, string> currentRoles;

    private Dictionary<string, string> CurrentUserRoles
    {
        get
        {
            if (currentRoles == null)
            {
                currentRoles = MarketingLogic.GetUserRoles();
            }
            return currentRoles;
        }
    }

    private int? currentAgentsCount;

    private int CurrentAgentsCount
    {
        get
        {
            if (!currentAgentsCount.HasValue)
            {
                currentAgentsCount = MarketingLogic.GetUserRoles(AgentUserRole).Count;
            }
            return currentAgentsCount.Value;
        }
    }
    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!Page.IsPostBack)
        {
            BindData();
        }
        
    }

    private void BindData()
    {
        string qsPagename = this.Request.QueryString["page"] != null ? this.Request.QueryString["page"] : string.Empty;
        string qsProductId = this.Request.QueryString["product"] != null ? this.Request.QueryString["product"] : string.Empty;
        string aplId = CurrentUserRoles.Keys.FirstOrDefault() ?? string.Empty;
        SSOMessage request = null;

        if (CurrentAgentsCount > 1)
        {
            ListItem liAgent;
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
        else if (CurrentUserRoles.Count > 0)
        {
            dAgents.Visible = false;
            request = Genworth.SitecoreExt.Marketing.Controller.BuildMarcomCentralAuthenticationRequest(aplId, qsPagename, qsProductId);
            htmlSource = Genworth.SitecoreExt.Marketing.Controller.SendMarcomCentralAuthenticationRequest(request);
        }        
    }
    
</script>

<script type="text/javascript">

    $(document).ready(function () {
        ConfigureiFrameLoadedEvent();
        ConfigureLoadingDialog();
        ConfigureAgentListDropDown();
        LoadDefaultUserLandingPage();
    });

    var urlParams;
    (window.onpopstate = function () {
        var match,
            pl = /\+/g,  // Regex for replacing addition symbol with a space
            search = /([^&=]+)=?([^&]*)/g,
            decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
            query = window.location.search.substring(1);

        urlParams = {};
        while (match = search.exec(query))
            urlParams[decode(match[1])] = decode(match[2]);
    })();

    ShowLoading = function () {
        $("#loadingMessage").dialog("open");
    }

    HideLoading = function () {
        $("#loadingMessage").dialog("close");
    }

    ConfigureiFrameLoadedEvent = function () {
        $("#iframeChild").load(function () {
            HideLoading();
        });
    }

    ConfigureLoadingDialog = function () {
        $("#loadingMessage").dialog({
            resizable: false,
            height: 160,
            zIndex: 10000,
            modal: true,
            autoOpen: false
        });
    }

    ConfigureAgentListDropDown = function () {
        var agentListDropDown = $("#<%= dAgents.ClientID %>");
        agentListDropDown.change(function () {
            var agentId = this.value;
            if (agentId == '') return;
            ShowLoading();
            RemoveEmptyOptionFromSelector();
            AgentIdSelected(agentId);
        });
    }

    RemoveEmptyOptionFromSelector = function () {
        var emptyOption = $("#<%= dAgents.ClientID %> option[value='']");
        if (emptyOption.length > 0) {
            emptyOption.remove();
            var anchors = $('.selectBox-dropdown-menu-list-container .selectBox-dropdown-menu-list li a');
            if (anchors.length > 0) {
                anchors.each(function (i) {
                    if ($(this).attr('rel') == '') {
                        $(this).hide();
                    }
                });
            }
        }
    }

    LoadDefaultUserLandingPage = function () {
        var agentsCount = parseInt($('#hdAgentsCount').attr('value'));
        if (agentsCount > 1) {
            var agentListContainer = $("#agentList");
            var agentListDropDown = $("#<%= dAgents.ClientID %>");
            Genworth.Marketing.MarcomCentralAgentSelectorLoad();
            agentListDropDown.val('');
            agentListContainer.show();
        }
    }

    AgentIdSelected = function (agentId) {

        $.getJSON(GetServiceURL(agentId), null, function (url) {
            $('#iframeChild').attr('src', url);
        }).fail(function (jqxhr, textStatus, error) {
            alert(error);
        });
    }

    GetServiceURL = function (agentId) {
        var url = "/services/Marketing.svc/GetRequestUrl/" + agentId + "/";
        url = AttachParameters(url);
        return url;
    }

    AttachParameters = function (url) {
        var pageParam = urlParams['page'];
        var productParam = urlParams['product'];
        if (pageParam === undefined || pageParam == null || pageParam.length <= 0) {
            pageParam = '';
        }
        if (productParam === undefined || productParam == null || productParam.length <= 0) {
            productParam = '';
        }
        url += "?page=" + pageParam + "&product=" + productParam;;
        return url;
    }


</script>

<style type="text/css">
    .portal .grid-system .gc 
    {
        margin-left: -4px;
    }
</style>

<div class="Marketing">
<% 
            Item oCurrentItem = ContextExtension.CurrentItem;
            string agentsCount = CurrentAgentsCount.ToString();
            string userRolesCount = CurrentUserRoles.Count.ToString();
%>
    <div id="agentList">     
        <asp:DropDownList runat="server" AutoPostBack="false" CssClass="selChooseProvider" ID="dAgents" >
        </asp:DropDownList>
    </div>
    <div id="loadingMessage" title="Loading Data"><img src="/Shared/Images/gridLoader.gif" alt="Loading Data" /></div>
    <iframe id="iframeChild" name="portal_iframe" frameBorder="0" src="<%= htmlSource %>" height="780" width="990"></iframe>
    <input type="hidden" value="<%= agentsCount %>" ID="hdAgentsCount"/>
    <input type="hidden" value="<%= userRolesCount %>" ID="hdUserRolesCount"/>
</div>



 
