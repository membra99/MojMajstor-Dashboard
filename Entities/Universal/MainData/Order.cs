using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UsersId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? OrderStatus { get; set; }

        public Users Users { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
