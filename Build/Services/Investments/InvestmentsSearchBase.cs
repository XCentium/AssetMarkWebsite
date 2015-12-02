using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Lucene.Net.Documents;
using System.Web;
using Lucene.Net.Search;

namespace Genworth.SitecoreExt.Services.Investments
{
	[DataContract]
	public abstract class InvestmentsSearchBase:IInvestmentsSearch
	{
		[DataMember(Name = "Filters", Order = 1)]
		protected  List<Filter> oFilters;
		protected Query oAxiQuery;
		protected Search oSearch;
		protected bool bShowFilterBar;
		protected ItemCache oItemCache;
		public abstract string Code { get; }
		public bool ShowFilterBar { get { return bShowFilterBar; } }
        public string JsonServiceUrl { get {
            return Genworth.SitecoreExt.Constants.Investments.GridJsonServiceUrl;
        } }
		
		protected bool bAutoUpdateFilterAvailability;
		public InvestmentsSearchBase(bool bBypassItems)
		{
			//create a list of filters
			oFilters = new List<Filter>();
			//create a list to hold keyword searches
			oKeywordSearches = new List<string>();

			//creaet an item cache
			oItemCache = new ItemCache();

			//create a new search object
			oSearch = new Search(oItemCache,bBypassItems);

			

			//create the list of sorts
			oSorts = new List<ResultSort>();
			oSorts.Add(new ResultSort(Constants.Investments.Indexes.Fields.Date, false));

			//sorting is dirty by default
			bIsSortDirty = true;

			//set the auto update filter availability flag
			bAutoUpdateFilterAvailability = true;

			
		}
		public InvestmentsSearchBase(object bNone) { }

		#region DATE RANGES

		[DataMember(Name = "Months", Order = 2)]
		protected int iMonths;
		public int Months
		{
			get
			{
				return iMonths;
			}
			set
			{
				iMonths = value;

				//how many months were selected?
				if (iMonths == -1)
				{
					//user wants to see all documents, regardless of date
					dFromDate = DateTime.MinValue;
					dToDate = DateTime.MaxValue;

					//date filter changed
					SetDateFilterChanged();
				}
				else if (iMonths == -2)
				{
					//user wants to select a custom date range, make sure the range makes sense to the user
					if (dToDate == DateTime.MaxValue || dFromDate == DateTime.MinValue)
					{
						//specify date range baed on months
						dFromDate = (dToDate = DateTime.Today).AddMonths(-Constants.Investments.DefaultMonthRange);

						//date filter changed
						SetDateFilterChanged();
					}
				}
				else
				{
					//is month too small?
					iMonths = Math.Max(iMonths, Constants.Investments.MinimumMonthRange);

					//specify date range baed on months
					dFromDate = (dToDate = DateTime.Today).AddMonths(-iMonths);

					//date filter changed
					SetDateFilterChanged();
				}
			}
		}

		private void SetDateFilterChanged()
		{
			//set the date filter
			oSearch.SetDateFilter(dFromDate, dToDate);

			//move to page 1
			iPage = 0;

			//update the filter availability
			AutoUpdateFilterAvailability();

			//sort is dirty
			bIsSortDirty = true;
		}

		[DataMember(Name = "FromDate", Order = 3)]
		public string FromDate
		{
			get { return dFromDate != DateTime.MinValue ? dFromDate.ToString(Constants.Investments.DateFormat) : string.Empty; }
			set
			{
				if (!DateTime.TryParse(value, out dFromDate))
				{
					dFromDate = DateTime.MinValue;
				}

				//set the date filter changed
				SetDateFilterChanged();
			}
		}
		protected DateTime dFromDate;

		[DataMember(Name = "ToDate", Order = 4)]
		public string ToDate
		{
			get { return dToDate != DateTime.MaxValue ? dToDate.ToString(Constants.Investments.DateFormat) : string.Empty; }
			set
			{
				if (!DateTime.TryParse(value, out dToDate))
				{
					dToDate = DateTime.Today;
				}
				else if (dToDate > DateTime.Today)
				{
					dToDate = DateTime.Today;
				}
			}
		}
		protected DateTime dToDate;

