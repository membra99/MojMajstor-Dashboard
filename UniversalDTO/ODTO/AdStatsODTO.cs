using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class AdStatsODTO
    {
        public int TotalAds { get; set; }          
        public int Aktivni { get; set; }         
        public int Istaknuti { get; set; }       
        public int XL { get; set; }       

        public List<string> Professions { get; set; } = new List<string>();
        public List<int> ViewsByProfession { get; set; } = new List<int>();

        public List<string> PeriodLabels { get; set; } = new List<string>();
        public List<int> ViewsByPeriod { get; set; } = new List<int>();
    }
}
