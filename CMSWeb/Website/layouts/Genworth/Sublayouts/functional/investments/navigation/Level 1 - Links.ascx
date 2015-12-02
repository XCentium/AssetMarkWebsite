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

        // go get the proper bundle url from EWM and then inject into the head block on this page.
        string bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/Investments", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Scripts);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddJsToPage(this.Page, bundleUrl);
        }

        bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/Investments", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Styles);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddCssToPage(this.Page, bundleUrl);
        }
        
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

	private void BindData()
	{
		Item oRootItem;
		List<Item> oParentItems;

		//get the parent items		
		oParentItems = ContextExtension.CurrentItem.GetParentItems();

		//reverse the parent items		
		oParentItems.Reverse();
		
		//skip 1 level deep
		if ((oRootItem = oParentItems.Skip(1).FirstOrDefault()) != null)
		{
			rItems.DataSource = oRootItem.GetChildrenOfTemplate(new string[] { "Web Base", "Document Base" }).Where(oItem => oItem.GetText("Include in Navigation").Equals("1"));
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
			rItems.DataBind();
			
			//hide the items if nothing to show
			pItems.Visible = rItems.Items.Count > 0;
		}
		else
		{
			//hide the items
			pItems.Visible = false;
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
			
			//append the styling
			if (sClass.Length > 0)
			{
				lListItem.Attributes.Add("class", sClass.ToString().Trim());
			}
		}
	}
</script>
<asp:PlaceHolder ID="pItems" runat="server">
	<div class="secondaryMenuWrapper">
		<div class="secondaryMenuContainer">
			<ul class="secondaryMenu">
				<asp:Repeater ID="rItems" runat="server">
					<ItemTemplate>
						<li id="lListItem" runat="server"><img src="/CMSContent/Images/secondaryMenu_div.png" alt="" /><asp:HyperLink ID="hLink" runat="server" /></li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
		</div>
	</div>
</asp:PlaceHolder>