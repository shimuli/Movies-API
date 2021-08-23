using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Dto
{
    public class CreateMoviesDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public string Duration { get; set; }

        [Required]
        public DateTime PlayingDate { get; set; }

        [Required]
        public DateTime PlayingTIme { get; set; }

        [Required]
        public double TicketPrice { get; set; }

        [Required]
        public double Rating { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string TrailerUrl { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        public string ImageUrl { get; set; }

    }
}
