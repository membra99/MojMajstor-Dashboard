using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Universal.MainData
{
    public class Users
    {
        public int UsersId { get; set; }
        public int? MediaId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string City { get; set; }
        public string? Zip { get; set; }
        public string Country { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? UserTypes { get; set; }
        public bool IsActive { get; set; }

        public Media? Media { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}