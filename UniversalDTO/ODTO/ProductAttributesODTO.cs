using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class ProductAttributesODTO
    {
        public int ProductAttributeId { get; set; }
        public int? ProductId { get; set; }
        public bool IsDev { get; set; }
        public int? AttributesId { get; set; }
    }

    //public class AttributeODTO
    //{
    //    public int CategoryId { get; set; }
    //    public string CategoryName { get; set; }
    //    public List<string> Value { get; set; }
    //}
}