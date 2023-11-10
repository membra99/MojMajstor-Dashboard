using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class SaleIDTO
    {
        public int SaleId { get; set; }
        public int SaleTypeId { get; set; }
        public int ProductId { get; set; }
        public string? Value { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
