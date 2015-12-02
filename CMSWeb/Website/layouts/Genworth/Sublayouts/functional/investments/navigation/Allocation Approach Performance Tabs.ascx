<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
	private Item oSelectedAllocationApproach;
	string sDocument;
	
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
		string sCode;
		List<Item> oItems;
		Item[] oTempItems;
		List<KeyValuePair<string, Item>> oSubItems;
		Item oItem;

		//get the code
		sCode = (Request.QueryString["AllocationApproach"] ?? string.Empty).Trim();
		sDocument = (Request.QueryString["Document"] ?? string.Empty).Trim();

		//get the asset allocation approaches
		oItems = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Asset Allocation Approach");

		//is a code provided?
		if (!string.IsNullOrEmpty(sCode))
		{
			//use temp items as a placeholder to look up selected allocation approach
			oTempItems = Genworth.SitecoreExt.Constants.Investments.Items.AllocationApproachFolderItem.GetChildrenOfTemplate("Asset Allocation Approach").Where(oAllocationApproach => oAllocationApproach["Code"] == sCode).ToArray();
				//ContextExtension.CurrentDatabase.GetItem(Sitecore.Configuration.Settings.GetSetting("Genworth.SitecoreExt.Pages.Investments.Root")).Axes.SelectItems(new StringBuilder(@"Asset Allocation Approaches//*[@@TemplateName=""Asset Allocation Approach"" and @Code=""").Append(sCode).Append(@"""]").ToString());

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
		
		//bind the items
		rItems.DataSource = oItems;
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();

		//hide the items if nothing to show
		pItems.Visible = rItems.Items.Count > 0;
		
		if (oSelectedAllocationApproach != null)
		{
			oSubItems = new List<KeyValuePair<string, Item>>();
			if ((oItem = oSelectedAllocationApproach.GetListItem("Documents", "Calendar Month Performance")) != null)
			{
				oSubItems.Add(new KeyValuePair<string, Item>(string.Format("{0} - as of {1}",
					oSelectedAllocationApproach.GetText("Asset Allocation Approach", "Title"),
					oItem.GetField("Timing", "Date").GetDateString(Genworth.SitecoreExt.Constants.Investments.DateFormat, "Unspecified")),
					oItem));
			}
			if ((oItem = oSelectedAllocationApproach.GetListItem("Documents", "Calendar Year Performance")) != null)
			{
				oSubItems.Add(new KeyValuePair<string, Item>(string.Format("{0} - Calendar Year",
					oSelectedAllocationApproach.GetText("Asset Allocation Approach", "Title")),
					oItem));
			}
			
			//do we have a document?
			if (string.IsNullOrEmpty(sDocument) && oSubItems.Count > 0)
			{
				//get the id of the first item
				sDocument = oSubItems[0].Value.ID.ToString();
			}

			//rSubItems.DataSource = oSubItems;
			//rSubItems.ItemDataBound += new RepeaterItemEventHandler(rSubItems_ItemDataBound);
			//rSubItems.DataBind();

			////should we hide the subitems?
			//pSubItems.Visible = rSubItems.Items.Count > 0;
			
			
		}
		//else
		//{
		//    //hide the sub items
		//    pSubItems.Visible = false;
		//    hResearch.Visible = false;
		//}
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		HyperLink hLink;
		HtmlGenericControl lListItem;

		//does this repeater contain an item?
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
			
			//set the link text
			hLink.Text = oItem.DisplayName;
			hLink.NavigateUrl = string.Format("{0}?AllocationApproach={1}", Genworth.SitecoreExt.Constants.Investments.Items.PerformanceItem.GetURL(), oItem.GetText("Asset Allocation Approach", "Code"));

            if (ContextExtension.CurrentItem.InstanceOfTemplate("Strategy Performance Page"))
            {
                hLink.Attributes["href"] = "?AllocationApproach=" + oItem.GetText("Asset Allocation Approach", "Code");
            }

			//if we are in the selected path, set the css class			
			if (oItem.InSelectedPath() || (oSelectedAllocationApproach != null && oSelectedAllocationApproach.ID.Equals(oItem.ID)))
			{
				lListItem.Attributes.Add("class", "selected withHeader");
			}
            
            //Set Omniture tag
            oItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
		}
	}

	private void rSubItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		KeyValuePair<string, Item> oItem;
		HyperLink hLink;
		HtmlGenericControl lListItem;
		
		//does this repeater contain an item?
		if (e.Item.DataItem != null && e.Item.DataItem is KeyValuePair<string, Item>)
		{
			//get the item
			oItem = (KeyValuePair<string, Item>)e.Item.DataItem;
			
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");

			//set the link text
			hLink.Text = oItem.Key;
			hLink.NavigateUrl = string.Format("{0}?AllocationApproach={1}&Document={2}", Genworth.SitecoreExt.Constants.Investments.Items.PerformanceItem.GetURL(), oSelectedAllocationApproach.GetText("Asset Allocation Approach", "Code"), oItem.Value.ID.ToString());

			//if we are in the selected path, set the css class			
			if (oItem.Value.ID.ToString().Equals(sDocument))
			{
				lListItem.Attributes.Add("class", "selected ");
			}
		}
	}
</script>
<style>
	body.ModalWindowBody
	{
		background-color: #FFFFFF;
	}
</style>
<asp:PlaceHolder ID="pItems" runat="server">
	<div id="MyUniqeId" class="dialog-wrapper">
		<div class="dialog">
			<h2>
				Performance</h2>
			<div class="dialog-content">
				<div class="tabs-wrapper">
					<ul class="tabs">
						<asp:Repeater ID="rItems" runat="server">
							<ItemTemplate>
								<li id="lListItem" runat="server"><asp:HyperLink ID="hLink" runat="server" /></li>
							</ItemTemplate>
						</asp:Repeater>
					</ul>
				</div>
				<div class="clear"></div>
				<div class="tab-theater">
					<sc:Placeholder ID="Placeholder1" Key="TabContent" runat="server" />
				</div>
			</div>
		</div>
	</div>
</asp:PlaceHolder>
<%--<asp:PlaceHolder ID="pSubItems" runat="server">
	<div class="secondaryMenuWrapper">
		<div class="secondaryMenuContainer">
			<ul class="secondaryMenu">
				<asp:Repeater ID="rSubItems" runat="server">
					<ItemTemplate>
						<li id="lListItem" runat="server"><asp:HyperLink ID="hLink" runat="server" /></li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
		</div>
	</div>
</asp:PlaceHolder>
<asp:HyperLink ID="hResearch" runat="server" />--%>

