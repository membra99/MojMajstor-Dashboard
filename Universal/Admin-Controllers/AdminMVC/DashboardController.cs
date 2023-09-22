using AutoMapper;
using Entities.Universal.MainData;
using IronPdf;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.AWS;
using System.IO;
using System.Net.Mime;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using Universal.DTO.ViewDTO;

using Universal.DTO.ViewDTO;

using Universal.Util;
using static Universal.DTO.CommonModels.CommonModels;

namespace Universal.Admin_Controllers.AdminMVC
{
	public class DashboardController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IAWSS3FileService _AWSS3FileService;
		private readonly MainDataServices _mainDataServices;
		private readonly UsersServices _userDataServices;

		public DashboardController(IHttpClientFactory httpClientFactory, IAWSS3FileService AWSS3FileService, IHttpContextAccessor contextAccessor, MainDataServices mainDataServices, UsersServices usersServices)
		{
			_httpClient = httpClientFactory.CreateClient();
			_httpClient.BaseAddress = new Uri("https://localhost:7213"); ////TODO change later to be dynamic
			_httpContextAccessor = contextAccessor;
			_mainDataServices = mainDataServices;
			_userDataServices = usersServices;
			_AWSS3FileService = AWSS3FileService;
		}

		public IActionResult Index()
		{
			return View("Home");
		}

		#region Users

		public IActionResult NewUser()
		{
			CheckForToast();
			return View("User/NewUser");
		}

		public async Task<IActionResult> AllUsers()
		{
			try
			{
				CheckForToast();
				var users = await _userDataServices.GetAllUsers();
				return View("User/Users", users);
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
					_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "User with that mail already exists");
					_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
					return RedirectToAction("NewUser");
				}
				return RedirectToAction("AllUsers", "Dashboard");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> EditUser(int userId)
		{
			return View("User/EditUser", await _userDataServices.GetUserByIdForEdit(userId));
		}

		public async Task<IActionResult> GetImage(string path)
		{
			if (path == null)
				path = "DOT/noimg.jpg";

            var aa = await _AWSS3FileService.GetFile(path);
			byte[] bytes = null;
			using (MemoryStream ms = new MemoryStream())
			{
				aa.CopyTo(ms);
				bytes = ms.ToArray();
			}

			return File(bytes, MediaTypeNames.Text.Plain);

		}


