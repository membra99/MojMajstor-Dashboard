using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class Attributes
    {
        public int AttributesId { get; set; }
        public string? Value { get; set; }
        public int? CategoriesId { get; set; }
        public Categories Categories { get; set; }
        public ICollection<ProductAttributes> ProductAttributes { get; set; }
    }
}