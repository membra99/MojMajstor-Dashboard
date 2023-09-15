using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
	public class SeoODTO
	{
		public int SeoId { get; set; }
		public string? GoogleDesc { get; set; }
		public string? GoogleKeywords { get; set; }

		public int? LanguageID { get; set; }
		public string? LanguageName { get; set; }
	}
}