		#endregion

		#region PAGING

		[DataMember(Name = "ResultCount", Order = 5)]
		protected virtual int ResultCount { get { return oSearch != null ? oSearch.ResultDocuments.Count() : 0; } set { } }

		[DataMember(Name = "ResultsPerPage", Order = 6)]
		protected int iResultsPerPage = Constants.Investments.DefaultResultsPerPage;
		public int ResultsPerPage
		{
			set
			{
				int iTemp;

				//put the value in a temp
				iTemp = value;

				//a negative 1 means view all... if not negative 1, we must set a minimum
				if (iTemp != -1)
				{
					//set minimum
					iTemp = Math.Max(iTemp, Constants.Investments.MinResultsPerPage);
				}

				//are we actually changing the per page?
				if (iTemp != iResultsPerPage)
				{
					//set the results per page
					iResultsPerPage = iTemp;

					//move to page 1
					iPage = 0;
				}
			}
		}

		[DataMember(Name = "Page", Order = 7)]
		protected int iPage;
		public int Page
		{
			set
			{
				int iTemp;

				//put the value in a temp
				iTemp = value;

				//is the page number too high?
				if (iTemp < 0 || iTemp > (ResultCount / iResultsPerPage))
				{
					//the page is outside of range
					iTemp = 0;
				}

				//set the page to the temp value
				iPage = iTemp;
			}
		}

		#endregion

		#region SORTING

		[DataMember(Name = "Sort", Order = 8)]
		protected List<ResultSort> oSorts;

		protected bool bIsSortDirty;

		#endregion

		#region KEYWORDS

		[DataMember(Name = "KeywordSearches", Order = 9)]
		protected List<string> oKeywordSearches;

		#endregion

		public abstract Filter[] Filters {get;}
		

		/// <summary>
		/// This method will set the default filters or clear all the filters depending on the parameter received.
		/// </summary>
		/// <param name="bUseDefaultValues">True if the default values should be use, False for clearing all the filters</param>
		public virtual void Reset(bool bUseDefaultValues)
		{
			

			//reset the search
			oSearch.Reset();

			//set the keywords
			//SetKeywords(string.Empty);

			//clear all the filters (make everything unfiltered and available)
			oFilters.AsParallel().ForAll(oFilter =>
			{
				oFilter.Options.AsParallel().ForAll(oOption =>
				{
					oOption.Reset(bUseDefaultValues);
					
				});
			});


			SetFilters();
			//clear the sorts
			oSorts.Clear();
			oSorts.Add(new ResultSort(Constants.Investments.Indexes.Fields.Date, false));

			//sorting is dirty by default
			bIsSortDirty = true;
			
		}
		public void SetFilters()
		{
			foreach (Filter oFilter in oFilters)
			{
				oSearch.SetFilter(oFilter);
			}
		}

		public  void SetFilterOption(string sFilter, string sOption, bool bFiltered)
		{
			Filter oFilter;
			Filter.Option oOption;
			oFilter =GetFilter( sFilter);
			//did we get a filter?
			if (oFilter != null && (sOption = sOption.ToLower().Trim()).Length > 0)
			{
				//we have the filter, lets clean up the code
				sOption = Filter.Option.CodeCleaner.Replace(sOption, string.Empty).ToLower();

				if (sOption == "all")
				{
					oFilter.Options.ForEach(oLinqOption => { if (oLinqOption.Available) oLinqOption.Filtered = bFiltered; });
					//set the filter
					oSearch.SetFilter(oFilter);

					//update filter availability
					UpdateFilterAvailability();

					//sort is dirty
					bIsSortDirty = true;

					//move to first page
					iPage = 0;
				}
				else
				//does the filter contain the option?
				if ((oOption = oFilter.Options.Where(oLinq => oLinq.Code.Equals(sOption)).FirstOrDefault()) != null)
				{
					//is the option available?
					if (oOption.Available)
					{
						//set whether we are filtered
						oOption.Filtered = bFiltered;

						//set the filter
						oSearch.SetFilter(oFilter);

						//update filter availability
						UpdateFilterAvailability();

						//sort is dirty
						bIsSortDirty = true;

						//move to first page
						iPage = 0;
					}
				}
			}
		}

