using Genworth.SitecoreExt.Providers;
namespace Genworth.SitecoreExt.Services.Investments
{
    public interface IInvestmentsSearch : IJsonCollectionProvider
	{
		Filter[] Filters { get; }
		void Reset(bool bUseDefaultValues);
		void SetFilterOption(string sFilter, string sOption, bool bFiltered);
		int Months { get; }
    	bool ShowFilterBar { get; }
	}
}
