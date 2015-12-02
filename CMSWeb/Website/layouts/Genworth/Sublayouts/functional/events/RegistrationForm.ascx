<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Import Namespace=" Genworth.SitecoreExt.Security" %>

<script runat="server">

	#region ATTRIBUTES

    #region CONSTANTS

    #region EMAIL PARAMETER KEYS

    	private const string FirstNameParameterKey = "FirstName";
		private const string LastNameParameterKey = "LastName";
		private const string FirmNameParameterKey = "FirmName";
		private const string PhoneNumberParameterKey = "PhoneNumber";
        private const string EventNameParameterKey = "EventName";
        private const string SSOParameterKey = "SSO";
    
    #endregion

    #region VIEWSTATE KEYS

    private const string sCurrentModeViewstateKey = "CurrentMode";
  
	#endregion

    #endregion
    private Item oEventItem;

    /// <summary>
    /// Holds the item with the email template for the current registration
    /// </summary>
    private Item oEmailTemplateItem;

	#endregion

	#region PROPERTIES

	private RegistrationMode CurrentRegistrationMode
	{
		get
		{

			if (ViewState[sCurrentModeViewstateKey] == null)
			{
				ViewState[sCurrentModeViewstateKey] = RegistrationMode.Form;
			}

			return (RegistrationMode)ViewState[sCurrentModeViewstateKey];
		}

		set
		{
			ViewState[sCurrentModeViewstateKey] = value;
		}
	}


	private Item CurrentEventItem
	{
		get
		{
			string sEventId;

			if (oEventItem == null)
			{
				if (!string.IsNullOrEmpty(sEventId = Request.QueryString[Genworth.SitecoreExt.Constants.Event.RegistrationPage.EventIdQueryStringKey]))
				{
					hCancel.Attributes.Add("onclick", "parent.dialogBox.close();");

					//ToggleForm(CurrentRegistrationMode);

					oEventItem = ContextExtension.CurrentDatabase.GetItem(sEventId);

				}
				else
				{
					Response.Redirect(EventHelper.EventPageItem.GetURL(true), true);
				}
			}

			return oEventItem;
		}
	}

    /// <summary>
    /// Item that contains the Email template that we need to use to send the registration email
    /// </summary>
    private Item CurrentEmailTemplate
    {
        get
        {
            string sTemplateId;

            if (oEmailTemplateItem == null)
            {
                oEmailTemplateItem = CurrentEventItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Email.Templates.Email.Name).SingleOrDefault();
                if (oEmailTemplateItem == null)
                {
                    sTemplateId = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Events.DefaultRegistrationEmailTemplate, string.Empty);

                    if (!string.IsNullOrEmpty(sTemplateId))
                    {
                        oEmailTemplateItem = ContextExtension.CurrentDatabase.GetItem(sTemplateId);
                    }
                }

                if (oEmailTemplateItem == null)
                {
                    Sitecore.Diagnostics.Log.Error("Unable to find email template for event registration email", this);
                }
            }

            return oEmailTemplateItem;
        }
    }
    
	#endregion

	#region ENUMS

	private enum RegistrationMode : int
	{
		Form = 0,
		Confirmation = 1
	}

	#endregion

	#region PAGE HANDLERS

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}


	void bRegister_Click(object sender, EventArgs e)
	{
		CurrentRegistrationMode = RegistrationMode.Confirmation;
		ToggleForm(CurrentRegistrationMode);
	}

	#endregion

	#region HELPER METHODS

	private void BindData()
	{

		hCancel.Attributes.Add("onclick", "parent.dialogBox.close();;");

		ToggleForm(CurrentRegistrationMode);

	}


	private void ToggleForm(RegistrationMode oMode)
	{
		string sEventTitle;
        string sToEmails;
        string sCCEmails;
        string sFromEmail;
        string sFromEmailName;
        string sSubject;
        string sPhoneNumber;
        string sFirstName;
        string sLastName;
        string sFirmName;
        string sSSO;
        Dictionary<string, string> oToEmails;
        Dictionary<string, string> oCCEmails;
        Authorization oAuthorization;
        Item oEmail;
        XElement oXSLTEmailTemplate;
        string sXSLTEmailTemplate;
        System.Xml.XmlReader oXMLReaderForEmailTemplate;
        
        GFWM.Shared.Entity.Data.SWTClaim oClaim;

        sEventTitle = string.Empty;
        
        if (CurrentEventItem != null)
        {
            sEventTitle = CurrentEventItem.GetText(Genworth.SitecoreExt.Constants.Page.Templates.PageBase.Sections.Page.Name, Genworth.SitecoreExt.Constants.Page.Templates.PageBase.Sections.Page.Fields.TitleFieldName, string.Empty);
        }
        else
        {
            Sitecore.Diagnostics.Log.Error("Registration Form - Unable to find the event", this);
        }
        
		if (oMode == RegistrationMode.Form)
		{
			fRegistrationFields.Visible = true;
			sRegister.Visible = true;
			hCancel.Text = "Cancel";
						         
			lTitle.Text = string.Format("Register for {0}", sEventTitle);
			lIntro.Text = "When you complete the information below, we will pre-register you for this event, contact you with any questions and request approval from your Broker-Dealer, if required.";			
		}
		else
		{
			if (Page.IsValid)
			{
				fRegistrationFields.Visible = false;
				sRegister.Visible = false;
                oEmail = CurrentEmailTemplate;
                lIntro.Text = "Thank you, the form has been submitted.";

                if (oEmail != null)
                {
                    sFromEmail = tbEmailAddress.Text.Trim();
                    sFirstName = tbFirstName.Text.Trim();
                    sLastName = tbLastName.Text.Trim();
                    sFirmName = tbFirmName.Text.Trim();
                    sPhoneNumber = tbPhoneNumber.Text.Trim();
                    sSubject = oEmail.GetText(Genworth.SitecoreExt.Constants.Email.Templates.Email.Sections.Email.Name, Genworth.SitecoreExt.Constants.Email.Templates.Email.Sections.Email.Fields.SubjectFieldName, string.Empty);
                    sXSLTEmailTemplate = oEmail.GetText("Xslt Template", "Xml", string.Empty);

                    if (!string.IsNullOrEmpty(sXSLTEmailTemplate))
                    {

                        Dictionary<string, string> oParameters = new Dictionary<string, string>();
                        oParameters.Add(FirstNameParameterKey, sFirstName);
                        oParameters.Add(LastNameParameterKey, sLastName);
                        oParameters.Add(FirmNameParameterKey, sFirmName);
                        oParameters.Add(PhoneNumberParameterKey, sPhoneNumber);
                        oParameters.Add(EventNameParameterKey, sEventTitle);

                        oAuthorization = Authorization.CurrentAuthorization;

                        if (oAuthorization != null && (oClaim = oAuthorization.Claim) != null)
                        {
                            sSSO = oClaim.LoggedInSSOGuid.ToString();
                        }
                        else
                        {
                            sSSO = Guid.Empty.ToString();
                            if (!oAuthorization.IsTestMode)
                            {
                                Sitecore.Diagnostics.Log.Error("Registration Form, unable to retrieve SSO for current user", this);
                            }
                        }

                        oParameters.Add(SSOParameterKey, sSSO);


                        try
                        {
                            oXSLTEmailTemplate = XElement.Parse(sXSLTEmailTemplate);
                            oXMLReaderForEmailTemplate = oXSLTEmailTemplate.CreateReader();
                            sToEmails = oEmail.GetText(Genworth.SitecoreExt.Constants.Email.Templates.Email.Sections.Email.Name, Genworth.SitecoreExt.Constants.Email.Templates.Email.Sections.Email.Fields.ToFieldName, String.Empty);
                            sCCEmails = oEmail.GetText(Genworth.SitecoreExt.Constants.Email.Templates.Email.Sections.Email.Name, Genworth.SitecoreExt.Constants.Email.Templates.Email.Sections.Email.Fields.CCFieldName, String.Empty);
                            oToEmails = Genworth.SitecoreExt.Utilities.Email.ParseSemicolonDelimitedEmailList(sToEmails);
                            oCCEmails = Genworth.SitecoreExt.Utilities.Email.ParseSemicolonDelimitedEmailList(sCCEmails);
                            
                            if (string.IsNullOrEmpty(sSubject))
                            {
                                sSubject = string.Format("Registration request for {0}", sEventTitle);
                            }

                            if (oToEmails != null && oToEmails.Count > 0)
                            {
                                sFromEmailName = !string.IsNullOrEmpty(sFirmName)?  string.Format("{0}, {1} ({2})", sLastName, sFirstName, sFirmName):  string.Format("{0}, {1}", sLastName, sFirstName);

                                Genworth.SitecoreExt.Utilities.Email.SendEmail(oToEmails, oCCEmails, sFromEmail, sFromEmailName, sSubject, oXMLReaderForEmailTemplate, oParameters);
                            }
                            else
                            {
                                Sitecore.Diagnostics.Log.Error("Error trying to send pre-registration email. No destination addresses defined in the email template", this);
                            }
                        }
                        catch (Exception oRegistrationEmailGeneration)
                        {
                            Sitecore.Diagnostics.Log.Error(string.Format("Error trying to send pre-registration email for event {0}", CurrentEventItem.ID), oRegistrationEmailGeneration, this);
                        }
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error(string.Format("Error trying to send pre-registration email for event {0}. Email template is missing", CurrentEventItem.ID), this);
                    }
                }
                else
                {
                    lIntro.Text = "There is an error please contact the web site administrator";
                    Sitecore.Diagnostics.Log.Error(string.Format("Error trying to send pre-registration email for event {0} Email configuration is missing", CurrentEventItem.ID), this);
                }
                hCancel.Text = "Close";
            }

        }
	}


	#endregion
	
