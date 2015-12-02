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
    public class Result : ResultBase
    {
        [DataMember(Name = "Icon", Order = 2)]
        protected string sIcon;
        [DataMember(Name = "Category", Order = 4)]
        private string sCategory;
        [DataMember(Name = "Source", Order = 5)]
        private string sSource;

        [DataMember(Name = "Manager", Order = 7)]
        private string sManager;
        [DataMember(Name = "AllocationApproach", Order = 8)]
        private string sAllocationApproach;
        [DataMember(Name = "Url", Order = 10)]
        public string sUrl;
        [DataMember(Name = "Audience", Order = 11)]
        public string sAudience;

        private string sExtension;

        private string sId;

        #region READ ONLY PROPERTIES

        public string Icon { get { return sIcon; } }
        public string Category { get { return sCategory; } }
        public string Source { get { return sSource; } }
        public string Manager { get { return sManager; } }
        public string AllocationApproach { get { return sAllocationApproach; } }
        public string Extension { get { return sExtension; } }
        public string Id { get { return sId; } }
        public string Url { get { return sUrl; } }
        public string Audience { get { return sAudience; } }
        #endregion

        public Result(Document oDocument)
            : base(oDocument)
        {
            Field oField;

            //set the fields
            sIcon = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Icon)) != null ? oField.StringValue : string.Empty;
            sCategory = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Category)) != null ? oField.StringValue : string.Empty;
            sSource = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Source)) != null ? oField.StringValue : string.Empty;
            sManager = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Manager)) != null ? oField.StringValue : string.Empty;
            sAllocationApproach = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.AllocationApproach)) != null ? oField.StringValue : string.Empty;
            sExtension = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Extension)) != null ? oField.StringValue : string.Empty;
            sUrl = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Url)) != null ? oField.StringValue : string.Empty;
            sId = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Id)) != null ? oField.StringValue : string.Empty;
            sAudience = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Audience)) != null ? GetAudience(oField.StringValue) : string.Empty;

        }

        private string GetAudience(string value)
        {
            string result = string.Empty;

            if(!string.IsNullOrWhiteSpace(value))
            { 
                if(value.ToLowerInvariant().Contains(Constants.Investments.Audiences.Client))
                {
                    result = Constants.Investments.AudienceOptions.Client;
                }
                else
                { 
                    result = Constants.Investments.AudienceOptions.Advisor; 
                }
            }

            return result;
        }
    }
}
