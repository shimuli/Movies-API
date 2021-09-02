using AutoMapper;
using moviesApi.Dto;
using moviesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Mappers
{
    public class CinemaMapping : Profile
    {
        public CinemaMapping()
        {
            CreateMap<User, RegitserDto>().ReverseMap();
            CreateMap<User, LoginDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, VerifyPhoneDto>().ReverseMap();

            CreateMap<Movie, CreateMoviesDto>().ReverseMap();
            CreateMap<Movie, MoviesDto>().ReverseMap();

            CreateMap<Reservation, CreateReservationDto>().ReverseMap();
            CreateMap<Reservation, UpdateWatchedMovieDto>().ReverseMap();
        }
    }
}
