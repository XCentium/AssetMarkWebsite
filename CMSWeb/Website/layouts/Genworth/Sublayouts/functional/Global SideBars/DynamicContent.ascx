<%@ Control Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt.Helpers" %>

<script runat="server">

    List<Item> oItems;
    
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
        var sideBarItems = ContextExtension.CurrentItem.GetMultilistItems("Items").Where(sb => sb.InstanceOfTemplate("Sidebar Content")).ToArray();
        SideBarRepeater.DataSource = sideBarItems;
        SideBarRepeater.DataBind();
    }

    protected void SideBarRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {

        Item contentItem = e.Item.DataItem as Item;
        Panel panel = e.Item.FindControl("panel") as Panel;
        HtmlGenericControl panelStyles = e.Item.FindControl("panelStyles") as HtmlGenericControl;        
        
        oItems = contentItem.GetMultilistItems("Items");

        if (oItems != null && oItems.Count > 0)
        {
            string sClass = contentItem.GetText("class");
            if (!string.IsNullOrWhiteSpace(sClass))
            {
                panel.CssClass = sClass;
            }

            string styleBlock = contentItem.GetText("style");
            if (string.IsNullOrWhiteSpace(styleBlock))
            {
                panelStyles.Visible = false;
            }
            else
            {
                panelStyles.InnerText = styleBlock;
            }

            foreach (Item oItem in oItems)
            {
                if (oItem.InstanceOfTemplate("Sidebar Title"))
                {
                    LiteralControl titleControl = new LiteralControl();
                    Sitecore.Data.Fields.ReferenceField headingField = oItem.Fields["HeadingType"];
                    Item targetHeadingItem = headingField.TargetItem;

                    if (targetHeadingItem != null && !string.IsNullOrWhiteSpace(targetHeadingItem["Heading"]))
                    {
                        string heading = targetHeadingItem["Heading"];
                        titleControl.Text = "<" + heading + ">" + oItem.GetText("Title") + "</" + heading + ">";
                    }
                    else
                    {
                        titleControl.Text = oItem.GetText("Title");
                    }
                    panel.Controls.Add(titleControl);

                }
                else if (oItem.InstanceOfTemplate("Sidebar Text"))
                {
                    LiteralControl bodyControl = new LiteralControl();
                    bodyControl.Text = oItem.GetText("Description");
                    panel.Controls.Add(bodyControl);

                }
                else if (oItem.InstanceOfTemplate("Sidebar Link"))
                {
                    Image image = null;
                    string linkText = oItem.GetText("LinkText");
                    string imageUrl = oItem.GetImageURL("Preview Thumbnail");

                    Sitecore.Data.Fields.LinkField linkField = oItem.Fields["URL"];
                    string anchorUrl = linkField.Url;

                    if (!string.IsNullOrWhiteSpace(imageUrl))
                    {
                        image = new Image();
                        image.ImageUrl = string.Concat("~/", imageUrl);
                        image.AlternateText = linkText;
                    }

                    if (!string.IsNullOrWhiteSpace(anchorUrl))
                    {
                        HtmlAnchor anchor = new HtmlAnchor();
                        anchor.HRef = anchorUrl;

                        if (image != null)
                        {
                            anchor.Controls.Add(image);
                        }
                        else if (!string.IsNullOrEmpty(linkText))
                        {
                            anchor.InnerHtml = linkText;
                        }
                        else
                        {
                            anchor.InnerHtml = anchorUrl;
                        }

                        panel.Controls.Add(anchor);
                    }
                    else if (image != null)
                        panel.Controls.Add(image);
                }
            }
        }
    }
</script>

 <asp:Repeater runat="server" ID="SideBarRepeater" onitemdatabound="SideBarRepeater_ItemDataBound">
   <ItemTemplate>
     <asp:Panel runat="server" ID="panel">
     </asp:Panel>

     <style type="text/css" runat="server" ID="panelStyles">
     </style>
   </ItemTemplate>
 </asp:Repeater>
        
