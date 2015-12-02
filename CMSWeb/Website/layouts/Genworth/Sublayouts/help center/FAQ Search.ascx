<%@ Control Language="c#" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>
<script runat="server">
    
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
		Item oParent;
		List<Item> oCategories;
		// text that will be shown in the drop down box
		string sCategoryName;
		
		// load/bind the categories
		//
		ddListCategory.Items.Add(new ListItem(Genworth.SitecoreExt.Constants.HelpCenter.FAQ.CategoryAll, 
											  Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue));
		
		if ((oParent = Sitecore.Context.Database.GetItem("/sitecore/content/Meta-Data/FAQ Categories/")) != null)
		{
			if ((oCategories = oParent.GetChildrenOfTemplate("FAQ Category")) != null)
			{
				foreach(var oCategory in oCategories)
				{				
					sCategoryName = (!String.IsNullOrWhiteSpace(sCategoryName = oCategory.GetText("FAQ Category", "Title"))) ? sCategoryName : oCategory.DisplayName;
					ddListCategory.Items.Add(new ListItem(sCategoryName, oCategory.ID.ToString()));
				}
			}
		}

		SetDefaultValues();
		SetSearchedData();
	}

	/// <summary>
	/// This method will set the values that were searched by the user in the 
	/// correct form. 
	/// For instance, if the user searched a specific category, that category
	/// should remain selected. Also the text that was searched.
	/// </summary>
	private void SetSearchedData()
	{
		GenworthQueryString oQueryString = GenworthQueryString.Current;
		string sKeyword = oQueryString.FAQKeyword;
		string sCategory = oQueryString.FAQCategory ?? String.Empty;

		// set the search box text
		if (!String.IsNullOrWhiteSpace(sKeyword))
		{
			tbSearchKeywords.Text = sKeyword;
		}
		
		// set the category
		ddListCategory.SelectedValue = (!String.IsNullOrWhiteSpace(sCategory) && IsValidCategory(sCategory)) 
			? sCategory : Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue;
	}

	/// <summary>
	/// Set the watermark text for the Search box
	/// </summary>
	private void SetDefaultValues()
	{
		tbSearchKeywords.Attributes.Add("example", Genworth.SitecoreExt.Constants.HelpCenter.FAQ.DefaultSearchText);
	}

	/// <summary>
	/// Checks if the category is valid or was corrupted in the Url.
	/// </summary>
	/// <param name="sCategoryValue">The category name to validate</param>
	/// <returns></returns>
	private bool IsValidCategory(string sCategoryValue)
	{
		return (ddListCategory.Items.FindByValue(sCategoryValue) != null);
	}

	protected void ddListCategory_OnSelectedIndexChanged(Object sender, EventArgs e)
	{
		TriggerSearch();
	}
	
	protected void btnFAQSearch_Click(object sender, EventArgs e)
	{
		TriggerSearch();
	}

	/// <summary>
	/// Creates the URL with the Query Parameters needed to make the search
	/// and redirects to the same page in order to trigger the search in the
	/// FAQ List control
	/// </summary>
	private void TriggerSearch()
	{
		string sKeyword;
		GenworthQueryString oQs;

		// wrap the current query string
		//
		oQs = GenworthQueryString.Current;
		oQs.RemovePagination();
		
		if (!String.IsNullOrWhiteSpace(sKeyword = tbSearchKeywords.Text) && !sKeyword.Equals(Genworth.SitecoreExt.Constants.HelpCenter.FAQ.DefaultSearchText))
		{
			oQs.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword, tbSearchKeywords.Text, true);
		}

		oQs.Add(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Category, ddListCategory.SelectedValue,true);

		Response.Redirect(ContextExtension.CurrentItem.GetURL() + oQs.ToString(), true);
	}
	
</script>

<div class="search-block help-search">
	<asp:DropDownList runat="server" ID="ddListCategory" class="dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddListCategory_OnSelectedIndexChanged" />

	<span class="text-input search"> 
		<asp:TextBox ID="tbSearchKeywords" Width="150px" runat="server"></asp:TextBox>
		<img src="/CMSContent/Images/icon_magnify.png" onclick="$('#<% = btnFAQSearch.ClientID %>').click();" style="cursor: pointer;" class="magnify" />
		<input id="btnFAQSearch" type="image" src="/CMSContent/Images/icon_magnify_hl.png" runat="server" onserverclick="btnFAQSearch_Click" class="magnify" />
    </span>
</div>