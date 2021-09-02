using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int Quantity { get; set; }
        public string Phone { get; set; }

        public DateTime ReservationTime { get; set; }

        public bool Watched { get; set; }

        public int MovieId { get; set; }

        public int UserId { get; set; }
    }
}
