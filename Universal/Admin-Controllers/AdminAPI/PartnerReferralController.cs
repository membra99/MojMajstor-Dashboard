using Microsoft.AspNetCore.Cors;
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
    public class PartnerReferralController : ControllerBase
    {
        private readonly MainDataServices _mainDataServices;

        public PartnerReferralController(MainDataServices mainDataServices)
        {
            _mainDataServices = mainDataServices;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<List<PartnerReferralODTO>>> GetAll()
        {
            var referrals = await _mainDataServices.GetAllPartnerReferrals();
            return Ok(referrals);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<PartnerReferralODTO>> Add(PartnerReferralIDTO dto)
        {
            var result = await _mainDataServices.AddPartnerReferral(dto);
            return Ok(result);
        }

        [HttpPut("Edit")]
        public async Task<ActionResult<PartnerReferralODTO>> Edit(int id, [FromBody] PartnerReferralIDTO dto)
        {
            var result = await _mainDataServices.EditPartnerReferral(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("Stats")]
        public async Task<ActionResult<PartnerReferralStatsODTO>> Stats(int id, DateTime? from, DateTime? to)
        {
            var stats = await _mainDataServices.GetPartnerReferralStats(id, from, to);
            if (stats == null) return NotFound();
            return Ok(stats);
        }

        [HttpDelete("Delete")]
        public async Task<string> Delete(int id)
        {
            return await _mainDataServices.DeletePartnerReferral(id);
        }
    }
}
