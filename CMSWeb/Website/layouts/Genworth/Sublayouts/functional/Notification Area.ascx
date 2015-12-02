<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<script runat="server">
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
		List<Sitecore.Data.ID> oParentIds;

		if (ContextExtension.CurrentItem.ID == ItemExtension.RootItem.ID)
		{
			// the root item is Home
			//
			oRootItem = ItemExtension.RootItem;
		}
		else
		{
			// get all the parent items
			//
			oParentIds = ContextExtension.CurrentItem.GetParentItems().Select(oItem => oItem.ID).ToList();
			oRootItem = ItemExtension.RootItem.GetChildren().Where(oItem => oParentIds.Contains(oItem.ID)).FirstOrDefault();
		}

		if (oRootItem != null)
		{
			rItems.DataSource = oRootItem.GetChildrenOfTemplate(new string[] { "Notification" });
			rItems.ItemDataBound += new RepeaterItemEventHandler(rItems_ItemDataBound);
			rItems.DataBind();
		}

		pNotifications.Visible = rItems.Items.Count > 0;
	}

	private void rItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		Item oItem;
		HyperLink hLink;
		Item oLinkItem;
		string sURL, sSummary,sTarget=string.Empty;
		Literal lSummary;
		Sitecore.Data.Fields.Field oItemField;
		oItem = (Item)e.Item.DataItem;

		//does this repeater contain an item?
		if (oItem != null)
		{

			//get the web controls
			hLink = (HyperLink)e.Item.FindControl("hLink");
			lSummary = (Literal)e.Item.FindControl("lSummary");
			
			//get summary text
			sSummary = oItem.GetText("Summary");
			sTarget = oItem.GetText("Target");
				
			//oItem.ConfigureHyperlink(hLink);
			if ((oItemField = oItem.GetField("Item")) != null && (oLinkItem = oItemField.GetItem()) != null)
			{
				sURL = oLinkItem.GetURL();
				//set the link text
				
			}
			else
				if (string.IsNullOrWhiteSpace((sURL = oItem.GetText("URL"))))
				{
					hLink.Visible = false;
					lSummary.Text = sSummary;
					return;
				}
				
			hLink.NavigateUrl = sURL;
			hLink.Text = sSummary;
			hLink.Target = sTarget;
			//set the mouse over
			//hLink.Attributes.Add("onmouseover", "dropmenuOpen(this, '" + oItem.ID.Guid.ToString() + "', event);");
		}
	}
</script>
<asp:PlaceHolder ID="pNotifications" runat="server">
	<div id="genworth-message-wrapper" style="display: none;">
		<div id="genworth-message-container">
			<span id="genworth-message-nav" style="display: none;"><span id="genworth-message-nav-left">
				<img src="<%= Page.ResolveClientUrl("~/") %>CMSContent/Images/LeftArrow.png" />
			</span><span id="genworth-message-nav-right">
				<img src="<%= Page.ResolveClientUrl("~/") %>CMSContent/Images/RightArrow.png" />
			</span></span><span id="genworth-message" class="alert" style="display: none;"><span
				id="genworth-message-cap-left"></span><span id="genworth-message-html-container"><span
					id="genworth-message-html"></span></span><span id="genworth-message-cap-right">
				</span></span>
		</div>
		<ul id="genworth-message-data">
			<asp:Repeater ID="rItems" runat="server">
				<ItemTemplate>
					<li class="alert"><b>Alert:</b>
						<asp:HyperLink ID="hLink" runat="server" />
						<asp:Literal runat="server" ID="lSummary"></asp:Literal></li>
				</ItemTemplate>
			</asp:Repeater>
		</ul>
	</div>
</asp:PlaceHolder>
