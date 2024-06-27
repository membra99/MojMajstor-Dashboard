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
        public List<TopProductsChartODTO> TopProductsCharts { get; set; }
        public BarChartODTO barChartODTO { get; set; }
    }

    public class TopProductsChartODTO
    {
        public string ProductsName { get; set; }
        public int ProductOrders { get; set; }
        public double TotalOrdersPercentage { get; set; }
    }

    public class BarChartODTO
    {
        public List<string> Years { get; set; }
        public List<int> Values { get; set; }
    }
}
