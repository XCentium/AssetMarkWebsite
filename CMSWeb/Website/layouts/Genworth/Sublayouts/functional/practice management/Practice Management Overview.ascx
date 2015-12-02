<%@ Control Language="C#" Debug="true" %>
<div id="PracticeManagementOverview">
    <div class="gc c12 content">
        <sc:Placeholder ID="Content" runat="server" Key="Content" />
    <div class="clear"></div> 
    </div>
    <div class="gc c12 promos">
        <sc:Placeholder ID="Promos" runat="server" Key="Promos" />
    </div>
    <div class="clear"></div>
</div>
<script type="text/javascript">
    $(function () {
        Genworth.PracticeManagement.Overview();
    });
</script>