<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
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
		Item oEvent;
		DateTime? iBeginDate;
		DateTime? iEndDate;
		Dictionary<string, string> oEventInfo = new Dictionary<string, string>();
		oEvent = ContextExtension.CurrentItem;

		iBeginDate = oEvent.GetField("Begin Date").GetDate();
		iEndDate = oEvent.GetField("End Date").GetDate();
		lTitle.Text = oEvent.GetText("Page", "Title");

		string stemp;
		if (string.IsNullOrWhiteSpace(lEventBody.Text = oEvent.GetText("Page", "Body")))
		{

			pEventBody.Visible = false;
		}

		if (oEvent.InstanceOfTemplate(Event.Templates.MasteryProgramEvent.Name))
		{

			var oStatusList = oEvent.GetMultilistItems(Event.Templates.MasteryProgramEvent.Sections.Event.PCStatus);
			StringBuilder statusText = new StringBuilder();
			foreach (Item oStatus in oStatusList)
			{
				statusText.Append(oStatus.DisplayName);
				statusText.Append(", ");
			}
			stemp = statusText.ToString().TrimEnd(',', ' ');
			if (!string.IsNullOrWhiteSpace(stemp))
				oEventInfo.Add("Status:", statusText.ToString().TrimEnd(',', ' '));
			stemp = oEvent.GetText(Event.Templates.MasteryProgramEvent.Sections.Event.QualifyingAUM);
			if (!string.IsNullOrWhiteSpace(stemp))
				oEventInfo.Add("Minimum AUM Needed:", stemp);

			stemp = oEvent.GetText(Event.Templates.MasteryProgramEvent.Sections.Event.ContinuingEducationCredits);
			if (!string.IsNullOrWhiteSpace(stemp))
				oEventInfo.Add("CE Credits:", stemp);

		}
		stemp = oEvent.GetText("Event", "Venue");
		if (!string.IsNullOrWhiteSpace(stemp))
		{
			oEventInfo.Add("Venue:", stemp);
		}

		if (oEvent.InstanceOfTemplate("Broker Dealer Conference"))
		{
			oEventInfo.Add("Qualifies for BDA:", ((Sitecore.Data.Fields.CheckboxField)oEvent.GetField("Broker")).Checked && ((Sitecore.Data.Fields.CheckboxField)oEvent.GetField("Dealer")).Checked ? "Yes" : "No");
		}


		if (iBeginDate.HasValue)
		{
			lEventDate.Text = iBeginDate.Value.ToLongDateString();
			if (iEndDate.HasValue)
			{

				oEventInfo.Add("Duration:", ((iEndDate.Value - iBeginDate.Value).Days + 1).ToString() + " Day Conference");
			}

		}
		//Create the table
		HtmlTableRow orow = null;
		int iNumData = 0;
		if (oEventInfo.Count == 1)
		{
			oEventInfo.Add("", "");
		}
		foreach (KeyValuePair<string, string> oData in oEventInfo)
		{

			HtmlTableCell oCellTitle = new HtmlTableCell(), oCellContent = new HtmlTableCell();
			HtmlGenericControl olabel = new HtmlGenericControl("label");
			oCellTitle.Attributes.Add("width", "1");
			if (iNumData % 2 == 0)
			{
				orow = new HtmlTableRow();
				tEventInfo.Controls.Add(orow);
				oCellContent.Attributes.Add("width", "1");
			}
			else
			{
				oCellContent.Attributes.Add("width", "*");
			}
			orow.Controls.Add(oCellTitle);
			orow.Controls.Add(oCellContent);
			oCellTitle.Controls.Add(olabel);
			oCellContent.InnerText = oData.Value;
			olabel.InnerText = oData.Key;
			iNumData++;
		}


		stemp = oEvent.GetField("Photo").GetImageURL();
		if (!string.IsNullOrWhiteSpace(stemp))
		{
			imgEventImage.ImageUrl = string.Format("~/{0}?mh=320&mw=120", stemp);
			imgEventImage.Visible = true;
		}
		StringBuilder location = new StringBuilder(oEvent.GetText("City"));
		stemp = oEvent.GetText("State");
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

		if (string.IsNullOrWhiteSpace(lSpeakers.Text = oEvent.GetText("Speakers")))
			pSpeakers.Visible = false;


	}
</script>
<div class="genworth-event-detail">
	<h3>
		Event Detail</h3>
	<a class="print-link" href="#" onclick="window.print();">Print</a>
	<h1>
		<asp:Literal runat="server" ID="lTitle" /></h1>
	<span class="event-date">
		<asp:Literal runat="server" ID="lEventDate"></asp:Literal><asp:Literal runat="server"
			ID="lLocation"></asp:Literal></span>
	<table class="event-data" cellpadding="0" width="100%" cellspacing="0" runat="server"
		id="tEventInfo">
	</table>
	<div class="clear">
	</div>
	<hr />
	<div class="event-details">
		<asp:Image runat="server" class="event-image" border="3" ID="imgEventImage" Visible="false" />
		<asp:PlaceHolder runat="server" ID="pEventBody">
			<h4>
				Description</h4>
			<asp:Literal runat="server" ID="lEventBody"></asp:Literal>
			<hr />
		</asp:PlaceHolder>
		
		<asp:PlaceHolder runat="server" ID="pSpeakers">
			<h4>
				Speakers</h4>
		
				<asp:Literal runat="server" ID="lSpeakers"></asp:Literal>
		
		</asp:PlaceHolder>
	</div>
	<div class="clear">
	</div>
</div>
