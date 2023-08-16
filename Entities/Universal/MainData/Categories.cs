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
        public string? CategoryName { get; set; }
        public bool IsAttribute { get; set; }

        public ICollection<ProductAttributes> ProductAttributes { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
