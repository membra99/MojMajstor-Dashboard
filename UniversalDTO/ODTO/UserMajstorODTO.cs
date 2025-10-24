using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ODTO
{
    public class UserMajstorODTO
    {
        public int UsersId { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public int RoleId { get; set; }
        public string Name { get; set; }
        public int? OpstineId { get; set; }
        public string? OpstinaIme { get; set; }
        public string? Professions { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public string? PhoneCode { get; set; }
        public bool IsVisible { get; set; }
        public string? UserToken { get; set; }
        public string ReferalCode { get; set; } = null!;
        public DateTime RegistrationDate { get; set; }
        
    }
}
