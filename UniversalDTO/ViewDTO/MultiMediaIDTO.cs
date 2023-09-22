using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Universal.DTO.IDTO;
using Universal.DTO.ODTO;

namespace Universal.DTO.ViewDTO
{
	public class MultiMediaIDTO
	{
		public MediaIDTO MediaIDTO { get; set; }
		public List<MediaODTO> MediaList { get; set; }
	}
}
