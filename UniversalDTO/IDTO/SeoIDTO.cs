using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
	public class SeoIDTO
	{
		public int SeoId { get; set; }
		public string? GoogleDesc { get; set; }
		public string? GoogleKeywords { get; set; }

		public int? LanguageID { get; set; }
	}
}