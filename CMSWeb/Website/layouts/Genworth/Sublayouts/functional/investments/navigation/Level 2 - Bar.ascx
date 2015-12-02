<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
	
	int iLast;
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
		Item oRootItem;
		Item oProductLinkItem;	//Brought over from Level 1 Links.ascx
		List<Item> oParentItems;
		IEnumerable<Item> oItems;
		//get the parent items		
		oParentItems = ContextExtension.CurrentItem.GetParentItems();

		//reverse the parent items		
		oParentItems.Reverse();
		
		//skip 1 level deep
		if ((oRootItem = oParentItems.Skip(2).FirstOrDefault()) != null)
		{
			oItems = oRootItem.GetChildrenOfTemplate(new string[] { "Web Base", "Document Base" }).Where(oItem => (oItem.InstanceOfTemplate("Link") || oItem.GetText("Include in Navigation").Equals("1")) && !oItem.GetText("Include in Footer").Equals("1"));
			if ((iLast = oItems.Count()) > 0)
			{
				iLast--;
				rItems.DataSource=oItems;
				rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
				rItems.DataBind();
			}
			else	//hide the items if nothing to show
				pItems.Visible = false;
		}
		else
		{
			//hide the items
			pItems.Visible = false;
		}

		// bind product link
		if ((oProductLinkItem = Genworth.SitecoreExt.Constants.Investments.Items.ProductLink) != null)
		{
			oProductLinkItem.ConfigureHyperlink(hProductLink);
			hProductLink.Text = oProductLinkItem.DisplayName;
			hProductLink.Visible = true;
            //Set Omniture tags
            oProductLinkItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hProductLink);
		}
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		HyperLink hLink;
		HtmlGenericControl lListItem;
		StringBuilder sClass;
		
		//does this repeater contain an item?
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
			
			//set the link text
			hLink.Text = oItem.DisplayName;
			oItem.ConfigureHyperlink(hLink);

			//create a stringbuilder to hold class info
			sClass = new StringBuilder();

			//if we are in the selected path, set the css class			
			if (oItem.InSelectedPath())
			{
				sClass.Append(" selected");
			}

			//is this the first item?
			if (e.Item.ItemIndex == 0)
			{
				sClass.Append(" first");
			}
			else if (e.Item.ItemIndex == iLast)
			{
				sClass.Append(" last");
			}

			//append the styling
			if (sClass.Length > 0)
			{
				lListItem.Attributes.Add("class", sClass.ToString().Trim());
			}
		}
	}
</script>
<asp:PlaceHolder ID="pItems" runat="server">
	<div class="grid-system g982">
		<div class="gc c12">
			<div class="investments-navigation">
				<span class="float-right">
					<asp:HyperLink ID="hProductLink" runat="server" Visible="false"></asp:HyperLink>
					<%--<img src="/CMSContent/Images/gridHeader_div.png">
					<a id="aPrintPdf" href="JavaScript: PrintPdf();" class="icon print-icon">Print</a>--%>
				</span>
				<ul class="horizontal-navigation">
					<asp:Repeater ID="rItems" runat="server">
						<ItemTemplate>
							<li id="lListItem" runat="server"><asp:HyperLink ID="hLink" runat="server" /></li>
						</ItemTemplate>
					</asp:Repeater>
				</ul>
				<div class="clear"></div>
			</div>
		</div>
		<div class="clear"></div>
	</div>
</asp:PlaceHolder>