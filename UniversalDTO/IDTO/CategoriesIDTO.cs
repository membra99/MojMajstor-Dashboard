using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
	public class CategoriesIDTO
	{
		public int CategoryId { get; set; }
		public int? ParentCategoryId { get; set; }
		public int? MediaId { get; set; }
		public string? CategoryName { get; set; }
		public bool IsAttribute { get; set; }
		public bool? IsActive { get; set; } = true;

		public int? LanguageID { get; set; }
		public SeoIDTO? SeoIDTO { get; set; } = new SeoIDTO();
	}
}