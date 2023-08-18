using Microsoft.AspNetCore.Mvc;
using Universal.DTO.ViewDTO;

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
			return RedirectToAction("Index", "Dashboard");
		}

	}
}
