using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
	public class PromoCodes
	{
		public int PromoCodesId { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public string PromoCode { get; set; }
		public string PromoCodeValue { get; set; }
		public string Message { get; set; }
	}
}
