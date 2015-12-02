<%@ Control Language="C#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">

	const string sText = "{0} {1} {2}";
	 Item oDocument = null;
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		
			BindData();

		
	}

	void lbDownload_Click(object sender, EventArgs e)
	{
		
		if (oDocument != null)
		{
			Byte[] obuffer = new Byte[1024];
			int ibytes;
			MediaItem oMedia;
			System.IO.Stream oStream;
			if ((oMedia = ((Sitecore.Data.Fields.FileField)oDocument.GetField("Document", "File")).MediaItem) == null || (oStream = oMedia.GetMediaStream()) == null || !oStream.CanRead)
				return;

            //The following two are a fix for IE 8 https file download
            Response.ClearHeaders();
            Response.Clear();
			Response.ContentType = oMedia.MimeType;
			Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}.{1}",oMedia.Name,oMedia.Extension));
			while ((ibytes = oStream.Read(obuffer, 0, 1024)) > 0)
			{
				Response.OutputStream.Write(obuffer, 0, ibytes);
				
			}

			Response.End();
		}
	}

	private void BindData()
	{
		string sCode, sdate;
		Item oSelectedAllocationApproach;
		Item[] oTempItems;
		List<Item> oItems;
		
		DateTime odate;
		//get the code
		sCode = (Request.QueryString["AllocationApproach"] ?? string.Empty).Trim();
		//get the asset allocation approaches
		oItems = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Asset Allocation Approach");
		//is a code provided?
		if (!string.IsNullOrEmpty(sCode))
		{
			oTempItems = Genworth.SitecoreExt.Constants.Investments.Items.AllocationApproachFolderItem.GetChildrenOfTemplate("Asset Allocation Approach").Where(oAllocationApproach => oAllocationApproach["Code"] == sCode).ToArray();
			//use temp items as a placeholder to look up selected allocation approach
			//oTempItems = Genworth.SitecoreExt.Constants.Investments.Items.InvestmentsRootItem.Axes.SelectItems(new StringBuilder(@"Asset Allocation Approaches//*[@@TemplateName=""Asset Allocation Approach"" and @Code=""").Append(sCode).Append(@"""]").ToString());

			if (oTempItems != null)
			{
				//get the selected allocation approach, or first one from oItems if none selected
				oSelectedAllocationApproach = oTempItems.FirstOrDefault() ?? oItems.FirstOrDefault();
			}
			else
			{
				//get the first item from all allocation approaches
				oSelectedAllocationApproach = oItems.FirstOrDefault();
			}
		}
		else
		{
			//selected should be first
			oSelectedAllocationApproach = oItems.FirstOrDefault();
		}
		//set up the research link
		Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureHyperlink(hResearch);
        string AllocationApproach = oSelectedAllocationApproach.GetText("Asset Allocation Approach", "Code");
		hResearch.NavigateUrl = string.Format("{0}?AllocationApproach={1}&category=performance", hResearch.NavigateUrl, AllocationApproach);
        Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hResearch);
        
        var hResearchTextSpan = new HtmlGenericControl("span")
            {
                InnerText = oSelectedAllocationApproach.GetText("Asset Allocation Approach Views", "View Historical Button Text")
            };
        hResearch.Controls.Add(hResearchTextSpan);
        
		hResearch.Target = "_top";
		if (oSelectedAllocationApproach != null)
		{
			//first start with month performance
			if ((oDocument = oSelectedAllocationApproach.GetListItem("Documents", "Calendar Year Performance")) != null)
			{
				oDocument.ConfigureHyperlink(hCalendarYear);
				hCalendarYear.Text = string.Format(sText, oDocument.GetText("Title"), "- Calendar Year", "");
			}
			else
			{

				hCalendarYear.Visible = false;
			}

			oDocument = oSelectedAllocationApproach.GetListItem("Documents", "Calendar Month Performance") ?? oDocument;

		}

		if (oDocument != null && !string.IsNullOrEmpty(sdate = oDocument.GetText("Timing", "Date")) && DateTime.TryParseExact(sdate, "yyyyMMddTHHmmss", System.Threading.Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.None, out odate))
		{
			
			lAssetAllocation.Text = lTitle.Text = string.Format(sText, oDocument.GetText("Title"), ": as of", odate.ToShortDateString());
		}
		else
		{
			lAssetAllocation.Visible = false;
		}

        if (oDocument != null)
        {
            //Set omniture tag
             oDocument.ConfigureOmnitureControl(ContextExtension.CurrentItem, lbDownload);
             oDocument.ConfigureOmnitureControl(ContextExtension.CurrentItem, hCalendarYear);
        }
	}
</script>
<div class="performance-table-wrapper">
	<div class="tab-header">
		<table width="100%" cellpadding="0px" cellspacing="0">
			<tr>
				<td>
					<label>
						<asp:Literal runat="server" ID="lAssetAllocation"></asp:Literal></label>
					<img src="/CMSContent/Images/gridHeader_div.png" />
					<asp:HyperLink runat="server" Text="Asset Allocation - Calendar Year" ID="hCalendarYear" Target="_blank"></asp:HyperLink>
					<img src="/CMSContent/Images/gridHeader_div.png" />
				</td>
				<td class="td-right">
					<asp:HyperLink runat="server" CssClass="button performanceButton" ID="hResearch" />
				</td>
			</tr>
		</table>
		<h1>
			<span class="float-right"><a id="aPrintPdf" href="JavaScript: PrintPdf();" class="icon print-icon">
				Print</a>
				<img src="/CMSContent/Images/gridHeader_div.png" />
				<asp:LinkButton Text="Download" runat="server" CssClass="icon download-icon" ID="lbDownload" OnClick="lbDownload_Click" />
			</span>
			<asp:Literal runat="server" ID="lTitle"></asp:Literal>
		</h1>
	</div>
</div>
<script language="javascript" type="text/javascript">

	function PrintPdf() {
		if ($.browser.msie || $.browser.mozilla) {
			document.getElementById("pdfobject").print();
		} else {
			document.getElementById('iFramePdfViewer').contentWindow.print();
		}
	}

</script>
