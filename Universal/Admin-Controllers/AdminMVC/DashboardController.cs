using Microsoft.AspNetCore.Mvc;
using Services;
using Services.AWS;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
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
		#region Users
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
				return View("../User/NewUser");
			}
			try
			{
				//						FILE UPLOAD SYSTEM
				AWSFileUpload awsFile = new AWSFileUpload();
				awsFile.Attachments = new List<IFormFile>();
				if (userIDTO.Avatar != null)
					awsFile.Attachments.Add(userIDTO.Avatar);
				var media = await _userDataServices.UploadUserPicture(awsFile);
				if (media != null) userIDTO.MediaId = media.MediaId;
				var users = await _userDataServices.AddUser(userIDTO);
				if (users == null)
				{
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
		#endregion

		#region Data/Products
		public async Task<IActionResult> AllData()
		{
			try
			{
				var users = await _mainDataServices.GetAllProducts();
				return View("../Data/Data", users);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}

		}
		public async Task<IActionResult> NewData()
		{
			var categories = await _mainDataServices.GetCategory();
			var declarations = await _mainDataServices.GetAllDeclarations();
			return View("../Data/NewData", new DataIDTO
			{
				CategoriesODTOs = categories,
				DeclarationODTOs = declarations,
				SaleTypeODTOs = new List<DTO.ODTO.SaleTypeODTO> //TODO add methods for sale types
				{
					new DTO.ODTO.SaleTypeODTO{ SaleTypeId = 1, Value = "TEST" }
				}
			});
		}
		public async Task<IActionResult> AllAttributesByCategory(int categoryId)
		{
			var attributes = await _mainDataServices.GetAttribute(categoryId);
			return Json(new { data = attributes });
		}
		public async Task<IActionResult> AddData(DataIDTO dataIDTO)
		{
			if (!ModelState.IsValid)
			{
				var categories = await _mainDataServices.GetCategory();
				var declarations = await _mainDataServices.GetAllDeclarations();
				return View("../Data/NewData", new DataIDTO
				{
					CategoriesODTOs = categories,
					DeclarationODTOs = declarations,
					SaleTypeODTOs = new List<DTO.ODTO.SaleTypeODTO> //TODO add methods for sale types
				{
					new DTO.ODTO.SaleTypeODTO{ SaleTypeId = 1, Value = "TEST" }
				}
				});
			}
			try
			{
				var product = await _mainDataServices.AddProduct(dataIDTO.ProductIDTO);

				AWSFileUpload awsFile = new AWSFileUpload();
				awsFile.Attachments = new List<IFormFile>();
				if (dataIDTO.FeaturedImage != null)
					awsFile.Attachments.Add(dataIDTO.FeaturedImage);
				//await _mainDataServices.UploadProductImage(awsFile, "Featured Image", product.ProductId); //TODO vrv puca zbog aws

				foreach (IFormFile file in dataIDTO.GalleryImages)
				{
					awsFile.Attachments = new List<IFormFile>
					{
						file
					};
					await _mainDataServices.UploadProductImage(awsFile, "Gallery", product.ProductId);
				}

				var attributes = dataIDTO.ProductAttributeTypes.Zip(dataIDTO.ProductAttributeValues, (type, value) => new Tuple<int, string>(type, value)).ToList();
				foreach (var attribute in attributes)
				{
					ProductAttributesIDTO productAttributesIDTO = new ProductAttributesIDTO()
					{
						CategoriesId = attribute.Item1,
						Value = attribute.Item2,
						ProductId = product.ProductId,
						IsDev = false //TODO check how IsDev is set
					};
					_mainDataServices.AddProductAttributes(productAttributesIDTO);
				}

				return RedirectToAction("AllData", "Dashboard");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}
		#endregion
	}
}
