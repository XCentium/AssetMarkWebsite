<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
    private ID oTopSecondarySectionSelectedID =null ;
    private int iMaxItems;
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
		BindMenu();
	}

	private void BindMenu()
	{
		List<ID> oParentIds;
		Item oRootItem;
        oParentIds = ContextExtension.CurrentItem.GetParentItems().Select(oItem => oItem.ID).ToList();
		if ((oRootItem = ItemExtension.RootItem.GetChildren().Where(oItem => oParentIds.Contains(oItem.ID)).FirstOrDefault()) != null)
		{
            List<Item> oChildren=oRootItem.GetChildrenOfTemplate(new string[] { "Web Base" });
            Item oItemSelected = oChildren.Where(oItem => oParentIds.Contains(oItem.ID)).FirstOrDefault();
            if(oItemSelected!=null)
                oTopSecondarySectionSelectedID = oItemSelected.ID;
            rItems.DataSource = oChildren.Where(oItem => (oItem.GetText("Include in Navigation").Equals("1")) && !oItem.GetText("Include in Footer").Equals("1"));
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
            iMaxItems = ((IEnumerable<Item>)rItems.DataSource).Count() - 1;
			rItems.DataBind();
		}
		pSecondaryMenu.Visible = rItems.Items.Count > 0;
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		HyperLink hLink;
		HtmlGenericControl lListItem;
		string sCssClass =string.Empty;
		
		oItem = (Item)e.Item.DataItem;
		
		//does this repeater contain an item?
		if (oItem != null)
		{
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
			
			//set the link text
			hLink.Text = oItem.DisplayName;
			
			//set the link
			oItem.ConfigureHyperlink(hLink);
			
			
            if (oTopSecondarySectionSelectedID == oItem.ID)
            {
                sCssClass = "selected";
                
            }

            if (e.Item.ItemIndex == 0)
            {
                sCssClass = string.Format("{0} {1}", sCssClass, "first").Trim();
            }
            else
                if (e.Item.ItemIndex == iMaxItems)
                {
                    sCssClass = string.Format("{0} {1}", sCssClass, "last").Trim();
                }

            if (!string.IsNullOrEmpty(sCssClass))
            {
                lListItem.Attributes.Add("class", sCssClass);
            }
		}
	}
</script>
<asp:PlaceHolder ID="pSecondaryMenu" runat="server">
   <div class="secondaryMenuWrapper">
        <div class="secondaryMenuContainer">
			<ul class="secondaryMenu">
				<asp:Repeater ID="rItems" runat="server">
					<ItemTemplate>
						<li id="lListItem" runat="server">
							<img src="/CMSContent/Images/secondaryMenu_div.png" alt="" />
							<asp:HyperLink ID="hLink" runat="server" />
						</li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
		</div>
    </div>
</asp:PlaceHolder>