		protected abstract Filter GetFilter(string sFilter);
		
		
		public abstract List<KeyValuePair<string, string>> Columns
		{
			get;
		}

		public abstract bool SetURLFilters(System.Collections.Specialized.NameValueCollection oQueryString);
		

		public abstract string URL
		{
			get;
		}
		public void SetFilterOption(string sFilter, string sOption, string sFiltered)
		{
			//set the filter option
			SetFilterOption(sFilter, sOption, sFiltered.Equals("1"));
		}
		public abstract ResultBase[] GetResults();
		
		protected void AutoUpdateFilterAvailability()
		{
			if (bAutoUpdateFilterAvailability)
			{
				UpdateFilterAvailability();
			}
		}
			
		protected virtual void UpdateFilterAvailability(bool ApplyValues =true)
		{
			//clear the filters
			HttpContext oContext = System.Web.HttpContext.Current;
			Query oActualAxiQuery;
			IEnumerable<Filter> oActualFilters = oFilters.Where(oFilter => !oFilter.Hide);
			if (oActualFilters.Count() > 0)
			{
				Search oFiltersearch = new Search(oItemCache, oSearch.BypassItems);
				if (ApplyValues)
				{
					oFiltersearch.SetDateFilter(dFromDate, dToDate);
					oActualAxiQuery = oSearch.Queries[Constants.Investments.Indexes.Fields.Date];
				}
				else
				{
					oFiltersearch.SetFilter(oFilters[0]);
					oActualAxiQuery = oSearch.Queries[oFilters[0].IndexField];
				}
				
				if (oAxiQuery == oActualAxiQuery)
				{
					return;
				}
				oAxiQuery = oActualAxiQuery;

				Document[] oDocuments = oFiltersearch.ResultDocuments;
				oFilters.Where(oFilter => !oFilter.Hide).AsParallel().ForAll(oFilter =>
				{
					System.Web.HttpContext.Current = oContext;



					//clear the available values
					oFilter.ClearAvailableValues();

					//should we search?

					//loop over the search results
					foreach (Document oDocument in oDocuments)
					{
						//get the available values from this document
						oFilter.GetAvailableValue(oDocument);
					}

					if (ApplyValues)
						//apply the available values
						oFilter.ApplyAvailableValues();

				});
			}
		}
		internal void Sort(string sField)
		{
			ResultSort oSort;

			//get the existing sort
			oSort = oSorts.Where(oItem => oItem.Field.Equals(sField)).FirstOrDefault();

			//is the sort null?
			if (oSort != null)
			{
				//remove the sort
				oSorts.Remove(oSort);

				//add the sort
				oSorts.Add(oSort);

				//set the sort
				oSort.Order = !oSort.Order;
			}
			else
			{
				//create the sort
				oSorts.Add(new ResultSort(sField, true));
			}

			Sort(oSorts);
		}
		protected virtual void Sort(List<ResultSort> oSorts)
		{
			//apply the sorts
			oSearch.ApplySort(oSorts);
		}
		internal void SetKeywords(string sKeywords)
		{
			//does the keyword search already contain these keywords?
			if (oKeywordSearches.Contains(sKeywords))
			{
				//remove the existing keywords
				oKeywordSearches.Remove(sKeywords);
			}

			if(!string.IsNullOrWhiteSpace(sKeywords))
				//add the search
				oKeywordSearches.Insert(0, sKeywords);

			//sort is dirty
			bIsSortDirty = true;

			//move to first page
			iPage = 0;

			//tell the search to filter on keywords
			oSearch.SetKeywordFilter(sKeywords);
		}
	}
}
