<%@ Page Language="C#" %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="Sitecore.Shell.Applications.ContentEditor" %>

<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
		string sType;
		string sCIID;
		string sPath;
		Item oItem;
        Item oHomeItem;
        List<Item> oHeaderItems;
        
		//string sContent;

		sType = Request.QueryString["type"];
		sCIID = Request.QueryString["ciid"];
		sPath = Request.QueryString["path"];

		oItem = null;
		if (!string.IsNullOrEmpty(sCIID))
		{
			oItem = Sitecore.Context.Database.GetItem(sCIID);
		}
		if(oItem == null && !string.IsNullOrEmpty(sPath)){

            oHomeItem = Sitecore.Context.Database.GetItem(Sitecore.Context.Site.StartPath);

            if (oHomeItem != null)
            {
                oHeaderItems = oHomeItem.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Navigation.Templates.LinkBase.Name);

                if (oHeaderItems != null)
                {
                    oItem = oHeaderItems.Where(m =>
                    {
                        string url = m.GetText(
                            Genworth.SitecoreExt.Constants.Navigation.Templates.LinkBase.Sections.Link.Name, 
                            Genworth.SitecoreExt.Constants.Navigation.Templates.LinkBase.Sections.Link.Fields.URLFieldName, 
                            string.Empty);
                        
                        // Equal to the url
                        if (string.Equals(url, sPath))
                        {
                            return true;
                        }
                        
                        // Have Url Alias
                        var urlAlias = m.GetChildrenOfTemplate(Genworth.SitecoreExt.Constants.Navigation.Templates.UrlAlias.Name);
                        foreach (var alias in urlAlias)
                        {
                            string aliasStartsWithUrl = alias.GetText(
                                Genworth.SitecoreExt.Constants.Navigation.Templates.UrlAlias.Sections.Data.Name,
                                Genworth.SitecoreExt.Constants.Navigation.Templates.UrlAlias.Sections.Data.Fields.StartsWith,
                                string.Empty);

                            if (!string.IsNullOrWhiteSpace(aliasStartsWithUrl) && sPath.StartsWith(aliasStartsWithUrl))
                            {
                                return true;
                            }
                        }
                        

                        // Starts with the same url
                        var urlSplit = url.Split(new Char[] { '/' });
                        string sUrlJoin;
                        
                        if (urlSplit.Length > 2)
                        {
                            sUrlJoin = string.Join("/", urlSplit.Take(3));
                            return sPath.StartsWith(sUrlJoin);
                        }                        
                        
                        return false;

                    }).FirstOrDefault();
                    
                    if (oItem == null)
                    {
                        oItem = Sitecore.Context.Database.SelectSingleItem(sPath);
                    }
                }
            }
		}
        
		if(oItem == null){
			oItem = ItemExtension.RootItem;
		}
		Sitecore.Context.Item = oItem;
		
		GetHeaderFooterContent(sType, sCIID, sPath);
	}

	public void GetHeaderFooterContent(string sType, string sCIID, string sPath)
	{
		string sControlPath;
		string sControl;

		switch ((sType ?? string.Empty).ToLower().Trim())
		{
			case "header":
				sControl = "Top-Primary";
				break;
			case "footer":
				sControl = "Footer";
				break;
			default:
				sControl = string.Empty;
				break;
		}
		if (!string.IsNullOrEmpty(sControl))
		{
			sControlPath = string.Format("~/layouts/Genworth/sublayouts/navigation/{0}.ascx", sControl);
			pControl.Controls.Add(Page.LoadControl(sControlPath));
		}
	}
</script>
<asp:PlaceHolder ID="pControl" runat="server" />
