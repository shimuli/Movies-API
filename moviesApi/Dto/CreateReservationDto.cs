using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Dto
{
    public class CreateReservationDto
    {

        [Required]
        public int Quantity { get; set; }
        [Required]
        public string Phone { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime ReservationTime { get; set; }
    }
}
