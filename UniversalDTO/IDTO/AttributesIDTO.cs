using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class AttributesIDTO
    {
        public int AttributesId { get; set; }
        public string? Value { get; set; }
        public int? CategoriesId { get; set; }
    }
}