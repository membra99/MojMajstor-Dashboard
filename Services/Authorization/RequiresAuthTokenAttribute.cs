using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public class RequiresAuthTokenAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;
        // Check if the auth token is present in the session
        if (context.HttpContext.Session.GetString("AuthToken") == null)
		{
			// Redirect to the login page or perform other actions as needed
			context.Result = new RedirectToRouteResult(new RouteValueDictionary
			{
				{ "controller", "Authentication" }, // Change to your login controller
                { "action", "Index" } // Change to your login action
            });
		}

		base.OnActionExecuting(context);
	}
}
