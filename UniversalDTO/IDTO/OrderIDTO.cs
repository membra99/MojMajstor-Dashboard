using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class OrderIDTO
    {
        public int OrderId { get; set; }
        public int UsersId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderStatus { get; set; }
    }
}
