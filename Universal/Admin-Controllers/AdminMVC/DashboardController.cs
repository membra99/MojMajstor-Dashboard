using Microsoft.AspNetCore.Mvc;

namespace Universal.Admin_Controllers.AdminMVC
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View("Home");
		}
	}
}
