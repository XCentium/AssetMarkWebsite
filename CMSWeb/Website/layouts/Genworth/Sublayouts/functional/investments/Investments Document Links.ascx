<%@ Control Language="c#" AutoEventWireup="true" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/administration/Generic Control.ascx"  TagName="GenericControl" %>
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
        bool bIsLeft = false;
		List<Item> oLinkGroups; 
		
		string sParameter =this.GetParameter("Render in Left Column");
		if (!string.IsNullOrWhiteSpace(sParameter) && sParameter.Equals("1"))
			bIsLeft = true;
      
        if ((oLinkGroups = GetLinkGroups(bIsLeft)).Count > 0)
        {
			rFirstLevel.DataSource = oLinkGroups;
            rFirstLevel.ItemDataBound += new RepeaterItemEventHandler(rFirstLevel_ItemDataBound);
            rFirstLevel.DataBind();
        }
    }

	private List<Item> GetLinkGroups(bool bIsLeft)
	{
		List<Item> oLinkGroups;
		List<Item> oColumns;
		Item oColumn;
		int iColumnIndex;

		// if this is diplaying the links of the left column we should take
		// the gruop links under the first column, if not, take the links
		// from the second column
		iColumnIndex = (bIsLeft) ? 0 : 1;
		
		//get the all column items and see if there is the required one
		if ((oColumns = ContextExtension.CurrentItem.GetChildrenOfTemplate("Generic Column")).Count > iColumnIndex)
		{
			// get the link groups under the column
			oColumn = oColumns[iColumnIndex];
			oLinkGroups = oColumn.GetChildrenOfTemplate("Link Group");
		}
		else
		{
			oLinkGroups = new List<Item>();
		}

		return oLinkGroups;
	}

    void rFirstLevel_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        GenericControl oGeneric;
        oGeneric = (GenericControl)e.Item.FindControl("oGeneric");
        oGeneric.BindingData((Item)e.Item.DataItem, 2);
    }
	
</script>
<asp:Repeater runat="server" ID="rFirstLevel">
    <ItemTemplate>
		<Gen:GenericControl runat="server" ID="oGeneric"></Gen:GenericControl>
    </ItemTemplate>
</asp:Repeater>
