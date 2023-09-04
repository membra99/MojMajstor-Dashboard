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
            return View("User/NewUser");
        }

        public IActionResult NewDeclaration()
        {
            return View("Declaration/NewDeclaration");
        }

        public async Task<IActionResult> AllUsers()
        {
            try
            {
                var users = await _userDataServices.GetAllUsers();
                return View("User/Users", users);
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
                return View("SiteContent/SiteContents", siteContent);
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
                return View("Order/Order", orders);
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
                var declaration = await _mainDataServices.GetAllDeclarations();
                return View("Declaration/Declaration", declaration);
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
                return View("Declaration/NewDeclaration", new DeclarationIDTO());
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

        public async Task<IActionResult> SetPassword(string key)
        {
            try
            {
				ViewBag.UserKey = key;
				return View("../Authentication/SetPassword");
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
				return View("User/NewUser", new UsersIDTO());
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
				if (users == null)
				{
					ModelState.AddModelError("UserExist", $"User with that mail alredy exist");
					return View("User/NewUser");
				}
				return RedirectToAction("AllUsers", "Dashboard");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		#region Data/Products

		public async Task<IActionResult> AllData()
        {
            try
            {
                var users = await _mainDataServices.GetAllProducts();
                return View("Data/Data", users);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public async Task<IActionResult> NewData()
        {
            var categories = await _mainDataServices.GetCategories();
            var declarations = await _mainDataServices.GetAllDeclarations();
            return View("Data/NewData", new DataIDTO
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
                var categories = await _mainDataServices.GetCategories();
                var declarations = await _mainDataServices.GetAllDeclarations();
                return View("Data/NewData", new DataIDTO
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
                        //TODO izmeniti atribute ovde
                        //CategoriesId = attribute.Item1,
                        //Value = attribute.Item2,
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

        #endregion Data/Products

        #region Categories

        public async Task<IActionResult> AllCategories()
        {
            var categories = await _mainDataServices.GetCategories(); //Dont pull non active categories
            var devidedList = new List<List<ChildODTO2>>();

            var rootList = categories.Where(x => x.ParentCategoryId == null).ToList();
            devidedList.Add(rootList);
            while (categories.Any(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)))
            {
                rootList = categories.Where(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)).ToList();
                devidedList.Add(rootList);
            }
            devidedList.Reverse();
            return View("Category/Categories", new CategoryAttributeIDTO { AllCategories = devidedList });
        }

        public async Task<IActionResult> AddCategory(CategoryAttributeIDTO categoryAttributeIDTO)
        {
            await _mainDataServices.AddCategory(categoryAttributeIDTO.CategoryIDTO);
            return RedirectToAction("AllCategories");
        }

        public async Task<IActionResult> EditCategory(CategoryAttributeIDTO categoryAttributeIDTO)
        {
            await _mainDataServices.EditCategory(categoryAttributeIDTO.CategoryIDTO);
            return RedirectToAction("AllCategories");
        }

        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            await _mainDataServices.DeleteCategory(categoryId);
            return RedirectToAction("AllCategories");
        }

        public async Task<IActionResult> AllAttributes(int categoryid)
        {
            try
            {
                var categories = await _mainDataServices.GetAllCategoriesWithAttributes();
                return View("Data/Attributes", categories);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        #endregion Categories
    }
}