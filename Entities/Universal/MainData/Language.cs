using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
	public class Language
	{
		public int LanguageID { get; set; }
		public string LanguageName { get; set; }

		public ICollection<Declaration> Declarations { get; set; }
		public ICollection<Product> Products { get; set; }
		public ICollection<Seo> Seos { get; set; }
		public ICollection<Categories> Categories { get; set; }
		public ICollection<Tag> Tags { get; set; }
		public ICollection<SiteContent> SiteContents { get; set; }
	}
}