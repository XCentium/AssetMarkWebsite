using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerLogic.SitecoreExt;

namespace Genworth.SitecoreExt.Services.Events
{
    public class EventsInPerson : EventsBase
    {
        public EventsInPerson()
            : base()
        {
            oSearch = new EventSearch();
            oSearch.SetEventType(this.Code);
            SetDefaultDateRange();

            bAutoUpdateFilterAvailability = true;
            AutoUpdateFilterAvailability();
        }

        protected override void SetDefaultDateRange()
        {
            oSearch.SetDateFilter(DateTime.Today, DateTime.MaxValue, isDefault: true);
        }

        protected override void SetSearchDateRange()
        {
            oSearch.SetDateFilter(DateTime.MinValue, DateTime.MaxValue, isDefault: false);
        }

        public const string CODE = Constants.Event.SearchType.InPerson; //"events-inperson";
        public override string Code
        {
            get { return CODE; }
        }

        protected override IEnumerable<KeyValuePair<string,string>> GetColumns()
        {
            return new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string,string>("Event Name","title"),
                        new KeyValuePair<string,string>("Dates","begin_date"),
                        new KeyValuePair<string,string>("Location","city"),
                        new KeyValuePair<string,string>("Registration","archiveUrl,url,by_invitation_only_text")
                    };
        }

        public override EventDataItem[] GetResults()
        {
            //is sort dirty?
            if (bIsSortDirty)
            {
                //set sorts
                oSearch.ApplySort(oSorts, false);

                //sort is no longer dirty
                bIsSortDirty = false;
            }

            //return the paged results
            return (iResultsPerPage == -1 ? oSearch.ResultDocuments : oSearch.ResultDocuments.Skip(iResultsPerPage * iPage).Take(iResultsPerPage)).Select(oDocument => new EventDataItem(oDocument)).ToArray();
        }

        public override string URL
        {
            get
            {
                return ServerLogic.SitecoreExt.ContextExtension.CurrentItem.GetURL();
            }
        }

        public override void Reset(bool bUseDefaultValues)
        {
            //we want to be sure we do not update filter availability while we are resetting data (saves cycles)
            bAutoUpdateFilterAvailability = false;

            base.Reset(bUseDefaultValues);
            //we are done resetting our data, turn on the auto-update
            bAutoUpdateFilterAvailability = true;
            //update availability
            AutoUpdateFilterAvailability();
        }
    }
}

