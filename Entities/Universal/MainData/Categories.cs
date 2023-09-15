using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
	public class Categories
	{
		public int CategoryId { get; set; }
		public int? ParentCategoryId { get; set; }
		public int? SeoId { get; set; }
		public int? MediaId { get; set; }
		public int? LanguageID { get; set; }
		public string? CategoryName { get; set; }
		public bool IsAttribute { get; set; }
		public bool? IsActive { get; set; }

		public Seo? Seo { get; set; }
		public Media? Media { get; set; }
		public Language? Languages { get; set; }
		public ICollection<Attributes> Attributes { get; set; }
		public ICollection<Product> Products { get; set; }
	}
}