using Sitecore.Pipelines.HttpRequest;
using System.Web;

namespace Genworth.SitecoreExt.Pipelines.HttpRequest
{
    class MimeTypeResolver : HttpRequestProcessor
    {


        #region METHODS
        /// <summary>
        /// Use this processor to resolve any conflict with a content type
        /// </summary>
        /// <param name="args"></param>
        public override void Process(HttpRequestArgs args)
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
            {
                return;
            }

            string requestUrl = context.Request.Url.ToString();

            if (!string.IsNullOrEmpty(requestUrl) && requestUrl.ToLower().EndsWith("robots.txt"))
            {
                context.Response.ContentType = "text/plain";
            }
        }

        #endregion
    }
}
