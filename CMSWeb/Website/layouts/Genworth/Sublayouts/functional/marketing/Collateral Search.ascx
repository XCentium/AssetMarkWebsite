<%@ Control Language="c#" AutoEventWireup="true" %>
<div class="search">
	<div><asp:TextBox ID="tSearch" runat="server" /><asp:LinkButton ID="lSearch" runat="server" OnClick="lSearch_Click" CssClass="autoenter"><asp:Image ID="iSearch" ImageUrl="~/media/img/search.png" runat="server" /></asp:LinkButton></div>
</div>