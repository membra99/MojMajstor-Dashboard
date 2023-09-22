using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class MediaIDTO
    {
        public int MediaId { get; set; }
        public int ProductId { get; set; }
        public int MediaTypeId { get; set; }
        public string? Src { get; set; }
        public string? Extension { get; set; }
		public string? MetaTitle { get; set; }
		public string? MetaDescription { get; set; }
		public string? AltTitle { get; set; }
	}
}
