<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.Text.RegularExpressions" %>
<script runat="server">
    
	protected override void OnLoad(EventArgs e)
	{
		Sitecore.Diagnostics.Log.Info("Event Location ", this);
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}


	private void BindData()
	{
		Item oEvent;
		string sMapLinkUrl;
		string sAddress;
		string sState;
		string sCity;
		string sZipCode;
        string sTelephone;
		string sCenterParameter;
		string sLocation;
		StringBuilder oAddress2Builder;
        StringBuilder oPhoneBuilder;

        
		//get the event from our context
		oEvent = ContextExtension.CurrentItem;

		//Check whether is an event with a valid location
		if (oEvent.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Name))
		{
			//Pull the map link information
			sAddress = oEvent.GetText(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Name, Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Fields.AddressFieldName);
			sState = oEvent.GetText(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Name, Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Fields.StateFieldName);
			sCity = oEvent.GetText(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Name, Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Fields.CityFieldName);
			sZipCode = oEvent.GetText(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Name, Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Fields.ZipCodeFieldName);
            sTelephone = oEvent.GetText(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Name, Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Fields.TelephoneFieldName);
            sCenterParameter = BuildMapMarkerParameter(sAddress, sCity, sState, sZipCode,sTelephone);
			sLocation = oEvent.GetText("Venue");
			if (!string.IsNullOrEmpty(sCenterParameter))
			{
                var mapsClienSideBaseURL = Genworth.SitecoreExt.Utilities.Maps.MapsClienSideBaseURL;
                if (!String.IsNullOrWhiteSpace(mapsClienSideBaseURL) && Uri.IsWellFormedUriString(mapsClienSideBaseURL, UriKind.RelativeOrAbsolute))
                {
                    Page.ClientScript.RegisterClientScriptInclude("mapsGoogleApi", mapsClienSideBaseURL);
                }
                var staticMapBaseURL = Genworth.SitecoreExt.Utilities.Maps.StaticMapBaseURL;

                hMapLink.NavigateUrl = string.Format(@"https://maps.googleapis.com/maps/api/staticmap?{0}&zoom=15&size=640x640&sensor=false", sCenterParameter);
                hMapLink.ImageUrl = string.Format(@"https://maps.googleapis.com/maps/api/staticmap?{0}&zoom=15&size=230x200&sensor=false", sCenterParameter);

			}
			else
			{
				sMapLinkUrl = oEvent.GetText(Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Name, Genworth.SitecoreExt.Constants.Event.Templates.OnSite.Sections.Event.Fields.MapLinkFieldName);
				if (!string.IsNullOrEmpty(sMapLinkUrl))
				{
					sMapLinkUrl = ValidateQueryStringParameter(sMapLinkUrl, "zoom", "15");
					sMapLinkUrl = ValidateQueryStringParameter(sMapLinkUrl, "sensor", "false");
					sMapLinkUrl = ValidateQueryStringParameter(sMapLinkUrl, "size", "640x640");
					hMapLink.NavigateUrl = sMapLinkUrl;
					sMapLinkUrl = ValidateQueryStringParameter(sMapLinkUrl, "size", "230x200");
					hMapLink.ImageUrl = sMapLinkUrl;
				}
				else
				{
					hMapLink.Visible = false;
				}
			}

			if (!string.IsNullOrEmpty(sAddress))
			{
				lAddress1.Text = sAddress;
			}
			else
			{
				lAddress1.Visible = false;
			}

			oAddress2Builder = new StringBuilder();
            oPhoneBuilder = new StringBuilder();

			if (!string.IsNullOrEmpty(sCity))
			{
				oAddress2Builder.Append(sCity);
			}

			if (!string.IsNullOrEmpty(sState))
			{
				if (oAddress2Builder.Length > 0)
				{
					oAddress2Builder.Append(string.Format(",{0}", sState));
				}
				else
				{
					oAddress2Builder.Append(string.Format("{0}", sState));
				}
			}

			if (!string.IsNullOrEmpty(sZipCode))
			{
				oAddress2Builder.Append(string.Format(" {0}", sZipCode));
			}

            if (!string.IsNullOrEmpty(sTelephone))
            {

                oPhoneBuilder.Append(string.Format(" {0}",  sTelephone));
                sTelephone = RemoveSpecialChars(sTelephone);
                lPhone.Text = Genworth.SitecoreExt.Constants.Event.fPhoneNumber(sTelephone);
            }

			lAddress2.Text = oAddress2Builder.ToString();
			if (string.IsNullOrEmpty(lAddress2.Text))
			{
				lAddress2.Visible = false;
			}

			if (!lAddress2.Visible && !lAddress1.Visible)
			{
				lLocatioName.Visible = false;
                pLocation.Visible = false;
			}
      
        	lLocatioName.Text = sLocation;
		}
        else
        {
            pLocation.Visible = false;
            lLocatioName.Visible = false;
            hMapLink.Visible = false;
        }
	}

    public string RemoveSpecialChars(string str)
    {
        string[] UnwantedChars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "'", "\"", ";", "_", "-", "(", ")", ":", "|", "[", "]" }; 

        for (int i = 0; i < UnwantedChars.Length; i++)
        {
            if (str.Contains(UnwantedChars[i]))
            {
                str = str.Replace(UnwantedChars[i], "");
            }
        }
        return str;
    }
    
    
	private string ValidateQueryStringParameter(string sURL, string sParameterName, string sExpectedValue)
	{
		Uri oMapUri;
		string sQueryParameter;
		NameValueCollection oQueryParameters;
		string sParameterMatchingExpression;

		if (!string.IsNullOrEmpty(sURL) && Uri.TryCreate(sURL, UriKind.Absolute, out oMapUri))
		{
			//Valid URL

			if (!string.IsNullOrEmpty(sQueryParameter = oMapUri.Query))
			{
				//It has parameters

				oQueryParameters = HttpUtility.ParseQueryString(sQueryParameter);
				if (string.IsNullOrEmpty(oQueryParameters.Get(sParameterName)))
				{
					//parameter not exists, we need to add it
					sURL = string.Format("{0}&{1}={2}", sURL, sParameterName, sExpectedValue);
				}
				else
				{
					//Parameter exists, We need to check the value
					sParameterMatchingExpression = string.Format("{0}=*+&", sParameterName);
					if (Regex.IsMatch(sURL, sParameterMatchingExpression, RegexOptions.IgnoreCase))
					{
						//Parameter is not the last parameter
						sURL = Regex.Replace(sURL, sParameterMatchingExpression, string.Format("{0}={1}&", sParameterName, sExpectedValue));
					}
					else
					{
						//Is the las parameter
						sParameterMatchingExpression = string.Format("{0}=*+$", sParameterName);
						sURL = Regex.Replace(sURL, sParameterMatchingExpression, string.Format("{0}={1}", sParameterName, sExpectedValue), RegexOptions.IgnoreCase);

					}
				}
			}
			else
			{
				//Parameters missing not a valid map link
				sURL = string.Empty;
			}

		}
		else
		{
			//this is not a valid URL
			sURL = string.Empty;
		}

		return sURL;
	}


	private string BuildMapMarkerParameter(string sAddress, string sCity, string sState, string sZipCode, string sTelephone)
	{
		StringBuilder oMarkerParameterBuilder;


		oMarkerParameterBuilder = new StringBuilder();

		oMarkerParameterBuilder.Append("markers=color:red|");
		if (!string.IsNullOrEmpty(sAddress))
		{
			sAddress = PrepareLocation(sAddress);

			if (sAddress != null)
			{
				oMarkerParameterBuilder.Append(string.Format("{0}", sAddress));
			}

		}

		if (!string.IsNullOrEmpty(sCity))
		{
			sCity = PrepareLocation(sCity);

			if (!string.IsNullOrEmpty(sCity))
			{
				if (oMarkerParameterBuilder.Length > 1)
				{
					oMarkerParameterBuilder.Append(string.Format(",{0}", sCity));
				}
				else
				{
					oMarkerParameterBuilder.Append(string.Format("{0}", sCity));
				}
			}

		}

		if (!string.IsNullOrEmpty(sState))
		{
			sState = PrepareLocation(sState);

			if (!string.IsNullOrEmpty(sState))
			{
				if (oMarkerParameterBuilder.Length > 1)
				{
					oMarkerParameterBuilder.Append(string.Format(",{0}", sState));
				}
				else
				{
					oMarkerParameterBuilder.Append(string.Format("{0}", sState));
				}
			}

		}
				

		if (!string.IsNullOrEmpty(sZipCode))
		{
			sZipCode = PrepareLocation(sZipCode);

			if (!string.IsNullOrEmpty(sZipCode))
			{
				if (oMarkerParameterBuilder.Length > 1)
				{
					oMarkerParameterBuilder.Append(string.Format(",{0}", sZipCode));
				}
				else
				{
					oMarkerParameterBuilder.Append(string.Format("{0}", sZipCode));
				}
			}

		}


        if (!string.IsNullOrEmpty(sTelephone))
        {
            sTelephone = PrepareLocation(sTelephone);

            if (!string.IsNullOrEmpty(sTelephone))
            {
                if (oMarkerParameterBuilder.Length > 1)
                {
                    oMarkerParameterBuilder.Append(string.Format(",{0}", sTelephone));
                }
                else
                {
                    oMarkerParameterBuilder.Append(string.Format("{0}", sTelephone));
                }
            }

        } 
		return oMarkerParameterBuilder.ToString();
	}

	private string PrepareLocation(string sLocation)
	{
		StringBuilder oLocationBuilder;
		string[] oLocationSplited;

		oLocationBuilder = new StringBuilder();

		if (!string.IsNullOrEmpty(sLocation))
		{
			oLocationSplited = sLocation.Split(' ');

			if (oLocationSplited.Length > 0)
			{
				oLocationBuilder.Append(string.Format("{0}", oLocationSplited[0]));
				oLocationSplited = oLocationSplited.Skip(1).ToArray();
				foreach (string sLocationSection in oLocationSplited)
				{
					if (!string.IsNullOrEmpty(sLocationSection))
					{
						oLocationBuilder.Append(string.Format("+{0}", sLocationSection));
					}
				}
			}

		}

		return oLocationBuilder.ToString();

	}
</script>
<asp:PlaceHolder ID="pLocation" runat="server">
	<h4>
		Location</h4>
	<div class="map">
		<asp:HyperLink ID="hMapLink" runat="server" Target="_blank" />
	</div>
	<span class="location-name">
		<asp:Literal ID="lLocatioName" runat="server">Location Name</asp:Literal></span>
	<span class="location-address">
		<asp:Literal ID="lAddress1" runat="server" /><br />
		<asp:Literal ID="lAddress2" runat="server" /><br />
        <asp:Literal ID="lPhone" runat="server" />
	</span>
	<hr />
	</asp:PlaceHolder>
