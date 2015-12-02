<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
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
		List<Item> oParentItems;
		ListItem oListItem;
		
		//get the parent items		
		oParentItems = ContextExtension.CurrentItem.GetParentItems();

		//reverse the parent items		
		oParentItems.Reverse();
		
		//skip 1 level deep
		if ((oRootItem = oParentItems.Skip(4).FirstOrDefault()) != null)
		{

			oRootItem.GetChildrenOfTemplate(new string[] { "Web Base", "Document Base" })
				.Where(oItem => (oItem.InstanceOfTemplate("Link") || oItem.GetText("Include in Navigation").Equals("1")) && !oItem.GetText("Include in Footer").Equals("1"))
				.ToList().ForEach(oItem =>
				{
					dSolutionType.Items.Add(oListItem = new ListItem(oItem.DisplayName, oItem.GetURL()));
					if (oItem.InSelectedPath())
					{
						oListItem.Selected = true;
					}
				});
		}
		else
		{
			//hide the items
			pItems.Visible = false;
		}
	}
</script>
<asp:PlaceHolder ID="pItems" runat="server">
	<span id="selSolutionType">
		<asp:DropDownList ID="dSolutionType" runat="server" />
	</span>
</asp:PlaceHolder>