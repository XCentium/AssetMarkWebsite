<%@ Page Language="C#" %>

<%@ Import Namespace="Sitecore.Data.Items" %>
<%@ Import Namespace="ServerLogic.SitecoreExt" %>
<%@ Import Namespace="Genworth.SitecoreExt" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="Sitecore.Data.Fields" %>
<%@ Import Namespace="Sitecore.Shell.Applications.ContentEditor" %>

<script runat="server">
    private const string handlerUrlSettingKey = "ExternalMediaHandlerUri";
    private const string originalMediaHandlerPrefix = "~/media/";
    private const string originalMediaHandlerPrefix2 = "%7E/media/";
    private const string completeMediaHandlerRegex = @"(http:(.)+)*((~/media/)|(%7E/media/))(\w)+(.ashx)[a-zA-Z0-9\?\&\=;\+!'\(\)\*\-\._~%]*";
    private const string idMediaHandlerRegex = @"\w{32}";
    private const string externalContentFolderPath = "/sitecore/content/External Content";
    private Regex regex = new Regex(completeMediaHandlerRegex);
    private Regex regex2 = new Regex(idMediaHandlerRegex);

    protected override void OnLoad(EventArgs e)
    {
        try
        {
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                string id = Request.QueryString["id"];

                Item oItem = null;
                if (!string.IsNullOrEmpty(id))
                {
                    oItem = Sitecore.Context.Database.GetItem(id);

                    //Validate the item exists and it is 
                    if (oItem != null)
                    {
                        lBody.Text = ReplaceImageUrls(GetExternalContent(oItem));
                    }
                    else
                    {
                        Sitecore.Diagnostics.Log.Error("Unable to render External Content item. The item id does not exist.", this);
                    }
                }
                else
                {
                    Sitecore.Diagnostics.Log.Error("Unable to render External Content item. The id was not provided.", this);
                }
            }
        }
        catch (Exception ex)
        {
            Sitecore.Diagnostics.Log.Error("Unable to render External Content item.", ex, this);
        }
    }

    protected string GetExternalContent(Item item)
    {
        string result = string.Empty;

        //Validate if the item is in the folder ExternalContent 
        if (item.Paths.FullPath.Contains(externalContentFolderPath))
        {
            if (item.InstanceOfTemplate("External Content"))
            {
                result = item.GetText("Body");
            }
            else if (item.InstanceOfTemplate("External Content Group"))
            {
                result = GetExternalGroupContent(item);
            }
            else
            {
                Sitecore.Diagnostics.Log.Error("Unable to render External Content item. The item is not of the template 'External Content' or 'External Content Group'. Id: " + item.ID, this);
            }
        }
        else
        {
            Sitecore.Diagnostics.Log.Error("Unable to render External Content item. The item is not at External Content folder. Id: " + item.ID, this);
        }

        return result;
    }

    protected string GetExternalGroupContent(Item item)
    {
        string result = string.Empty;

        var items = item.GetMultilistItems("Items");
        
        if (items != null && items.Count > 0)
        {
            StringBuilder body = new StringBuilder();
            string br = "<br/>";
            
            foreach (var child in items)
            {
                string tmpBody = GetExternalContent(child);
                if (!string.IsNullOrEmpty(tmpBody))
                {
                    body.Append(tmpBody);
                    body.Append(br);
                }
            }

            int brIndex = 0;
            string tmbResult = result = body.ToString();

            if (body.Length > 0 && tmbResult.Contains(br) && ((brIndex = tmbResult.LastIndexOf(br)) > 0))
            {
                result = tmbResult.Remove(brIndex, 5);
            }
        }

        return result;
    }

    protected string ReplaceImageUrls(string text)
    {
        string newText = text;
        string handlerUrl = string.Empty;

        if (!string.IsNullOrWhiteSpace(text) && (text.Contains(originalMediaHandlerPrefix) || text.Contains(originalMediaHandlerPrefix2)))
        {
            if (!string.IsNullOrEmpty(handlerUrl = Sitecore.Configuration.Settings.GetSetting(handlerUrlSettingKey)))
            {
                MatchCollection matches = regex.Matches(text);

                if (matches != null && matches.Count > 0)
                {
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string original = matches[i].Value;
                        Match match = regex2.Match(original);

                        if (match.Success)
                        {
                            newText = newText.Replace(original, handlerUrl + match.Value);
                        }
                    }
                }
            }
            else
            {
                Sitecore.Diagnostics.Log.Error("The ExternalMediaHandlerUri config setting was not provided.", this);
            }
        }


        return newText;
    }
</script>
<div>
    <asp:literal id="lBody" runat="server" />

</div>

