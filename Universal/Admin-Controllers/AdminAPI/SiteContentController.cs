using Microsoft.AspNetCore.Mvc;
using Services;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Universal.Admin_Controllers.AdminAPI
{
	[Route("api/[controller]")]
	[ApiController]
	public class SiteContentController : ControllerBase
	{
		private readonly MainDataServices _mainDataServices;

		public SiteContentController(MainDataServices mainServices)
		{
			_mainDataServices = mainServices;
		}

		[HttpPost]
		public async Task<ActionResult<SiteContentODTO>> PostSiteContent(SiteContentIDTO siteContentIDTO)
		{
			try
			{
				return await _mainDataServices.AddSiteContent(siteContentIDTO);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpGet("AllByType")]
		public async Task<ActionResult<IEnumerable<SiteContentODTO>>> GetAllSiteContentByTypes(string type, int langId)
		{
			try
			{
				return await _mainDataServices.GetAllSiteContentByType(type, langId);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpPut]
		public async Task<ActionResult<SiteContentODTO>> PutSiteContent(SiteContentIDTO siteContentIDTO)
		{
			try
			{
				return await _mainDataServices.EditSiteContent(siteContentIDTO);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpDelete("{id}")]
		public async Task<ActionResult<SiteContentODTO>> DeleteSiteContents(int id)
		{
			try
			{
				var siteContent = await _mainDataServices.DeleteSiteContent(id);
				if (siteContent == null) return NotFound();
				return siteContent;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpDelete("Tag{id}")]
		public async Task<ActionResult<TagODTO>> DeleteTag(int id)
		{
			try
			{
				return await _mainDataServices.DeleteTag(id);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		[HttpGet("GetAllTags")]
        public async Task<ActionResult<IEnumerable<TagODTO>>> GetAllTags()
        {
            try
            {
                return await _mainDataServices.GetTags();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}