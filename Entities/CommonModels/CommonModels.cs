using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Universal.MainData;
using Microsoft.AspNetCore.Http;
using Universal.DTO.ODTO;

namespace Universal.DTO.CommonModels
{
    public class CommonModels
    {
        //authenticate models
        public class AuthenticateResponse
        {
            public int Id { get; set; }
            public string? FullName { get; set; }
            public string? Username { get; set; }
            public string Token { get; set; }

            public AuthenticateResponse(Universal.MainDataNova.User user, string token)
            {
                Id = user.UsersId;
                FullName = user.FullName;
                Username = user.Email;
                Token = token;
            }
        }

        public class AuthenticateRequest
        {
            [Required]
            public string? Username { get; set; }

            [Required]
            public string? Password { get; set; }
        }

        public class ResponseAuthModel
        {
            public int? id { get; set; }
            public string? firstName { get; set; }
            public string? lastName { get; set; }
            public string? username { get; set; }
            public string? token { get; set; }
        }

        //AWS models
        public class AWSFileUpload
        {
            public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
        }

        public class ServiceConfiguration
        {
            public AWSS3Configuration AWSS3 { get; set; }
        }

        public class AWSS3Configuration
        {
            public string BucketName { get; set; }
            public string BucketURL { get; set; }
        }

        public class CategoriesWithAttributes
        {
            public List<CategoriesODTO> CategoriesODTOs { get; set; } = new List<CategoriesODTO>();
            public List<AttributeODTO> AttributeODTOs { get; set; } = new List<AttributeODTO>();
        }
    }
}