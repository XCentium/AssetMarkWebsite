<%@ Control Language="c#" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Threading" %>
<%@ Import Namespace="ServerLogic.Core.Web.Utilities" %>
<%@ Import Namespace="Lucene.Net.Documents" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt.Services.Events" %>


<script runat="server">
    public static ArrayList PastEvents { get; set; }
    public static ArrayList FutureEvents { get; set; }
    public static ArrayList DateInfo { get; set; }
    private static ArrayList DateRangeEvents { get; set; }    
    private static ArrayList DateRangeEvents_Clone { get; set; }
    private DateTime CalendarDate { get; set; }
    private static string PathURL { get; set; }
    private static StringDictionary URLList { get; set;}
    private static string TypeofSelectedStyle { get; set; }
    private static string TypeofTodayStyle { get; set; }
    private static DateTime SelectedDayPostback { get; set; }
    private string SectionType;
    private static string CurrentURL;
    private static string SectionMenu;
    private static int PostbackCounter = 0;
    private static bool eventDetailflag = false;
    
    private static string AllEventsURL = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Search.EventsAllURL", "#");
    private static string ArchiveEventsURL = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Search.EventsArchiveURL", "#");
    private static string InPersonEventsURL = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Search.EventsInPersonURL", "#");
    private static string WebinarEventsURL = Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Search.EventsWebinarURL", "#");
   
    
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
        if (!Page.IsPostBack)
        {
            cCalendar.VisibleMonthChanged += new MonthChangedEventHandler(cCalendar_VisibleMonthChanged);
            cCalendar.SelectionChanged += new EventHandler(cCalendar_SelectionChanged);
            cCalendar.DayRender += new DayRenderEventHandler(cCalendar_DayRender);
                   
            InitSectionType();
            InitializeCalendar();
            BindEventDates();
        }        
	}

    private void InitSectionType()
    {
        SectionType = ContextExtension.CurrentItem.GetText("Section", "Section Type", null) ?? this.GetParameter("Search Type");
       
        if (ContextExtension.CurrentItem.GetText("Section", "Section Type", null) == "events-archive")
        {
            lblUpcomingEvents.Text = "Past Events";
        }

        if (PostbackCounter == 0 && SectionType != string.Empty)
        {
            SectionMenu = SectionType;
            //CurrentURL = Request.RawUrl;            
        }
    }
	
	private void InitializeCalendar()
{
		string sMonth;
		string sDate;
		DateTime dDate;
        DateTime dMonthDate = DateTime.MinValue;        
        		
		//get the date from the querys tring
		if (string.IsNullOrEmpty(sDate = Request.QueryString["date"]) || !DateTime.TryParse(sDate, out dDate))
		{
            if (string.IsNullOrEmpty(sMonth = Request.QueryString["month"]) || !DateTime.TryParse(sMonth, out dMonthDate))
            {
                //get current date
                dDate = DateTime.Today;
            }
            else
            {
                MoveCalendarMonth();
            }
		}        
		else
		{
			//current date should be set too
			cCalendar.TodaysDate = dDate;
            cCalendar.SelectedDate = dDate;
            MoveCalendarMonth();
		}

        if (SectionType == string.Empty)
            PostbackCounter = 0;
        else
            PostbackCounter++;
        
        if (SectionMenu != SectionType)
        {
            SelectedDayPostback = new DateTime();
            SectionMenu = SectionType;            
        }
               
		//set the calendar styling
		cCalendar.BorderWidth = 0;
		cCalendar.OtherMonthDayStyle.ForeColor = Color.FromArgb(255, 165, 165, 165);
		cCalendar.Width = Unit.Percentage(100);
		cCalendar.CellSpacing = 1;
		cCalendar.BackColor = Color.FromArgb(255, 212, 212, 212);
		cCalendar.DayStyle.Height = 27;
		cCalendar.DayStyle.BorderWidth = 0;
		//cCalendar.DayStyle.ForeColor = Color.FromArgb(255, 85, 85, 85);
		cCalendar.DayStyle.CssClass = "day";
		cCalendar.TodayDayStyle.CssClass = "today";        
        cCalendar.SelectedDayStyle.ForeColor = Color.FromArgb(255, 0, 0, 0);
        cCalendar.SelectedDayStyle.CssClass = TypeofSelectedStyle == null || TypeofSelectedStyle == string.Empty ? "selected-day": TypeofSelectedStyle;
		cCalendar.DayHeaderStyle.CssClass = "day-header";
		cCalendar.DayNameFormat = DayNameFormat.FirstLetter;
		cCalendar.TitleStyle.ForeColor = Color.White;
		cCalendar.TitleStyle.CssClass = "title";
		cCalendar.NextPrevStyle.ForeColor = Color.White;
		cCalendar.NextPrevStyle.CssClass = "next-prev-selector";

        if (TypeofSelectedStyle == "selected-day_future")
            cCalendar.SelectedDayStyle.BackColor = Color.FromArgb(255, 190, 217, 232);
        else if (TypeofSelectedStyle == "selected-day_past")
            cCalendar.SelectedDayStyle.BackColor = Color.FromArgb(255, 221, 221, 221);

	}    
    
    private void focusInMonth()
    {
        DateInfo = new ArrayList();

        if (cCalendar.VisibleDate == new DateTime() && CalendarDate == new DateTime())
            cCalendar.VisibleDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
        else if (CalendarDate != new DateTime())
            cCalendar.VisibleDate = CalendarDate;       
    }

    private void MoveCalendarMonth()
    {
        string month = Request.QueryString["month"];
        string date = Request.QueryString["date"];
        if (!string.IsNullOrWhiteSpace(month))
        {
            DateTime monthDate = DateTime.MinValue;
            if (DateTime.TryParse(month, out monthDate))
            {
                CalendarDate = monthDate;
            }
        }
        else if (!string.IsNullOrWhiteSpace(date))
        {
            DateTime selectedDate = DateTime.MinValue;
            if (DateTime.TryParse(date, out selectedDate))
            {
                CalendarDate = selectedDate;
            }
        }
    }

    private void BindEventDates()
    {
        DateTime currentEventDate;
        PastEvents = new ArrayList();
        FutureEvents = new ArrayList();
        PathURL = string.Empty;
        URLList = new StringDictionary();
        DateRangeEvents = new ArrayList();
        DateRangeEvents_Clone = new ArrayList();
        
        focusInMonth();
        
        int currentYear = cCalendar.VisibleDate.Year;
        int currentMonth = cCalendar.VisibleDate.Month;
        int lastDayInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
        
        //Get each event and its dates
        foreach (Document doc in GetEvents(SectionType, currentYear, currentMonth, lastDayInMonth))
        {
            DateTime eventDateStart = GetDate(GetFieldsByLucene(doc, Event.Indexes.EventsIndex.Fields.BeginDate));
            DateTime eventDateEndMax = GetDate(GetFieldsByLucene(doc, Event.Indexes.EventsIndex.Fields.EndDate));
            PathURL = GetFieldsByLucene(doc, Event.Indexes.EventsIndex.Fields.Path);

            if (DetectEventDays(eventDateStart, eventDateEndMax, DateTime.Today))
            {
                DateRangeEvents.Add(eventDateStart);
                DateRangeEvents.Add(eventDateEndMax);                
            }
            
            UpdateURLList(SectionType, eventDateStart, eventDateEndMax);            
            
            //Update FutureEvents with all valid dates
            currentEventDate = eventDateStart;

            while (currentEventDate <= eventDateEndMax)
            {
                //add past events
                if (DateTime.Today > eventDateEndMax.Date)
                {
                    //add dates to ArrayList in order to detect the past events                    
                    PastEvents.Add(currentEventDate);
                }
                else
                {
                    // detect future events
                    // BUG 20438
                    if (SectionType != Event.SearchType.Archive)
                    {
                        FutureEvents.Add(currentEventDate);
                    }
                }

                //add a day
                currentEventDate = currentEventDate.AddDays(1);
            }                    
        }

        //TODO: Find a better solution 
        //Calendar.SelectedDates with single value won't fire SelectionChanged							
        //Added one last date to the collection - after looping through the dataset- which would never appear on the calendar and would cause the SelectionChanged event to fire regardless of the date clicked.
        if (cCalendar.SelectedDates.Count == 1)
        {
            cCalendar.SelectedDates.Add((DateTime)Convert.ToDateTime("1/1/1900"));
        }
        
        DateRangeEvents_Clone.Clear();
        DateRangeEvents_Clone = DaysPrecedenceInBlue(DateRangeEvents);
    }

    private void UpdateURLList(string sectionType, DateTime eventDateStart, DateTime eventDateEndMax)
    {
        string dateEnd = string.Empty;
        string dateTemp = string.Empty;
        DateTime tempDate = eventDateStart;
        DateTime today = DateTime.Today;

        dateTemp = tempDate.ToString("MM/dd/yyyy");
        
        while (tempDate <= eventDateEndMax)
        {
            // Record the URL's that come from the Service into a StringDictionary object to Redirect the selected day to the right URL.
            if (URLList.ContainsKey(dateTemp))
            {
                URLList.Remove(dateTemp);
                switch (sectionType)
                {
                    case Event.SearchType.All:
                        URLList.Add(dateTemp, AllEventsURL); break;
                    case Event.SearchType.InPerson:
                        URLList.Add(dateTemp, InPersonEventsURL); break;
                    case Event.SearchType.Webinar:
                        URLList.Add(dateTemp, WebinarEventsURL); break;
                    case Event.SearchType.Archive:
                        if (today > eventDateEndMax)
                            URLList.Add(dateTemp, ArchiveEventsURL);
                        else
                            URLList.Add(dateTemp, AllEventsURL);
                        break;
                    default:
                        URLList.Add(dateTemp, AllEventsURL); break;                        
                }
            }
            else
            {
                if (sectionType != Event.SearchType.Archive)
                {
                    URLList.Add(dateTemp, PathURL);
                }
                else
                {
                    // if in Archive, and the event end date is before today, then it can be added; otherwise it's an ongoing event
                    if (today > eventDateEndMax)
                    {
                        URLList.Add(dateTemp, PathURL);
                    }
                }
            }

            tempDate = tempDate.AddDays(1);
            dateTemp = tempDate.ToString("MM/dd/yyyy");
        }        
    }

    private string GetTabSectionURL(string sectionType)
    {
        string url = "#";
        switch (sectionType)
        {
            case Event.SearchType.All:
                url = AllEventsURL; break;
            case Event.SearchType.InPerson:
                url = InPersonEventsURL; break;
            case Event.SearchType.Webinar:
                url = WebinarEventsURL; break;
            case Event.SearchType.Archive:
                url = ArchiveEventsURL; break;
            default:
                url = AllEventsURL; break;
        }
        return url;
    }

    private string GetFieldsByLucene(Document oDocument, string field)
    {
        Field oField;
        return (oField = oDocument.GetField(field)) != null ? oField.StringValue : string.Empty;
    }

    private DateTime GetDate(string value)
    {
        DateTime tempDate, dDate = DateTime.MinValue;
        string parseFormat = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Indexing.DataFormat.DateTimeStringFormat, "yyyyMMdd");
        
        if (!string.IsNullOrEmpty(value) && DateTime.TryParseExact(value, parseFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out tempDate))
        {
            dDate = tempDate.Date;
        }

        return dDate;
    }
    
    private List<Lucene.Net.Documents.Document> GetEvents(string eventType, int currentYear, int currentMonth, int daysInMonth)
    {
        List<Document> response = new List<Document>();
        
        Genworth.SitecoreExt.Security.Authorization oAuthorization = Genworth.SitecoreExt.Security.Authorization.CurrentAuthorization;
        
        string[] pcStatus = new string[0];

        if (oAuthorization.IsAgent)
        {
            pcStatus = oAuthorization.PC_Status;
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>();
        parameters.Add(Genworth.SitecoreExt.Constants.Event.SearchFilters.EventType, eventType);
        parameters.Add(Genworth.SitecoreExt.Constants.Event.SearchFilters.BeginDate, new DateTime(currentYear, currentMonth, 1).AddMonths(-1));
        parameters.Add(Genworth.SitecoreExt.Constants.Event.SearchFilters.EndDate, new DateTime(currentYear, currentMonth, daysInMonth).AddMonths(1));
        parameters.Add(Genworth.SitecoreExt.Constants.Event.SearchFilters.PCStatus, pcStatus);
        parameters.Add(Genworth.SitecoreExt.Constants.Event.SearchFilters.Keywords, string.Empty);

        var newItems = Genworth.SitecoreExt.Helpers.EventHelper.SearchEvents(parameters);
        if (newItems != null && newItems.Count > 0)
        {
            response.AddRange(newItems);
        }

        return response;
    }


    private ArrayList DaysPrecedenceInBlue(ArrayList arrlRangeEvents)
    {       
        ArrayList arrl = new ArrayList();
        ArrayList DateRangeEventsCopy = new ArrayList();

        if (arrlRangeEvents.Count > 0)
        {
            DateRangeEventsCopy.AddRange(arrlRangeEvents);
            DateRangeEventsCopy.Sort();

            arrl.Add(Convert.ToDateTime(DateRangeEventsCopy[0]));
            arrl.Add(Convert.ToDateTime(DateRangeEventsCopy[(DateRangeEventsCopy.Count - 1)]));

            arrlRangeEvents.Clear();
            arrlRangeEvents = arrl;
        }

        return arrlRangeEvents;
    }

    private bool DetectEventDays(DateTime dateStart, DateTime dateEnd, DateTime currentDatetime)
    {
        bool flag = false;

        if (currentDatetime >= dateStart && currentDatetime <= dateEnd)
        {
            flag = true;
        }
        
        return flag;        
    }

    private bool DetectEventDays(ArrayList days, DateTime dt)
    {
        bool flag = false;

        if (days.Count > 0)
        {
            if (dt >= Convert.ToDateTime(days[0]) && dt <= Convert.ToDateTime(days[1]))
            {
                flag = true;
            }
        }

        return flag;
    }
    
    
    private void cCalendar_DayRender(object sender, DayRenderEventArgs e)
    {
        //Allows if date is selectable according to section type
        bool isSelectable = IsSelectableDate(e.Day.Date);
        e.Day.IsSelectable = isSelectable;
        //string[] newURL = new string[0];
        //newURL = Request.RawUrl.Split('?');
        
        if (!isSelectable)
        {            
            if (e.Day.Date != DateTime.Today)
                e.Cell.CssClass = "non-selectable-day";
        }
            
        //Assign the Style (selected-day_past) only to the Past Events.     
        if (PastEvents.Contains(e.Day.Date))
        {
            if (e.Day.Date < DateTime.Today)
                e.Cell.CssClass = "day_past";

            if (TypeofSelectedStyle == "selected-day_past")
            {
                if (cCalendar.TodaysDate.Date == e.Day.Date)
                    e.Cell.CssClass = "selected-day_past";
            }
        }
        //Assign the Style (selected-day_future) only to the Future Events. 
        else if (FutureEvents.Contains(e.Day.Date))
        {

            if (SectionType != Event.SearchType.Archive)
            {
                e.Cell.CssClass = "day_future";

                if (TypeofSelectedStyle == "selected-day_future")
                {
                    if (cCalendar.TodaysDate.Date == e.Day.Date)
                        e.Cell.CssClass = "selected-day_future";
                }
            }
        }

        if (e.Day.Date == DateTime.Today && SectionType == Event.SearchType.Archive)
        {
            cCalendar.TodayDayStyle.CssClass = "todayArchive";
            e.Cell.CssClass += " todayArchive";
        }
        else if (e.Day.Date == DateTime.Today && SectionType != Event.SearchType.Archive) 
        {
            if (FutureEvents.Contains(e.Day.Date))
            {                
                cCalendar.TodayDayStyle.CssClass = "day_future";
                e.Cell.CssClass = "day_future restofThem-today";
            }
            else
            {
                cCalendar.TodayDayStyle.CssClass = "restofThem-today";
                e.Cell.CssClass += " restofThem-today";
            }
        }
        
        // Highlight the events in blue, no matters if they are paste events
        if (DetectEventDays(DateRangeEvents_Clone, e.Day.Date))
        {
            if (SectionType != Event.SearchType.Archive)
            {                
                e.Cell.CssClass = "day_future";
                e.Cell.BackColor = Color.FromArgb(190, 217, 232);

                if (e.Day.Date == DateTime.Today)
                {
                    e.Cell.CssClass += " restofThem-today";
                }                
            }            
        }

        if (eventDetailflag)
            SelectedDayPostback = new DateTime();
                      
              
        // If you want to add the bold to Today's date, you need to add a validation that takes the DateTime.Today
        // and also don't forge to apply the style "restofThem-today" 
        if (e.Day.Date == SelectedDayPostback && e.Day.Date != DateTime.Today)
        {
            if (DetectEventDays(DateRangeEvents, e.Day.Date) || FutureEvents.Contains(e.Day.Date))
            {
                if (SectionType != Event.SearchType.Archive)            
                    e.Cell.CssClass = "selected-day_future";
            }
            else
                e.Cell.CssClass = "selected-day_past";            
        }
    }

    private bool IsSelectableDate(DateTime date)
    {
        if (URLList.ContainsKey(date.ToString("MM/dd/yyyy")))
            return true;
        return false;
    }

    private void DefineTypeofSelectedDay(DateTime dt)
    {
        TypeofSelectedStyle = string.Empty;

        if (PastEvents != null && PastEvents.Contains(dt))
            TypeofSelectedStyle = "selected-day_past";
        else if (FutureEvents != null && FutureEvents.Contains(dt) && SectionType != Event.SearchType.Archive)
            TypeofSelectedStyle = "selected-day_future";
        else if (cCalendar.SelectedDate.Date == DateTime.Today)
            TypeofSelectedStyle = "selected-today";      
    }
    
	private void cCalendar_SelectionChanged(object sender, EventArgs e)
	{
		QueryString oQueryString;			
		oQueryString = QueryString.Current;
		oQueryString.Clear();
                
        oQueryString.Add("date", cCalendar.SelectedDate.ToString("MM-dd-yyyy"), true);

        // Define the Style of a selected day
        DefineTypeofSelectedDay(cCalendar.SelectedDate.Date);
        SelectedDayPostback = cCalendar.SelectedDate.Date;

                                     
        if (URLList != null && URLList.ContainsKey(cCalendar.SelectedDate.ToString("MM/dd/yyyy")))
        {
            eventDetailflag = false;
            Sitecore.Diagnostics.Log.Info("Event Calendar: Redirecting to: " + URLList[cCalendar.SelectedDate.ToString("MM/dd/yyyy")].ToString(), this);
            Response.Redirect(string.Format("{0}{1}", URLList[cCalendar.SelectedDate.ToString("MM/dd/yyyy")].ToString(), oQueryString.ToString()), true);                             
        }
        else
        {
            eventDetailflag = true; 
            Response.Redirect(string.Format("{0}{1}", GetTabSectionURL(SectionType), oQueryString.ToString(), true));
        }           
	
	}

	private void cCalendar_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
    {
        CalendarDate = cCalendar.VisibleDate;
        SelectedDayPostback = new DateTime();
        QueryString oQueryString;
		oQueryString = QueryString.Current;
        if (oQueryString.Contains("date"))
        {
            oQueryString.Remove("date");
        }
		oQueryString.Add("month", e.NewDate.ToString("yyyy-MM-dd"), true);
		Response.Redirect(string.Format("{0}{1}", Sitecore.Links.LinkManager.GetItemUrl(ContextExtension.CurrentItem), oQueryString.ToString()), true);
	}
</script>
<div class="snackbox">
	<h4 class="upcoming-events">
	<asp:Literal ID = "lblUpcomingEvents" runat = "server">Upcoming Events</asp:Literal> </h4>
	<div>
		<asp:Calendar ID="cCalendar" runat="server" CssClass="event-calendar" OnSelectionChanged="cCalendar_SelectionChanged"
         OnVisibleMonthChanged="cCalendar_VisibleMonthChanged" ViewStateMode="Enabled" EnableViewState="true" />
	</div>
</div>
<div class="cal-legend">    
        <table cellpadding="0" cellspacing="0">    
    <tr>
        <td>
            <div style="width: 15px; height: 11px; background: #BBBBBB"></div>
        </td>
        <td style="width: 2px"></td>
        <td>
              = Past Event
        </td>
        <td style="width: 2px"></td>
         <td>
            <img src="/CMSContent/Images/cal-future.png" width="15" height="11" alt="Future Event">
        </td>
        <td style="width: 2px"></td>
        <td>
              = Upcoming Event
        </td>
    </tr>
    <tr>
        <td>
            <div style="width: 11px; height: 9px; border: 2px solid #0C395E" ></div>
        </td>
        <td style="width: 2px"></td>
        <td>
             = Today
        </td>
        <td style="width: 4px"></td>                
        <td>
            <div style="width: 12px; height: 11px; font-weight:bold; font-family:Arial; font-size:small; text-align: center;">b</div>
        </td>
        <td style="width: 2px"></td>
        <td>
             = Selected Day
        </td>    
    </tr>
    </table>      
    
</div>
