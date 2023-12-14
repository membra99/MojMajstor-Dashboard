using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Universal.Admin_Controllers.AdminAPI
{
    [Route("api/[controller]")]
    [ApiController]
	[EnableCors("CorsPolicy")]
	public class AttributesController : ControllerBase
    {
        private readonly MainDataServices _mainDataServices;

        public AttributesController(MainDataServices mainServices)
        {
            _mainDataServices = mainServices;
        }

        [HttpGet("ById")]
        public async Task<ActionResult<AttributesODTO>> GetAttributesById(int id)
        {
            try
            {
                return await _mainDataServices.GetAttributesById(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        [HttpGet("GetAllAttributesByCategoryName")]
        public async Task<ActionResult<IEnumerable<CategoriesODTO>>> GetAllAttributesByCategoryName(int categoryId)
        {
            try
            {
                return await _mainDataServices.GetAllAttributesByCategoryName(categoryId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        [HttpGet("GetAllCategoriesWithAttributes")]
        public async Task<ActionResult<IEnumerable<CategoriesODTO>>> GetAllCategoriesWithAttributes()
        {
            try
            {
                return await _mainDataServices.GetAllCategoriesWithAttributes();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("GetAllAttributesValueByAttributeName")]
        public async Task<ActionResult<IEnumerable<AttributesODTO>>> GetAllAttributesValueByAttributeName(int categoryId)
        {
            try
            {
                return await _mainDataServices.GetAllAttributesValueByAttributeName(categoryId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AttributesODTO>> PostAttributes(AttributesIDTO attributesIDTO)
        {
            try
            {
                return await _mainDataServices.AddAttributes(attributesIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<AttributesODTO>> PutAttributes(AttributesIDTO attributesIDTO)
        {
            try
            {
                return await _mainDataServices.EditAttributes(attributesIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<AttributesODTO>> DeleteAttributes(int id)
        {
            try
            {
                return await _mainDataServices.DeleteAttributes(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}