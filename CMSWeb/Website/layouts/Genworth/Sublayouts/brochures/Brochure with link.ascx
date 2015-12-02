<%@ Control Language="C#" Debug="true" ClassName="BrochureWithLink" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			BindData();
		}
	}

	private Item _contextItem;
	private bool isContextSet;

	public void SetContextItems(Item contextItems)
	{
		_contextItem = contextItems;
		isContextSet = true;
	}

	private void BindData()
	{
		if (isContextSet)
		{
			Item oCurrentItem = _contextItem;
			description.InnerHtml = oCurrentItem.GetText("Content", "Body");
			title.InnerText = oCurrentItem.GetText("Header", "Title");

			//Set color class
			string itemClassName = oCurrentItem.GetText("Header", "Class Name");
			string className = "blue";
			if (itemClassName != null)
			{
				className = itemClassName;
			}
			container.Attributes["class"] += " " + className;

			oCurrentItem.ConfigureHyperlink(link);
			//This will add Omniture Tags to the links found on Brochure.
			oCurrentItem.ConfigureOmnitureControl(ContextExtension.CurrentItem, link);
		}
	}
</script>

<div class="brochure_card" id="container" runat="server">
	<h2 runat="server" id="title"></h2>
	 <div class="container">
		 <div class="bodyContent" id="description" runat="server"></div>
	</div>
	<asp:HyperLink runat="server" ID="link">Learn More &gt;</asp:HyperLink>
	<div class="clear"></div>
</div>