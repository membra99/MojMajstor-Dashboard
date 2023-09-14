using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace Universal.Util.HtmlHelperExtensions
{
	public static class DateExtensions
	{
		public static IHtmlContent DateRangeFor<TModel, TValue>(
			this IHtmlHelper<TModel> htmlHelper, 
			Expression<Func<TModel, TValue>> expression,
			object htmlAttributes = null)
		{
			TagBuilder inputBuilder = new TagBuilder("input");

			if (htmlAttributes != null)
			{
				inputBuilder.MergeAttributes(JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(htmlAttributes)));
			}
			return inputBuilder;
		}
	}
}
