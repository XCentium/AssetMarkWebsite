<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<script runat="server">
	
	int iLast;
	private bool bWhitHeader;
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

	protected override void OnPreRender(EventArgs e)
	{
		base.OnPreRender(e);
		pTabHeader.Visible = pTabHeaderContent.Controls.Count > 0;
	}

	private void BindData()
	{
		Item oRootItem;
		List<Item> oParentItems;
		IEnumerable<Item> oItems;
		bWhitHeader = this.GetParameter("Gray Header") == "1";
		int iLevel;
		string sParameter = this.GetParameter("HideTab Bottom");
		if (!string.IsNullOrWhiteSpace(sParameter) && sParameter.Equals("1"))
		{
			imgBottom.Visible = false;
		}
		
		sParameter = this.GetParameter("Top Only");
		if (!string.IsNullOrWhiteSpace(sParameter) && sParameter.Equals("1"))
		{
			dTabThaeter.Attributes["class"] += " top-only";
		}
		int.TryParse(this.GetParameter("Level"), out iLevel);
		//get the parent items		
		oParentItems = ContextExtension.CurrentItem.GetParentItems();

		//reverse the parent items		
		oParentItems.Reverse();
		
		//skip 1 level deep
		if ((oRootItem = oParentItems.Skip(iLevel).FirstOrDefault()) != null)
		{
			oItems = oRootItem.GetChildrenOfTemplate(new string[] { "Web Base", "Document Base" }).Where(oItem => (oItem.InstanceOfTemplate("Link") || oItem.GetText("Include in Navigation").Equals("1")) && !oItem.GetText("Include in Footer").Equals("1"));
			if ((iLast = oItems.Count()) > 0)
			{
				iLast--;
				rItems.DataSource = oItems;
				rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
				rItems.DataBind();
			}
			else
				//hide the items if nothing to show
				pItems.Visible = false;
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
			else if (e.Item.ItemIndex == iLast)
			{
				sClass.Append(" last");
			}
			else
			{
				if (bWhitHeader)
					sClass.Append(" withHeader");
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
	<div class="gc c12">
		<div class="tabs-wrapper">
			<ul class="tabs">
				<asp:Repeater ID="rItems" runat="server">
					<ItemTemplate>
						<li id="lListItem" runat="server"><asp:HyperLink ID="hLink" runat="server" /></li>
					</ItemTemplate>
				</asp:Repeater>
			</ul>
			<div class="clear"></div>
			<div class="tab-theater" runat="server" id="dTabThaeter">
				<img src="/CMSContent/Images/tab-theater-top.png" class="tab-theater-top" alt="" />
				<img src="/CMSContent/Images/tab-theater-bottom.png" class="tab-theater-bottom" alt="" runat="server" id="imgBottom" />
				<asp:PlaceHolder ID="pTabHeader" runat="server">
					<div class="tab-header">
						<asp:Literal ID="lDebug" runat="server" />
						<sc:Placeholder Key="Level3-SelectedTab" runat="server" ID="pTabHeaderContent" />
					</div>
				</asp:PlaceHolder>
				<sc:Placeholder Key="Level3-Content" runat="server" />
			</div>
			<sc:Placeholder Key="Level3-Footer" runat="server" />
		</div>
		<sc:Placeholder Key="OuterContent" runat="server" />
	</div>
</asp:PlaceHolder>