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
	public class CategoriesController : ControllerBase
	{
		private readonly MainDataServices _mainDataServices;

		public CategoriesController(MainDataServices mainServices)
		{
			_mainDataServices = mainServices;
		}

		//GET: api/Categories
		[HttpGet("{id}")]
		public async Task<ActionResult<CategoriesODTO>> GetById(int id)
		{
			var categories = await _mainDataServices.GetCategoriesById(id);
			if (categories == null)
			{
				return NotFound();
			}
			return categories;
		}

		//PUT: api/Categories
		[HttpPut]
		public async Task<ActionResult<CategoriesODTO>> PutCategories(CategoriesIDTO categoriesIDTO)
		{
			try
			{
				return await _mainDataServices.EditCategory(categoriesIDTO);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		//POST: api/Categories
		[HttpPost]
		public async Task<ActionResult<CategoriesODTO>> PostCategories(CategoriesIDTO categoriesIDTO)
		{
			try
			{
				return await _mainDataServices.AddCategory(categoriesIDTO);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		//DELETE: api/Categories/1
		[HttpDelete("{id}")]
		public async Task<ActionResult<CategoriesODTO>> DeleteCategories(int id)
		{
			try
			{
				var categories = await _mainDataServices.DeleteCategory(id);
				if (categories == null) return NotFound();
				return categories;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}