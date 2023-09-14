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
	public class LanguagesController : ControllerBase
	{
		private readonly MainDataServices _mainDataServices;

		public LanguagesController(MainDataServices mainServices)
		{
			_mainDataServices = mainServices;
		}

		//GET: api/Language
		[HttpGet("{id}")]
		public async Task<ActionResult<LanguageODTO>> GetById(int id)
		{
			var language = await _mainDataServices.GetLanguageById(id);
			if (language == null)
			{
				return NotFound();
			}
			return language;
		}

		//PUT: api/Language
		[HttpPut]
		public async Task<ActionResult<LanguageODTO>> PutLanguage(LanguageIDTO languageIDTO)
		{
			try
			{
				return await _mainDataServices.EditLanguage(languageIDTO);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		//POST: api/Language
		[HttpPost]
		public async Task<ActionResult<LanguageODTO>> PostLanguage(LanguageIDTO languageIDTO)
		{
			try
			{
				return await _mainDataServices.AddLanguage(languageIDTO);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		//DELETE: api/Language/1
		[HttpDelete("{id}")]
		public async Task<ActionResult<LanguageODTO>> DeleteLanguage(int id)
		{
			try
			{
				var language = await _mainDataServices.DeleteLanguage(id);
				if (language == null) return NotFound();
				return language;
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}
	}
}