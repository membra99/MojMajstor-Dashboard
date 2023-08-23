using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class OrderDetailsIDTO
    {
        public int OrderDetailsId { get; set; }
        public List<int> ProductList{ get; set; }
        public UsersIDTO? UsersIDTO { get; set; }
    }
}
