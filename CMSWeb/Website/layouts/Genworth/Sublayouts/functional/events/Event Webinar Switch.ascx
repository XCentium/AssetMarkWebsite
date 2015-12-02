<%@ Control Language="c#" %>
<!-- adding this comment for ticket 20857 -->
<script runat="server">
	protected override void OnLoad(EventArgs e)
	{
        // go get the proper bundle url from EWM and then inject into the head block on this page.
        string bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/Events", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Scripts);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddJsToPage(this.Page, bundleUrl);
        }

        bundleUrl = Genworth.SitecoreExt.Utilities.BundleHelper.GetBundle("Sitecore/Events", Genworth.SitecoreExt.Utilities.BundleHelper.BundleType.Styles);

        if (!String.IsNullOrEmpty(bundleUrl))
        {
            Genworth.SitecoreExt.Helpers.HtmlHeaderHelper.AddCssToPage(this.Page, bundleUrl);
        }
    }
</script>

<div class="events-switch-links">
    <span>Keyword Search</span>
</div>
