using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Validators;
using Sitecore.Data.Items;
using ServerLogic.SitecoreExt;
using System.Runtime.Serialization;
using Sitecore.Data.Fields;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Validators
{
	[Serializable]

	public class FeatureArticleValidator : StandardValidator
	{
		public FeatureArticleValidator()
		{

		}
		public FeatureArticleValidator(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		protected override ValidatorResult Evaluate()
		{
			Item oCurrentItem = base.GetItem();
			Database oDb = oCurrentItem.Database;
			if (oCurrentItem.ID.ToString() == "{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}" && oCurrentItem.GetField("Page", "Items").Value.Split(new char[]{'|'},StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(sIDItem => oDb.GetItem(sIDItem).InstanceOfTemplate("Article"))==null)
			{
				base.Text = base.GetText("A Feature Article is Missing. {0}","");
				return base.GetFailedResult(ValidatorResult.FatalError);

			}
			return ValidatorResult.Valid;

		}
		
		protected override ValidatorResult GetMaxValidatorResult()
		{
			return base.GetFailedResult(ValidatorResult.FatalError);

		}

		public override string Name
		{
			get { return "Feature Article"; }
		}
	}
}
