using System;
using Sitecore.Analytics.Data.Items;
using Sitecore.Analytics.Testing;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Sitecore;

namespace Genworth.SitecoreExt.Pipelines.Response.CustomizeRendering.Analytics
{
    public class SelectVariation : Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering.SelectVariation
	{
		protected override void Evaluate(CustomizeRenderingArgs args)
		{
			Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
			Sitecore.Layouts.RenderingReference renderingReference = SelectVariation.GetRenderingReference(args.Rendering, Context.Language, args.PageContext.Database);
			if (string.IsNullOrEmpty(renderingReference.Settings.MultiVariateTest))
			{
				return;
			}
			using (new Sitecore.SecurityModel.SecurityDisabler())
			{
				Sitecore.Data.Items.Item item = args.PageContext.Database.GetItem(renderingReference.Settings.GetMultiVariateTestForLanguage(Context.Language));
				if (item == null)
				{
					return;
				}
				MultivariateTestValueItem variation = this.GetVariation(item);
				if (variation == null)
				{
					return;
				}
				ComponentTestContext context = new ComponentTestContext(variation, renderingReference, new List<Sitecore.Layouts.RenderingReference>
				{
					renderingReference
				});
				this.ApplyVariation(args, context);
			}
			args.IsCustomized = true;
		}

		public static Sitecore.Layouts.RenderingReference GetRenderingReference(Sitecore.Mvc.Presentation.Rendering rendering, Sitecore.Globalization.Language language, Sitecore.Data.Database database)
		{
			Sitecore.Diagnostics.Assert.IsNotNull(rendering, "rendering");
			Sitecore.Diagnostics.Assert.IsNotNull(language, "language");
			Sitecore.Diagnostics.Assert.IsNotNull(database, "database");
			string text = rendering.Properties["RenderingXml"];
			if (!string.IsNullOrEmpty(text))
			{
				return new Sitecore.Layouts.RenderingReference(XElement.Parse(text).ToXmlNode(), language, database);
			}
			return null;
		}
	}
}

