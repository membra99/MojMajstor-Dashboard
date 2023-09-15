using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
	public class CategoriesODTO
	{
		public int CategoryId { get; set; }
		public int? ParentCategoryId { get; set; }
		public string? CategoryName { get; set; }
		public bool IsAttribute { get; set; }
		public bool? IsActive { get; set; }
		public string? GoogleDesc { get; set; }
		public string? GoogleKeywords { get; set; }
		public int? LanguageID { get; set; }
		public string? LanguageName { get; set; }
	}
}