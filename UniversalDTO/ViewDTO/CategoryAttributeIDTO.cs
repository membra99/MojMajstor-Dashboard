using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Universal.DTO.ViewDTO
{
	public class CategoryAttributeIDTO
	{
		public List<List<ChildODTO2>> AllCategories { get; set; }
		public CategoriesIDTO? CategoryIDTO { get; set; }
		public List<IFormFile>? CategoryImage { get; set; } = new List<IFormFile>();
	}
}
