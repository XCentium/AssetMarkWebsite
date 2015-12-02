<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Collections" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">
		

	protected void lbEventSearch_Click(object sender, EventArgs e)
	{
		double dRadioInmiles;

		if (Page.IsValid)
		{
			if (!string.IsNullOrEmpty(tbSearchKeywords.Text))
			{
				if (!string.IsNullOrEmpty(tbZipcode.Text))
				{
					double.TryParse(ddlProximity.SelectedValue, out dRadioInmiles);
					EventHelper.SearchEvents(tbSearchKeywords.Text, dRadioInmiles, tbZipcode.Text);
				}
				else
				{
					//Search without zip code
					EventHelper.SearchEvents(tbSearchKeywords.Text);
				}
			}
		}
	}

</script>
<div class="event-search inline-form-block">
    <h3>
        Search Events</h3>
    <span class="text-input event-search-string">
		<asp:TextBox ID="tbSearchKeywords" runat="server"></asp:TextBox>        
    </span>within <span class="select-input event-search-proximity" style="width: 100px;">
		<asp:DropDownList ID="ddlProximity" runat="server">
			<asp:ListItem Text="50 Miles" Value="50"></asp:ListItem>
			<asp:ListItem Text="100 Miles" Value="100"></asp:ListItem>
		</asp:DropDownList>        
    </span>of <span class="text-input event-search-zipcode">
		<asp:TextBox ID="tbZipcode" runat="server" Width="75"></asp:TextBox>        
    </span><span class="button event-search-button"><asp:LinkButton ID="lbEventSearch" runat="server" Text="search"></asp:LinkButton><asp:RequiredFieldValidator ID="rfvSearchKeywords" runat="server" ControlToValidate="tbSearchKeywords"></asp:RequiredFieldValidator> </span>
    
    <!-- OR USE ANY ONE OF THESE
                <span class="button event-search-button">
                    <input type="button" value="Search Button Text" />
                </span>

                <span class="button event-search-button">
                    <input type="submit" value="Search" />
                </span>

                <span class="button event-search-button">
                    <button value="Search">Search</button>
                </span>

                <span class="button event-search-button">
                    <span>Search</span>
                </span>
                -->
</div>