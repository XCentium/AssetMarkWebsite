using Sitecore;
using Sitecore.Analytics.Data.Items;
using Sitecore.Analytics.Shell.Applications.WebEdit;
using Sitecore.Analytics.Testing.TestingUtils;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering;
using Sitecore.Web;
using System;
using System.Linq;

namespace Genworth.SitecoreExt.Pipelines.Response.CustomizeRendering.ExperienceEditor
{

    public class SelectVariation : Genworth.SitecoreExt.Pipelines.Response.CustomizeRendering.Analytics.SelectVariation
	{
		public override void Process(CustomizeRenderingArgs args)
		{
			Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");
			if (args.IsCustomized || !Context.PageMode.IsPageEditor || !Sitecore.Configuration.Settings.Analytics.Enabled)
			{
				return;
			}
			this.Evaluate(args);
		}

		protected override MultivariateTestValueItem GetVariation(Sitecore.Data.Items.Item variableItem)
		{
			Sitecore.Diagnostics.Assert.ArgumentNotNull(variableItem, "variableItem");
			MultivariateTestVariableItem multivariateTestVariableItem = (MultivariateTestVariableItem)variableItem;
			if (multivariateTestVariableItem == null)
			{
				return null;
			}
			MultivariateTestDefinitionItem multivariateTestDefinitionItem = (MultivariateTestDefinitionItem)variableItem.Parent;
			if (multivariateTestDefinitionItem != null)
			{
                Genworth.SitecoreExt.Pipelines.Response.CustomizeRendering.ExperienceEditor.SelectVariation.UpdateTestSettings(multivariateTestDefinitionItem);
			}
			return TestingUtil.MultiVariateTesting.GetVariableValues(multivariateTestVariableItem).LastOrDefault<MultivariateTestValueItem>();
		}

		private static void UpdateTestSettings(MultivariateTestDefinitionItem testDefinition)
		{
			Sitecore.Diagnostics.Assert.ArgumentNotNull(testDefinition, "testDefinition");
			if (Sitecore.Web.WebEditUtil.Testing.CurrentSettings != null)
			{
				return;
			}
			bool flag = TestingUtil.MultiVariateTesting.IsTestRunning(testDefinition);
			Sitecore.Web.WebEditUtil.Testing.CurrentSettings = new Sitecore.Web.WebEditUtil.Testing.TestSettings(testDefinition, Sitecore.Web.WebEditUtil.Testing.TestType.Multivariate, flag);
			if (!flag)
			{
				return;
			}
			TestDefinitionItem testDefinitionItem = (TestDefinitionItem)testDefinition;
			Sitecore.Diagnostics.Assert.IsNotNull(testDefinitionItem, "testDefinitionItem");
			PageStatisticsContext.SaveTestStatisticsToSession(PageStatisticsContext.GetTestStatistics(testDefinitionItem, true, false));
		}
	}
}
