using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Authorization;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Universal.Admin_Controllers.AdminAPI
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	[EnableCors("CorsPolicy")]
	public class PromocodesController : ControllerBase
	{
		private readonly MainDataServices _mainDataServices;

		public PromocodesController(MainDataServices mainServices)
		{
			_mainDataServices = mainServices;
		}

		[HttpGet("GetAllPromocodes")]
		public async Task<ActionResult<IEnumerable<PromocodesODTO>>> GetAll()
		{
			var promocodesODTOs = await _mainDataServices.GetAllPromoCodes();
			if (promocodesODTOs == null)
			{
				return NotFound();
			}
			return promocodesODTOs;
		}

		[HttpPost("CheckPromocode")]
		public async Task<ActionResult<PromoCodeCheckIDTO>> CheckPromocode(PromoCodeCheckIDTO promoCodeCheckIDTO)
		{
			var checkPromoCode = await _mainDataServices.CheckPromocode(promoCodeCheckIDTO);
			if(checkPromoCode == null)
			{
				return NotFound("Uneti promo kod nije validan");
			}
				
			return checkPromoCode;
		}

		[HttpDelete("DeletePromocode")]
		public async Task<string> DeletePromocode(int promoCodesId)
		{
			return await _mainDataServices.DeletePromocode(promoCodesId);
		}
	}
}
