<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">

	
	string sDocumentViewerItem;
	string sResearchURL;
	// this will hold the current allocation approach being processed
	string sAllocationApproachCode;
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
		
		List<Item> oAllocationApproaches;
		List<List<Item>> oColumns;
		
		//get the allocation approaches
		oAllocationApproaches = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Asset Allocation Approach");
		
		//should we be binding?
		if (pItems.Visible = oAllocationApproaches.Count > 0)
		{
			//Get DocumentViewerItem url
			HyperLink olink = new HyperLink();
			Genworth.SitecoreExt.Constants.Investments.Items.DocumentViewerItem.ConfigureHyperlink(olink);
     
			 sDocumentViewerItem = olink.NavigateUrl;
			 Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureHyperlink(olink);
			 sResearchURL = olink.NavigateUrl;
			//load the columns
			oColumns = new List<List<Item>>();
			oColumns.Add(oAllocationApproaches.Take(2).ToList());
			oColumns.Add(oAllocationApproaches.Skip(2).Take(2).ToList());

			rColumns.DataSource = oColumns;
			rColumns.ItemDataBound += new RepeaterItemEventHandler(rColumns_ItemDataBound);
			rColumns.DataBind();
			
		}
	}
	
	private void rColumns_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		HtmlGenericControl dDiv;
		Repeater rItems;

		//get the controls
		dDiv = (HtmlGenericControl)e.Item.FindControl("dDiv");
		rItems = (Repeater)e.Item.FindControl("rItems");

		//are we the first div?
		dDiv.Attributes.Add("class", string.Format("split-section {0}", e.Item.ItemIndex == 0 ? "split-left split-blue" : "split-right split-orange"));
		
		//get the item
		rItems.DataSource = e.Item.DataItem;
		rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
		rItems.DataBind();
		
		
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		Label lTitle;
		HtmlGenericControl h3Title;
		HyperLink hCompare;
		HyperLink hPerformance;
		HtmlGenericControl dDiv;
		Repeater rStrategies;
		Dictionary<ID,KeyValuePair<Item,List<Item>>> oStrategies;
        Item oHoverHelptextItem;
        
		//does this repeater contain an item?
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			lTitle = (Label)e.Item.FindControl("lTitle");
			h3Title = (HtmlGenericControl)e.Item.FindControl("h3Title");
			hCompare = (HyperLink)e.Item.FindControl("hCompare");
			hPerformance = (HyperLink)e.Item.FindControl("hPerformance");
			dDiv = (HtmlGenericControl)e.Item.FindControl("dDiv");
			rStrategies = (Repeater)e.Item.FindControl("rStrategies");

			//are we the first div?
			dDiv.Attributes.Add("class", string.Format("split-col {0}", e.Item.ItemIndex == 0 ? "first" : "last"));
			
			//get the allocation approach code (used for URL construction)
			sAllocationApproachCode = oItem.GetText("Asset Allocation Approach", "Code");
			
			//bind the controls
			lTitle.Text = oItem.DisplayName;
          
            oHoverHelptextItem = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Name).SingleOrDefault(oHelpItem =>
                                                                                                                                                                string.Equals(
                                                                                                                                                                                oHelpItem.GetText(
                                                                                                                                                                                                    Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name, 
                                                                                                                                                                                                    Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.FieldSelectorFieldName, 
                                                                                                                                                                                                    string.Empty
                                                                                                                                                                                                  ),
                                                                                                                                                                                lTitle.ID
                                                                                                                                                                              )
                                                                                                                                                   );
            if(oHoverHelptextItem != null)
            {
                lTitle.ToolTip = oHoverHelptextItem.GetText(
								                                Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Name,
                                                            Genworth.SitecoreExt.Constants.HelpCenter.Templates.ContextSensitiveHelp.Sections.ContextSensitiveHelp.Fields.TextFieldName,
                                                            string.Empty
                                                            );
            }
			
			Investments.Items.CompareItem.ConfigureHyperlink(hCompare);
			hCompare.NavigateUrl = string.Format("{0}?AllocationApproach={1}", hCompare.NavigateUrl, sAllocationApproachCode);
			Investments.Items.PerformanceItem.ConfigureHyperlink(hPerformance);
			hPerformance.NavigateUrl = string.Format("{0}?AllocationApproach={1}", hPerformance.NavigateUrl, sAllocationApproachCode);

            //Set Omniture tag
            Investments.Items.PerformanceItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hPerformance);
            Investments.Items.CompareItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hCompare);
            
            oStrategies = new Dictionary<Sitecore.Data.ID, KeyValuePair<Item, List<Item>>>();
			string sid = oItem.ID.ToString();
            foreach (var oAllocationApproach in Investments.Items.StrategitsFolderItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategist).SelectMany(oChild => oChild.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy).Where(oGranChild => oGranChild["Allocation Approach"] == sid)))
            {
                List<Item> oSolutionsType;
                if (oStrategies.ContainsKey(oAllocationApproach.ParentID))
                {
                    oSolutionsType = oStrategies[oAllocationApproach.ParentID].Value;
                }
                else
                {
                    oSolutionsType = new List<Item>();
                    oStrategies.Add(oAllocationApproach.ParentID,new KeyValuePair<Item,List<Item>>(oAllocationApproach.Parent,oSolutionsType));
                }

                oSolutionsType.AddRange(oAllocationApproach.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution));
            }
			
		

			if ((iLast=oStrategies.Count())>0)
			{
				iLast--;	
			//find and bind the strategies associated with this allocation approach
                rStrategies.DataSource = oStrategies.Values;
			
			rStrategies.ItemDataBound += new RepeaterItemEventHandler(rStrategies_ItemDataBound);
			rStrategies.DataBind();
			}
		}
	}

	private void rStrategies_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
        KeyValuePair<Item, List<Item>> oItem;
		Item oStrategist;
		Image iLogo;
		PlaceHolder pSolutions;
		Repeater rSolutions;
		HyperLink hResearch;
		HtmlGenericControl lListItem;
		StringBuilder sClass;

		//does this repeater contain an item?
		if (e.Item.ItemType== ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        {
            oItem = (KeyValuePair<Item, List<Item>>)e.Item.DataItem;
		
			//get the web controls
			lListItem = (HtmlGenericControl)e.Item.FindControl("lListItem");
			iLogo = (Image)e.Item.FindControl("iLogo");
			pSolutions = (PlaceHolder)e.Item.FindControl("pSolutions");
			rSolutions = (Repeater)e.Item.FindControl("rSolutions");
			hResearch = (HyperLink)e.Item.FindControl("hResearch");

			//get strategist
            oStrategist = oItem.Key;
			
			//bind the controls
			iLogo.Attributes.Add("src", string.Format("{0}?mw={1}&mh={2}", oStrategist.GetImageURL("Strategist", "Logo"), 120, 60));
			
			//lTitle.Text = oStrategist.GetText("Name");
			rSolutions.DataSource = oItem.Value;
			rSolutions.ItemDataBound += new RepeaterItemEventHandler(rSolutions_ItemDataBound);
			rSolutions.DataBind();


			hResearch.NavigateUrl = string.Format("{0}?strategist={1}&allocationapproach={2}", 
										sResearchURL, 
										oStrategist.GetText("Strategist", "Code"),
										sAllocationApproachCode);
            Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hResearch);

			//create a stringbuilder to hold class info
			sClass = new StringBuilder();

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

	private void rSolutions_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		HyperLink hLink;

		//does this repeater contain an item?
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");

			//bind the controls
			//Genworth.SitecoreExt.Constants.Investments.Items.DocumentViewerItem.ConfigureHyperlink(hLink);
			hLink.NavigateUrl = string.Format("{0}?Document={1}&{2}={3}", sDocumentViewerItem, oItem.ID,
												Genworth.SitecoreExt.Constants.Investments.QueryParameters.AllocationApproach, sAllocationApproachCode);
			hLink.Text = oItem.DisplayName;
            //Set omniture tag
            oItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
		}
	}
