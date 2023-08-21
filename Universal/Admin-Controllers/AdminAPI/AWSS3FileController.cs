using Microsoft.AspNetCore.Mvc;
using Services.AWS;
using static Universal.DTO.CommonModels.CommonModels;
using System.Web;

namespace Universal.Admin_Controllers.AdminAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class AWSS3FileController : ControllerBase
    {
        private readonly IAWSS3FileService _AWSS3FileService;
        public AWSS3FileController(IAWSS3FileService AWSS3FileService)
        {
           _AWSS3FileService = AWSS3FileService;
        }

        [Route("UploadFile")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFileAsync([FromForm] AWSFileUpload files)
        {
            var result = await _AWSS3FileService.UploadFile(files);
            return Ok(new { isSucess = result });
        }
        [Route("FilesList")]
        [HttpGet]
        public async Task<IActionResult> FilesListAsync(string folder)
        {
            var result = await _AWSS3FileService.FilesList(folder);
            return Ok(result);
        }

        [Route("FilesListSearch/{fileName}")]
        [HttpGet]
        public async Task<IActionResult> FilesListSearchAsync(string fileName)
        {
            var result = await _AWSS3FileService.FilesListSearch(HttpUtility.UrlDecode(fileName));
            return Ok(result);
        }
        [Route("GetFile/{fileName}")]
        [HttpGet]
        public async Task<IActionResult> GetFile(string fileName)
        {
            try
            {
                var result = await _AWSS3FileService.GetFile(HttpUtility.UrlDecode(fileName));
                return File(result, "image/png");
            }
            catch
            {
                return Ok("NoFile");
            }

        }
        //[Route("updateFile")]
        //[HttpPut]
        //public async Task<IActionResult> UpdateFile(UploadFileName uploadFileName, string fileName)
        //{
        //    var result = await _AWSS3FileService.UpdateFile(uploadFileName, fileName);
        //    return Ok(new { isSucess = result });
        //}
        [Route("DeleteFile/{fileName}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var result = await _AWSS3FileService.DeleteFile(HttpUtility.UrlDecode(fileName));
            return Ok(new { isSucess = result });
        }

        [Route("DeleteMultipleFiles")]
        [HttpPost]
        public async Task<IActionResult> DeleteMultipleFiles(string[] fileNames)
        {
            fileNames = fileNames.Select(x => HttpUtility.UrlDecode(x)).ToArray();
            var result = await _AWSS3FileService.DeleteMultipleFiles(fileNames);
            return Ok(new { isSucess = result });
        }
    }
}
