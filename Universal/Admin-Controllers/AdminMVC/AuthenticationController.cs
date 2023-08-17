using Microsoft.AspNetCore.Mvc;
using Universal.DTO.IDTO;

namespace Universal.Controllers
{
	public class AuthenticationController : Controller
	{
		public IActionResult Index()
		{
			return View("Login");
		}

		[HttpPost]
		public IActionResult Login(LoginIDTO loginIDTO)
		{
			if (!ModelState.IsValid) return View("../Authentication/Login");
			return Content("Validation Success");
		}

	}
}