		public async Task<IActionResult> EditUserAction(UsersIDTO userIDTO)
		{
			if (!ModelState.IsValid)
			{
				return View("User/EditUser", await _userDataServices.GetUserByIdForEdit((int)userIDTO.UsersId));
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
				var users = await _userDataServices.EditUser(userIDTO);
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

		public async Task<IActionResult> PreviewUser(int userId)
		{
			return View("User/PreviewUser", await _userDataServices.GetUserById(userId));
		}

		#endregion Users

		public IActionResult NewDeclaration()
		{
			return View("Declaration/NewDeclaration");
		}

		public IActionResult AddMedia()
		{
			return View("Media/AddMedia");
		}

        public async Task<IActionResult> uploadMedia(IFormFile file)
        {
            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
                awsFile.Attachments.Add(file);
            var media = await _userDataServices.UploadUserPicture(awsFile);
			return Ok();
        }

		public IActionResult AllGallery()
		{
			return View("Media/Gallery");
		}

		public async Task<IActionResult> EditOrders(int id)
		{
			var Order = await _mainDataServices.GetFullOrderById(id);

			return View("Order/ViewOrders", Order);
		}

		public async Task<IActionResult> PostSiteContent(int siteContentType)
		{
			SiteContentIDTO contentIDTO = new SiteContentIDTO();
			contentIDTO.SiteContentTypeId = siteContentType;
			contentIDTO.TagODTOs = await _mainDataServices.GetTags();
			return View("SiteContent/AddSiteContent", contentIDTO);
		}

		public async Task<IActionResult> AddSiteContent(SiteContentIDTO siteContentIDTO)
		{
			if (!ModelState.IsValid)
			{
				SiteContentIDTO contentIDTO = new SiteContentIDTO();
				contentIDTO.SiteContentTypeId = siteContentIDTO.SiteContentTypeId;
				contentIDTO.TagODTOs = await _mainDataServices.GetTags();
				return View("SiteContent/AddSiteContent", contentIDTO);
			}
			try
			{
				siteContentIDTO.IsActive = true;
				var siteContent = await _mainDataServices.AddSiteContent(siteContentIDTO);
				var site = (siteContentIDTO.SiteContentTypeId == 3) ? "Page" : "Blog";
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "" + site + " added successfully!");
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");

				return RedirectToAction("AllSiteContent", "Dashboard", new { type = site });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> EditStatus(int orderId, string status)
		{
			var order = await _mainDataServices.EditStatus(orderId, status);

			return View("Order/ViewOrders", order);
		}

		public async Task<IActionResult> AllSiteContent(string type, int langId)
		{
			try
			{
				CheckForToast();
				var siteContent = await _mainDataServices.GetAllSiteContentByType(type, langId);
				ViewBag.siteType = type;
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

		public async Task<IActionResult> AddTags(TagIDTO tagIDTO)
		{
            if (!ModelState.IsValid)
            {
                return View("Tag/NewTag", new TagIDTO());
            }

            //						FILE UPLOAD SYSTEM
            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            if (tagIDTO.TagImage != null)
                awsFile.Attachments.Add(tagIDTO.TagImage);
            try
            {
                var media = await _userDataServices.UploadUserPicture(awsFile);
                if (media != null) tagIDTO.MediaId = media.MediaId;
                var tag = await _mainDataServices.AddTag(tagIDTO);
                return RedirectToAction("AllTags", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public async Task<IActionResult> EditTag(int id)
        {
            try
            {
                var tag = await _mainDataServices.GetTagForEditById(id);
                return View("Tag/EditTag", tag);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

		public async Task<IActionResult> EditTagModel(TagIDTO tagIDTO)
		{
			if (!ModelState.IsValid)
			{
				return View("Tag/EditTag", tagIDTO);
			}
			AWSFileUpload awsFile = new AWSFileUpload();
			awsFile.Attachments = new List<IFormFile>();
			if (tagIDTO.TagImage != null)
				awsFile.Attachments.Add(tagIDTO.TagImage);
			try
			{
				var media = await _userDataServices.UploadUserPicture(awsFile);
				if (media != null) tagIDTO.MediaId = media.MediaId;
				await _mainDataServices.EditTag(tagIDTO);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}

			_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Tag edited successfully!");
			_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");

			return RedirectToAction("AllTags");
		}

		public async Task<IActionResult> AllTags()
        {
			CheckForToast();
            try
            {
                var tags = await _mainDataServices.GetTags();
                return View("Tag/AllTags", tags);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public async Task<IActionResult> NewTag()
		{
			return View("Tag/NewTag");
		}

		public async Task<IActionResult> AllInvoices()
		{
			try
			{
				var invoices = await _mainDataServices.GetAllInvoices();
				return View("Invoice/Invoice", invoices);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> PDFViewer(string path)
		{
			var stream = await _mainDataServices.GetStreamForInvoice(path);
			byte[] bytes = null;
			using (MemoryStream ms = new MemoryStream())
			{
				stream.CopyTo(ms);
				bytes = ms.ToArray();
			}

			return File(bytes, "application/pdf");
		}

		public async Task<IActionResult> AllDeclaration()
        {
            try
            {
                CheckForToast();
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

				_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Declaration added successfully!");
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");

				return RedirectToAction("AllDeclaration", "Dashboard");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> EditDeclaration(int id)
		{
			try
			{
				var declaration = await _mainDataServices.GetDeclarationForEditById(id);
				return View("Declaration/EditDeclaration", declaration);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> PreviewDeclaration(int id)
		{
			try
			{
				var declaration = await _mainDataServices.GetDeclarationForEditById(id);
				return View("Declaration/PreviewDeclaration", declaration);
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

		#region Data/Products

		public async Task<IActionResult> AllData(int langId)
		{
			try
			{
				CheckForToast();
				var products = await _mainDataServices.GetAllProducts(langId);
				return View("Data/Data", products);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> DeleteData(int dataId)
		{
			try
			{
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Successfully Deleted Product");
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
				CheckForToast();
				var data = await _mainDataServices.DeleteProduct(dataId);
				var products = await _mainDataServices.GetAllProducts(0);
				return View("Data/Data", products);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> NewData()
		{
			var categories = await _mainDataServices.GetAllCategoriesWithAttributes();
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
			var attributes = await _mainDataServices.GetAllAttributesByCategoryName(categoryId);
			Dictionary<int, List<AttributesODTO>> attributeValues = new Dictionary<int, List<AttributesODTO>>();
			foreach (var attribute in attributes)
			{
				var attrValues = await _mainDataServices.GetAllAttributesValueByAttributeName(attribute.CategoryId);
				attributeValues.Add(attribute.CategoryId, attrValues);
			}

			return Json(new { data = new { attrs = attributes, attrValues = attributeValues } });
		}

		public async Task<IActionResult> AddData(DataIDTO dataIDTO)
		{
			if (!ModelState.IsValid)
			{
				var categories = await _mainDataServices.GetAllCategoriesWithAttributes();
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

				await _mainDataServices.DeleteAllProductAttributes(dataIDTO.ProductIDTO.ProductId);

				foreach (var attributeID in dataIDTO.ProductAttributeValues)
				{
					ProductAttributesIDTO productAttributesIDTO = new ProductAttributesIDTO()
					{
						ProductId = product.ProductId,
						IsDev = false, //TODO check how IsDev is set
						AttributesId = attributeID
					};
					await _mainDataServices.AddProductAttributes(productAttributesIDTO);
				}
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Successfully added Product");
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
				return RedirectToAction("AllData", "Dashboard");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> EditData(int dataId)
		{
			var product = await _mainDataServices.GetProductsByIdForEdit(dataId);
			var categories = await _mainDataServices.GetAllCategoriesWithAttributes();
			var declarations = await _mainDataServices.GetAllDeclarations();
			var productAtributes = await _mainDataServices.GetAllProductAttributes(dataId);
			return View("Data/EditData", new DataIDTO
			{
				ProductIDTO = product,
				CategoriesODTOs = categories,
				DeclarationODTOs = declarations,
				ProductAttributeValues = productAtributes,
				SaleTypeODTOs = new List<DTO.ODTO.SaleTypeODTO> //TODO add methods for sale types
				{
					new DTO.ODTO.SaleTypeODTO{ SaleTypeId = 1, Value = "TEST" }
				}
			});
		}

		public async Task<IActionResult> EditDataAction(DataIDTO dataIDTO)
		{
			if (!ModelState.IsValid)
			{
				var product = await _mainDataServices.GetProductsByIdForEdit(dataIDTO.ProductIDTO.ProductId);
				var categories = await _mainDataServices.GetAllCategoriesWithAttributes();
				var declarations = await _mainDataServices.GetAllDeclarations();
				var productAtributes = await _mainDataServices.GetAllProductAttributes(dataIDTO.ProductIDTO.ProductId);
				return View("Data/EditData", new DataIDTO
				{
					ProductIDTO = product,
					CategoriesODTOs = categories,
					DeclarationODTOs = declarations,
					ProductAttributeValues = productAtributes,
					SaleTypeODTOs = new List<DTO.ODTO.SaleTypeODTO> //TODO add methods for sale types
				{
					new DTO.ODTO.SaleTypeODTO{ SaleTypeId = 1, Value = "TEST" }
				}
				});
			}
			try
			{
				var product = await _mainDataServices.EditProduct(dataIDTO.ProductIDTO);

				if (dataIDTO.ProductIDTO.SaleIDTO != null)
				{
					var sale = await _mainDataServices.AddSale(dataIDTO.ProductIDTO.SaleIDTO, dataIDTO.ProductIDTO.ProductId);
				}

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

				await _mainDataServices.DeleteAllProductAttributes(dataIDTO.ProductIDTO.ProductId);

				foreach (var attributeID in dataIDTO.ProductAttributeValues)
				{
					ProductAttributesIDTO productAttributesIDTO = new ProductAttributesIDTO()
					{
						ProductId = product.ProductId,
						IsDev = false, //TODO check how IsDev is set
						AttributesId = attributeID
					};
					await _mainDataServices.AddProductAttributes(productAttributesIDTO);
				}
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Successfully updated");
				_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
				return RedirectToAction("AllData", "Dashboard");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View("Home");
			}
		}

		public async Task<IActionResult> PreviewData(int dataId)
		{
			var product = await _mainDataServices.GetProductsByIdForEdit(dataId);
			var categories = await _mainDataServices.GetAllCategoriesWithAttributes();
			var declarations = await _mainDataServices.GetAllDeclarations();
			var productAtributes = await _mainDataServices.GetAllProductAttributes(dataId);
			return View("Data/PreviewData", new DataIDTO
			{
				ProductIDTO = product,
				CategoriesODTOs = categories,
				DeclarationODTOs = declarations,
				ProductAttributeValues = productAtributes,
				SaleTypeODTOs = new List<DTO.ODTO.SaleTypeODTO> //TODO add methods for sale types
				{
					new DTO.ODTO.SaleTypeODTO{ SaleTypeId = 1, Value = "TEST" }
				}
			});
		}

		#endregion Data/Products

		#region Categories

		public async Task<IActionResult> AllCategories()
		{
			var categories = await _mainDataServices.GetCategories(); //Dont pull non active categories
			var devidedList = new List<List<ChildODTO2>>();

			if (categories != null)
			{
				var rootList = categories.Where(x => x.ParentCategoryId == null).ToList();
				devidedList.Add(rootList);
				while (categories.Any(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)))
				{
					rootList = categories.Where(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)).ToList();
					devidedList.Add(rootList);
				}
				devidedList.Reverse();
			}
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

		public async Task<IActionResult> EditDeclarationModel(DeclarationIDTO declarationIDTO)
		{
			if (!ModelState.IsValid)
			{
				return View("Declaration/EditDeclaration", declarationIDTO);
			}
			await _mainDataServices.EditDeclaration(declarationIDTO);

			_httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Declaration " + declarationIDTO.DeclarationName + " edited successfully!");
			_httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");

			return RedirectToAction("AllDeclaration");
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

		#region Helpers

		private void CheckForToast()
		{
			ViewBag.ToastMessage = _httpContextAccessor.HttpContext.Session.Get<string>("ToastMessage");
			ViewBag.ToastType = _httpContextAccessor.HttpContext.Session.Get<string>("ToastType");
			_httpContextAccessor.HttpContext.Session.Remove("ToastMessage");
			_httpContextAccessor.HttpContext.Session.Remove("ToastType");
		}

		#endregion Helpers
	}
}