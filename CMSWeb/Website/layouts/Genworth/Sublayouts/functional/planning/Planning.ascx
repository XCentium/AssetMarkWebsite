<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<div id="Planning">
    <sc:Placeholder ID="Placeholder1" Key="Planning" runat="server" />
</div>

<script type="text/javascript">
    $(function () {
        var body = $('body');
        if (body.is('.finance-logix-client-experience')) {
            Genworth.Planning.ClientExperience();
        }
        if (body.is('.overview')) {
            Genworth.Planning.Overview();
        }
    });
</script>
