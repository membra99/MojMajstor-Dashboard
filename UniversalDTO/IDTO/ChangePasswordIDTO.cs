using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
	public class ChangePasswordIDTO
	{
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
		public string Key { get; set; }
	}
}
