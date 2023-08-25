using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class ProductAttributesIDTO
    {
        public int ProductAttributeId { get; set; }
        public int? ProductId { get; set; }
        public bool IsDev { get; set; }
        public int? AttributesId { get; set; }
    }
}