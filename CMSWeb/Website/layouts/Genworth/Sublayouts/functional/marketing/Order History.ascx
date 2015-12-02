<%@ Control Language="c#" %>
<asp:Repeater ID="rEventListing" runat="server">
	<ItemTemplate>
		<asp:HyperLink ID="hLink" runat="server" />
		<asp:Literal ID="lIntroduction" runat="server" />
		<asp:Literal ID="lOrderDate" runat="server" />
	</ItemTemplate>
</asp:Repeater>