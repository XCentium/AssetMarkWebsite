<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>

<script runat="server">
	
	private Item oRegistrationPageItem;
    
      
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

        //bool isPremierEvent = ContextExtension.CurrentItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.PCTemplate.PremierEventName);

        Item oItem = ContextExtension.CurrentItem;
        const string ViewArchive = "View Archive";
        const string RegisterNow = "Register Now";
        
        bool isConferenceEvent = oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.ConferenceCallEvent.ConferenceCallEventName);
        bool isWebinar = oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.WebinarEvent.WebinarEventName);
        bool isInPerson = !(isConferenceEvent || isWebinar);

        DateTime? iBeginDate = ContextExtension.CurrentItem.GetField("Begin Date").GetDate();

        bool isArchive = iBeginDate.HasValue && iBeginDate.Value.Date < DateTime.Today;

        if (isArchive)
        {
            if (isWebinar)
            {
                setButton(ViewArchive, oItem.GetText("Event", "Archive URL"), true);
            }
            else if (isConferenceEvent)
            {
                string url = oItem.GetImageURL("Event", "Event Recording");
                if(!string.IsNullOrWhiteSpace(url))
                {
                    url = "/" + url;
                }
                setButton(ViewArchive, url, true);
            }
            else
            {
                setButton(ViewArchive, null, false);
            }
        }
        else
        {
            if (oItem.GetText("Registration", "By Invitation Only", "0").Equals("1"))
            {
                txtInvitationOnly.Visible = true;
                txtInvitationOnly.Text = oItem.GetText("Registration", "By Invitation Only Text");
                sButtonContainer.Visible = false;
            }
            else
            {
                if (isWebinar)
                {
                    setButton(RegisterNow, oItem.GetText("Event", "Event URL"), true);
                }
                else if (isConferenceEvent)
                {
                    registerNowSpan.Visible = false;
                }
                else
                {
                    setButton(RegisterNow, Genworth.SitecoreExt.Helpers.EventHelper.GetRegisterPageURL(oItem.ID.ToString()), false);
                }
            }
        }
        
        oItem.ConfigureOmnitureControl(oItem, lRSVP);
    }

    private void setButton(string buttonText, string url, bool isBlank)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            registerNowSpan.Visible = false;
        }
        else
        {
            lRSVP.Text = buttonText;
            lRSVP.NavigateUrl = url;
            if (isBlank)
            {
                lRSVP.Attributes.Remove("rel");
                lRSVP.Target = "_blank";
            }
        }
    }
   
   		
</script>
<div class="register-now" runat="server" id="registerNowSpan">
    <span class="button-big" runat="server" id="sButtonContainer">
        <asp:HyperLink Text="Register Now" runat="server" ID="lRSVP" rel="shadowbox;width=600px;height=300px;"></asp:HyperLink>
    </span>
    <asp:Literal Text="" Visible="false" ID="txtInvitationOnly" runat="server" />
     <div class="hr-wrapper" id = "ExtraLine" runat = "server"></div>
</div>

