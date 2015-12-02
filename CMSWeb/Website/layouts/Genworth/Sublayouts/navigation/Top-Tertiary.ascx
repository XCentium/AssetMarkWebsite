<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
     private ID oTopSecondarySectionSelectedID =null ;
     private int iMaxItems;
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
    	BindMenu();
	}

	private void BindMenu()
	{
		List<ID> oParentIds;
		Item oRootItem;
		Item oCurrentItem =ContextExtension.CurrentItem;
		oParentIds = oCurrentItem.GetParentItems().Select(oItem => oItem.ID).ToList();
        //Get firts level
        oRootItem = ItemExtension.RootItem.GetChildren().Where(oItem => oParentIds.Contains(oItem.ID)).FirstOrDefault();
        if (oRootItem != null)
        {
            //Get Second level
            oRootItem = oRootItem.GetChildren().Where(oItem => oParentIds.Contains(oItem.ID)).FirstOrDefault();
			
            if (oRootItem != null)
            {
                //Get Items on third level
                List<Item> oChildren = oRootItem.GetChildrenOfTemplate(new string[] { "Web Base" });
                Item oItemSelected = oChildren.Where(oItem => oParentIds.Contains(oItem.ID)).FirstOrDefault();
                if (oItemSelected != null)
                    oTopSecondarySectionSelectedID = oItemSelected.ID;
				//logic for prudential bold menu
				if (oCurrentItem.ID.ToString() == Genworth.SitecoreExt.Constants.Administration.PrudentialItemId)
				{
					oTopSecondarySectionSelectedID = new ID(Genworth.SitecoreExt.Constants.Administration.GenworthFinancialWealthManagementId);
				}
                rItems.DataSource = oChildren.Where(oItem => (oItem.InstanceOfTemplate("Link") || oItem.GetText("Include in Navigation").Equals("1")) && !oItem.GetText("Include in Footer").Equals("1"));
                rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
                iMaxItems = ((IEnumerable<Item>)rItems.DataSource).Count() - 1;
                rItems.DataBind();
            }
        }
		if (rItems.Items.Count == 0)
		{
			uTertiaryMenu.Visible = false;
			divTertiaryWrapper.Visible = false;
		}
	}

    private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        HyperLink hLink;
        HtmlGenericControl lListItem;
        HtmlImage iSeparator;
        string sCssClass = String.Empty;

        oItem = (Item)e.Item.DataItem;

        //does this repeater contain an item?
        if (oItem != null)
        {
            //get the web controls
            hLink = (HyperLink)e.Item.FindControl("hLink");
            lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
            iSeparator = (HtmlImage)e.Item.FindControl("iSeparator");
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
                iSeparator.Visible = false;
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
<div style="margin: 15px auto;" id="divTertiaryWrapper" runat="server">
	<ul class="AdminTertiaryMenu" runat="server" id="uTertiaryMenu">
		<asp:Repeater runat="server" ID="rItems">
			<ItemTemplate>
				<li  runat="server" id="lListItem">
					<img src="/CMSContent/Images/divider-tertiary.png" runat="server" id="iSeparator" /><asp:HyperLink ID="hLink"
						runat="server" /></li>
			</ItemTemplate>
		</asp:Repeater>
    
	</ul>
</div>
