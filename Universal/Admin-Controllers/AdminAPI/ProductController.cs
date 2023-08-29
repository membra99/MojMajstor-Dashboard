using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Universal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly MainDataServices _mainDataServices;

        public ProductController(MainDataServices mainServices)
        {
            _mainDataServices = mainServices;
        }

        //GET: api/Product
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductODTO>> GetById(int id)
        {
            var product = await _mainDataServices.GetProductsById(id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<IEnumerable<ProductODTO>>> GetAll()
        {
            var products = await _mainDataServices.GetAllProducts();
            if (products == null)
            {
                return NotFound();
            }
            return products;
        }

        [HttpGet("GetCategories")]
        public async Task<ActionResult<IEnumerable<ChildODTO2>>> GetCategories()
        {
            var categories = await _mainDataServices.GetCategories();
            if (categories == null)
            {
                return NotFound();
            }
            return categories;
        }

        //[HttpGet("GetAttributes")]
        //public async Task<ActionResult<IEnumerable<AttributeODTO>>> GetAttributes(int CategoryId)
        //{
        //    var categories = await _mainDataServices.GetAttribute(CategoryId);
        //    if (categories == null)
        //    {
        //        return NotFound();
        //    }
        //    return categories;
        //}

        [HttpGet("GetProductDataByCategoryName")]
        public async Task<ActionResult<ParentChildODTO>> GetProductDatas(int Id)
        {
            var products = await _mainDataServices.GetTree(Id);
            if (products == null)
            {
                return NotFound();
            }
            return products;
        }

        //PUT: api/Product
        [HttpPut]
        public async Task<ActionResult<ProductODTO>> PutProduct(ProductIDTO productIDTO)
        {
            try
            {
                return await _mainDataServices.EditProduct(productIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //POST: api/Product
        [HttpPost]
        public async Task<ActionResult<ProductODTO>> PostProduct(ProductIDTO productIDTO)
        {
            try
            {
                return await _mainDataServices.AddProduct(productIDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        //DELETE: api/Product/1
        [HttpDelete("{id}")]
        public async Task<ActionResult<ProductODTO>> DeleteProduct(int id)
        {
            try
            {
                var product = await _mainDataServices.DeleteProduct(id);
                if (product == null) return NotFound();
                return product;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}