using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Data.Items;
using ServerLogic.Core.Web.Utilities;
using ServerLogic.SitecoreExt;


namespace Genworth.SitecoreExt.Utilities
{
	public class GenworthQueryString : QueryString
	{
		#region Contruction / Destruction -->
		
		public GenworthQueryString() { }
		
		public GenworthQueryString(string queryString) : base(queryString) { }

		public GenworthQueryString(GenworthQueryString queryString)
		{
			this.Add(queryString);
		}

		new public static GenworthQueryString Current
		{
			get
			{
				GenworthQueryString retVal = new GenworthQueryString();
				retVal.FromCurrent();
				return retVal;
			}
		}

		#endregion

		#region Article list functionality -->

		public bool ContainsArticleSearch
		{
			get
			{
				return (!string.IsNullOrWhiteSpace(base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword]));
			}
		}

		public string ArticleKeyword
		{
			get
			{
				return base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword];
			}
		}

		/// <summary>
		/// True if the query string contains parameters for ascending article sort, false in all other circumstances
		/// </summary>
		public bool ArticleSortAscending
		{
			get { return base["sort"] == "date"; }
		}

		#endregion

		#region FAQ list  functionality -->

		public bool ContainsFAQSearch
		{
			get
			{
				return !(String.IsNullOrWhiteSpace(base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword])
					&& String.IsNullOrWhiteSpace(base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Category]));
			}
		}

		public string FAQCategory
		{
			get
			{
				return base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Category];
			}
		}

		public string FAQKeyword
		{
			get
			{
				return base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword];
			}
		}
		#endregion


		#region Glossary list related functionality -->

		public bool ContainsGlossarySearch
		{
			get
			{
				return !(String.IsNullOrWhiteSpace(base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword])
					&& String.IsNullOrWhiteSpace(base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.StartsWith]));
			}
		}

		public string GlossaryKeyword
		{
			get
			{
				return base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.Keyword];
			}
		}

		public string GlossaryLetter
		{
			get
			{
				return base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.StartsWith];
			}
		}

        public string GlossaryTermID
        {
            get
            {
                return base[Genworth.SitecoreExt.Constants.HelpCenter.QueryParameters.ID];
            }
        }

		#region

		public void RemovePagination()
		{
			Remove("perpage");
			Remove("pagenumber");
		}

		#endregion

		#endregion
	}
}
