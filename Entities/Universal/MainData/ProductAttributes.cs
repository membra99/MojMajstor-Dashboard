using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class ProductAttributes
    {
        public int ProductAttributeId { get; set; }
        public int? CategoryId { get; set; }
        public int ProductId { get; set; }
        public bool IsDev { get; set; }
        public string? Value { get; set; }

        public Categories Categories { get; set; }
        public Product Product { get; set; }
    }
}
