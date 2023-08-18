using Microsoft.AspNetCore.Mvc;
using Universal.DTO.ViewDTO;

namespace Universal.Admin_Controllers.AdminMVC
{
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			return View("Home");
		}

		public IActionResult TEMPUSER()
		{
			return View("../User/NewUser", new UserIDTO());
		}

        public IActionResult TEMPallUSER()
        {
            return View("../User/Users");
        }

        public IActionResult TEMPnewUSER(UserIDTO userIDTO)
		{
			if(!ModelState.IsValid) return View("../User/NewUser", new UserIDTO());

			var filePath = Path.Combine("wwwroot\\images\\", userIDTO.Avatar.FileName);
			using (FileStream fs = System.IO.File.Create(filePath))
			{
				userIDTO.Avatar.CopyTo(fs);
			}

			return View("../User/Users");
		}
	}
}
