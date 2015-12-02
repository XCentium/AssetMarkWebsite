using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Contracts.Data
{
    /// <summary>
    /// The resultsort keeps track of the fields being sorted, their order and wether they are ascending or descending.
    /// </summary>
    [DataContract]
    public class ResultSort
    {
        [DataMember(Name = "Field", Order = 1)]
        private string sField;
        [DataMember(Name = "Order", Order = 2)]
        private bool bOrder;

        public string Field { get { return sField; } }
        public bool Order { set { bOrder = value; } get { return bOrder; } }

        public ResultSort(string sField, bool bOrder)
        {
            this.sField = sField;
            this.bOrder = bOrder;
        }
    }
}
