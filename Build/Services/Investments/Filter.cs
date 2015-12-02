using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using ServerLogic.SitecoreExt;
using Lucene.Net.Documents;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace Genworth.SitecoreExt.Services.Investments
{
    /// <summary>
    /// A filter represents a set of possible values within an index. 
    /// </summary>
    [DataContract]
    public class Filter
    {
        private FilterGroup oGroup;
        public FilterGroup Group { get { return oGroup; } }

        private string sName;
        public string Name { get { return sName; } }

        private string sIndexField;
        internal string IndexField { get { return sIndexField; } }
        
        private string sDataField;
        public string DataField { get { return sDataField; } set { sDataField = value; } }

        [DataMember(Name = "Code", Order = 1)]
        private string sCode;
        public string Code { get { return sCode; } }

        [DataMember(Name = "Options", Order = 2)]
        private List<Option> oOptions;
        internal List<Option> Options { get { return oOptions; } }
        public Option[] OptionArray { get { return oOptions.ToArray(); } }

        private HashSet<string> sAvailableValues;

        private bool bShowAll;
        private bool bHide;

        public bool Hide
        {
            get { return bHide; }
            set { bHide = value; }
        }
        public bool ShowAll
        {
            get { return bShowAll; }
            set { bShowAll = value; }
        }

        public Filter(FilterGroup oGroup, string sName, string sCode, string sIndexField, bool bShowAll = true)
        {
            //set the internal fields
            this.oGroup = oGroup;
            this.sName = sName;
            this.sCode = sCode;
            this.sIndexField = sIndexField;
            this.bShowAll = bShowAll;
            //create the list
            oOptions = new List<Option>();

            //create an empty set of available values
            sAvailableValues = new HashSet<string>();
        }

        public Filter(FilterGroup oGroup, string sName, string sCode, string sIndexField, string sQuery, Func<Item, string> oNameFunction, Func<Item, string> oCodeFunction, Func<Item, bool> isAvalible, bool bShowAll = true)
            : this(oGroup, sName, sCode, sIndexField, bShowAll)
        {
            //using the current database, run the query
            foreach (Item oItem in ContextExtension.CurrentDatabase.SelectItems(sQuery))
            {
                //add the item to the list
                oOptions.Add(new Option(this, oItem.ID.ToString(), oNameFunction(oItem), oCodeFunction(oItem), false, isAvalible(oItem), false));
            }
        }

        public Filter(FilterGroup oGroup, string sName, string sCode, string sIndexField, string sQuery, Func<Item, string> oNameFunction, Func<Item, string> oCodeFunction, bool bShowAll = true)
            : this(oGroup, sName, sCode, sIndexField, bShowAll)
        {

            //using the current database, run the query
            foreach (Item oItem in ContextExtension.CurrentDatabase.SelectItems(sQuery))
            {
                //add the item to the list
                oOptions.Add(new Option(this, oItem.ID.ToString(), oNameFunction(oItem), oCodeFunction(oItem), false, true, false));
            }
        }

        internal void SetDefaultValues(string sValue)
        {
            string[] sDefaultValues;
            if (!string.IsNullOrWhiteSpace(sValue))
            {
                sDefaultValues = sValue.ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (sDefaultValues.Contains("all"))
                {
                    Options.ForEach(oFilter => oFilter.Default = oFilter.Filtered = true);
                }
                else
                    if (sDefaultValues.Contains("hide"))
                    {
                        this.bHide = true;
                    }
                    else
                    {
                        Options.ForEach(oFilter => { if (sDefaultValues.Contains(oFilter.Code)) oFilter.Default = oFilter.Filtered = true; });
                    }
            }
        }

        internal void AddOption(string sId, string sName, string sCode)
        {
            //add the item to the list
            oOptions.Add(new Option(this, sId, sName, sCode, false, true, false));
        }

        internal void ClearAvailableValues()
        {
            sAvailableValues.Clear();
        }

        internal void GetAvailableValue(Document oDocument)
        {
            Field oField;
            string sValue;
            char[] cSplit = new char[] { '|' };
            if ((oField = oDocument.GetField(sIndexField)) != null)
            {
                if (!string.IsNullOrEmpty(sValue = oField.StringValue) && !sAvailableValues.Contains(sValue))
                {
                    sValue.Split(cSplit, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(sCode => { if (!sAvailableValues.Contains(sCode))sAvailableValues.Add(sCode); });

                }
            }
        }
        internal void SetAvailableValue(HashSet<string> sAvailableCodeValues)
        {

            foreach (string sCode in sAvailableCodeValues)
            {
                Filter.Option oFilterOption = this.oOptions.SingleOrDefault(oOption => oOption.Code == sCode);
                if (oFilterOption != null && !sAvailableValues.Contains(oFilterOption.Id))
                    sAvailableValues.Add(oFilterOption.Id);

            }
        }

        internal void ApplyAvailableValues()
        {
            if (!this.Hide)
                oOptions.AsParallel().ForAll(oOption => { oOption.Available = sAvailableValues.Contains(oOption.Id); });
        }
        public void ApplyAvailabeValuesbyCode(HashSet<string> sAvailableCodeValues, bool ApplyDefault)
        {
            if (!this.Hide)
                oOptions.AsParallel().ForAll(oOption => { if (sAvailableCodeValues.Contains(oOption.Code)) { oOption.Available = true; if (ApplyDefault) oOption.Filtered = oOption.Default; } });
        }
        internal void ApplyAvailableValues(bool bAvailable)
        {
            if (!this.Hide)
                oOptions.AsParallel().ForAll(oOption => { if (!(oOption.Available = bAvailable)) oOption.Filtered = false; });

        }

        [DataContract]
        public class Option
        {
            public static readonly Regex CodeCleaner = new Regex("[^a-z0-9]", RegexOptions.IgnoreCase);

            private string sName;
            public string Name { get { return sName; } }

            [DataMember(Name = "Code", Order = 1)]
            private string sCode;
            public string Code { get { return sCode; } }

            [DataMember(Name = "Filtered", Order = 2)]
            private bool bFiltered;
            public bool Filtered { get { return bFiltered; } set { bFiltered = value; } }

            [DataMember(Name = "Available", Order = 3)]
            private bool bAvailable;
            public bool Available { get { return bAvailable; } set { bAvailable = value; } }
            private bool bDefault;

            public bool Default
            {
                get { return bDefault; }
                set { bDefault = value; }
            }
            private string sId;
            public string Id { get { return sId; } }

            private Filter oFilter;
            public Filter Filter { get { return oFilter; } }

            public Option(string sId, string sName, string sCode, bool bFiltered, bool bAvailable, bool bDefault) : this(null, sId, sName, sCode, bFiltered, bAvailable, bDefault) { }

            public Option(Filter oFilter, string sId, string sName, string sCode, bool bFiltered, bool bAvailable, bool bDefault)
            {
                this.oFilter = oFilter;
                this.sId = sId;
                this.sName = sName;
                this.sCode = CodeCleaner.Replace(sCode, string.Empty).ToLower();
                this.bFiltered = bFiltered;
                this.bAvailable = bAvailable;
                this.bDefault = bDefault;
            }

            /// <summary>
            /// This method will set the default filters or clear all the filters depending on the parameter received.
            /// </summary>
            /// <param name="bUseDefaultValues">True if the default value should be use, False for clearing the filter</param>
            public void Reset(bool bUseDefaultValue)
            {
                bFiltered = bUseDefaultValue ? bDefault : false;
                this.bAvailable = true;
            }
        }
    }
}
