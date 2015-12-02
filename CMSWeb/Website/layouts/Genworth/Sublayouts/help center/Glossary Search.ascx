<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>
<script runat="server">
    
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!Page.IsPostBack)
		{
			LoadSearchText();
		}
		
	}

	private void LoadSearchText()
	{
		string sKeyword = Request.QueryString[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword];

		if (!String.IsNullOrWhiteSpace(sKeyword))
		{
			tbSearchKeywords.Text = sKeyword;
		}
	}
	
	/// <summary>
	/// When the user clicks the search button, it will redirect it to the same page with the query string parameters
	/// for searching with those keywords
	/// </summary>
	void ibSearchButton_Click(Object sender, EventArgs e)
	{
		// add the textbox contents to the existing query string
		//
		GenworthQueryString oQs = GenworthQueryString.Current;
		oQs.Clear();
        oQs.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword, tbSearchKeywords.Text, true);
		Response.Redirect(ContextExtension.CurrentItem.GetURL() + oQs.ToString(), true);
	}
</script>

<div class="search-block help-search">
    <span class="text-input search"> 
        <span class="input">
		    <asp:TextBox ID="tbSearchKeywords" example="Search Glossary" Width="150px" runat="server"></asp:TextBox>
		    <img src="/CMSContent/Images/icon_magnify.png" onclick="$('#<% = imgGlossarySearch.ClientID %>').click();" style="cursor: pointer;" class="magnify" />
		    <input id="imgGlossarySearch" type="image" src="/CMSContent/Images/icon_magnify_hl.png" runat="server" onserverclick="ibSearchButton_Click" class="magnify" />
        </span>
    </span>
	<sc:Placeholder ID="scAlphabet" Key="alphabet" runat="server" />
</div> 
