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
    }
}
