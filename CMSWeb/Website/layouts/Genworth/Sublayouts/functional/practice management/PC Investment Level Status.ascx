<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="Sitecore.Collections" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt.Utilities.GridComponent" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>
<%@ Register Src="~/layouts/Genworth/Sublayouts/grid/Html Table.ascx" TagPrefix="Gen" TagName="HtmlTable" %>

<script runat="server">
	
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

	private void BindData()
	{
		Dictionary<string, GridTable> tables = HtmlTableHelper.GetInvestmentLevelStatusTables();

		if (tables != null)
		{

			if (tables.ContainsKey(HtmlTableHelper.ILSFirmCODE))
			{
				HtmlTable1.DataSource = tables[HtmlTableHelper.ILSFirmCODE];
				HtmlTable1.DataBind();
			}
			else
			{
				top.Visible = false;
			}

			if (tables.ContainsKey(HtmlTableHelper.ILSAdvisorCODE))
			{
				HtmlTable2.DataSource = tables[HtmlTableHelper.ILSAdvisorCODE];
				HtmlTable2.DataBind();
			}
			else
			{
				NoSecondData.Visible = true;
			}
		}
	}

</script>
<sc:Placeholder Key="top" ID="top" runat="server" />
<Gen:HtmlTable runat="server" ID="HtmlTable1" />
<sc:Placeholder Key="middle" runat="server" />
<Gen:HtmlTable runat="server" ID="HtmlTable2" />
<sc:Placeholder Key="bottom" runat="server" />
<p runat="server" id="NoSecondData" visible="false">
	Net Contributions, Eligible AUM and related BDA amounts are only shown to advisors who are the key office contacts for their firm. For more information about the key office contact, Net Contributions, Eligible AUM or BDA accruals, please contact Advisor Services at 1-800-664-5345, option 1.
</p>