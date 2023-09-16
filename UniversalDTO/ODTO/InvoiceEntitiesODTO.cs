using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
	public class InvoiceEntitiesODTO
	{
		public int InvoiceId { get; set; }
		public int MediaId { get; set; }
		public string Src { get; set; }
		public string PdfName { get; set; }
		public DateTime DateOfPayment { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
