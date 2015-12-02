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
    private const string HtmlTarget = "portal_iframe";
    private const string DefaultAgentId = "AG1634";
    
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
        Item oCurrentItem = ContextExtension.CurrentItem;
      
        oShadowPage = oCurrentItem.GetChildrenOfTemplate("Shadow Page").FirstOrDefault();

        string shadowPageUrl = oShadowPage.GetURL();

        string qsUrl = this.Request.QueryString["url"] != null ? this.Request.QueryString["url"] : string.Empty;
        string aplId = CurrentUserRoles.Keys.FirstOrDefault() ?? string.Empty;

        if (CurrentAgentsCount > 1)
        {
            string newURL = string.Empty;
            aplId = DefaultAgentId;

            if (!string.IsNullOrEmpty(qsUrl))
            {
                newURL = string.Format("{0}?url={1}", shadowPageUrl, qsUrl);
            }
            else
            {
                newURL = shadowPageUrl;
            }

            hSelector.NavigateUrl = newURL;
            hSelector.Attributes.Add("rel", "shadowbox;width=600px;height=400px;modal=yes;hideClose=yes");
        }
        else if (CurrentUserRoles.Count > 0)
        {
            // Hide original button
            spanButton.Visible = false;
            hSelector.Visible = false;
        }

        // Create a form on the fly.
        if (string.IsNullOrEmpty(qsUrl))
        {
            lHtmlForms.Text = Genworth.SitecoreExt.Marketing.Controller.CreateStandardRegisterHtmlForm(aplId, HtmlTarget);
        }
        else
        {
            if (CurrentAgentsCount > 1)
            {
                // ignore url parameter if multi-agent, since default authentication is made with AG1634 agent
                lHtmlForms.Text = Genworth.SitecoreExt.Marketing.Controller.CreateStandardRegisterHtmlForm(aplId, HtmlTarget);
            }
            else
            {
                lHtmlForms.Text = Genworth.SitecoreExt.Marketing.Controller.CreateStandardRegisterHtmlForm(aplId, HtmlTarget, scriptSubmit: false, actionUrl: qsUrl);
            }
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

    LoadDefaultUserLandingPage = function () {
        // default action
        // 1. Submit form        
        $('form[id^="postForm"]').submit();
        var $hSelector = $("#<%= hSelector.ClientID %>");
        if ($hSelector.length > 0) {

            // 2. Find Order button in case of multiple users
            $hSelector.click(function (e) {
                e.preventDefault();
                var href = this.href;
                $.get(href, function () {
                    LoadAgentSelectorShadowPage();
                });
            });
            $hSelector.click();
        } else {
            ShowLoading();
        }
    }

    LoadAgentSelectorShadowPage = function () {
        var url = GetUrlParameter();
        var agentsCount = parseInt($('#hdAgentsCount').attr('value'));

        // Multiple agents
        if (agentsCount > 1) {
            var shadowUrl = $('#hdShadowPageUrl').attr('value');
            var hShadow = $('#<%= hShadow.ClientID %>');

            // deeplinking
            if (!(url === undefined || url == null || url.length <= 0)) {
                hShadow.attr('href', shadowUrl + '?url=' + url);
            } else {
                hShadow.attr('href', shadowUrl);
            }
            hShadow.click();
        }
    }

    AgentIdSelected = function (agentId) {

        var wintarget = "<%= HtmlTarget %>";

        $.getJSON(GetServiceURL(agentId), null, function (data) {
            data = $.extend({
                target: wintarget
            }, data);
            ShowLoading();
            Genworth.flyForm(data);
            window.dialogBox.close();
            GetAgentList(agentId);
        }).fail(function (jqxhr, textStatus, error) {
            alert(error);
        });
    }

    GetServiceURL = function (agentId) {
        var url = "/services/Marketing.svc/GetFormData/" + agentId + "/";

        var urlParam = GetUrlParameter();
        if (!(urlParam === undefined || urlParam == null || urlParam.length <= 0)) {
            url += "?url=" + urlParam;
        }

        return url;
    }

    GetUrlParameter = function () {
        var url = '';
        var params = '';
        var urlFound = false;
        if (window.location.href.indexOf('?')) {
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                if (hashes[i].indexOf('url=') >= 0) {
                    var start = hashes[i].indexOf('=') + 1;
                    var end = hashes[i].length;
                    url = hashes[i].substring(start, end);
                    urlFound = true;
                }
                else {
                    params = params + '&' + hashes[i]
                }
            }
            if (urlFound) {
                url = url + params;
            }
        }
        return url;
    }

    GetAgentList = function (agentId) {

        var agentsCount = parseInt($('#hdAgentsCount').attr('value'));
        if (agentsCount > 1) {
            var userRole = "6";
            var url = "/services/Marketing.svc/GetUserRoles/" + userRole;
            $.getJSON(url, null, function (data) {
                LoadAgentDropDownList(agentId, data)
            }).fail(function (jqxhr, textStatus, error) {
                alert(error);
            });
        }
    }

    LoadAgentDropDownList = function (agentId, data) {

        var agentListContainer = $("#agentList");
        var agentListDropDown = $("#dAgents");

        if ($("#dAgents:empty").length > 0) {
            $('<option value="">Choose an Advisor ID||</option>').appendTo(agentListDropDown);
            $.each(data, function () {
                $('<option value="' + this.Key + '">' + this.Value + '</option>').appendTo(agentListDropDown);
            });
        }
        agentListDropDown.val(agentId);
        Genworth.Marketing.AgentSelectorLoad();
        agentListContainer.show();
    }

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
        var agentListDropDown = $("#dAgents");
        agentListDropDown.change(function () {
            var agentId = this.value;
            if (agentId == '') return;
            AgentIdSelected(agentId);
        });
    }

</script>

<div class="Marketing">
<% 
            Item oCurrentItem = ContextExtension.CurrentItem;
            oShadowPage = oCurrentItem.GetChildrenOfTemplate("Shadow Page").FirstOrDefault();
            string shadowPageUrl = oShadowPage.GetURL();
            string agentsCount = CurrentAgentsCount.ToString();
            string userRolesCount = CurrentUserRoles.Count.ToString();
%>
    <div id="agentList">     
        <select class="selChooseProvider" id="dAgents"></select>   
    </div>
    <div id="loadingMessage" title="Loading Data"><img src="/Shared/Images/gridLoader.gif" alt="Loading Data" /></div>
    <iframe id="iframeChild" name="<%= HtmlTarget %>" frameBorder="0" src="" scrolling="yes" height="850" width="975"></iframe>
    <asp:Literal ID="lHtmlForms" runat="server" />
    <span runat="server" id="spanButton" class="button" style="display:none;">
        <asp:HyperLink runat="server" ID="hSelector" Text="Order" ></asp:HyperLink>
        <asp:HyperLink runat="server" ID="hShadow" Text="Order" ></asp:HyperLink>
    </span>
   <input type="hidden" value="<%= shadowPageUrl %>" ID="hdShadowPageUrl"/>
    <input type="hidden" value="<%= agentsCount %>" ID="hdAgentsCount"/>
    <input type="hidden" value="<%= userRolesCount %>" ID="hdUserRolesCount"/>
</div>



 