</script>
<style type="text/css">
	div.allocation-approach
	{
		width:180px;
		float:left;
	}
	div.strategist-box
	{
		width:160px;
		min-height:80px;
		border:1px solid blue;
		margin:10px 0px 10px 0px;
	}
</style>
<script language="javascript" type="text/javascript">
	jQuery(function ($) {
		$('h3 span[title]').genworthtooltip();
	});
</script>
<asp:PlaceHolder ID="pItems" runat="server">
	<div class="split-wrapper">
		<asp:Repeater ID="rColumns" runat="server">
			<ItemTemplate>
				<div runat="server" id="dDiv">
					<asp:Repeater ID="rItems" runat="server">
						<ItemTemplate>
							<div id="dDiv" runat="server">
								<ul>
									<li class="split-col-head">
										<h3 id="h3Title" runat="server"><asp:Label ID="lTitle" runat="server"/></h3>
										<asp:HyperLink ID="hCompare" runat="server" Text="Compare" CssClass="compare" /><!--
										--><asp:HyperLink ID="hPerformance" runat="server" Text="Performance" CssClass="performance" />
									</li>
									<asp:Repeater ID="rStrategies" runat="server">
										<ItemTemplate>
											<li id="lListItem" runat="server">
												<div class="split-col-cell">
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
										</ItemTemplate>
									</asp:Repeater>
								</ul>
							</div>
						</ItemTemplate>
					</asp:Repeater>
				</div>
			</ItemTemplate>
		</asp:Repeater>
		<div class="clear"></div>
	</div>
</asp:PlaceHolder>
