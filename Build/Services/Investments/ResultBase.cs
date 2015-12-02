using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Lucene.Net.Documents;

namespace Genworth.SitecoreExt.Services.Investments
{
    /// <summary>
    /// A result represents a single document in the Lucene index, returned by a search.
    /// </summary>
    [DataContract]
    public class ResultBase
    {
        protected string sId;
        [DataMember(Name = "Path", Order = 2)]
        protected string sPath;
        [DataMember(Name = "Title", Order = 3)]
        protected string sTitle;
        [DataMember(Name = "Date", Order = 9)]
        protected string sDate;
        [DataMember(Name = "Strategist", Order = 6)]
        protected string sSrategist;
        [DataMember(Name = "OmnitureId")]
        protected string sOmnitureId;

        #region READ ONLY PROPERTIES

        public string Id { get { return sId; } }
        public string Path { get { return sPath; } }
        public string Date { get { return sDate; } }
        public string Srategist { get { return sSrategist; } }
        public string Title { get { return sTitle; } }
        public string OmnitureId { get { return sOmnitureId; } }
        #endregion
        public ResultBase(Document oDocument)
        {
            Field oField;
            DateTime dDate;
            //set the fields
            sId = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Id)) != null ? oField.StringValue : string.Empty;
            sTitle = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Title)) != null ? oField.StringValue : string.Empty;
            sPath = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Path)) != null ? oField.StringValue : string.Empty;
            sSrategist = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Strategist)) != null ? oField.StringValue : string.Empty;
            sDate = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Date)) != null ? oField.StringValue : string.Empty;

            string parseFormat = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Indexing.DataFormat.DateTimeStringFormat, "yyyyMMdd");
            if (!string.IsNullOrEmpty(sDate) && DateTime.TryParseExact(sDate, parseFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dDate))
            {
                //format the date properly
                sDate = dDate.ToString(Constants.Investments.DateFormat);
            }
            sOmnitureId = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.OmnitureId)) != null ? oField.StringValue : string.Empty;
        }


        public ResultBase(string sTitle, string sPath, string sStrategist, string sDate)
        {

            DateTime dDate;
            //set the fields            
            this.sTitle = sTitle;
            this.sPath = sPath;
            this.sSrategist = sStrategist;
            this.sDate = sDate;
            string parseFormat = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Indexing.DataFormat.DateTimeStringFormat, "yyyyMMdd");
            if (!string.IsNullOrEmpty(sDate) && DateTime.TryParseExact(sDate, parseFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dDate))
            {
                //format the date properly
                this.sDate = dDate.ToString(Constants.Investments.DateFormat);
            }
        }
    }
}