</script>
<div id="MyUniqeId" class="dialog-wrapper">
	<div class="dialog">
		<h2>
			<asp:Literal ID="lTitle" runat="server"></asp:Literal></h2>
		<div class="dialog-content">
			<p>
				<asp:Literal ID="lIntro" runat="server"></asp:Literal>
			</p>
			<fieldset runat="server" id="fRegistrationFields">
				<table>
					<tr>
						<td>
							<label for="<%= tbFirstName.ClientID %>">First name:</label>
						</td>
						<td>
							<asp:TextBox ID="tbFirstName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
								runat="server" ID="vFirstName" ControlToValidate="tbFirstName" Text="*"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td>
							<label for="<%= tbLastName.ClientID %>">Last name:</label>
						</td>
						<td>
							<asp:TextBox ID="tbLastName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
								runat="server" ID="RequiredFieldValidator1" ControlToValidate="tbLastName" Text="*"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td>
							<label for="<%= tbFirmName.ClientID %>">Firm name:</label>
						</td>
						<td>
							<asp:TextBox ID="tbFirmName" runat="server"></asp:TextBox><asp:RequiredFieldValidator
								runat="server" ID="RequiredFieldValidator2" ControlToValidate="tbFirmName" Text="*"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td>
							<label for="<%= tbPhoneNumber.ClientID %>">Phone number:</label>
						</td>
						<td>
							<asp:TextBox ID="tbPhoneNumber" runat="server"></asp:TextBox><asp:RequiredFieldValidator
								runat="server" ID="RequiredFieldValidator3" ControlToValidate="tbPhoneNumber"
								Text="*"></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td>
							<label for="<%= tbEmailAddress.ClientID %>">Email address:</label>
						</td>
						<td>
							<asp:TextBox ID="tbEmailAddress" runat="server"></asp:TextBox><asp:RequiredFieldValidator
								runat="server" ID="RequiredFieldValidator4" ControlToValidate="tbEmailAddress"
								Text="*"></asp:RequiredFieldValidator>
						</td>
					</tr>
				</table>
			</fieldset>
			<div class="clear">
			</div>
			<div class="submit inline-form-block">
				<span class="button" id="sRegister" runat="server">
					<asp:Button runat="server" ID="bRegister" Text="Register" OnClick="bRegister_Click" />
				</span>
				<span class="button" id="sCancel" runat="server">
					<asp:HyperLink runat="server" ID="hCancel" Text="Cancel" CausesValidation="false" />
				</span>
			</div>
		</div>
	</div>
</div>
