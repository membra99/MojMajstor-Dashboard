using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
	public class SiteContentODTO
	{
		public int SiteContentId { get; set; }
		public string? Title { get; set; }
		public int? SiteContentTypeId { get; set; }
		public string? GoogleDesc { get; set; }
		public string? GoogleKeywords { get; set; }
		public int? MediaId { get; set; }
		public int? SeoId { get; set; }
		public int? TagId { get; set; }
		public string? Content { get; set; }
		public bool IsActive { get; set; }

		public int? LanguageID { get; set; }
		public string? LanguageName { get; set; }
	}
}