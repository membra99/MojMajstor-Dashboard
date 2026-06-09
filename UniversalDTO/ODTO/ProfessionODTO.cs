using System;
using System.Collections.Generic;

namespace Universal.DTO.ODTO
{
    public class ProfessionODTO
    {
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
        public string? Translations { get; set; }
        public List<ProfessionTypeODTO> ProfessionTypes { get; set; }
    }
}
