using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Services;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Universal.Admin_Controllers.AdminAPI
{
    [Route("api/[controller]")]
    [ApiController]
	[EnableCors("CorsPolicy")]
	public class DeclarationController : ControllerBase
    {
        private readonly MainDataServices _mainDataServices;

        public DeclarationController(MainDataServices mainServices)
        {
            _mainDataServices = mainServices;
        }

        [HttpGet("ById")]
        public async Task<ActionResult<DeclarationODTO>> GetDeclarationsById(int id)
        {
            try
            {
                return await _mainDataServices.GetDeclarationById(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<DeclarationODTO>>> GetAll()
        {
            try
            {
                return await _mainDataServices.GetAllDeclarations();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DeclarationODTO>> PostDeclaration(DeclarationIDTO declarationIDTO)
        {
            try
            {
               return await _mainDataServices.AddDeclaration(declarationIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<DeclarationODTO>> PutDeclaration(DeclarationIDTO declarationIDTO)
        {
            try
            {
               return await _mainDataServices.EditDeclaration(declarationIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DeclarationODTO>> DeleteDeclarations(int id)
        {
            try
            {
                return await _mainDataServices.DeleteDeclaration(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
