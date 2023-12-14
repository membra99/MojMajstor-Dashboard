using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.AWS;
using Universal.DTO.ODTO;

namespace Universal.Admin_Controllers.AdminAPI
{
    [Route("api/[controller]")]
    [ApiController]
	[EnableCors("CorsPolicy")]
	public class MediaController : ControllerBase
    {
        private readonly MainDataServices _mainDataServices;
		private readonly IAWSS3FileService _AWSS3FileService;


		public MediaController(MainDataServices mainServices, IAWSS3FileService AWSS3FileService)
        {
            _mainDataServices = mainServices;
			_AWSS3FileService = AWSS3FileService;
		}

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<MediaODTO>>> GetAllMedias()
        {
            try
            {
                return await _mainDataServices.GetAllMedia();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

		[HttpGet("CheckMediaId")]
		public async Task<ActionResult<string>> GetMediaExists(int mediaId)
		{
			try
			{
				return await _mainDataServices.GetStringForModal(mediaId);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}
