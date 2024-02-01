using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Universal.Util.HtmlHelperExtensions
{
	public static class LabelExtensions
	{
		public static IHtmlContent LabelRequiredFor<TModel, TValue>(
			this IHtmlHelper<TModel> htmlHelper,
			Expression<Func<TModel, TValue>> expression,
			string labelText,
			bool required,
			object htmlAttributes = null)
		{
			var htmlField = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(ModelExpressionProvider)) as ModelExpressionProvider;
			string htmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlField.GetExpressionText(expression));
			TagBuilder labelBuilder = new TagBuilder("label");
			labelBuilder.Attributes["for"] = TagBuilder.CreateSanitizedId(htmlFieldName, " ");
			labelBuilder.InnerHtml.AppendHtml(labelText);

			if (required)
			{
				TagBuilder spanBuilder = new TagBuilder("span");
				spanBuilder.AddCssClass("text-danger");
				spanBuilder.AddCssClass("mx-1");
				spanBuilder.InnerHtml.AppendHtml("*");
				labelBuilder.InnerHtml.AppendHtml(spanBuilder);
			}

			if (htmlAttributes != null)
			{
				labelBuilder.MergeAttributes(JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(htmlAttributes)));
			}
			return labelBuilder;
		}
	}
}
