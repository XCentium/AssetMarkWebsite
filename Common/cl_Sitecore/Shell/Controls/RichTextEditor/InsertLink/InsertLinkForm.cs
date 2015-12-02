using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.IO;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Shell.Framework;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.WebControls;
using System;
using System.Linq;

namespace ServerLogic.SitecoreExt.Shell.Controls.RichTextEditor.InsertLink
{

    /// <summary>
    /// Represents a InsertLinkForm.
    /// </summary>
    public class InsertLinkForm : DialogForm
    {
        /// <summary>
        /// The internal link data context.
        /// </summary>
        protected DataContext InternalLinkDataContext;
        /// <summary>
        /// The internal link tab.
        /// </summary>
        protected Tab InternalLinkTab;
        /// <summary>
        /// The internal link tree view.
        /// </summary>
        protected TreeviewEx InternalLinkTreeview;
        /// <summary>
        /// The media data context.
        /// </summary>
        protected DataContext MediaDataContext;
        /// <summary>
        /// The media tab.
        /// </summary>
        protected Tab MediaTab;
        /// <summary>
        /// The media tree view.
        /// </summary>
        protected TreeviewEx MediaTreeview;
        /// <summary>
        /// The tabs.
        /// </summary>
        protected Tabstrip Tabs;
        /// <summary>
        /// Gets or sets the upload button.
        /// </summary>
        /// <value>
        /// The upload button.
        /// </value>
        protected Button BtnUpload
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        protected string Mode
        {
            get
            {
                string @string = StringUtil.GetString(base.ServerProperties["Mode"]);
                if (!string.IsNullOrEmpty(@string))
                {
                    return @string;
                }
                return "shell";
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                base.ServerProperties["Mode"] = value;
            }
        }
        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            if (message.Name == "item:load")
            {
                this.LoadItem(message);
                return;
            }
            Dispatcher.Dispatch(message, this.GetCurrentItem(message));
            base.HandleMessage(message);
        }
        /// <summary>
        /// Handles a click on the Cancel button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        /// <remarks>When the user clicksCancel, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.</remarks>
        protected override void OnCancel(object sender, System.EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            if (this.Mode == "webedit")
            {
                base.OnCancel(sender, args);
                return;
            }
            SheerResponse.Eval("scCancel()");
        }
        /// <summary>Raises the load event.</summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        /// <remarks>This method notifies the server control that it should perform actions common to each HTTP
        /// request for the page it is associated with, such as setting up a database query. At this
        /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
        /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
        /// property to determine whether the page is being loaded in response to a client postback,
        /// or if it is being loaded and accessed for the first time.</remarks>
        protected override void OnLoad(System.EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            this.Tabs.OnChange += new System.EventHandler(this.OnActiveTabChanged);
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
            {
                return;
            }
            this.Mode = WebUtil.GetQueryString("mo");
            this.InternalLinkDataContext.GetFromQueryString();
            this.MediaDataContext.GetFromQueryString();
            string text = WebUtil.GetQueryString("fo");
            if (text.Length <= 0)
            {
                return;
            }
            if (!string.IsNullOrEmpty(WebUtil.GetQueryString("databasename")))
            {
                this.InternalLinkDataContext.Parameters = "databasename=" + WebUtil.GetQueryString("databasename");
                this.MediaDataContext.Parameters = "databasename=" + WebUtil.GetQueryString("databasename");
            }
            if (text.EndsWith(".aspx", System.StringComparison.OrdinalIgnoreCase))
            {
                if (!text.StartsWith("/sitecore", System.StringComparison.InvariantCulture))
                {
                    text = FileUtil.MakePath("/sitecore/content", text, '/');
                }
                if (text.EndsWith(".aspx", System.StringComparison.InvariantCulture))
                {
                    text = StringUtil.Left(text, text.Length - 5);
                }
                this.InternalLinkDataContext.Folder = text;
            }
            else
            {
                if (ShortID.IsShortID(text))
                {
                    text = ShortID.Parse(text).ToID().ToString();
                    Item item = Client.ContentDatabase.GetItem(text);
                    if (item != null)
                    {
                        if (!item.Paths.IsMediaItem)
                        {
                            this.InternalLinkDataContext.Folder = text;
                        }
                        else
                        {
                            this.MediaDataContext.Folder = text;
                            this.MediaTab.Active = true;
                        }
                    }
                }
                else
                {
                    Item item2 = this.InternalLinkDataContext.GetDatabase().GetItem(text);
                    if (item2 != null && item2.Paths.IsMediaItem)
                    {
                        this.MediaTab.Active = true;
                    }
                }
            }
            this.SetUploadButtonAvailability();
        }
        private bool isContentItem(Item item)
        {
            bool response = false;
            if(item.Paths.IsContentItem)
            {
                response = true;
            }
            else
            {
                var root = ContextExtension.CurrentDatabase.GetItem("/sitecore/content/Meta-Data/Site Reference");
                if (root != null)
                {
                    string path = item.Paths.FullPath;
                    var sites = root.GetMultilistItems("Items");
                    foreach (var site in sites)
                    {
                        string start = site.Paths.FullPath;
                        if (response = path.StartsWith(start))
                        {
                            break;
                        }
                    }
                }
            }
            return response;
        }
        /// <summary>
        /// Handles a click on the OK button.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        /// <remarks>
        /// When the user clicks OK, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.
        /// </remarks>
        protected override void OnOK(object sender, System.EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            string displayName;
            string text;
            if (this.Tabs.Active == 0 || this.Tabs.Active == 2)
            {
                Item selectionItem = this.InternalLinkTreeview.GetSelectionItem();
                if (selectionItem == null)
                {
                    SheerResponse.Alert("Select an item.", new string[0]);
                    return;
                }
                displayName = selectionItem.DisplayName;
                if (selectionItem.Paths.IsMediaItem)
                {
                    text = InsertLinkForm.GetMediaUrl(selectionItem);
                }
                else
                {
                    if (!isContentItem(selectionItem))
                    {
                        SheerResponse.Alert("Select either a content item or a media item.", new string[0]);
                        return;
                    }
                    LinkUrlOptions options = new LinkUrlOptions();
                    text = LinkManager.GetDynamicUrl(selectionItem, options);
                }
            }
            else
            {
                MediaItem mediaItem = this.MediaTreeview.GetSelectionItem();
                if (mediaItem == null)
                {
                    SheerResponse.Alert("Select a media item.", new string[0]);
                    return;
                }
                displayName = mediaItem.DisplayName;
                text = InsertLinkForm.GetMediaUrl(mediaItem);
            }
            if (this.Mode == "webedit")
            {
                SheerResponse.SetDialogValue(StringUtil.EscapeJavascriptString(text));
                base.OnOK(sender, args);
                return;
            }
            SheerResponse.Eval(string.Concat(new string[]
			{
				"scClose(",
				StringUtil.EscapeJavascriptString(text),
				",",
				StringUtil.EscapeJavascriptString(displayName),
				")"
			}));
        }
        /// <summary>Gets the media URL.</summary>
        /// <param name="item">The item.</param>
        /// <returns>The media URL.</returns>
        /// <contract>
        ///   <requires name="item" condition="not null" />
        ///   <ensures condition="not null" />
        /// </contract>
        private static string GetMediaUrl(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return MediaManager.GetMediaUrl(item, MediaUrlOptions.GetShellOptions());
        }
        /// <summary>Gets the current item.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The current item.</returns>
        private Item GetCurrentItem(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            string text = message["id"];
            Item selectionItem;
            if (this.Tabs.Active == 0)
            {
                selectionItem = this.InternalLinkTreeview.GetSelectionItem();
            }
            else
            {
                selectionItem = this.MediaTreeview.GetSelectionItem();
            }
            if (selectionItem == null)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(text) && ID.IsID(text))
            {
                return selectionItem.Database.GetItem(ID.Parse(text), selectionItem.Language, Sitecore.Data.Version.Latest);
            }
            return selectionItem;
        }
        /// <summary>Loads the item.</summary>
        /// <param name="message">The message.</param>
        private void LoadItem(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            Item selectionItem = this.MediaTreeview.GetSelectionItem();
            Language language = Context.Language;
            if (selectionItem != null)
            {
                language = selectionItem.Language;
            }
            Item item = Client.ContentDatabase.GetItem(ID.Parse(message["id"]), language);
            if (item == null)
            {
                return;
            }
            this.MediaDataContext.SetFolder(item.Uri);
            this.MediaTreeview.SetSelectedItem(item);
        }
        /// <summary>
        /// Called when media tree view clicked.
        /// </summary>
        [UsedImplicitly]
        private void OnMediaTreeviewClicked()
        {
            this.SetUploadButtonAvailability();
        }
        /// <summary>
        /// Called when active tab changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="T:System.EventArgs" /> instance containing the event data.</param>
        private void OnActiveTabChanged(object sender, System.EventArgs args)
        {
            this.SetUploadButtonAvailability();
            this.AdjustTabContentWindow();
        }
        /// <summary>
        /// Adjusts tab content window for Firefox
        /// </summary>
        private void AdjustTabContentWindow()
        {
            if (this.Tabs.Active == 0 && UIUtil.IsFirefox())
            {
                string key = "adjusted";
                string a = "false";
                object obj = base.ServerProperties[key];
                if (obj != null)
                {
                    a = obj.ToString();
                }
                if (a == "false")
                {
                    base.ServerProperties[key] = "true";
                    SheerResponse.Eval("scForm.browser.adjustFillParentElements()");
                }
            }
        }
        /// <summary>
        /// Sets the upload button availability.
        /// </summary>
        private void SetUploadButtonAvailability()
        {
            if (this.Tabs.Active == 1)
            {
                Item selectionItem = this.MediaTreeview.GetSelectionItem();
                if (selectionItem != null && selectionItem.Access.CanCreate())
                {
                    this.BtnUpload.Disabled = false;
                    return;
                }
                this.BtnUpload.Disabled = true;
            }
        }
    }

}
