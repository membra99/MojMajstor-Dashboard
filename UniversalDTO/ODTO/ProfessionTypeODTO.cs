using System;

namespace Universal.DTO.ODTO
{
    public class ProfessionTypeODTO
    {
        public int ProfessionTypeId { get; set; }
        public int ProfessionId { get; set; }
        public string ProfessionTypeName { get; set; }
        public string ProfessionName { get; set; }
        public string? Translations { get; set; }
    }
}
