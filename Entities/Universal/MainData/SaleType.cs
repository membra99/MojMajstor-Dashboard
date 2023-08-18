using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class SaleType
    {
        public int SaleTypeId { get; set; }
        public string Value { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}
