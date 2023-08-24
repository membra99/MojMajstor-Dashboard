using Microsoft.AspNetCore.Mvc;
using Services;
using Universal.DTO.ODTO;

namespace Universal.Admin_Controllers.AdminAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController
    {
        private readonly MainDataServices _mainDataServices;

        public MediaController(MainDataServices mainServices)
        {
            _mainDataServices = mainServices;
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
    }
}
