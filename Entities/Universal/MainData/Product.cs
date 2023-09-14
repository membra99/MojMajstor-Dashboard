using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
	public class Product
	{
		public int ProductId { get; set; }
		public int CategoriesId { get; set; }
		public int? DeclarationId { get; set; }
		public int? SeoId { get; set; }
		public int? LanguageID { get; set; }
		public string? ProductName { get; set; }
		public double Price { get; set; }
		public bool IsOnSale { get; set; }
		public string? Description { get; set; }
		public string? Specification { get; set; }
		public bool Recommended { get; set; }
		public bool BestProduct { get; set; }
		public string? ProductCode { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public bool? IsActive { get; set; }
		public int? Quantity { get; set; }

		public Categories Categories { get; set; }
		public Declaration? Declaration { get; set; }
		public Seo? Seo { get; set; }
		public Language? Language { get; set; }
		public ICollection<ProductAttributes> ProductAttributes { get; set; }
		public ICollection<Media> Medias { get; set; }
		public ICollection<Sale> Sales { get; set; }
		public ICollection<OrderDetails> OrderDetails { get; set; }
	}
}