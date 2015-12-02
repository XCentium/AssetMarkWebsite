using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Lucene.Net.Documents;
using ServerLogic.SitecoreExt;
using Sitecore.Data.Items;

namespace Genworth.SitecoreExt.Services.Events
{
    [DataContract]
    public class EventDataItem
    {
        [DataMember(Name = "Id")]
        private string id;
        [DataMember(Name = "EventName")]
        private string sEventName;
        [DataMember(Name = "BeginDate")]
        private string sBeginDate;
        [DataMember(Name = "EndDate")]
        private string sEndDate;
        [DataMember(Name = "Location")]
        private string sLocation;
        [DataMember(Name = "EventType")]
        private string sEventType;
        [DataMember(Name = "IsGwnHosted")]
        private bool isGwnHosted;
        [DataMember(Name = "IsInvitationOnly")]
        private bool isInvitationOnly;
        [DataMember(Name = "InvitationOnlyText")]
        private string sInvitationOnlyText;
        [DataMember(Name = "ActionUrl")]
        private string sActionUrl;
        [DataMember(Name = "RowUrl")]
        private string sRowUrl;
        [DataMember(Name = "IsPastEvent")]
        private bool isPastEvent;

        private DateTime dBeginDate;
        private DateTime dEndDate;

        private Document oDocument;

        const string DateFormat = "MM/dd/yyyy";

        public EventDataItem(Document oDocument)
        {
            this.oDocument = oDocument;

            id = GetField(Constants.Event.Indexes.EventsIndex.Fields.Id).Replace("{", string.Empty).Replace("}", string.Empty);
            sEventName = GetField(Constants.Event.Indexes.EventsIndex.Fields.Title);
            sBeginDate = GetDateFromString(GetField(Constants.Event.Indexes.EventsIndex.Fields.BeginDate), out dBeginDate);
            sEndDate = GetDateFromString(GetField(Constants.Event.Indexes.EventsIndex.Fields.EndDate), out dEndDate);
            sEventType = GetField(Constants.Event.Indexes.EventsIndex.Fields.Type);
            isInvitationOnly = GetBoolFromString(GetField(Constants.Event.Indexes.EventsIndex.Fields.InvitationOnly));
            sInvitationOnlyText = GetField(Constants.Event.Indexes.EventsIndex.Fields.InvitationOnlyTxt);
            sRowUrl = GetField(Constants.Event.Indexes.EventsIndex.Fields.Path);
            isPastEvent = dBeginDate < DateTime.Now;
            isGwnHosted = GetBoolFromString(GetField(Constants.Event.Indexes.EventsIndex.Fields.IsGwnHosted));

            SetLocation();
            SetEventUrl();

        }

        private void SetLocation()
        {
            sLocation = "N/A";

            if (sEventType == Constants.Event.Types.InPerson)
            {
                string sCity; string sState;
                bool hasCity = !string.IsNullOrWhiteSpace(sCity = GetField(Constants.Event.Indexes.EventsIndex.Fields.City));
                bool hasState = !string.IsNullOrWhiteSpace(sState = GetField(Constants.Event.Indexes.EventsIndex.Fields.State));

                if (hasCity && hasState)
                {
                    sLocation = string.Format("{0}, {1}", sCity, sState);
                }
                else if (hasCity && !hasState)
                {
                    sLocation = sCity;
                }
                else if (!hasCity && hasState)
                {
                    sLocation = sState;
                }
            }
        }

        private void SetEventUrl()
        {
            if (isPastEvent)
            {
                sActionUrl = GetField(Constants.Event.Indexes.EventsIndex.Fields.ArchiveUrl);
            }
            else if (sEventType == Constants.Event.Types.InPerson)
            {
                sActionUrl = Helpers.EventHelper.GetRegisterPageURL(id);
            }
            else
            {
                sActionUrl = GetField(Constants.Event.Indexes.EventsIndex.Fields.Url);
            }
        }

        private string GetField(string field)
        {
            Field oField;
            return (oField = oDocument.GetField(field)) != null ? oField.StringValue : string.Empty;
        }

        private bool GetBoolFromString(string stringValue)
        {
            return stringValue == "1";
        }

        private string GetDateFromString(string stringValue, out DateTime dDate)
        {
            dDate = DateTime.MinValue;
            string parseFormat = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Indexing.DataFormat.DateTimeStringFormat, "yyyyMMdd");
            if (!string.IsNullOrEmpty(stringValue) && DateTime.TryParseExact(stringValue, parseFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dDate))
            {
                //format the date properly

                stringValue = dDate.ToString(DateFormat);
            }

            return stringValue;
        }
    }
}
