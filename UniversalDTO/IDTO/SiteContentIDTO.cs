using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.DTO.ODTO;

namespace Universal.DTO.IDTO
{
	public class SiteContentIDTO
	{
		public int SiteContentId { get; set; }
		public int? LanguageID { get; set; }
		public string Title { get; set; }
		public int? SiteContentTypeId { get; set; }
		public int? MediaId { get; set; }
		public int? SeoId { get; set; }
		public int? TagId { get; set; }
		public string Content { get; set; }
		public bool IsActive { get; set; }
		public IFormFile? Image { get; set; }
		public string? IsImageChanged { get; set; }
		public string? Photo { get; set; }
		public SeoIDTO? SeoIDTO { get; set; }
		public List<TagODTO>? TagODTOs { get; set; }
	}
}