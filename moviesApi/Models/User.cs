using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Models
{
    public class User
    {
       
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        
        public string Phone { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        [NotMapped]
        public IFormFile UserImage { get; set; }
        public string ImageUrl { get; set; }

        public string confirmCode { get; set; }

        public string Verificationtoken { get; set; }

        public bool EmailVerified { get; set; }

        public bool PhoneVerified { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }
}
