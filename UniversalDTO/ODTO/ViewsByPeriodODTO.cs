using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class ViewsByPeriodODTO
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<int> Counts { get; set; } = new List<int>();
    }
}
