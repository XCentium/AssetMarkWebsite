<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">
	
    int iIndex = 1;

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
        Item oCurrentItem;
        List<Item> oItems;

        //get hte current item
        oCurrentItem = ContextExtension.CurrentItem;

        //get the items
        if ((oItems = oCurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Solution Type")).Count > 0)
        {
            rItems.DataSource = oItems;
            rItems.ItemDataBound += new RepeaterItemEventHandler(rSolutionTypes_ItemDataBound);
            rItems.DataBind();
        }
        else if ((oItems = oCurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Strategist")).Count > 0)
        {
            rItems.DataSource = oItems;
            rItems.ItemDataBound += new RepeaterItemEventHandler(rStrategists_ItemDataBound);
            rItems.DataBind();
        }

        //hide the items if nothing to show
        pItems.Visible = rItems.Items.Count > 0;
    }

   
    private void rSolutionTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        Repeater rStrategies;
        Dictionary<ID, KeyValuePair<Item, List<Item>>> oStrategists;
        //does this repeater contain an item?
        if ((oItem = (Item)e.Item.DataItem) != null)
        {
            //get the web controls
            rStrategies = (Repeater)e.Item.FindControl("rStrategies");

            //find and bind the strategies associated with this allocation approach
            string sid = oItem.ID.ToString();


            //rStrategies.DataSource = Genworth.SitecoreExt.Constants.Investments.Items.InvestmentsRootItem.Axes.SelectItems(new StringBuilder(@"strategists//*[@@TemplateName=""Strategist""]//*[@@TemplateName=""Solution"" AND contains(@Type, """).Append(oItem.ID.ToString()).Append(@""")]").ToString());
            //Line above removed for perfomance issues, new line (below)
            oStrategists = new Dictionary<Sitecore.Data.ID, KeyValuePair<Item, List<Item>>>();
            foreach (Item oStrategist in Genworth.SitecoreExt.Constants.Investments.Items.StrategitsFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategist))
            {
                foreach (Item oAllocationapproach in oStrategist.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy))
                {
                    List<Item> oSolutionsType = oAllocationapproach.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution).Where(oGranChild => oGranChild["Type"] == sid).ToList();
                    if (oSolutionsType.Count > 0)
                    {
                        if (!oStrategists.ContainsKey(oStrategist.ID))
                        {
                            oStrategists.Add(oStrategist.ID, new KeyValuePair<Item, List<Item>>(oStrategist, oSolutionsType));
                        }
                        else
                            oStrategists[oStrategist.ID].Value.AddRange(oSolutionsType);
                    }
                }
            }
            foreach (Item oStrategist in Genworth.SitecoreExt.Constants.Investments.Items.StrategitsFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.StrategistNoAllocation))
            {
                List<Item> oSolutionsType = oStrategist.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution).Where(oGranChild => oGranChild["Type"] == sid).ToList();
                if (oSolutionsType.Count > 0)
                {
                    if (!oStrategists.ContainsKey(oStrategist.ID))
                    {
                        oStrategists.Add(oStrategist.ID, new KeyValuePair<Item, List<Item>>(oStrategist, oSolutionsType));
                    }
                    else
                        oStrategists[oStrategist.ID].Value.AddRange(oSolutionsType);
                }

            }


            //This is the launch day code with special ordering for Altegris and GPS Strategists
            //rStrategies.DataSource = Genworth.SitecoreExt.Constants.Investments.Items.StrategitsFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategist).SelectMany(oChild => oChild.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy).SelectMany(o2Child => o2Child.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution).Where(oGranChild => oGranChild["Type"] == sid)))
            //    .Concat(Genworth.SitecoreExt.Constants.Investments.Items.StrategitsFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.StrategistNoAllocation).SelectMany(oChild => oChild.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution).Where(oGranChild => oGranChild["Type"] == sid))); 
            //Patch for altergis and GPS that don't have strategy
            rStrategies.DataSource = oStrategists.Values.OrderBy(oStrategist=> oStrategist.Key.DisplayName);
            rStrategies.ItemDataBound += new RepeaterItemEventHandler(rStrategies_ItemDataBound);
            rStrategies.DataBind();
        }
    }

    private void rStrategists_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        Repeater rStrategies;

        //does this repeater contain an item?
        if ((oItem = (Item)e.Item.DataItem) != null)
        {
            //get the web controls
            rStrategies = (Repeater)e.Item.FindControl("rStrategies");

            //find and bind the strategies associated with this allocation approach
            rStrategies.DataSource = new Item[] { oItem };
            rStrategies.ItemDataBound += new RepeaterItemEventHandler(rStrategies_ItemDataBound);
            rStrategies.DataBind();
        }
    }

    private void rStrategies_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        Item oStrategist;
        Image iLogo;
        Panel pIcon;
        PlaceHolder pSolutions;
        Repeater rSolutions;
        HyperLink hResearch;
        HtmlGenericControl lListItem;
        StringBuilder sClass;
        Panel pClear;
       
        //does this repeater contain an item?
        if (e.Item.ItemType== ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            //get the web controls
            lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
            iLogo = (Image)e.Item.FindControl("iLogo");
            pIcon = (Panel)e.Item.FindControl("pIcon");
            pSolutions = (PlaceHolder)e.Item.FindControl("pSolutions");
            rSolutions = (Repeater)e.Item.FindControl("rSolutions");
            hResearch = (HyperLink)e.Item.FindControl("hResearch");
            pClear = (Panel)e.Item.FindControl("pClear");
            
            //bind the controls
            if (e.Item.DataItem is KeyValuePair<Item, List<Item>>)
            {
                KeyValuePair<Item, List<Item>> oData =( KeyValuePair<Item, List<Item>>)e.Item.DataItem;
               
                oStrategist =oData.Key;
                rSolutions.DataSource = oData.Value;
            }

            else
            {
                oItem=(Item) e.Item.DataItem;
                oStrategist = oItem;
                if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.StrategistNoAllocation))
                    rSolutions.DataSource = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution);
                else
                    rSolutions.DataSource = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy).SelectMany(oStrategy => oStrategy.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution));
            }
            rSolutions.ItemDataBound += new RepeaterItemEventHandler(rSolutions_ItemDataBound);
            rSolutions.DataBind();
           
            //bind the controls
            iLogo.Attributes.Add("src", string.Format("{0}?mw={1}&mh={2}", oStrategist.GetImageURL("Strategist", "Logo"), 120, 60));

            Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureHyperlink(hResearch);
            hResearch.NavigateUrl = string.Format("{0}?strategist={1}", hResearch.NavigateUrl, oStrategist.GetText("Strategist", "Code"));
            Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hResearch);
            
            //create a stringbuilder to hold class info
            sClass = new StringBuilder();

            //is this the first item?
            if (iIndex == 1)
            {
                sClass.Append(" first");
            }
            else if (iIndex % 4 == 0)
            {
                sClass.Append(" last");
                pClear.Visible = true;
            }
            iIndex++;
            sClass.Append(" blue");

            //append the styling
            if (sClass.Length > 0)
            {
                lListItem.Attributes.Add("class", sClass.ToString().Trim());
            }
        }
    }

    private void rSolutions_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        Item oItem;
        HyperLink hLink;
        Item oAllocationApproach;
        string sAllocationApproachParameter;

        //does this repeater contain an item?
        if ((oItem = (Item)e.Item.DataItem) != null)
        {
            //get the web controls
            hLink = (HyperLink)e.Item.FindControl("hLink");

            //bind the controls
            Genworth.SitecoreExt.Constants.Investments.Items.DocumentViewerItem.ConfigureHyperlink(hLink);

            // add the allocation approach if there is one available
            sAllocationApproachParameter = ((oAllocationApproach = GetAllocationApproach(oItem)) != null) ?
                String.Format("&{0}={1}", Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach, oAllocationApproach.GetText("Code")) : String.Empty;

            hLink.NavigateUrl = string.Format("{0}?Document={1}{2}", hLink.NavigateUrl, oItem.ID, sAllocationApproachParameter);
            hLink.Text = GetLinkTextFromFromSolution(oItem);
            
            //Set Omniture tag
            oItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
        }
    }

    private Item GetAllocationApproach(Item oSolutionItem)
    {
        Item oStrategy = oSolutionItem.Parent;

        // find the Strategy related to this solution in order to gets its related Allocation Approach
        return (oStrategy.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy)) ? oStrategy.GetListItem("Allocation Approach") : null;
    }

    /// <summary>
    /// Gets the text that will be displayed in the solution link.
    /// </summary>
    /// <param name="oSolutionItem">An item based on the Solution template</param>
    /// <returns>The text used for a Link</returns>
    private string GetLinkTextFromFromSolution(Item oSolutionItem)
    {
        string sLinkText;
        Item oAllocationApproach;

        sLinkText = oSolutionItem.GetText("Mandate");//Genworth.SitecoreExt.Constants.Investments.Templates.SolutionTypeFields.Mandate);
        if (string.IsNullOrEmpty(sLinkText))
        {
            sLinkText = oSolutionItem.DisplayName;
            // find the Strategy related to this solution in order to gets its related Allocation Approach
            if ((oAllocationApproach = GetAllocationApproach(oSolutionItem)) != null)
            {
                sLinkText = String.Format("{0} - {1}", sLinkText, oAllocationApproach.GetText("Title"));
            }
        }

        return sLinkText;
    }
	
	
</script>
<asp:PlaceHolder ID="pItems" runat="server">
    <ul class="info-card-list info-card-list-fill">
        <asp:Repeater ID="rItems" runat="server">
            <ItemTemplate>
                <asp:Repeater ID="rStrategies" runat="server">
                    <ItemTemplate>
                        <li id="lListItem" runat="server">
                            <div class="info-card">
                                <asp:Image ID="iLogo" runat="server" />
                                <asp:PlaceHolder ID="pSolutions" runat="server">
                                    <asp:Repeater ID="rSolutions" runat="server">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hLink" runat="server" />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <asp:HyperLink ID="hResearch" runat="server" CssClass="foot-link">View Strategist Research</asp:HyperLink>
                                </asp:PlaceHolder>
                            </div>
                        </li>
                        <asp:Panel ID="pClear" runat="server" CssClass="clear" Visible="false" />
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>
    </ul>
    <div class="clear">
    </div>
</asp:PlaceHolder>
