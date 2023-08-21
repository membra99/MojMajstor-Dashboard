using Microsoft.AspNetCore.Mvc;
using Services;
using Universal.DTO.IDTO;
using static Universal.DTO.CommonModels.CommonModels;

namespace Universal.Admin_Controllers.AdminMVC
{
	public class DashboardController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly MainDataServices _mainDataServices;
		private readonly UsersServices _userDataServices;

		public DashboardController(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor, MainDataServices mainDataServices, UsersServices usersServices)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri("https://localhost:7213"); ////TODO change later to be dynamic
			_httpContextAccessor = contextAccessor;
			_mainDataServices = mainDataServices;
			_userDataServices = usersServices;
		}

		public IActionResult Index()
		{
			return View("Home");
		}

		public IActionResult NewUser()
		{
			return View("../User/NewUser");
		}

		public async Task<IActionResult> AllUsers()
		{
			try
			{
				var users = await _userDataServices.GetAllUsers();
				return View("../User/Users", users);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}

		}

		public async Task<IActionResult> AddUser(UsersIDTO userIDTO)
		{
			if (!ModelState.IsValid)
			{
				return View("../User/NewUser", new UsersIDTO());
			}

			//						FILE UPLOAD SYSTEM
			//var filePath = Path.Combine("wwwroot\\images\\", userIDTO.Avatar.FileName);
			//using (FileStream fs = System.IO.File.Create(filePath))
			//{
			//	userIDTO.Avatar.CopyTo(fs);
			//}

			try
			{
				var users = await _userDataServices.AddUser(userIDTO);
				return RedirectToAction("AllUsers", "Dashboard");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}
	}
}
