<%@ Control Language="c#" AutoEventWireup="true"
	ClassName="GenericControl" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/administration/Custodial Services Links.ascx"
	TagName="CustodialServicesLinks" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/administration/Custodial Services Tax Resources.ascx"
	TagName="CustodialServicesTaxResources" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/administration/Custodial Services Text.ascx"
	TagName="CustodialServicesText" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/administration/Custodial Services Tax Quarterly Updates.ascx"
	TagName="CustodialServicesTaxQuarterlyUpdates" %>
<script runat="server">
	
	int iLevel;
	
	enum ObjectType
	{
		LinkGroup,
		Documents,
		Page,
		TaxQuarterlyUpdate,
		TaxSeasonUpdate,
		None
	}
	
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			//BindData();
		}
	}

	public void BindingData(Item oItem, int iLevel)
	{
		this.iLevel = iLevel;
		//Creating the Hx tag, x depends on the iLevel
		HtmlGenericControl oH = new HtmlGenericControl();
		switch (iLevel)
		{
			case 1:
				oH.TagName = "h2";
				break;
			case 2:
				oH.TagName = "h4";
				break;
		}
		pH.Controls.Add(oH);
		oH.Controls.Add(lTitle);
		lTitle.Text = oItem.GetText("Title");
		lDescription.Text = oItem.GetText("Description");
		rItem.DataSource = SplitContent(oItem.GetChildrenOfTemplate(new string[] { "Link Group", "Link" }));
		rItem.ItemDataBound += new RepeaterItemEventHandler(rItem_ItemDataBound);
		rItem.DataBind();

	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="oItems">List of children from a item</param>
	/// <returns>A collection of KeyValuePair that the key specify the type for each element</returns>
	IEnumerable<KeyValuePair<ObjectType, object>> SplitContent(List<Item> oItems)
	{
		Queue<KeyValuePair<ObjectType, object>> oResult = new Queue<KeyValuePair<ObjectType, object>>();
		List<Item> oTempList = null;
		ObjectType eType = ObjectType.Documents, eOldType = ObjectType.None;
		foreach (Item oItem in oItems)
		{
			if (oItem.InstanceOfTemplate("Link Group"))
			{
				if (oTempList != null)
				{
					oResult.Enqueue(new KeyValuePair<ObjectType, object>(eType, oTempList));
					oTempList = null;
				}
				eType = (int)ObjectType.LinkGroup;
				oResult.Enqueue(new KeyValuePair<ObjectType, object>(eType, oItem));
			}
			else
			{
				Item oLinkItem = oItem.GetField("Item").GetItem();

                if (oLinkItem == null || oLinkItem.InstanceOfTemplate("Document Base") || oLinkItem.InstanceOfTemplate("Article") || oLinkItem.InstanceOfTemplate("Custodial Service and Form"))
				{
					eType = ObjectType.Documents;
					oLinkItem = oItem;
				}
				else if (oLinkItem.InstanceOfTemplate("Preview Link"))
				{
					eType = ObjectType.Page;
				}
				else if (oLinkItem.InstanceOfTemplate("Tax Season Update"))
				{
					eType = ObjectType.TaxSeasonUpdate;
				}
				// /sitecore/templates/Genworth/Data/Quarter 
				else if (oLinkItem.InstanceOfTemplate("Quarter"))
				{
					eType = ObjectType.TaxQuarterlyUpdate;
				}
				else
				{
					eType = ObjectType.Documents;
				}
				
				
				if (eType != eOldType)
				{
					if (oTempList != null)
					{
						oResult.Enqueue(new KeyValuePair<ObjectType, object>(eOldType, oTempList));
					}
					
					oTempList = new List<Item>();
				}

				oTempList.Add(oLinkItem);

			}
			
			eOldType = eType;
		}
		
		if (oTempList != null)
			oResult.Enqueue(new KeyValuePair<ObjectType, object>(eType, oTempList));
		return oResult;
	}
	
	void rItem_ItemDataBound(object sender, RepeaterItemEventArgs e)
	{
		KeyValuePair<ObjectType, object> oData;
		oData = (KeyValuePair<ObjectType, object>)e.Item.DataItem;
		PlaceHolder pSubContent;
		pSubContent = (PlaceHolder)e.Item.FindControl("pSubContent");
		switch (oData.Key)
		{
			case ObjectType.LinkGroup:

				GenericControl oCtrl = new GenericControl();
				pSubContent.Controls.Add(oCtrl);
				oCtrl.BindingData((Item)oData.Value, this.iLevel + 1);
				break;
			case ObjectType.Documents:
				CustodialServicesLink oCustodialServicesLinkCtrl = new CustodialServicesLink();
				pSubContent.Controls.Add(oCustodialServicesLinkCtrl);
				oCustodialServicesLinkCtrl.BindData((List<Item>)oData.Value);
				break;
			case ObjectType.Page:
				CustodialServicesText oCustodialServicesTextCtrl = new CustodialServicesText();
				pSubContent.Controls.Add(oCustodialServicesTextCtrl);
				oCustodialServicesTextCtrl.BindData((List<Item>)oData.Value);
				break;
			case ObjectType.TaxSeasonUpdate:
				CustodialServicesTaxReources oCustodialServicesTaxReourcesCtrl = new CustodialServicesTaxReources();
				pSubContent.Controls.Add(oCustodialServicesTaxReourcesCtrl);
				oCustodialServicesTaxReourcesCtrl.BindData((List<Item>)oData.Value);
				break;
			case ObjectType.TaxQuarterlyUpdate:
				CustodialServicesTaxQuarterlyUpdates oCustodialServicesTaxQuarterlyUpdatesCtrl = new CustodialServicesTaxQuarterlyUpdates();
				pSubContent.Controls.Add(oCustodialServicesTaxQuarterlyUpdatesCtrl);
				oCustodialServicesTaxQuarterlyUpdatesCtrl.BindData((List<Item>)oData.Value);
				break;
			default:
				break;
		}

	}
       
</script>
<asp:PlaceHolder ID="pH" runat="server"></asp:PlaceHolder>
<asp:Literal runat="server" ID="lTitle"></asp:Literal>
<asp:Literal runat="server" ID="lDescription"></asp:Literal>
<asp:Repeater ID="rItem" runat="server">
	<ItemTemplate>
		<asp:PlaceHolder runat="server" ID="pSubContent"></asp:PlaceHolder>
	</ItemTemplate>
</asp:Repeater>
