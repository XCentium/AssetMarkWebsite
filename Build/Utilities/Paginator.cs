using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using ServerLogic.Core.Web.Utilities;

namespace Genworth.SitecoreExt.Utilities
{
	public class Paginator<T>
	{
		private const int I_MINIMUM_PER_PAGE = 10;
		private const int I_DEFAULT_PER_PAGE = 10;
		private const int I_MAXIMUM_PER_PAGE = 1000;

		private string sPrefix;
		private int iPageNumber;
		private int iPerPage;
		private int iQuantity;
		private int iLastPage;
		private IEnumerable<T> oItems;

		public int PageNumber { get { return iPageNumber; } }
		public int PerPage { get { return iPerPage; } }
		public int Quantity { get { return iQuantity; } }
		public int LastPage { get { return iLastPage; } }
		public IEnumerable<T> Items { get { return oItems; } }

		public Paginator(int iPageNumber, int iQuantity)
		{
			this.iPageNumber = iPageNumber;
			this.iQuantity = iQuantity;
		}

		private Paginator() { }

		public static Paginator<T> GetPaginator(IEnumerable<T> oItems)
		{
			return GetPaginator(oItems, I_DEFAULT_PER_PAGE);
		}

		public static Paginator<T> GetPaginator(IEnumerable<T> oItems, int iPerPage)
		{
			return GetPaginator(oItems, iPerPage, string.Empty);
		}

		public static Paginator<T> GetPaginator(IEnumerable<T> oItems, int iPerPage, string sPrefix)
		{
			return GetPaginator(oItems, iPerPage, sPrefix, I_MINIMUM_PER_PAGE, I_MAXIMUM_PER_PAGE);
		}

		public static Paginator<T> GetPaginator(IEnumerable<T> oItems, int iPerPage, string sPrefix, int iMinimumPerPage, int iMaximumPerPage)
		{
			object oObject;
			Paginator<T> oPaginator;
			string sKey;

			//do we have a paginator?
			if ((oObject = System.Web.HttpContext.Current.Items[sKey = (sPrefix + "Genworth.SitecoreExt.Utilities.Paginator")]) != null && oObject is Paginator<T>)
			{
				//cast the paginator
				oPaginator = (Paginator<T>)oObject;
			}
			else
			{
				//create the paginator
				oPaginator = new Paginator<T>();
				oPaginator.Load(oItems, iPerPage, sPrefix, iMinimumPerPage, iMaximumPerPage);
				System.Web.HttpContext.Current.Items[sKey] = oPaginator;
			}

			//return the paginator
			return oPaginator;
		}

		private void Load(IEnumerable<T> oItems, int iDefaultPerPage, string sPrefix, int iMinimumPerPage, int iMaximumPerPage)
		{
			string sPageNumber;
			string sPerPage;

			//set the prefix
			this.sPrefix = sPrefix;

			//get the page number
			sPageNumber = System.Web.HttpContext.Current.Request.QueryString[sPrefix + "pagenumber"] ?? string.Empty;
			sPerPage = System.Web.HttpContext.Current.Request.QueryString[sPrefix + "perpage"] ?? string.Empty;

			//set the count
			this.iQuantity = oItems.Count();

			//try to parse the per page from query string value
			if (int.TryParse(sPerPage, out iPerPage))
			{
				//get the items per page
				if (iPerPage < iMinimumPerPage)
				{
					iPerPage = iMinimumPerPage;
				}
				else if (iPerPage > iMaximumPerPage)
				{
					iPerPage = iMaximumPerPage;
				}
			}
			else
			{
				//could not parse items per page from query string, use default as provided
				iPerPage = iDefaultPerPage;
			}

			//set our local reference to the items
			this.oItems = oItems;

			//get the last page
			iLastPage = (iQuantity / iPerPage);
			if (iQuantity % iPerPage > 0)
			{
				iLastPage++;
			}

			//get the page number
			if (!int.TryParse(sPageNumber, out iPageNumber) || iPageNumber < 1)
			{
				iPageNumber = 1;
			}
			else if (iPageNumber > iLastPage)
			{
				iPageNumber = iLastPage;
			}
		}

		public IEnumerable<T> GetDataset()
		{
			//our page numbers are (1) based, not zero based
			return oItems.Skip(Math.Min(Math.Max((iPageNumber - 1), 0), (iLastPage - 1)) * iPerPage).Take(iPerPage);
		}

		public IEnumerable<int> GetPageNumbers()
		{
			int i;
			int j;

			for (i = 0, j = 0; i < iQuantity; i += iPerPage)
			{
				//add the item to the list
				yield return ++j;
			}
		}

		public void ConfigureHyperLinkPage(HyperLink hLink, string sUrl, int iPageNumber)
		{
			ConfigureHyperLinkPage(hLink, sUrl, QueryString.Current, iPageNumber);
		}

		public void ConfigureHyperLinkPage(HyperLink hLink, string sUrl, QueryString oQueryString, int iPageNumber)
		{
			if (hLink != null)
			{
				QueryString pageQueryString = new QueryString(oQueryString.ToString());
				pageQueryString.Add("pagenumber", iPageNumber.ToString(), true);
				pageQueryString.Add("perpage", this.PerPage.ToString(), true);

				hLink.NavigateUrl = sUrl + pageQueryString.ToString();
			}
		}

		public void ConfigureHyperLinkNext(HyperLink hLink, string sUrl)
		{
			ConfigureHyperLinkNext(hLink, sUrl, QueryString.Current);
		}

		public void ConfigureHyperLinkNext(HyperLink hLink, string sUrl, QueryString oQueryString)
		{
			if (hLink != null)
			{
				if (hLink.Visible = (this.PageNumber != this.LastPage && this.LastPage != 0))
				{
					hLink.NavigateUrl = sUrl + GetPageQueryString(oQueryString, this.PageNumber + 1);
				}
			}
		}

		public void ConfigureHyperLinkPrevious(HyperLink hLink, string sUrl)
		{
			ConfigureHyperLinkPrevious(hLink, sUrl, QueryString.Current);
		}

		public void ConfigureHyperLinkPrevious(HyperLink hLink, string sUrl, QueryString oQueryString)
		{
			if (hLink != null)
			{
				if (hLink.Visible = (this.PageNumber != 1))
				{
					hLink.NavigateUrl = sUrl + GetPageQueryString(oQueryString, this.PageNumber - 1);
				}
			}
		}

		private string GetPageQueryString(QueryString oQueryString, int pageNum)
		{
			// create a new query string using the provided query string
			//
			QueryString currString = new QueryString(oQueryString.ToString());

			return currString.Add("pagenumber", pageNum.ToString(), true)
				.Add("perpage", this.PerPage.ToString(), true).ToString();
		}

	}
}
