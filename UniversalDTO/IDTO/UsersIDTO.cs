using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.IDTO
{
    public class UsersIDTO
    {
        public int? UsersId { get; set; }
        public int? MediaId { get; set; }

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
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? UserType { get; set; }
        public string? UserTypes { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}