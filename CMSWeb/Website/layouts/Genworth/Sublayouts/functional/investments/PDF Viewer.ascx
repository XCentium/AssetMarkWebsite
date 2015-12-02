<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
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
		//what kind of PDF are we rendering?
		if (ContextExtension.CurrentItem.InstanceOfTemplate(new []{"Strategist Performance", "Strategy Performance Page"}))
		{
			BindStrategistPerformancePDF();
		}
	}
	
	private void BindStrategistPerformancePDF()
	{
		Item oSelectedAllocationApproach;
		Item[] oItems;
		string sDocument;
		string sCode;
		Item oDocument;

		//initialize our document to null
		oDocument = null;
		
		//do we have a document code?
		if (!string.IsNullOrEmpty(sDocument = (Request.QueryString["Document"] ?? string.Empty).Trim()))
		{
			//do we have document?
			oDocument = ContextExtension.CurrentDatabase.GetItem(sDocument);
		}
		
		//is our document still null?
		if (oDocument == null)
		{
			//get the code for the allocation approach
			sCode = (Request.QueryString["AllocationApproach"] ?? string.Empty).Trim();

			//do we have a code and selected allocation approach?
			if (!string.IsNullOrEmpty(sCode))
			{
				//get the items that could be holding the selected allocation approach
				oItems = Genworth.SitecoreExt.Constants.Investments.Items.AllocationApproachFolderItem.GetChildrenOfTemplate("Asset Allocation Approach").Where(oAllocationApproach => oAllocationApproach["Code"] == sCode).ToArray();
				
					//ContextExtension.CurrentDatabase.GetItem(Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.Root")).Axes.SelectItems(new StringBuilder(@"Asset Allocation Approaches//*[@@TemplateName=""Asset Allocation Approach"" and @Code=""").Append(sCode).Append(@"""]").ToString());

				//get the selected allocation approach
				oSelectedAllocationApproach = oItems != null ? oItems.FirstOrDefault() : ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Asset Allocation Approach").FirstOrDefault();
			}
			else
			{
				//get the selected allocation approach
				oSelectedAllocationApproach = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Asset Allocation Approach").FirstOrDefault();
			}

			//is the allocation approach non-null?
			if (oSelectedAllocationApproach != null)
			{
				//first start with month performance
				if ((oDocument = oSelectedAllocationApproach.GetListItem("Documents", "Calendar Month Performance")) == null)
				{
					//if no month performance, move on to yaer performance
					oDocument = oSelectedAllocationApproach.GetListItem("Documents", "Calendar Year Performance");
				}
			}
		}
		
		//do we have a document?
		if (oDocument != null)
		{
			//bind the document
			BindPDF(oDocument);
		}
	}

	private void BindPDF(Item document)
	{
		string path = document.GetImageURL("Document", "File");

		//does the document have a file?
		if (!string.IsNullOrEmpty(path))
		{
			//output an iframe
			System.Web.HttpBrowserCapabilities browser = Request.Browser;

			if (browser.Browser.Equals("IE", StringComparison.OrdinalIgnoreCase) ||
				browser.Browser.Equals("FIREFOX", StringComparison.OrdinalIgnoreCase))
			{
				lPDF.Text = new StringBuilder("<object id=\"pdfobject\" data=\"")
					.Append(path)
					.Append("#view=FitH&amp;pagemode=thumbs&amp;toolbar=0&amp;statusbar=0&amp;messages=0&amp;navpanes=0\" width=\"100%\" type=\"application/pdf\" height=\"1000px\"></object>")
					.ToString();
			}
			else
			{
				lPDF.Text = new StringBuilder("<iframe id=\"iFramePdfViewer\" height=\"600px\" width=\"100%\" src=\"")
					.Append(path)
					.Append("\">Your browser does not support IFrames.</iframe>")
					.ToString();
			}
		}
	}

</script>
<style>
		/*.secondaryMenuWrapper
		{
			width:80%;
		}
		iframe
		{
			width:80%;
			height:80%;
		}
		*/
</style>
<asp:Literal ID="lPDF" runat="server" />