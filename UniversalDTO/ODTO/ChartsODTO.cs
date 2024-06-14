using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class ChartsODTO
    {
        public List<int> SumByYear { get; set; }
        public int SumRegistredUser { get; set; }
        public int SumOrders { get; set; }
        public int TotalByYear { get; set; }
        public int TotalProductsDelivered { get; set; }
    }
}
