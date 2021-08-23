using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using moviesApi.Data;
using moviesApi.Dto;
using moviesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private MoviesDbContext _moviesDbContext;
        private readonly IMapper _mapper;

        public ReservationsController(MoviesDbContext moviesDbContext, IMapper mapper)
        {
            _moviesDbContext = moviesDbContext;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> createReservation([FromForm] CreateReservationDto createReservationDto)
        {

            var reservation = _mapper.Map<Reservation>(createReservationDto);
            createReservationDto.ReservationTime = DateTime.Now;
            await _moviesDbContext.Reservations.AddAsync(reservation);
            await _moviesDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, createReservationDto);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetReservarions()
        {
            // using join
           var booking = await (from reservation in  _moviesDbContext.Reservations
            join customer in _moviesDbContext.Users on reservation.UserId equals
            customer.Id
            join movie in _moviesDbContext.Movies on reservation.MovieId equals movie.Id
            select new
            {
                Id = reservation.Id,
                MovieName = movie.Name,
                CusomerName = customer.Name,             
                ReservationTime = reservation.ReservationTime,
            }).ToListAsync();
            return Ok(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetReservarionsDetails(int id)
        {
            // using join
            var booking = await (from reservation in _moviesDbContext.Reservations
                          join customer in _moviesDbContext.Users on reservation.UserId equals
                          customer.Id
                          join movie in _moviesDbContext.Movies on reservation.MovieId equals movie.Id
                          where reservation.Id == id
                          select new
                          {
                              Id = reservation.Id,
                              MovieName = movie.Name,
                              CusomerName = customer.Name,
                              ReservationTime = reservation.ReservationTime,
                              Email = customer.Email,
                              Quantity = reservation.Quantity,
                              Price = movie.TicketPrice,
                              TotalCost = reservation.Quantity * movie.TicketPrice,
                              PlayingDate = movie.PlayingDate,
                              PlayTime = movie.PlayingTIme,
                          }).FirstOrDefaultAsync();
            return Ok(booking);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getUserReservations(int id)
        {
            // using join
            var booking = await (from reservation in _moviesDbContext.Reservations
                           join customer in _moviesDbContext.Users on reservation.UserId equals
                           customer.Id
                           join movie in _moviesDbContext.Movies on reservation.MovieId equals movie.Id
                           where customer.Id == id
                           select new
                           {
                               Id = reservation.Id,
                               MovieName = movie.Name,
                               CusomerName = customer.Name,
                               ReservationTime = reservation.ReservationTime,
                               Email = customer.Email,
                               Quantity = reservation.Quantity,
                               Price = movie.TicketPrice,
                               TotalCost = reservation.Quantity * movie.TicketPrice,
                               PlayingDate = movie.PlayingDate,
                               PlayTime = movie.PlayingTIme,
                           }).ToListAsync();
            return Ok(booking);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _moviesDbContext.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            _moviesDbContext.Reservations.Remove(reservation);
            _moviesDbContext.SaveChanges();
            return Ok("Deleted succefully");
        }
    }
}
