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
	public class ProductAttributesController : ControllerBase
    {
        private readonly MainDataServices _mainDataServices;

        public ProductAttributesController(MainDataServices mainServices)
        {
            _mainDataServices = mainServices;
        }

        //GET: api/ProductAttributes
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductAttributesODTO>> GetById(int id)
        {
            var productAttributes = await _mainDataServices.GetProductAttributesById(id);
            if (productAttributes == null)
            {
                return NotFound();
            }
            return productAttributes;
        }

        //PUT: api/ProductAttributes
        [HttpPut]
        public async Task<ActionResult<ProductAttributesODTO>> PutProductAttributes(ProductAttributesIDTO productAttributesIDTO)
        {
            try
            {
                return await _mainDataServices.EditProductAttributes(productAttributesIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //POST: api/ProductAttributes
        [HttpPost]
        public async Task<ActionResult<ProductAttributesODTO>> PostProductAttributes(ProductAttributesIDTO productAttributesIDTO)
        {
            try
            {
                return await _mainDataServices.AddProductAttributes(productAttributesIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE: api/ProductAttributes/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductAttributesODTO>> DeleteProductAttributes(int id)
        {
            try
            {
                var productAttributes = await _mainDataServices.DeleteProductAttributes(id);
                if (productAttributes == null) return NotFound();
                return productAttributes;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}