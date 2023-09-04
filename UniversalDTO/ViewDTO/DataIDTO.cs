using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;
using Microsoft.AspNetCore.Http;

namespace Universal.DTO.ViewDTO
{
	public class DataIDTO
	{
		public ProductIDTO ProductIDTO { get; set; }
		public List<int>? ProductAttributeValues { get; set; } = new List<int>();
		public IFormFile FeaturedImage { get; set; }
		public List<IFormFile>? GalleryImages { get; set; } = new List<IFormFile>();

		public List<DeclarationODTO>? DeclarationODTOs { get; set; }
		public List<ChildODTO2>? CategoriesODTOs { get; set; }
		public List<SaleTypeODTO>? SaleTypeODTOs { get; set; }
	}
}
