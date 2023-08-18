using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ViewDTO
{
	public class UserIDTO
	{
		[Required]
		public int RoleId { get; set; }
		public List<(int Id, string Name)> Role { get; set; } = new List<(int Id, string Name)>() { (1, "User"), (2, "Admin") };
		[Required]
		public string Email { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
		public string LastName { get; set; }
		public string? Phone { get; set; }
		public string? Address { get; set; }
		public string? City { get; set; }
		public string? Zip { get; set; }
		public string? Country { get; set; }
		[Required]
		public string Password { get; set; }
		[Required]
		public IFormFile Avatar { get; set; }
	}
}
