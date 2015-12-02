<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
	private Item oSelectedAllocationApproach;

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

		//get the code
		sCode = (Request.QueryString["AllocationApproach"] ?? string.Empty).Trim();

		//get the asset allocation approaches
		oItems = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate(new string[] { "Asset Allocation Approach", "Strategist Comparison Glossary" });

		if (!string.IsNullOrEmpty(sCode))
		{
			//get the selected allocation approach

			oSelectedAllocationApproach = Genworth.SitecoreExt.Constants.Investments.Items.AllocationApproachFolderItem.GetChildrenOfTemplate("Asset Allocation Approach").Where(oAllocationApproach => oAllocationApproach["Code"] == sCode).FirstOrDefault();
			//.Axes.SelectItems(new StringBuilder(@"Asset Allocation Approaches//*[@@TemplateName=""Asset Allocation Approach"" and @Code=""").Append(sCode).Append(@"""]").ToString()).FirstOrDefault();
		}

		//bind the items
		rItems.DataSource = oItems;
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();

		//hide the items if nothing to show
		pItems.Visible = rItems.Items.Count > 0;
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

			//is this an asset allocation approach?
			if (oItem.InstanceOfTemplate("Asset Allocation Approach"))
			{
				//set the link text
				hLink.Text = oItem.DisplayName;
				hLink.NavigateUrl = string.Format("{0}?AllocationApproach={1}", Genworth.SitecoreExt.Constants.Investments.Items.CompareItem.GetURL(), oItem.GetText("Asset Allocation Approach", "Code"));
			}
			else
			{
				//bind normally
				hLink.NavigateUrl = oItem.GetURL();
				hLink.Text = oItem.GetText("Page", "Title");
			}

			//if we are in the selected path, set the css class			
			if (oItem.InSelectedPath() || (oSelectedAllocationApproach != null && oSelectedAllocationApproach.ID.Equals(oItem.ID)))
			{
				lListItem.Attributes.Add("class", "selected");
			}
			if (e.Item.ItemIndex == 0)
			{
				lListItem.Attributes.Add("class", string.Concat("first ", lListItem.Attributes["class"]));
			}
		}
	}
</script>
<%--<style>
	body.ModalWindowBody
	{
		background-color: #FFFFFF;
	}
</style>--%>

<asp:PlaceHolder ID="pItems" runat="server">
	<div id="MyUniqeId" class="dialog-wrapper">
	<div class="dialog">
		<h2>
			Comparison</h2>
		<div class="dialog-content">
			<div class="comparison-table-wrapper">
				<div class="tabs-wrapper">
					<ul class="tabs">
						<asp:Repeater ID="rItems" runat="server">
							<ItemTemplate>
								<li id="lListItem" runat="server">
									<asp:HyperLink ID="hLink" runat="server" /></li>
							</ItemTemplate>
						</asp:Repeater>
					</ul>
					<div class="clear">
					</div>
					<div class="tab-theater">
					<sc:Placeholder Key="TabContent" runat="server" />
					</div>
				</div>
			</div>
		</div>
	</div>
</div>
</asp:PlaceHolder>
