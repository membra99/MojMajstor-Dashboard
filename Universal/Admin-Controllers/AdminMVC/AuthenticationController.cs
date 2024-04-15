using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using Universal.DTO.ViewDTO;
using static Universal.DTO.CommonModels.CommonModels;
using System.Net.Http;
using Universal.Util;
using Services;
using Microsoft.AspNetCore.Components;

namespace Universal.Controllers
{
	public class AuthenticationController : Controller
	{

		private readonly HttpClient _httpClient;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly MainDataServices _mainDataServices;
		private readonly UsersServices _userDataServices;

		public AuthenticationController(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor, MainDataServices mainDataServices, UsersServices usersServices)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri("https://localhost:7213"); ////TODO change later to be dynamic
			_httpContextAccessor = contextAccessor;
			_mainDataServices = mainDataServices;
			_userDataServices = usersServices;
		}

		public IActionResult Index()
		{
			return View("Login");
		}


		[HttpPost]
		public async Task<IActionResult> Login(LoginIDTO loginIDTO)
		{
			if (!ModelState.IsValid)
			{
				return View("../Authentication/Login");
			}

			try
			{
				var model = await _userDataServices.Authenticate(new AuthenticateRequest { Username = loginIDTO.Email, Password = loginIDTO.Password });

				if (model != null)
				{
					_httpContextAccessor.HttpContext.Session.Set("UserId", model.Id); //save user to session and token for further use
					_httpContextAccessor.HttpContext.Session.Set("AuthToken", model.Token);
					return RedirectToAction("AllUsers", "Dashboard", new { layout = "_LayoutDashboard" });
				}
				else
				{
					// handle login failure
					ModelState.AddModelError("", "Invalid credentials. Please try again.");
					return View("../Authentication/Login");
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("../Authentication/Login");
			}
		}

		public IActionResult Logout()
		{
			try
			{
				_httpContextAccessor.HttpContext.Session.Remove("UserId");
				_httpContextAccessor.HttpContext.Session.Remove("AuthToken");
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return RedirectToAction("Index", "Dashboard");
			}
		}

		[HttpPost]
		public async Task<IActionResult> LoginWithApi(LoginIDTO loginIDTO) //used for example how to work with API's
		{
			if (!ModelState.IsValid)
			{
				return View("../Authentication/Login");
			}

			try
			{
				var model = new AuthenticateRequest { Username = loginIDTO.Email, Password = loginIDTO.Password };
				string requestJson = JsonSerializer.Serialize(model);
				HttpContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

				HttpResponseMessage response = await _httpClient.PostAsync("/api/Users/Authenticate", content);

				var responseJson = await response.Content.ReadAsStringAsync();

				var returnedObject = JsonSerializer.Deserialize<ResponseAuthModel>(responseJson);

				if (response.IsSuccessStatusCode)
				{
					_httpContextAccessor.HttpContext.Session.Set("UserId", returnedObject.id.ToString()); //save user to session and token for further use
					_httpContextAccessor.HttpContext.Session.Set("AuthToken", returnedObject.token);
					return RedirectToAction("Index", "Dashboard");
				}
				else
				{
					// handle login failure
					ModelState.AddModelError("", "Invalid credentials. Please try again.");
					return View("../Authentication/Login");
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("../Authentication/Login");
			}
		}

	}
}
