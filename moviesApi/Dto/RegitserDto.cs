using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Dto
{
    public class RegitserDto
    {

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        public string Role { get; set; }

        public IFormFile UserImage { get; set; }

        public string ImageUrl { get; set; }

       /* public string Verificationtoken { get; set; }*/

    }
}
