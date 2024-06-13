using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
	public class PromocodesODTO
	{
		public int PromoCodesId { get; set; }
		public string? StartDate { get; set; }
		public string? EndDate { get; set; }
		public string? CreatedAt { get; set; }
		public string PromoCode { get; set; }
		public string PromoCodeValue { get; set; }
		public string Message { get; set; }
		public bool IsActive { get; set; }
	}
}
