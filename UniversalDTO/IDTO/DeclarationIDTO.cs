using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class DeclarationIDTO
    {
        public int DeclarationId { get; set; }
        public string DeclarationName { get; set; }
        public string Model { get; set; }
        public string NameAndTypeOfProduct { get; set; }
        public string Distributor { get; set; }
        public string CountryOfOrigin { get; set; }
        public string ConsumerRights { get; set; }
    }
}
