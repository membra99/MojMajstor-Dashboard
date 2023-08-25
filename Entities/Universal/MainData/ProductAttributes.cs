using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class ProductAttributes
    {
        public int ProductAttributeId { get; set; }
        public int? ProductId { get; set; }
        public bool IsDev { get; set; }
        public int? AttributesId { get; set; }

        public Product Product { get; set; }
        public Attributes Attributes { get; set; }
    }
}