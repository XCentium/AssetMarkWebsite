<%@ Control Language="c#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data" %>
<%@ Register TagPrefix="Gen" Src="~/layouts/Genworth/Sublayouts/functional/investments/guided portfolios/Brochure Button.ascx" TagName="BrochureButtonCtrl" %>

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
        Item oCurrentItem = ContextExtension.CurrentItem;
        
        description.InnerHtml = oCurrentItem.GetText("Page", "Body");
            
        string itemClassName = oCurrentItem.GetText("Header", "Class Name");
        string className = "blue";
        if (itemClassName != null)
        {
            className = itemClassName;
        }
        container.Attributes["class"] += " " + className;
            
        string logoUrl = oCurrentItem.GetImageURL("Header", "Logo");
        string title = oCurrentItem.GetText("Page", "Title");
        iPicture.Src = string.Concat("~/", logoUrl);
        iPicture.Alt = title;
            
        //Todo: delete this
            
        Item brochureButtonData = oCurrentItem.GetMultilistItems("Items")
                            .Where(item => item.InstanceOfTemplate("Brochure"))
                            .FirstOrDefault();
        brochureButton.SetContextItems(brochureButtonData);
        
	}
</script>
<div class="brochure_card gps_solution_brochure_card" id="container" runat="server">
    <h2>
        <img src="" id="iPicture" runat="server" width="112" height="34" alt="brochure" />
     </h2>
     <div class="container">
         <div class="description" id="description" runat="server">
            
         </div>
         <div class="brochure">
	        <Gen:BrochureButtonCtrl ID="brochureButton" runat="server" />
        </div>
    </div>
    <div class="clear"></div>
</div>