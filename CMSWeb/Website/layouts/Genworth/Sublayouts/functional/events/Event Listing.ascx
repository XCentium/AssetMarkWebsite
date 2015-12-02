<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="ServerLogic.Core.Web.Utilities" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities" %>
<script runat="server">
    
    //local constants begins

    /// <summary>
    /// When this value is selected in the ratio search drop down list it means that we are not going to filter by zip code
    /// </summary>
    private const int RatioSearchForAll = 0;
    
    //local constants ends
    
	int iMaxItems;
	Paginator<Item> oPaginator;

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
		string sDate;
		DateTime dDate;
		DateTime dStart;
		DateTime dEnd;
		string sSearchKeywords;
		string sZipCode;
		bool bUseSearchKeywords;
		bool bUseSearchZip;
		List<Item> oEvents;
		double dRadioInmiles = 0;
		string sRadio;

		// wrap the current query string
		//
		GenworthQueryString oQs = GenworthQueryString.Current;

		// get search values from the query string
		//
		bUseSearchKeywords = (!string.IsNullOrEmpty(sSearchKeywords = oQs["search"]));
		bUseSearchZip = (!string.IsNullOrEmpty(sZipCode = oQs["zip"]));
		bUseSearchZip = (!string.IsNullOrEmpty(sRadio = oQs["radio"]) && double.TryParse(sRadio, out dRadioInmiles));

		if (string.IsNullOrEmpty(sDate = oQs["date"]) || !DateTime.TryParse(sDate, out dDate))
		{
			//get current date
			dStart = DateTime.Today;
			dStart.AddDays(-dStart.Day + 1);
			dEnd = dStart.AddMonths(1);
		}
		else
		{
			dStart = dDate;
			dEnd = dStart.AddDays(1).AddSeconds(-1);
		}

		if (bUseSearchKeywords)
		{
			tbSearchKeywords.Text = sSearchKeywords;
			if (bUseSearchZip)
			{
				ddlProximity.SelectedValue = sRadio;
				tbZipcode.Text = sZipCode;
				oEvents = EventHelper.SearchEvents(sSearchKeywords, dRadioInmiles, sZipCode);
			}
			else
			{
				oEvents = EventHelper.SearchEvents(sSearchKeywords);
			}
		}
		else if (bUseSearchZip)
		{
			ddlProximity.SelectedValue = sRadio;
			tbZipcode.Text = sZipCode;
			oEvents = EventHelper.SearchEvents(string.Empty, dRadioInmiles, sZipCode);
		}
		else
		{
			//bind the events
			oEvents = EventHelper.GetEventsByDateRange(dStart, dEnd);
            
		}
		if (oEvents != null && oEvents.Count > 0)
		{

			BindEventListing(oEvents);
		}
		else
			if ((bUseSearchZip || bUseSearchKeywords)) //if it a search and there aren't events show message 
			{
				hNoresults.Visible = true;
				dResults.Visible = false;
			}



	}

	public void BindEventListing(IEnumerable<Item> oEvents)
	{
		GenworthQueryString oQs;

		oPaginator = GenworthPaginator.GetPaginator(oEvents, "EventList");
		rEventListing.DataSource = oPaginator.GetDataset();
		rEventListing.ItemDataBound += new RepeaterItemEventHandler(rEventListing_ItemDataBound);
		iMaxItems = ((IEnumerable<Item>)rEventListing.DataSource).Count();
		rEventListing.DataBind();
		//get our page numbers and bind them to the pages list
		rPages.DataSource = oPaginator.GetPageNumbers();
		rPages.ItemDataBound += new RepeaterItemEventHandler(rPages_ItemDataBound);
		rPages.DataBind();
		rPages.Visible = rPages.Items.Count > 1;

		// bind the paginator
		//
		oQs = GenworthQueryString.Current;
		oPaginator.ConfigureHyperLinkNext(lNext, ContextExtension.CurrentItem.GetURL());
		oPaginator.ConfigureHyperLinkPrevious(lPrevious, ContextExtension.CurrentItem.GetURL());
	}


	private void rPages_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HyperLink hPage;
		Label lSelectedPage;
		int iPageNumber;

		iPageNumber = (int)e.Item.DataItem;

		hPage = (HyperLink)e.Item.FindControl("hPage");
		lSelectedPage = (Label)e.Item.FindControl("lSelectedPage");

		if (oPaginator.PageNumber != iPageNumber)
		{
			oPaginator.ConfigureHyperLinkPage(hPage, ContextExtension.CurrentItem.GetURL(), iPageNumber);
			hPage.Text = iPageNumber.ToString();
		}
		else
		{
			hPage.Visible = false;
			lSelectedPage.Visible = true;
			lSelectedPage.Text = iPageNumber.ToString();
		}

	}

	private void rEventListing_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oEventItem;
		HyperLink hLink;
		Literal lEventSummary;
		Literal lEventDate;
		Literal lEventStatus;
		Literal lEventVenue;
		Literal lEventDuration;
		HyperLink hEventDetails;
		Literal lLocation;
		Image imgEventImage;
		HtmlGenericControl liItem;
		PlaceHolder pStatus;
		PlaceHolder pVenue;
		PlaceHolder pEventDuration;
		DateTime? iBeginDate;
		DateTime? iEndDate;

		//get the event item
		if ((oEventItem = (Item)e.Item.DataItem) != null)
		{


			//get the fields
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lEventSummary = (Literal)e.Item.FindControl("lEventSummary");
			lEventDate = (Literal)e.Item.FindControl("lEventDate");
			lEventStatus = (Literal)e.Item.FindControl("lEventStatus");
			lEventVenue = (Literal)e.Item.FindControl("lEventVenue");
			lEventDuration = (Literal)e.Item.FindControl("lEventDuration");
			hEventDetails = (HyperLink)e.Item.FindControl("hEventDetails");
			lLocation = (Literal)e.Item.FindControl("lLocation");
			imgEventImage = (Image)e.Item.FindControl("imgEventImage");
			liItem = (HtmlGenericControl)e.Item.FindControl("liItem");
			pStatus = (PlaceHolder)e.Item.FindControl("pStatus");
			pVenue = (PlaceHolder)e.Item.FindControl("pVenue");
			pEventDuration = (PlaceHolder)e.Item.FindControl("pEventDuration");
			//bind the fields
			if (e.Item.ItemIndex == 0)
				liItem.Attributes.Add("class", "item first");
			else
				if (e.Item.ItemIndex == iMaxItems)
					liItem.Attributes.Add("class", "item last");
			oEventItem.ConfigureHyperlink(hLink);
			hLink.Text = oEventItem.GetText("Page", "Title");
			lEventVenue.Text = oEventItem.GetText("Event", "Venue");
			string stemp = oEventItem.GetText("Page", "Summary");
			if (!string.IsNullOrWhiteSpace(stemp))
			{
				lEventSummary.Text = stemp;
				lEventSummary.Visible = true;
			}
			stemp = oEventItem.GetText("Event", "Venue");
			if (!string.IsNullOrWhiteSpace(stemp))
			{
				pVenue.Visible = true;
				lEventVenue.Text = stemp;
			}

			iBeginDate = oEventItem.GetField("Begin Date").GetDate();
			iEndDate = oEventItem.GetField("End Date").GetDate();
			if (iBeginDate.HasValue)
			{
				lEventDate.Text = iBeginDate.Value.ToLongDateString();
				if (iEndDate.HasValue)
				{

					lEventDuration.Text = ((iEndDate.Value - iBeginDate.Value).Days + 1).ToString() + " Day Conference";
				}
				else
				{
					pEventDuration.Visible = false;
				}
			}
			oEventItem.ConfigureHyperlink(hEventDetails);
			stemp = oEventItem.GetField("Photo").GetImageURL();
			if (!string.IsNullOrWhiteSpace(stemp))
			{
				imgEventImage.ImageUrl = string.Format("~/{0}?mh=320&mw=120", stemp);
				imgEventImage.Visible = true;
			}
			StringBuilder location = new StringBuilder(oEventItem.GetText("City"));
			stemp = oEventItem.GetText("State");
			if (!string.IsNullOrWhiteSpace(stemp))
			{
				// just if the City was entered add an comma before the 
				// state
				if (location.Length > 0)
				{
					location.Append(", ");
				}
				location.Append(stemp);
			}

			stemp = location.ToString();
			if (!string.IsNullOrWhiteSpace(stemp))
				lLocation.Text = string.Format(" | {0}", stemp);
			if (oEventItem.InstanceOfTemplate(Event.Templates.MasteryProgramEvent.Name))
			{

				var oStatusList = oEventItem.GetMultilistItems(Event.Templates.MasteryProgramEvent.Sections.Event.PCStatus);
				StringBuilder statusText = new StringBuilder();
				foreach (Item oStatus in oStatusList)
				{
					statusText.Append(oStatus.DisplayName);
					statusText.Append(", ");
				}
				lEventStatus.Text = statusText.ToString().TrimEnd(',', ' ');
				pStatus.Visible = !string.IsNullOrWhiteSpace(lEventStatus.Text);

			}
		}
	}

	protected void lbEventSearch_Click(object sender, EventArgs e)
	{
		string sSearchCriteria;
		string sZipCode;
		GenworthQueryString oQueryString;
		string sRedirectURL;
        int iSelectedRatio;
        
		if (Page.IsValid)
		{
			oQueryString = GenworthQueryString.Current;
			oQueryString.RemovePagination();

			sRedirectURL = ContextExtension.CurrentItem.GetURL();

			if (!string.IsNullOrEmpty(sSearchCriteria = tbSearchKeywords.Text.Trim()))
			{
				oQueryString.Add("search", sSearchCriteria, true);
				oQueryString.Remove("zip");
				oQueryString.Remove("radio");

				Response.Redirect(string.Format("{0}{1}", sRedirectURL, oQueryString.ToString()), true);
			}
			else if (!string.IsNullOrEmpty(sZipCode = tbZipcode.Text.Trim()))
			{
                if (int.TryParse(ddlProximity.SelectedValue, out iSelectedRatio) && iSelectedRatio > RatioSearchForAll)
                {
                    oQueryString.Add("zip", sZipCode, true);
                    oQueryString.Add("radio", ddlProximity.SelectedValue, true);
                    oQueryString.Remove("search");

                    Response.Redirect(string.Format("{0}{1}", sRedirectURL, oQueryString.ToString()), true);
                }
                else
                {
                    //Nothing to filter we will show the view for events withtout any filtering
                    Response.Redirect(sRedirectURL);
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
            <asp:ListItem Text="250 Miles" Value="250"></asp:ListItem>
            <asp:ListItem Text="All" Value="0"></asp:ListItem>
		</asp:DropDownList>
	</span>of <span class="text-input event-search-zipcode">
		<asp:TextBox ID="tbZipcode" runat="server" Width="75"></asp:TextBox>
	</span><span class="button event-search-button">
		<asp:Button ID="lbEventSearch" runat="server" Text="Search" OnClick="lbEventSearch_Click">
		</asp:Button></span>
</div>
<h4 runat="server" id="hNoresults" visible="false">
	No results were found</h4>
<div class="genworth-event-list-wrapper" runat="server" id="dResults">
	<ul class="genworth-event-list">
		<asp:Repeater ID="rEventListing" runat="server">
			<ItemTemplate>
				<li class="item" runat="server" id="liItem">
					<asp:HyperLink ID="hLink" runat="server" CssClass="event-title" />
					<span class="event-date">
						<asp:Literal ID="lEventDate" runat="server" /><asp:Literal ID="lLocation" runat="server" /></span>
					<span class="show-event-details">View Details</span>
					<dl class="event-data">
						<asp:PlaceHolder runat="server" ID="pStatus" Visible="false">
							<dt>Status:</dt>
							<dd>
								<asp:Literal ID="lEventStatus" runat="server" /></dd>
						</asp:PlaceHolder>
						<asp:PlaceHolder runat="server" ID="pVenue" Visible="false">
							<dt>Venue:</dt>
							<dd>
								<asp:Literal ID="lEventVenue" runat="server" /></dd>
						</asp:PlaceHolder>
						<asp:PlaceHolder runat="server" ID="pEventDuration">
							<dt>Duration:</dt>
							<dd>
								<asp:Literal ID="lEventDuration" runat="server" /></dd>
						</asp:PlaceHolder>
					</dl>
					<div class="clear">
					</div>
					<div class="event-details">
						<asp:Image ID="imgEventImage" runat="server" Height="120px" Width="180px" border="1"
							CssClass="event-thumb" Visible="false" />
						<asp:Literal ID="lEventSummary" runat="server" Visible="false" />
						<div class="clear">
						</div>
						<span class="button view-event-link">
							<asp:HyperLink ID="hEventDetails" runat="server">View Event</asp:HyperLink>
						</span><span class="hide-event-details">Hide Details</span>
					</div>
					<div class="clear">
					</div>
				</li>
			</ItemTemplate>
		</asp:Repeater>
	</ul>
</div>
<div class="genworth-list-foot">
	<div class="pagination-wrapper">
		<div class="pagination">
			<asp:HyperLink ID="lPrevious" runat="server" CssClass="previous" Visible="false">Previous</asp:HyperLink>
			<ul>
				<asp:Repeater ID="rPages" runat="server">
					<ItemTemplate>
						<li>
							<asp:Label ID="lSelectedPage" runat="server" Visible="false" />
							<asp:HyperLink ID="hPage" runat="server" />
						</li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
			<asp:HyperLink ID="lNext" runat="server" CssClass="next" Visible="false">Next</asp:HyperLink>
		</div>
	</div>
</div>
