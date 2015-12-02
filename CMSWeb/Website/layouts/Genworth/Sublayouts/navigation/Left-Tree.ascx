<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
	
    private int iNavigationDepth;
    private static readonly int MAX_DEPTH = 3;
    private Item oCurrentItem;
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        BindData();
        HideTitleIfNeeded();
    }

    private void BindData()
    {
        Item oRootItem;
        List<Item> oParentItems;
        oCurrentItem = ContextExtension.CurrentItem;

        oParentItems = oCurrentItem.GetParentItems();
        
        oRootItem = oParentItems.Where(item => item.InstanceOfTemplate("Link")).FirstOrDefault();
        if (oRootItem != null)
        {
            lSummary.Text = oRootItem.DisplayName.ToUpper();
        }
        oParentItems.Reverse();
        if ((oRootItem = oParentItems.Skip(2).FirstOrDefault()) != null)
        {
            rNavigation.DataSource = oRootItem.GetChildrenOfTemplate(new string[] { "Web Base", "Document Base" }).Where(oItem => (oItem.InstanceOfTemplate("Link") || oItem.GetText("Include in Navigation").Equals("1")) && !oItem.GetText("Include in Footer").Equals("1"));
            rNavigation.ItemDataBound += new RepeaterItemEventHandler(rNavigation_ItemDataBound);
            rNavigation.DataBind();
        }
    }

    protected void rNavigation_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        Label lItem;
        HyperLink hLink;
        Repeater rNavigation;
        HtmlGenericControl lListItem;
		HtmlGenericControl ulChildList;
		bool hasChildren;
		
        string sCssClass =string.Empty;

        //increment navigation depth
        iNavigationDepth++;

        //get the item
        oItem = (Item)e.Item.DataItem;

        //get the link, label and repeater
        lItem = (Label)e.Item.FindControl("lItem");
        hLink = (HyperLink)e.Item.FindControl("hLink");
        rNavigation = (Repeater)e.Item.FindControl("rNavigation");
		lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
		ulChildList = (HtmlGenericControl)e.Item.FindControl("ulChildList");
		hasChildren = false;

        //is this a folder?
        if (oItem.InstanceOfTemplate("Folder"))
        {
            //just output a title
            lItem.Text = oItem.DisplayName;

            //hide the link
            hLink.Visible = false;
        }
        else
        {
            //bind the link
            hLink.Text = "<img src='/CMSContent/Images/sec-nav-active-bg.png' class='selectedArrow'><span class='linkText'>" + oItem.DisplayName + "<span class='icon'></span></span>";
            if (oItem.InstanceOfTemplate("Calculator"))
            {
                hLink.NavigateUrl = oItem.GetURL();
            }
            else
            {
                oItem.ConfigureHyperlink(hLink);
            }
            //hide the label
            lItem.Visible = false;
        }

		if (iNavigationDepth < MAX_DEPTH)
		{
			//set up the child
			System.Collections.Generic.IEnumerable<Sitecore.Data.Items.Item> lDataSource;

			lDataSource = oItem.GetChildrenOfTemplate(new string[] { "Web Base", "Document Base" }).Where(oTemp => (oTemp.InstanceOfTemplate("Link") || oTemp.GetText("Include in Navigation").Equals("1")) && !oTemp.GetText("Include in Footer").Equals("1"));
			hasChildren = lDataSource.Count() > 0;
			if (hasChildren)
			{
				ulChildList.Visible = true;
				rNavigation.DataSource = lDataSource;
				rNavigation.ItemTemplate = ((Repeater)e.Item.Parent).ItemTemplate;
				rNavigation.ItemDataBound += new RepeaterItemEventHandler(rNavigation_ItemDataBound);
				rNavigation.DataBind();
			}
			else
			{
				ulChildList.Visible = false;
			}
		}
		else {
			ulChildList.Visible = false;	
		}

		if (hasChildren) {
            sCssClass = string.Concat(sCssClass, "hasChildren ");
		}
        if( oItem.ID == oCurrentItem.ID  )
        {
            sCssClass = string.Concat(sCssClass, "selected ");
        }
        if (e.Item.ItemIndex == 0 )
        {
            sCssClass=string.Concat(sCssClass,"first ");
            
        }
        lListItem.Attributes.Add("class", sCssClass);
        //decrement navigation depth
        iNavigationDepth--;
    }

    private void HideTitleIfNeeded()
    {
        // Check the parameter called "Hide Menu Title" found on the
        // presentation detail of the Left-Tree control to see if the
        // title of the menu should be displayed or not.
        var sHideMenu = this.GetParameter("Hide Menu Title");
        
        if (sHideMenu != null && sHideMenu.Equals("1"))
        {
            lMenuTitle.Visible = false;
        }
    }

</script>
<ul class="verticalTabs" id="verticalTabsMenu">
    <li class="summary" id="lMenuTitle" runat="server">
        <p class="nav-title">
            <asp:Literal runat="server" ID="lSummary"></asp:Literal>
            </p>
            
    </li>
    <asp:Repeater ID="rNavigation" runat="server">
        <ItemTemplate>
            <li id="lListItem" runat="server">
                <asp:Label ID="lItem" runat="server" />
                <asp:HyperLink ID="hLink" runat="server" />
                <ul id="ulChildList" runat="server">
                    <asp:Repeater ID="rNavigation" runat="server" />
                </ul>
            </li>
        </ItemTemplate>
    </asp:Repeater>
</ul>
<script type="text/javascript">
    Genworth.PracticeManagement.MenuExpand();
</script>