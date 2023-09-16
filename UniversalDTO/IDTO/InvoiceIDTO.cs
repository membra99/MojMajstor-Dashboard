using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
	public class InvoiceIDTO
	{
		public string htmltable { get; set; }
		public string Dateofpayment { get; set; }
		public int TotalPrice { get; set; }
		public int OrderNumber { get; set; }
		public string BuyerName { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
	}

	public class InvoiceEntitiesIDTO
	{
		public int InvoiceId { get; set; }
		public string PdfName { get; set; }
		public DateTime DateOfPayment { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}
