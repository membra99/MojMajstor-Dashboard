using Microsoft.AspNetCore.Mvc;
using Services;
using Services.AWS;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using Universal.DTO.ViewDTO;
using Universal.DTO.ViewDTO;
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

		public IActionResult NewDeclaration()
		{
			return View("../Declaration/NewDeclaration");
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

		public async Task<IActionResult> AllSiteContent(string type)
		{
			try
			{
				var siteContent = await _mainDataServices.GetAllSiteContentByType(type);
				return View("../SiteContent/SiteContents", siteContent);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}

		}

        public async Task<IActionResult> AllOrders()
        {
			try
			{
				var orders = await _mainDataServices.GetAllOrder();
                foreach (var item in orders)
                {
                    string newDate = item.OrderDate.ToString("yyyy-MM-dd HH:mm:ss");
                    item.OrderDate = DateTime.Parse(newDate);

					string updatedDate = item.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss");
					item.UpdatedAt = DateTime.Parse(updatedDate);
				}
				return View("../Order/Order", orders);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}

		}

		public async Task<IActionResult> AllDeclaration()
        {
            try
            {
                var declaration = await _mainDataServices.GetAll();
                return View("../Declaration/Declaration", declaration);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }

        }

        public async Task<IActionResult> AddDeclarations(DeclarationIDTO declarationIDTO)
        {
			if (!ModelState.IsValid)
			{
				return View("../Declaration/NewDeclaration", new DeclarationIDTO());
			}
			try
			{
				var declaration = await _mainDataServices.AddDeclaration(declarationIDTO);
				return RedirectToAction("AllDeclaration", "Dashboard");
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
            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            if (userIDTO.Avatar != null)
                awsFile.Attachments.Add(userIDTO.Avatar);
            try
            {
                var media = await _userDataServices.UploadUserPicture(awsFile);
                if (media != null) userIDTO.MediaId = media.MediaId;
                var users = await _userDataServices.AddUser(userIDTO);
                if(users == null) {
                    ModelState.AddModelError("UserExist", $"User with that mail alredy exist");
                    return View("../User/NewUser");
                }
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
