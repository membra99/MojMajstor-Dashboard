using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
	public class DeclarationODTO
	{
		public int DeclarationId { get; set; }
		public string DeclarationName { get; set; }
		public string Model { get; set; }
		public string NameAndTypeOfProduct { get; set; }
		public string Distributor { get; set; }
		public string CountryOfOrigin { get; set; }
		public string ConsumerRights { get; set; }
		public int? LanguageID { get; set; }
		public string? LanguageName { get; set; }
	}
}