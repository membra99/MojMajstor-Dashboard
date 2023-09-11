using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Universal.Util.HtmlHelperExtensions
{
	public static class ActionLinkExtensions
	{
		/// <summary>
		/// Returns an anchor (&lt;a&gt;) element that contains a Icon and URL path to the specified action.
		/// </summary>
		/// <param name="helper">The <see cref="IHtmlHelper"/> instance this method extends.</param>
		/// <param name="linkText">The inner text of the anchor element. Must not be <c>null</c>.</param>
		/// <param name="actionName">The name of the action.</param>
		/// <param name="controllerName">The name of the controller.</param>
		/// <param name="routeValues">
		/// An <see cref="object"/> that contains the parameters for a route. The parameters are retrieved through
		/// reflection by examining the properties of the <see cref="object"/>. This <see cref="object"/> is typically
		/// created using <see cref="object"/> initializer syntax. Alternatively, an
		/// <see cref="System.Collections.Generic.IDictionary{String, Object}"/> instance containing the route
		/// parameters.
		/// </param>
		/// <param name="htmlAttributes">
		/// An <see cref="object"/> that contains the HTML attributes for the element. Alternatively, an
		/// <see cref="System.Collections.Generic.IDictionary{String, Object}"/> instance containing the HTML
		/// attributes.
		/// </param>
		/// <returns>A new <see cref="IHtmlContent"/> containing the anchor element (&lt;a&gt;&lt;span&gt;&lt;i&gt;).</returns>
		public static IHtmlContent ActionLink(this IHtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, string icon)
		{
			TagBuilder iconBuilder = new TagBuilder("i");
			iconBuilder.AddCssClass("fa");
			iconBuilder.AddCssClass(icon);
			iconBuilder.AddCssClass("icon-spacing");

			TagBuilder spanBuilder = new TagBuilder("span");
			spanBuilder.InnerHtml.AppendHtml(iconBuilder);

			TagBuilder anchorBuilder = new TagBuilder("a");
			anchorBuilder.Attributes.Add("href", $"/{controllerName}/{actionName}");
			var attributes = JsonConvert.SerializeObject(htmlAttributes);
			anchorBuilder.MergeAttributes(JsonConvert.DeserializeObject<Dictionary<string, string>>(attributes));
			anchorBuilder.InnerHtml.AppendHtml(spanBuilder);
			anchorBuilder.InnerHtml.AppendHtml(linkText);

			return anchorBuilder;		
		}
	}
}
