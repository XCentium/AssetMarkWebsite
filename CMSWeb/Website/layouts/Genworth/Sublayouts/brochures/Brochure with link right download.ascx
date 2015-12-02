<%@ Control Language="C#" Debug="true" ClassName="BrochureWithLink" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Genworth.SitecoreExt.Constants" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/investments/guided portfolios/Brochure Button Link.ascx" TagName="BrochureButtonCtrl" %>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		if (!Page.IsPostBack)
		{
			if (!isContextSet)
			{
				var items = ContextExtension.CurrentItem.GetChildrenAndItemsOfTemplate("Brochure with link");

				int indexItem = 0;
				if (int.TryParse(this.GetParameter("Control to select"), out indexItem))
				{
					indexItem--;
				}

				_contextItem = items.ElementAtOrDefault(indexItem);
			}
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
		if (_contextItem != null)
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
			var brochureItem = oCurrentItem.GetChildrenAndItemsOfTemplate("Brochure").FirstOrDefault();
			if (brochureItem != null)
			{
				brochureButton.SetContextItems(brochureItem);
				brochureButton.DataBind();
				
				
			}
		}
		else
		{
			container.Visible = false;
		}
	}
</script>

<div class="brochure_card brochure_with_link_right_download" id="container" runat="server">
	<h2 runat="server" id="title"></h2>
	<div class="container">
		<div class="bodyContent" id="description" runat="server"></div>
		<asp:HyperLink runat="server" ID="link">Learn More &gt;</asp:HyperLink>
	</div>

	<div class="brochure">
		<Gen:BrochureButtonCtrl ID="brochureButton" runat="server" />
	</div>
	<div class="clear"></div>
</div>
