using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services;
using Services.AWS;
using Services.Helpers;
using System.Net.Mime;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using Universal.DTO.ViewDTO;
using Universal.Universal.MainDataNova;
using Universal.Util;
using static Universal.DTO.CommonModels.CommonModels;

namespace Universal.Admin_Controllers.AdminMVC
{
    [RequiresAuthToken]
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAWSS3FileService _AWSS3FileService;
        private readonly MainDataServices _mainDataServices;
        private readonly UsersServices _userDataServices;
        private readonly IOptions<EmailSettings> _emailSettings;

        public DashboardController(IHttpClientFactory httpClientFactory, IAWSS3FileService AWSS3FileService, IHttpContextAccessor contextAccessor, MainDataServices mainDataServices, UsersServices usersServices, IOptions<EmailSettings> emailSettings)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7213"); ////TODO change later to be dynamic
            _httpContextAccessor = contextAccessor;
            _mainDataServices = mainDataServices;
            _userDataServices = usersServices;
            _AWSS3FileService = AWSS3FileService;
            _emailSettings = emailSettings;
        }

        public async Task<IActionResult> Index()
        {
            var overview = await _mainDataServices.GetOverview();
            return View("Overview/Overview", overview);
        }

        #region Users

        public async Task<IActionResult> NewUser()
        {
            CheckForToast();
            var data = await _userDataServices.GetUserIDTO();
            return View("User/NewUser", data);
        }

        [HttpGet]
        public async Task<IActionResult> GetViewsByPeriod(DateTime startDate, DateTime endDate)
        {
            var data = await _userDataServices.GetViewsByPeriod(startDate, endDate);
            return Json(data);
        }

        public async Task<IActionResult> GetOverview()
        {
            try
            {
                var overview = await _mainDataServices.GetOverview();
                return View("Overview/Overview", overview);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public async Task<IActionResult> AllUsers()
        {
            try
            {
                CheckForToast();
                var users = await _userDataServices.GetAllMajstorUsers();
                return View("User/Users", users);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public async Task<IActionResult> AllAdvertisements()
        {
            try
            {
                var add = await _userDataServices.GetAllAdvertisements();
                return View("Advertisements/Advertisements", add);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public IActionResult GetBannersAction()
        {
            return View("Banners/Banners");
        }

        public async Task<IActionResult> AllTokens()
        {
            try
            {
                var tokens = await _mainDataServices.GetAllTokens();
                return View("Tokens/Tokens", tokens);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public async Task<IActionResult> AddUser(UserMajstorIDTO userIDTO)
        {
            if (!ModelState.IsValid)
            {
                return View("User/NewUser", new UsersIDTO());
            }
            
            var users = await _userDataServices.AddUserMojMajstor(userIDTO);
            if (users == null)
            {
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "User with that mail already exists");
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                return RedirectToAction("NewUser");
            }
            return RedirectToAction("AllUsers", "Dashboard");
        }

        public static bool IsSupportedExtension(string extension)
        {
            string[] supportedExtensions = { ".jpg", ".png", ".svg", ".webp", ".avif" };
            return Array.Exists(supportedExtensions, ext => ext == extension);
        }

        public async Task<IActionResult> EditUser(int userId)
        {
            return View("User/EditUser", await _userDataServices.GetUserByIdForEdit(userId));
        }

        public async Task<IActionResult> MetricByUserPartial(int userId)
        {
            var model = await _userDataServices.GetMetricByUser(userId);
            return PartialView("Metric/MetricByUser", model);
        }

        public async Task<IActionResult> GetImage(string path)
        {
            if (path == null)
                path = "Universal/noimage_202402021126392851.jpg";

            var picture = await _AWSS3FileService.GetFile(path);
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                picture.CopyTo(ms);
                bytes = ms.ToArray();
            }

            return File(bytes, MediaTypeNames.Text.Plain);
        }

        public async Task<string> DeleteUploadedImage(string img)
        {
            return await _mainDataServices.DeleteUploadedImage(img);
        }

        public IActionResult ImportExc()
        {
            CheckForToast();
            return View("ImportData/Import");
        }

        public async Task<IActionResult> DownloadExcelTemplate()
        {
            var picture = await _AWSS3FileService.GetFile("Universal/ExcelTemplate.xlsx");
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                picture.CopyTo(ms);
                bytes = ms.ToArray();
            }

            return File(bytes, MediaTypeNames.Text.Plain, "ExcelTemplate.xlsx");
        }

        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            if (file != null)
            {
                string extension = System.IO.Path.GetExtension(file.FileName)?.ToLower();
                if (extension != ".xlsx")
                {
                    _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "You need to enter Excel file");
                    _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                    return RedirectToAction("ImportExc");

                }
                var excelImport = await _mainDataServices.ImportFromExcel(file);
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Import data added successfully!");
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                return RedirectToAction("ImportExc", "Dashboard");
            }
            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "You need to upload file before importing!");
            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "warning");
            return RedirectToAction("ImportExc", "Dashboard");
        }


        public async Task<IActionResult> EditUserAction(UserMajstorIDTO userIDTO)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;

                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Polje: {key}, Greška: {error.ErrorMessage}");
                    }
                }
                return View("User/EditUser", await _userDataServices.GetUserByIdForEdit((int)userIDTO.UsersId));
            }

            //						FILE UPLOAD SYSTEM
            //AWSFileUpload awsFile = new AWSFileUpload();
            //awsFile.Attachments = new List<IFormFile>();
            //if (userIDTO.Avatar != null)
            //    awsFile.Attachments.Add(userIDTO.Avatar);
            try
            {
                //if (awsFile.Attachments.Count > 0)
                //{
                //    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                //    if (!IsSupportedExtension(extension))
                //    {
                //        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                //        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                //        return RedirectToAction("NewUser");
                //    }
                //    else
                //    {
                //        if (awsFile.Attachments[0].Length > 1000000)
                //        {
                //            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                //            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                //            return RedirectToAction("NewUser");
                //        }
                //        else
                //        {
                //            var media = await _userDataServices.UploadUserPicture(awsFile, null);
                //            if (media != null) userIDTO.MediaId = media.MediaId;
                //            var users = await _userDataServices.EditUser(userIDTO);
                //            if (users == null)
                //            {
                //                ModelState.AddModelError("UserExist", $"User with that mail alredy exist");
                //                return View("User/NewUser");
                //            }
                //            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "User is updated");
                //            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                //            return RedirectToAction("AllUsers", "Dashboard");
                //        }
                //    }
                //}
                var users = await _userDataServices.EditUser(userIDTO);
                if (users == null)
                {
                    ModelState.AddModelError("UserExist", $"User with that mail alredy exist");
                    return View("User/NewUser");
                }
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "User is updated");
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
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
            int mediatypeId = (file.ContentType == "video/mp4" || file.ContentType == "video/x-msvideo" || file.ContentType == "video/mpeg") ? 5 : 3;
            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            awsFile.Attachments.Add(file);
            var media = await _userDataServices.UploadUserPicture(awsFile, mediatypeId);
            return Ok();
        }

        public async Task<List<int>> UploadedImageHandler(List<string> imgNames)
        {
            return await _mainDataServices.UploadedImageHandler(imgNames);
        }

        public async Task<IActionResult> AllGallery()
        {
            var media = await _mainDataServices.GetAllImagesRoute();
            return View("Media/Gallery", new MultiMediaIDTO { MediaList = media });
        }

        public async Task<IActionResult> AllVideos()
        {
            var video = await _mainDataServices.GetAllVideoRoute();
            return View("Media/Video", new MultiMediaIDTO { MediaList = video });
        }

        public async Task<IActionResult> GalleryGrid()
        {
            var media = await _mainDataServices.GetAllImagesRoute();
            return PartialView("/Views/Partials/_GalleryGrid.cshtml", new MultiMediaIDTO { MediaList = media });
        }

        public async Task<IActionResult> VideoGrid()
        {
            var media = await _mainDataServices.GetAllVideoRoute();
            return PartialView("/Views/Partials/_VideoGrid.cshtml", new MultiMediaIDTO { MediaList = media });
        }

        public async Task<IActionResult> EditMeidaImage(MediaIDTO mediaIDTO)
        {
            await _mainDataServices.EditMediaImageMetaProperties(mediaIDTO);
            //TODO Maybe change name on server?
            return RedirectToAction("AllGallery");
        }

        public async Task<IActionResult> DeleteMeidaImage(MediaIDTO mediaIDTO)
        {
            //await _mainDataServices.DeleteMediaImage(mediaIDTO.MediaId, mediaIDTO.MediaTypeId);

            await _AWSS3FileService.DeleteFile(mediaIDTO.MetaTitle, mediaIDTO.MediaId, mediaIDTO.MediaTypeId);
            //TODO Delete from server
            return RedirectToAction("AllGallery");
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

        public async Task<IActionResult> EditSiteContent(int siteContentId)
        {
            return View("SiteContent/EditSiteContent", await _mainDataServices.GetSiteContentByIdForEdit(siteContentId));
        }

        public async Task<IActionResult> EditSiteContentAction(SiteContentIDTO siteContentIDTO)
        {
            if (!ModelState.IsValid)
            {
                return View("SiteContent/EditSiteContent", await _mainDataServices.GetSiteContentByIdForEdit(siteContentIDTO.SiteContentId));
            }

            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            if (siteContentIDTO.Image != null)
                awsFile.Attachments.Add(siteContentIDTO.Image);


            try
            {
                if (awsFile.Attachments.Count > 0)
                {
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("NewUser");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("NewUser");
                        }
                        else
                        {
                            var media = await _userDataServices.UploadUserPicture(awsFile, null);
                            if (media != null) siteContentIDTO.MediaId = media.MediaId;
                            var siteContent = await _mainDataServices.EditSiteContent(siteContentIDTO);
                            if (siteContent.SiteContentTypeName == "Blog")
                            {
                                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Blog is updated");
                            }
                            else
                            {
                                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Page is updated");
                            }
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                            return RedirectToAction("AllSiteContent", "Dashboard", new { type = siteContent.SiteContentTypeName, langId = siteContent.LanguageID });
                        }
                    }
                }
                else
                {
                    var siteContent = await _mainDataServices.EditSiteContent(siteContentIDTO);
                    if (siteContent.SiteContentTypeName == "Blog")
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Blog is updated");
                    }
                    else
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Page is updated");
                    }
                    _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                    return RedirectToAction("AllSiteContent", "Dashboard", new { type = siteContent.SiteContentTypeName, langId = siteContent.LanguageID });
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        public async Task<IActionResult> AddSiteContent(SiteContentIDTO siteContentIDTO)
        {
            CheckForToast();
            if (!ModelState.IsValid)
            {
                SiteContentIDTO contentIDTO = new SiteContentIDTO();
                contentIDTO.SiteContentTypeId = siteContentIDTO.SiteContentTypeId;
                contentIDTO.TagODTOs = await _mainDataServices.GetTags();
                return View("SiteContent/AddSiteContent", contentIDTO);
            }
            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            if (siteContentIDTO.Image != null)
                awsFile.Attachments.Add(siteContentIDTO.Image);
            try
            {
                if (awsFile.Attachments.Count != 0)
                {
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("AddSiteContent");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("AddSiteContent");
                        }
                        else
                        {
                            var media = await _userDataServices.UploadUserPicture(awsFile, 3);
                            if (media != null) siteContentIDTO.MediaId = media.MediaId;
                            siteContentIDTO.IsActive = true;
                            var siteContent = await _mainDataServices.AddSiteContent(siteContentIDTO);
                            var site = (siteContentIDTO.SiteContentTypeId == 1) ? "Page" : "Blog";
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "" + site + " added successfully!");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");

                            return RedirectToAction("AllSiteContent", "Dashboard", new { type = site });
                        }
                    }
                }
                else
                {
                    siteContentIDTO.IsActive = true;
                    siteContentIDTO.LanguageID = 1; //TODO - Set LanguageID dinamicaly
                    var siteContent = await _mainDataServices.AddSiteContent(siteContentIDTO);
                    var site = (siteContentIDTO.SiteContentTypeId == 1) ? "Page" : "Blog";
                    _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "" + site + " added successfully!");
                    _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");

                    return RedirectToAction("AllSiteContent", "Dashboard", new { type = site });
                }


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

            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            if (tagIDTO.TagImage != null)
                awsFile.Attachments.Add(tagIDTO.TagImage);

            try
            {
                if (awsFile.Attachments.Count != 0)
                {
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("EditTag");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("EditTag");
                        }
                        else
                        {
                            var media = await _userDataServices.UploadUserPicture(awsFile, 5);
                            if (media != null) tagIDTO.MediaId = media.MediaId;
                            await _mainDataServices.AddTag(tagIDTO);
                        }
                    }
                }
                else
                {
                    await _mainDataServices.AddTag(tagIDTO);
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }

            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Tag added successfully!");
            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");

            return RedirectToAction("AllTags");

        }

        public async Task<IActionResult> EditTag(int id)
        {
            try
            {
                CheckForToast();
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
                if (awsFile.Attachments.Count != 0)
                {
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("EditTag");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("EditTag");
                        }
                        else
                        {
                            var media = await _userDataServices.UploadUserPicture(awsFile, 5);
                            if (media != null) tagIDTO.MediaId = media.MediaId;
                            await _mainDataServices.EditTag(tagIDTO);
                        }
                    }
                }
                else
                {
                    await _mainDataServices.EditTag(tagIDTO);
                }

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

        [AllowAnonymous]
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
                ViewData.Add("Languages", await _mainDataServices.GetAllLanguages());
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
                ViewData.Add("Languages", await _mainDataServices.GetAllLanguages());
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

        public async Task<string> GetImageFromGallery(int mediaId)
        {
            return await _mainDataServices.GetImageSrcFromGallery(mediaId);
        }

        public async Task<List<string>> GetGalleryImagesSrc(List<int> mediaIds)
        {
            return await _mainDataServices.GetGalleryImagesSrc(mediaIds);
        }

        public async Task<IActionResult> NewData()
        {
            int? productID = (int?)TempData["productID"];
            var categories = await _mainDataServices.GetAllCategoriesWithAttributes();
            var allCategories = await _mainDataServices.GetCategories();
            var devidedList = new List<List<ChildODTO2>>();
            var product = new ProductIDTO();
            if (allCategories != null)
            {
                var rootList = allCategories.Where(x => x.ParentCategoryId == null).ToList();
                devidedList.Add(rootList);
                while (allCategories.Any(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)))
                {
                    rootList = allCategories.Where(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)).ToList();
                    devidedList.Add(rootList);
                }
                devidedList.Reverse();
            }
            if (productID != null)
            {
                product = await _mainDataServices.GetProductsByIdForEdit((int)productID);
                product.ProductName = "";
                product.Description = null;
                product.Specification = null;
            }
            var declarations = await _mainDataServices.GetAllDeclarations();
            ViewData.Add("Languages", await _mainDataServices.GetAllLanguages());
            return View("Data/NewData", new DataIDTO
            {
                ProductIDTO = product,
                CategoriesODTOs = categories,
                DeclarationODTOs = declarations,
                SaleTypeODTOs = await _mainDataServices.GetAllSaleType(),
                CategoriesTree = new CategoryAttributeIDTO { AllCategories = devidedList }
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
                var allCategories = await _mainDataServices.GetCategories();
                var devidedList = new List<List<ChildODTO2>>();

                if (allCategories != null)
                {
                    var rootList = allCategories.Where(x => x.ParentCategoryId == null).ToList();
                    devidedList.Add(rootList);
                    while (allCategories.Any(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)))
                    {
                        rootList = allCategories.Where(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)).ToList();
                        devidedList.Add(rootList);
                    }
                    devidedList.Reverse();
                }

                var declarations = await _mainDataServices.GetAllDeclarations();
                ViewData.Add("Languages", await _mainDataServices.GetAllLanguages());
                return View("Data/NewData", new DataIDTO
                {
                    CategoriesODTOs = categories,
                    DeclarationODTOs = declarations,
                    SaleTypeODTOs = await _mainDataServices.GetAllSaleType(),
                    CategoriesTree = new CategoryAttributeIDTO { AllCategories = devidedList }
                    //ViewData.Add("Languages", await _mainDataServices.GetAllLanguages());
                });
            }
            try
            {
                var product = await _mainDataServices.AddProduct(dataIDTO.ProductIDTO);

                if (dataIDTO.MediaId != null)
                {
                    await _mainDataServices.AddMediaForProduct(dataIDTO.MediaId, "Featured Image", product.ProductId);
                }

                AWSFileUpload awsFile = new AWSFileUpload();
                awsFile.Attachments = new List<IFormFile>();
                if (dataIDTO.FeaturedImage != null)
                    awsFile.Attachments.Add(dataIDTO.FeaturedImage);

                if (awsFile.Attachments.Count != 0)
                {
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("AllData");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("AllData");
                        }
                        else
                        {
                            await _mainDataServices.UploadProductImage(awsFile, "Featured Image", product.ProductId, 1, null);
                        }
                    }
                }


                foreach (IFormFile file in dataIDTO.GalleryImages)
                {
                    awsFile.Attachments = new List<IFormFile>
                    {
                        file
                    };
                    string extension = System.IO.Path.GetExtension(file.FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("AllData");
                    }
                    else
                    {
                        if (file.Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("AllData");
                        }
                        else
                        {
                            await _mainDataServices.UploadProductImage(awsFile, "Gallery", product.ProductId, 1, null);
                        }
                    }
                }

                await _mainDataServices.SetProperGallery(dataIDTO.ProductIDTO.GalleyImg, dataIDTO.MediaIds, dataIDTO.UploadedImages, product.ProductId);

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
                TempData["productID"] = product.ProductId != null ? null : product.ProductId;
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Successfully added Product");
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                return RedirectToAction(nameof(AllData));
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

            var allCategories = await _mainDataServices.GetCategories();
            var devidedList = new List<List<ChildODTO2>>();

            if (allCategories != null)
            {
                var rootList = allCategories.Where(x => x.ParentCategoryId == null).ToList();
                devidedList.Add(rootList);
                while (allCategories.Any(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)))
                {
                    rootList = allCategories.Where(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)).ToList();
                    devidedList.Add(rootList);
                }
                devidedList.Reverse();
            }

            return View("Data/EditData", new DataIDTO
            {
                ProductIDTO = product,
                CategoriesODTOs = categories,
                DeclarationODTOs = declarations,
                ProductAttributeValues = productAtributes,
                CategoriesTree = new CategoryAttributeIDTO { AllCategories = devidedList },
                SaleTypeODTOs = await _mainDataServices.GetAllSaleType()
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

                var allCategories = await _mainDataServices.GetCategories();
                var devidedList = new List<List<ChildODTO2>>();

                if (allCategories != null)
                {
                    var rootList = allCategories.Where(x => x.ParentCategoryId == null).ToList();
                    devidedList.Add(rootList);
                    while (allCategories.Any(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)))
                    {
                        rootList = allCategories.Where(x => rootList.Exists(y => y.CategoryId == x.ParentCategoryId)).ToList();
                        devidedList.Add(rootList);
                    }
                    devidedList.Reverse();
                }

                return View("Data/EditData", new DataIDTO
                {
                    ProductIDTO = product,
                    CategoriesODTOs = categories,
                    DeclarationODTOs = declarations,
                    ProductAttributeValues = productAtributes,
                    SaleTypeODTOs = await _mainDataServices.GetAllSaleType(),
                    CategoriesTree = new CategoryAttributeIDTO { AllCategories = devidedList }
                });
            }
            try
            {
                var product = await _mainDataServices.EditProduct(dataIDTO.ProductIDTO);

                if (dataIDTO.ProductIDTO.SaleIDTO != null)
                {
                    var sale = await _mainDataServices.AddSale(dataIDTO.ProductIDTO.SaleIDTO, dataIDTO.ProductIDTO.ProductId);
                }

                if (dataIDTO.MediaId != null)
                {
                    await _mainDataServices.EditFeaturedImage(dataIDTO.ProductIDTO.ProductId, (int)dataIDTO.MediaId);
                }

                AWSFileUpload awsFile = new AWSFileUpload();
                awsFile.Attachments = new List<IFormFile>();
                if (dataIDTO.FeaturedImage != null)
                {
                    awsFile.Attachments.Add(dataIDTO.FeaturedImage);
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("AllData");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("AllData");
                        }
                        else
                        {
                            await _mainDataServices.DeleteCurrentFeatureImage(product.ProductId, "Featured Image");
                            await _mainDataServices.UploadProductImage(awsFile, "Featured Image", product.ProductId, 1, null);
                        }
                    }
                }

                await _mainDataServices.SetProperGallery(dataIDTO.ProductIDTO.GalleyImg, dataIDTO.MediaIds, dataIDTO.UploadedImages, dataIDTO.ProductIDTO.ProductId);

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
            var productSale = await _mainDataServices.GetAllSaleType();
            return View("Data/PreviewData", new DataIDTO
            {
                ProductIDTO = product,
                CategoriesODTOs = categories,
                DeclarationODTOs = declarations,
                ProductAttributeValues = productAtributes,
                SaleTypeODTOs = productSale
            });
        }

        #endregion Data/Products

        #region Categories

        public async Task<IActionResult> AllCategories()
        {
            CheckForToast();
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
            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            if (categoryAttributeIDTO.CategoryImage != null)
                awsFile.Attachments.Add(categoryAttributeIDTO.CategoryImage);
            try
            {
                if (awsFile.Attachments.Count != 0)
                {
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("AllCategories");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("AllCategories");
                        }
                        else
                        {
                            var media = await _userDataServices.UploadUserPicture(awsFile, 6);
                            if (media != null) categoryAttributeIDTO.CategoryIDTO.MediaId = media.MediaId;
                            await _mainDataServices.AddCategory(categoryAttributeIDTO.CategoryIDTO);
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "New Category added successfully");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                            return RedirectToAction("AllCategories");
                        }
                    }
                }
                else
                {
                    await _mainDataServices.AddCategory(categoryAttributeIDTO.CategoryIDTO);
                    _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "New Category added successfully");
                    _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                    return RedirectToAction("AllCategories");
                }

            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }

        }

        public async Task<IActionResult> EditCategory(CategoryAttributeIDTO categoryAttributeIDTO)
        {
            AWSFileUpload awsFile = new AWSFileUpload();
            awsFile.Attachments = new List<IFormFile>();
            if (categoryAttributeIDTO.CategoryImage != null)
            {
                awsFile.Attachments.Add(categoryAttributeIDTO.CategoryImage);
                categoryAttributeIDTO.CategoryIDTO.CategoryImage = categoryAttributeIDTO.CategoryImage.FileName;
            }

            try
            {
                if (categoryAttributeIDTO.CategoryImage != null)
                {
                    string extension = System.IO.Path.GetExtension(awsFile.Attachments[0].FileName)?.ToLower();
                    if (!IsSupportedExtension(extension))
                    {
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Extension is not supported");
                        _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                        return RedirectToAction("AllCategories");
                    }
                    else
                    {
                        if (awsFile.Attachments[0].Length > 1000000)
                        {
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "This image is big dimension");
                            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "error");
                            return RedirectToAction("AllCategories");
                        }
                        else
                        {
                            var media = await _userDataServices.UploadUserPicture(awsFile, 6);
                            if (media != null) categoryAttributeIDTO.CategoryIDTO.MediaId = media.MediaId;
                            await _mainDataServices.EditCategory(categoryAttributeIDTO.CategoryIDTO);
                            return RedirectToAction("AllCategories");
                        }
                    }
                }
                else
                {
                    await _mainDataServices.EditCategory(categoryAttributeIDTO.CategoryIDTO);
                    return RedirectToAction("AllCategories");
                }



            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }

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

        #region Newsletter
        public async Task<ActionResult> SendNewsLetter()
        {
            return View("Newsletters/SendNewsLetter");
        }

        public async Task<ActionResult> SendNewsletterAction(string newsletterContent)
        {
            var newslettersUsers = await _mainDataServices.GetAllNewsletterMails();

            MailService ms = new MailService(_emailSettings);
            int newsletterCount = newslettersUsers.Count();
            for (int i = 0; i < newsletterCount; i++)
            {
                await Task.Delay(1000);
                ms.SendEmail(new EmailIDTO
                {
                    To = newslettersUsers[i].NewsletterMail,
                    Subject = "Check out news from Exclusive cookies",
                    Body = newsletterContent + "\n" + "<div style='text-align: center;'><a style='text-decoration: none; color: grey;' href='https://localhost:7213/Dashboard/UnsubscribeFromNewsletter?useremail=" + newslettersUsers[i].NewsletterMail.ToString() + "'>Odjavite se sa newslettera</a></div>"
                });
            }
            return RedirectToAction(nameof(SendNewsLetter));
        }

        public async Task<ActionResult> UnsubscribeFromNewsletter(string useremail)
        {
            var response = await _mainDataServices.UnsubscribeFromNewsletter(useremail);
            return View("~/Views/Authentication/NewsletterUnsubscribe.cshtml", response);
        }
        #endregion

        #region PromoCodes
        public async Task<ActionResult> PromoCodes()
        {
            CheckForToast();
            var promCodes = await _mainDataServices.GetAllPromoCodes();
            return View("PromoCodes/AllPromoCodes", promCodes);
        }

        public async Task<ActionResult> AddNewPromocode()
        {
            CheckForToast();
            return View("PromoCodes/AddNewPromoCode");
        }

        public async Task<IActionResult> AddNewPromocodeAction(PromoCodesIDTO promoCodeIDTO)
        {
            if (ModelState.IsValid)
            {
                await _mainDataServices.AddNewPromocode(promoCodeIDTO);
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Successfully added promo code");
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                return RedirectToAction("PromoCodes");
            }
            return View("PromoCodes/AddNewPromoCode");
        }

        public async Task<ActionResult> EditPromoCode(int promoCodesId)
        {
            var promoCode = await _mainDataServices.GetPromocodeById(promoCodesId);
            return View("PromoCodes/EditPromoCode", promoCode);
        }

        public async Task<ActionResult> EditPromoCodeAction(PromoCodesIDTO promoCodeIDTO)
        {
            if (!ModelState.IsValid)
            {
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Error while updating promo code");
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "warning");
            }
            try
            {
                var promocode = await _mainDataServices.EditPromocode(promoCodeIDTO);
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Successfully updated promo code");
                _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
                return RedirectToAction("PromoCodes", "Dashboard");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return View("PromoCodes/EditPromoCode", await _mainDataServices.GetPromocodeById((int)promoCodeIDTO.PromoCodesId));

        }

        public async Task<ActionResult> SendPromoCode(int promoCodesId)
        {
            var promoCode = await _mainDataServices.GetPromocodeById(promoCodesId);
            return View("PromoCodes/SendPromocodes", promoCode);
        }

        public async Task<ActionResult> SendPromoCodeAction(PromoCodesIDTO promoCodeIDTO)
        {
            await _mainDataServices.SendPromoCode(promoCodeIDTO);
            _httpContextAccessor.HttpContext.Session.Set<string>("ToastMessage", "Promocode has been sent");
            _httpContextAccessor.HttpContext.Session.Set<string>("ToastType", "success");
            return RedirectToAction("PromoCodes");
        }
        #endregion

        #region Helpers

        private void CheckForToast()
        {
            ViewBag.ToastMessage = _httpContextAccessor.HttpContext.Session.Get<string>("ToastMessage");
            ViewBag.ToastType = _httpContextAccessor.HttpContext.Session.Get<string>("ToastType");
            _httpContextAccessor.HttpContext.Session.Remove("ToastMessage");
            _httpContextAccessor.HttpContext.Session.Remove("ToastType");
        }

        #endregion Helpers

        public async Task<IActionResult> GetCharts()
        {
            try
            {
                var orders = await _mainDataServices.GetOrderForChart(null);
                return View("Charts/Charts", orders);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View("Home");
            }
        }

        [HttpGet]
        public async Task<List<int>> GetChartDataByYear(int year)
        {
            var val = await _mainDataServices.GetOrderForChart(year);
            return val.SumByYear;
        }

        [HttpPost]
        public IActionResult UpdatePrice(int tokenId, double newPrice)
        {
            _mainDataServices.UpdateTokenPrice(tokenId, newPrice);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SaveBanner(IFormFile? BannerImage, int position, string url)
        {
            if (BannerImage == null || BannerImage.Length == 0)
                return Json(new { success = false, message = "No file uploaded" });

            AWSFileUpload aWSFileUpload = new AWSFileUpload();
            aWSFileUpload.Attachments.Add(BannerImage);

            Medium imageUrl = await _mainDataServices.UploadProductImage(aWSFileUpload, "Banner", null, position, url);
            return Json(new { success = true, imageUrl });
        }

        [HttpGet]
        public async Task<IActionResult> GetBanners()
        {
            var banners = await _mainDataServices.GetBannersImages();
            var result = new List<object>();
            foreach (var banner in banners)
            {
                result.Add(new
                {
                    id = banner.MediaId,
                    imageUrl = "https://somsed.s3.eu-central-1.amazonaws.com/" + banner.Src,
                    position = banner.Postition,
                    Url = banner.Url
                });
            }
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            try
            {
                await _mainDataServices.DeleteUploadedImageMajstor(id);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}