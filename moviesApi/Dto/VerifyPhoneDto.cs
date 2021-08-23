using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Dto
{
    public class VerifyPhoneDto
    {
        [Required]
        public string Phone { get; set; }

        [Required]
        public string confirmCode { get; set; }


    }
}
