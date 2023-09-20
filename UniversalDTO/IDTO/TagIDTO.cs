using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
	public class TagIDTO
	{
		public int TagId { get; set; }
		public int? MediaId { get; set; }
		public int? LanguageID { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public IFormFile? TagImage { get; set; }
		public string? Photo { get; set; }
	}
}
