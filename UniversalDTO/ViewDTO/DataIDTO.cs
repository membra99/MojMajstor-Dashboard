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
        public int? MediaId { get; set; }
        public List<int?> ProductAttributeValues { get; set; } = new List<int?>();
        public IFormFile? FeaturedImage { get; set; }
        public List<IFormFile>? GalleryImages { get; set; } = new List<IFormFile>();
		public List<int>? MediaIds { get; set; }
		public List<int>? UploadedImages { get; set; }

		public List<DeclarationODTO>? DeclarationODTOs { get; set; }
        public List<CategoriesODTO>? CategoriesODTOs { get; set; }
        public List<SaleTypeODTO>? SaleTypeODTOs { get; set; }
        public SaleTypeODTO? SaleTypeODTO { get; set; }
        public CategoryAttributeIDTO? CategoriesTree { get; set; }
    }
}