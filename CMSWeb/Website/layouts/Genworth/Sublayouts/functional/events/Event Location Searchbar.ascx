<%@ Control Language="c#" %>
<!-- adding this comment for ticket 20857 -->
<div class="filter-results-block events-location-switch" id="LocationSearchBar">
	<div class="InvestmentSearchBar filter-results-search-block">
        <span class="inline-form-block">
            within 
            <span class="select-input event-search-proximity" style="width: 100px;">
		        <asp:DropDownList ID="ddlProximity" runat="server">            
			        <asp:ListItem Text="50 Miles" Value="50"></asp:ListItem>
			        <asp:ListItem Text="100 Miles" Value="100"></asp:ListItem>
                    <asp:ListItem Text="250 Miles" Value="250"></asp:ListItem>
                    <asp:ListItem Text="All" Value="0"></asp:ListItem>
		        </asp:DropDownList>
	        </span>
            of 
            <span class="text-input event-search-zipcode">
		        <asp:TextBox ID="tbZipcode" runat="server" Width="75"></asp:TextBox>
	        </span>
            <span class="results-search"><a href="#" class="submit"></a></span>
        </span>
	</div>
</div>