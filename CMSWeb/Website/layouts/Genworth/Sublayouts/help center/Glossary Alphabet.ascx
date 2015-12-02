<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<script runat="server">
    
	// will hold the value of the parameter StartsWith on the QueryString
	private string sStartsWith;
	private int iTotalLetterLinks;
	private Dictionary<string, int> oAvailableLetters;

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);

		if (!Page.IsPostBack)
		{
			string sKeyword = Request.QueryString[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword];
            string sGlossaryTermID = Request.QueryString[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.ID];
            sStartsWith = Request.QueryString[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.StartsWith];

            if (String.IsNullOrWhiteSpace(sKeyword) && String.IsNullOrWhiteSpace(sStartsWith) && String.IsNullOrEmpty(sGlossaryTermID))
			{
				sStartsWith = Genworth.SitecoreExt.Constants.HelpCenter.Alphabet.DefaultLetter;
			}

			// Avoid null exceptions having this variable set to empty instad of null
			sStartsWith = (String.IsNullOrWhiteSpace(sStartsWith)) ? String.Empty : sStartsWith;

			BindData();
		}
	}

	public void BindData()
	{
		List<string> lAlphabet = new List<string>();

		// Create a list with all the alphabet
		for (char i = 'A'; i <= 'Z'; i++)
		{
			lAlphabet.Add(i.ToString());
		}

		lAlphabet.Add(Genworth.SitecoreExt.Constants.HelpCenter.Alphabet.SearchAllDisplay);
		iTotalLetterLinks = lAlphabet.Count;

		rGlossaryTerms.DataSource = lAlphabet;
		rGlossaryTerms.DataBind();

	}

	void rGlossaryTerms_ItemDataBound(Object Sender, RepeaterItemEventArgs e)
	{
		if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
		{
			string sAlphabetLetter;
			HyperLink hLetterLink;
			Label lLetter;
			RepeaterItem oRepeaterItem = e.Item;
			sAlphabetLetter = oRepeaterItem.DataItem as string;
			bool bLetterHasResults;

			if (sAlphabetLetter != null)
			{

				// in case there are items with that initial letter a link should be created
				// if not we should jsut add a plain letter
				if (bLetterHasResults = this.LetterHasResults(sAlphabetLetter))
				{
					hLetterLink = oRepeaterItem.FindControl("hLetterLink") as HyperLink;

					if (hLetterLink != null)
					{
						hLetterLink.Text = sAlphabetLetter;
						hLetterLink.CssClass = GetCssClass(e.Item.ItemIndex, sAlphabetLetter, true);
						SetNavigationUrl(hLetterLink, sAlphabetLetter);
						hLetterLink.Visible = true;
					}
				}
				else
				{
					lLetter = oRepeaterItem.FindControl("lLetter") as Label;
					if (lLetter != null)
					{
						lLetter.Text = sAlphabetLetter;
						lLetter.Visible = true;
						lLetter.CssClass = GetCssClass(e.Item.ItemIndex, sAlphabetLetter, false);
					}
				}
			}
		}
	}

	/// <summary>
	/// This will return true in case there are some result for that letter
	/// </summary>
	/// <param name="sLetter"></param>
	/// <returns></returns>
	private bool LetterHasResults(string sLetter)
	{
		return AvailableLetters.Keys.Contains(sLetter) ||
			(sLetter.Equals(Genworth.SitecoreExt.Constants.HelpCenter.Alphabet.SearchAllDisplay) && AvailableLetters.Keys.Count > 0);
	}

	private Dictionary<string, int> AvailableLetters
	{
		get
		{
			if (oAvailableLetters == null)
			{
				oAvailableLetters = HelpCenterHelper.GetInitialLetters();
			}

			return oAvailableLetters;
		}
	}

	private string GetCssClass(int iCurrentIndex, string sCurrentLetter, bool bLetterHasResults)
	{
		List<string> sCssClasses = new List<string>();

		// if is the first link we should add the class "first" 
		if (iCurrentIndex == 0)
		{
			sCssClasses.Add("first");
		}
		// if is the final link we shoild add the class "last"
		else if (iTotalLetterLinks > 1 && iCurrentIndex == (iTotalLetterLinks - 1))
		{
			sCssClasses.Add("last");
		}

		// if the current letter is the same that the user selected add the class "selected"
		if (sCurrentLetter.Equals(sStartsWith)
			|| (sCurrentLetter.Equals(Genworth.SitecoreExt.Constants.HelpCenter.Alphabet.SearchAllDisplay)
				&& (sStartsWith.Equals(Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue))))
		{
			sCssClasses.Add("selected");
		}
		else
			// if the current letter doesn't have any results show it with a special style (currently gray)
			if (!bLetterHasResults)
			{
				sCssClasses.Add("no-results");
			}

		return String.Join(" ", sCssClasses.ToArray());
	}

	private void SetNavigationUrl(HyperLink lLetterLink, string sCurrentLetter)
	{

		string sSearchValue = (sCurrentLetter.Equals(Genworth.SitecoreExt.Constants.HelpCenter.Alphabet.SearchAllDisplay))
						? Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.SearchAllValue : sCurrentLetter;

		lLetterLink.NavigateUrl = string.Format("{0}?{1}={2}", ContextExtension.CurrentItem.GetURL(), Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.StartsWith.ToString(), sSearchValue);
	} 
	
</script>
<span class="search-by-letter">
	<asp:Repeater ID="rGlossaryTerms" runat="server" OnItemDataBound="rGlossaryTerms_ItemDataBound">
		<ItemTemplate>
			<asp:HyperLink ID="hLetterLink" runat="server" Visible="false"></asp:HyperLink>
			<asp:Label ID="lLetter" runat="server" Visible="false"></asp:Label>
		</ItemTemplate>
	</asp:Repeater>
</span>