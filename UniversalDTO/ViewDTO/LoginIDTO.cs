using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal.DTO.ViewDTO
{
    public class LoginIDTO
    {
        [Required(ErrorMessage = "Email is a required field!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is a required field!")]
        public string Password { get; set; }
    }
}
