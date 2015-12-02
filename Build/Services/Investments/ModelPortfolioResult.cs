using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;
using System.Runtime.Serialization;
using ServerLogic.SitecoreExt;

namespace Genworth.SitecoreExt.Services.Investments
{
    [DataContract]
    public class ModelPortfolioResult : ResultBase
    {
        private static Dictionary<string, string> oCustodiansList;

        public static Dictionary<string, string> CustodiansList
        {
            get { return ModelPortfolioResult.oCustodiansList; }
            set { ModelPortfolioResult.oCustodiansList = value; }
        }
        private static Dictionary<string, string> oSolutionTypes;

        public static Dictionary<string, string> SolutionTypes
        {
            get { return ModelPortfolioResult.oSolutionTypes; }
            set { ModelPortfolioResult.oSolutionTypes = value; }
        }
        public const string NoCustodianSet = "-";

        [DataMember(Name = "Custodian", Order = 3)]
        private string sCustodian;
        [DataMember(Name = "SolutionType", Order = 4)]
        private string sSolutionType;
        [DataMember(Name = "Icon", Order = 5)]
        private string sIcon;
        [DataMember(Name = "OmnitureParam")]
        private string sOmnitureParam;

        #region READ ONLY PROPERTIES
        public string SolutionType { get { return sSolutionType; } }
        public string Custodian { get { return sCustodian; } }
        public string Icon { get { return sIcon; } }
        #endregion

        private string sCustodianCode;

        internal string CustodianCode
        {
            get { return sCustodianCode; }

        }

        private string sSolutionTypeCode;

        internal string SolutionTypeCode
        {
            get { return sSolutionTypeCode; }

        }


        internal string OmnitureParam
        {
            get { return sOmnitureParam; }

        }

        static ModelPortfolioResult()
        {

            oCustodiansList = ContextExtension.CurrentDatabase.SelectItems("/sitecore/content/Meta-Data/Security/Custodians/*").ToDictionary(oItem => oItem.GetText("Code").ToLower(), oItem => oItem.DisplayName);
            oSolutionTypes = ContextExtension.CurrentDatabase.SelectItems("/sitecore/content/Meta-Data/Solution Types/*").ToDictionary(oItem => oItem.GetText("Code").ToLower(), oItem => oItem.DisplayName);

        }
        public ModelPortfolioResult(Document oDocument)
            : base(oDocument)
        {
            Field oField;

            sCustodian = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Custodian)) != null ? oField.StringValue : NoCustodianSet;
            sSolutionType = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.SolutionType)) != null ? oField.StringValue : string.Empty;
            sIcon = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.Extension)) != null ? oField.StringValue : string.Empty;
            sOmnitureId = (oField = oDocument.GetField(Constants.Investments.Indexes.Fields.OmnitureId)) != null ? oField.StringValue : string.Empty;

        }

        public ModelPortfolioResult(string sTitle, string sPath, string sStrategist, string sDate, string sCustodian, string sSolutionType, string omnitureParam)
            : base(sTitle, sPath, sStrategist, sDate)
        {
            sCustodian = sCustodian.ToLower().Trim();
            sSolutionType = sSolutionType.ToLower().Trim();
            if (string.IsNullOrEmpty(sCustodian))
            {
                this.sCustodian = this.sCustodianCode = NoCustodianSet;

            }
            else
            {
                if (string.IsNullOrWhiteSpace(sCustodian))
                {
                    this.sCustodian = NoCustodianSet;
                }
                else
                    if (oCustodiansList.ContainsKey(sCustodian))
                    {
                        this.sCustodian = oCustodiansList[sCustodian];
                    }
                    else
                        this.sCustodian = this.sSolutionType = string.Format("Custodian not avilable {0}", sCustodian);

                this.sCustodianCode = sCustodian;
            }
            if (oSolutionTypes.ContainsKey(sSolutionType))
            {
                this.sSolutionType = oSolutionTypes[sSolutionType];

            }
            else
                this.sSolutionType = string.Format("Solution Type not avilable {0}", sSolutionType);
            this.sSolutionTypeCode = sSolutionType;
            sIcon = "pdf";

            this.sOmnitureParam = omnitureParam;
        }

        public string GetField(string sField)
        {
            string sFieldValue = string.Empty;
            switch (sField)
            {
                case "custodian":
                    sFieldValue = sCustodian;
                    break;
                case "date":
                    sFieldValue = sDate;
                    break;
                case "title":
                    sFieldValue = sTitle;
                    break;
                case "strategist":
                    sFieldValue = sSrategist;
                    break;
                case "solutionType":
                    sFieldValue = sSolutionType;
                    break;

            }
            return sFieldValue;
        }

        /// <summary>
        /// Returns the value of the field in a way that can be sorted as a string in a correct way
        /// Note: In case we notice this affect the performance we can add this field in the 
        /// document index
        /// </summary>
        /// <param name="sField">Name of the field</param>
        /// <returns></returns>
        public string GetSortableField(string sField)
        {
            string sSortableField = GetField(sField);

            // The date is currently in format "MM/dd/yyyy" so is not ready for be sorted
            // correctly as a string
            if (sField.Equals("date") && !String.IsNullOrWhiteSpace(sSortableField) && sSortableField.Length == 10)
            {
                // we will change it to yyyyMMdd avoid converting it to Date and back to string
                sSortableField = String.Format("{0}{1}{2}",
                                    sSortableField.Substring(6, 4),
                                    sSortableField.Substring(0, 2),
                                    sSortableField.Substring(3, 2));

            }

            return sSortableField;
        }
    }
}
