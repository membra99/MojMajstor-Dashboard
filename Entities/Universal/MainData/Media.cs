using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class Media
    {
        public int MediaId { get; set; }
        public int? ProductId { get; set; }
        public int MediaTypeId { get; set; }
        public string? Src { get; set; }
        public string? Extension { get; set; }

        public MediaType MediaType { get; set; }
        public Product? Product { get; set; }
        public ICollection<Users> Users { get; set; }
    }
}
