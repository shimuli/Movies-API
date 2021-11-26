using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Dto
{
    public class MoviesDto
    {
        public int Id { get; set; }

        public string MovieName { get; set; }

        public string Description { get; set; }
        public string Language { get; set; }

        public string Duration { get; set; }

        public DateTime PlayingDate { get; set; }

        public DateTime PlayingTIme { get; set; }

        public double TicketPrice { get; set; }

        public double Rating { get; set; }

        public string Genre { get; set; }

        public string TrailerUrl { get; set; }


        public string ImageUrl { get; set; }
    }
}
