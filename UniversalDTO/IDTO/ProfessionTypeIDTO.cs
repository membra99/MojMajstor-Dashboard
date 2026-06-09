using System;
using System.ComponentModel.DataAnnotations;

namespace Universal.DTO.IDTO
{
    public class ProfessionTypeIDTO
    {
        public int ProfessionTypeId { get; set; }

        [Required(ErrorMessage = "Profession is required")]
        public int ProfessionId { get; set; }

        [Required(ErrorMessage = "Sub-profession name is required")]
        public string ProfessionTypeName { get; set; }

        public string? Translations { get; set; }

        // For editing a single language translation
        public string? SelectedLanguage { get; set; }
        public string? TranslationValue { get; set; }
    }
}
