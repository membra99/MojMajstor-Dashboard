using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class UserMajstorIDTO
    {
        public int UsersId { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? FullName { get; set; }

        public int RoleId { get; set; }

        public int? OpstineId { get; set; }

        public string? OpstinaIme { get; set; }

        public string? Professions { get; set; }

        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public string? PhoneCode { get; set; }

        public bool IsVisible { get; set; }

        public string? UserToken { get; set; }

        public string? ReferalCode { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int? UkupanBrOglasa { get; set; }

        public int? TrenutnoAktivniOglasi { get; set; }

        public int? BrojPostignutihDogovora { get; set; }

        public int? BrojNepostignutihDogovora { get; set; }

        public int? BrojKorisnikaPrekoReferala { get; set; }

        public BrojKupljenihPateka? BrojKupljenihPateka { get; set; }

        public List<OpstineDropDown>? OpstineDropDowns { get; set; }

        public List<int> SelectedProfessionIds { get; set; } = new();

        public List<ProfessionDropDown>? ProfessionDropDowns { get; set; } 
    }

    public class OpstineDropDown
    {
        public int OpstineId { get; set; }
        public string OpstineIme { get; set; }
    }

    public class ProfessionDropDown
    {
        public int ProfessionId { get; set; }
        public string ProfessionName { get; set; }
    }

    public class BrojKupljenihPateka
    {
        public int S { get; set; }
        public int M { get; set; }
        public int L { get; set; }
        public int XL { get; set; }
    }
}
