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
        StringBuilder sb = new StringBuilder();
        
        Item contentItem = null;        
        
        foreach (Item contextItem in ContextExtension.CurrentItem.GetMultilistItems("Items"))
        {
            if (contextItem.InstanceOfTemplate("Sidebar Content"))
            {
                contentItem = contextItem;
                oItems = contentItem.GetMultilistItems("Items");

                if (oItems != null && oItems.Count > 0)
                {
                    Sitecore.Data.Fields.ReferenceField classField = contentItem.Fields["class"];
                    Item targetClassItem = classField.TargetItem;

                    if (targetClassItem != null && !string.IsNullOrWhiteSpace(targetClassItem["styleName"]))
                    {
                        panel.CssClass = targetClassItem["styleName"];
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
        }
        
      
    }
 </script>

 <asp:Panel runat="server" ID="panel">
 </asp:Panel>
