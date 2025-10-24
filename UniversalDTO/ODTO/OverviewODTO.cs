using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class OverviewODTO
    {
        public OverviewKorisnici OverviewKorisnici { get; set; }
        public OverviewOglasi OverviewOglasi { get; set; }
        public int UkupanPrihod { get; set; }
        public int BrojAktivnihDogovora { get; set; }
        public string? TopLokacija {  get; set; }
        public string? NajuspesnijaProfesija7Dana { get; set; }
        public int NoviOglasi7Dana { get; set; }
        public int NoviKorisnici7Dana { get; set; }
    }

    public class OverviewKorisnici
    {
        public int BrojMajstora { get; set; }
        public int BrojKlijenata { get; set; }
    }

    public class OverviewOglasi
    {
        public int UkupanBrOglasa { get; set; }
        public int AktivniOglasi { get; set; }
        public int PremiumOglasi { get; set; }
        public int IstaknutiOglasi { get; set; }
        public int EkonomicniOglasi { get; set; }
    }
}
