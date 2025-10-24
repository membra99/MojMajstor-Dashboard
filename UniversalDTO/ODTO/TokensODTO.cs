using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class TokensODTO
    {
        public int TokenId { get; set; }

        public int NumberOfToken { get; set; }

        public double Price { get; set; }

        public string Package { get; set; } = null!;

        public string Description { get; set; } = null!;

        public double OldPrice { get; set; }

        public bool IsRecommended { get; set; }
    }
}
