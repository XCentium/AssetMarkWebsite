using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.Pages;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace ServerLogic.SitecoreExt.Shell.Controls.RichTextEditor
{
    public class InsertVideo: DialogForm
    {
          // Fields
        protected Sitecore.Web.UI.HtmlControls.Edit videoId;
        protected Sitecore.Web.UI.HtmlControls.Edit videoHeight;
        protected Sitecore.Web.UI.HtmlControls.Edit videoWidth;

        protected string Mode
        {
            get
            {
                string mode = StringUtil.GetString(base.ServerProperties["Mode"]);

                if (!string.IsNullOrEmpty(mode))
                {
                    return mode;
                }

                return "shell";
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                base.ServerProperties["Mode"] = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);

            if (!Context.ClientPage.IsEvent)
            {
                Inialize();
            }
        }

        private void Inialize()
        {
            SetMode();
        }

        private void SetMode()
        {
            Mode = WebUtil.GetQueryString("mo");
        }

        private static string EscapeJavascriptString(string stringToEscape)
        {
            return StringUtil.EscapeJavascriptString(stringToEscape);
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            string id = videoId.Value;
            string height = videoHeight.Value;
            string width = videoWidth.Value;

            if (string.IsNullOrEmpty(id))
            {
                SheerResponse.ShowError("Missing Value", "The id is required please type it");
            }

            if (string.IsNullOrEmpty(height))
            {
                height = "240";
            }

            if (string.IsNullOrEmpty(width))
            {
                width = "320";
            }
            
            string javascriptArguments = string.Format("{0}, {1}, {2}", EscapeJavascriptString(id), EscapeJavascriptString(width), EscapeJavascriptString(height));

            if (IsWebEditMode())
            {
                SheerResponse.SetDialogValue(StringUtil.EscapeJavascriptString(javascriptArguments));
                base.OnOK(sender, args);
            }
            else
            {
                string closeJavascript = string.Format("scClose({0})", javascriptArguments);
                SheerResponse.Eval(closeJavascript);
            }
        }

        protected override void OnCancel(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            if (IsWebEditMode())
            {
                base.OnCancel(sender, args);
            }
            else
            {
                SheerResponse.Eval("scCancel()");
            }
        }

        private bool IsWebEditMode()
        {
            return string.Equals(Mode, "webedit", StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
