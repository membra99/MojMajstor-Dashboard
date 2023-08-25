using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class AttributesODTO
    {
        public int AttributesId { get; set; }
        public string? Value { get; set; }
        public int? CategoriesId { get; set; }
        public string? CategoryName { get; set; }
    }
}