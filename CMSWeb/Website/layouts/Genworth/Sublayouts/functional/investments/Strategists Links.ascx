<%@ Control Language="C#" Debug="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %> 

<script runat="server">
	
	bool bShowLogo;
	string sTemplateToUseForLinks;
	
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
		string sShowLogoParameter;

		// check if we should display the logos or not
		sShowLogoParameter = this.GetParameter("Show Logo");
		if (!string.IsNullOrWhiteSpace(sShowLogoParameter) && sShowLogoParameter.Equals("1"))
			bShowLogo = true;

		// depending on the investment page, we might want to display the
		// solution types (Alternative Investments) or the strategies (GPS Strategies page) in the links of the strategist
		sTemplateToUseForLinks = this.GetParameter("Template to Use");

        // by default we will use solutions, so in case is not a strategy use solution even if is that is not used in the parameter
        if (sTemplateToUseForLinks.ToLower().Equals(
            Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy.ToLower()))
        {
            sTemplateToUseForLinks = Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy;
        }
        else if (sTemplateToUseForLinks.ToLower().Equals(
            Genworth.SitecoreExt.Constants.Investments.Templates.Names.SimpleLinks.ToLower()))
        {
            sTemplateToUseForLinks = Genworth.SitecoreExt.Constants.Investments.Templates.Names.SimpleLinks;
        }
        else
        {
            sTemplateToUseForLinks = Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution;
        }
		
		//find and bind the strategies associated with this allocation approach
		rStrategists.DataSource = ContextExtension.CurrentItem.GetMultilistItems("Page", "Items").GetItemsOfTemplate("Strategist");
		rStrategists.ItemDataBound += new RepeaterItemEventHandler(rStrategists_ItemDataBound);
		rStrategists.DataBind();
	}

	private void rStrategists_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		Item oStrategist;
		Image iLogo;
		Repeater rSolutionsOrStrategies;
		HyperLink hResearch;
		StringBuilder sClass;

		//does this repeater contain an item?
		if ((oItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			iLogo = (Image)e.Item.FindControl("iLogo");
			rSolutionsOrStrategies = (Repeater)e.Item.FindControl("rSolutionsOrStrategies");
			hResearch = (HyperLink)e.Item.FindControl("hResearch");

			// show the logo if necesary
			iLogo.Visible = bShowLogo;
			
			//bind the controls
			if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution))
			{
				oStrategist = oItem.Parent.Parent;
				rSolutionsOrStrategies.DataSource = new Item[] { oItem };
			}
			else
			{
				oStrategist = oItem;

				if (sTemplateToUseForLinks.Equals(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy))
				{
					// the links will be to Strategies
					rSolutionsOrStrategies.DataSource = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy);
                    rSolutionsOrStrategies.ItemDataBound += new RepeaterItemEventHandler(rLinks_ItemDataBound);
                }
                else if (sTemplateToUseForLinks.Equals(Genworth.SitecoreExt.Constants.Investments.Templates.Names.SimpleLinks))
				{
					// Get simple links folder first
                    var simpleLinksFolder = oItem.GetChildByName(Genworth.SitecoreExt.Constants.Investments.Templates.Names.SimpleLinks);

                    if (simpleLinksFolder != null)
                    {
                        rSolutionsOrStrategies.DataSource = simpleLinksFolder.GetChildrenOfTemplate("Link");
                        rSolutionsOrStrategies.ItemDataBound += new RepeaterItemEventHandler(rSimpleLinks_ItemDataBound);
                    }
                }
				else
				{
					// the links will be to solutions
					if (oItem.InstanceOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.StrategistNoAllocation))
						rSolutionsOrStrategies.DataSource = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution);			
					else
					rSolutionsOrStrategies.DataSource = oItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Strategy).SelectMany(oStrategy => oStrategy.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Investments.Templates.Names.Solution));
					rSolutionsOrStrategies.ItemDataBound += new RepeaterItemEventHandler(rSolutions_ItemDataBound);
				}
			}

			rSolutionsOrStrategies.DataBind();

			//bind the controls
			iLogo.Attributes.Add("src", string.Format("{0}?mw={1}&mh={2}", oStrategist.GetImageURL("Strategist", "Logo"), 120, 60));

			Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureHyperlink(hResearch);
			hResearch.NavigateUrl = string.Format("{0}?strategist={1}", hResearch.NavigateUrl, oStrategist.GetText("Strategist", "Code"));
            Genworth.SitecoreExt.Constants.Investments.Items.ResearchItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hResearch);
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
			Genworth.SitecoreExt.Constants.Investments.Items.DocumentViewerItem.ConfigureHyperlink(hLink);
			hLink.NavigateUrl = string.Format("{0}?Document={1}", hLink.NavigateUrl, oItem.ID);
			hLink.Text = oItem.DisplayName;

            //Set omniture tag
            oItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, hLink);
		}
	}

    private void rSimpleLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oStrategyItem;
		HyperLink hLink;

		//does this repeater contain an item?
		if ((oStrategyItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");

            Item oLinkedItem = Sitecore.Context.Site.Database.GetItem(oStrategyItem.Fields["Item"].Value);

			//bind the controls
            if (oLinkedItem != null)
            {
                oLinkedItem.ConfigureDocumentHyperlink(hLink);
            }
            else
            {
                oStrategyItem.ConfigureDocumentHyperlink(hLink);
            }
                        
			hLink.Text = oStrategyItem.DisplayName;
            hLink.Target = oStrategyItem.GetText("Target");
		}
	}

	private void rLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oStrategyItem;
		HyperLink hLink;

		//does this repeater contain an item?
		if ((oStrategyItem = (Item)e.Item.DataItem) != null)
		{
			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");

			//bind the controls
			oStrategyItem.ConfigureHyperlink(hLink);
			hLink.Text = oStrategyItem.DisplayName;
		}
	}
	
</script>
<asp:Repeater ID="rStrategists" runat="server">
	<ItemTemplate>
		<ul class="info-card-list left">
			<li class="first last blue">
				<div class="info-card">
					<asp:Image ID="iLogo" runat="server" />
					<asp:Repeater ID="rSolutionsOrStrategies" runat="server">
						<ItemTemplate>
							<asp:HyperLink ID="hLink" runat="server" />
						</ItemTemplate>
					</asp:Repeater>
					<asp:HyperLink ID="hResearch" runat="server" CssClass="foot-link">View Strategist Research</asp:HyperLink>
				</div>
			</li>
		</ul>
		<br />
	</ItemTemplate>
</asp:Repeater>
