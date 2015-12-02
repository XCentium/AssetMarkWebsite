<%@ Control Language="c#" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt.Providers" %>
<script runat="server">
	private static Regex oIdCleaner = new Regex("[^a-z0-9]", RegexOptions.IgnoreCase);
    private IJsonCollectionProvider gridDataProvider;
    private string currentItemId;
	protected override void OnInit(EventArgs e)
	{
        currentItemId = ContextExtension.CurrentItem.ID.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
        
		//get a handle to the current research
        string SectionType = ContextExtension.CurrentItem.GetText("Section", "Section Type", null) ?? this.GetParameter("Search Type");

        gridDataProvider = Genworth.SitecoreExt.Helpers.GridHelper.GetProvider(SectionType);
		
		//let the base do its job first
		base.OnInit(e);
		
		//we want to do this during INIT to get ahead of other loading processes
        if (!HttpContext.Current.Request.Url.AbsolutePath.ToLower().Contains("research") && gridDataProvider.SetURLFilters(Request.QueryString))
		{
			//refresh
			Response.Redirect(gridDataProvider.URL);
		}
    }
	
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}

        // go get the proper bundle url from EWM and then inject into the head block on this page.
        string bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("/Sitecore/InvestmentsClientView/Research", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Scripts);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddJsToPage(this.Page, bundleUrl);
        }

        bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("/Sitecore/InvestmentsClientView/Research", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Styles);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddCssToPage(this.Page, bundleUrl);
        }
    }

	private void BindData()
	{
		
		rColumns.DataSource =gridDataProvider.Columns;
		rColumns.ItemDataBound += new RepeaterItemEventHandler(rColumns_ItemDataBound);
		rColumns.DataBind();
	}

	void rColumns_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		KeyValuePair<string, string> oColumn;
		Literal lColumn;

		oColumn = (KeyValuePair<string, string>)e.Item.DataItem;
		lColumn = (Literal)e.Item.FindControl("lColumn");
		lColumn.Text = string.Format("<a href=\"#\" onclick=\"{1}return false;\">{0}</a>", oColumn.Key, oColumn.Value.Length > 0 ? string.Format("researchSetSort('{0}'); ", oColumn.Value) : string.Empty);
	}
</script>
<script type="text/javascript">
	$().ready(function () {
	    GridInitialize('<%=gridDataProvider.Code %>', '<%=gridDataProvider.JsonServiceUrl %>', '<%=currentItemId %>');
	});
	
</script>
<div>
	<table class="InvestmentResearchGrid filter-results-table" width="100%" cellpadding="0px" cellspacing="0px">
		<thead>
			<asp:Repeater ID="rColumns" runat="server">
				<ItemTemplate>
					<th><asp:Literal ID="lColumn" runat="server" /></th>
				</ItemTemplate>
			</asp:Repeater>
		</thead>
		<tbody></tbody>
	</table>
</div>