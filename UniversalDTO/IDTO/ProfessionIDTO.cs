using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Universal.DTO.IDTO
{
    public class ProfessionIDTO
    {
        public int ProfessionId { get; set; }

        [Required(ErrorMessage = "Profession name is required")]
        public string ProfessionName { get; set; }

        public string? Translations { get; set; }

        // For editing a single language translation
        public string? SelectedLanguage { get; set; }
        public string? TranslationValue { get; set; }
    }
}
