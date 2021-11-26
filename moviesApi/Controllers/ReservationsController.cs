using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using moviesApi.Data;
using moviesApi.Dto;
using moviesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace moviesApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private MoviesDbContext _moviesDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(MoviesDbContext moviesDbContext, IMapper mapper, ILogger<ReservationsController> logger)
        {
            _moviesDbContext = moviesDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> createReservation([FromForm] CreateReservationDto createReservationDto)
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " created a new reservation with movie Id " + createReservationDto.MovieId);

            var reservation = _mapper.Map<Reservation>(createReservationDto);
            createReservationDto.ReservationTime = DateTime.Now;
            await _moviesDbContext.Reservations.AddAsync(reservation);
            await _moviesDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, createReservationDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> updateReservation(int id, [FromForm] CreateReservationDto updateReservationDto)
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Updated reservation with id " + id);
            var reservationUpdate = await _moviesDbContext.Reservations.FindAsync(id);
            if (reservationUpdate == null)
            {
                return NotFound();
            }
            else
            {
                var movies = _mapper.Map<Reservation>(updateReservationDto);

                reservationUpdate.UserId = updateReservationDto.UserId;
                reservationUpdate.Quantity = updateReservationDto.Quantity;
                reservationUpdate.MovieId = updateReservationDto.MovieId;
                reservationUpdate.Phone = updateReservationDto.Phone;
                reservationUpdate.Watched = updateReservationDto.Watched;
                reservationUpdate.ReservationTime = updateReservationDto.ReservationTime;

                await _moviesDbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, updateReservationDto);
            }
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetReservarions()
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Get all reservations");
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
                Watched = reservation.Watched
            }).ToListAsync();
            return Ok(booking);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetReservarionsDetails(int id)
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Get user reservation details with id " + id);
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
        public async Task<IActionResult> getUserReservations(int id, bool watched)
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Get user reservation with id " + id);
            // using join
            var booking = await (from reservation in _moviesDbContext.Reservations
                           join customer in _moviesDbContext.Users on reservation.UserId equals
                           customer.Id

                           join movie in _moviesDbContext.Movies on reservation.MovieId equals movie.Id
                           where customer.Id == id
                                 where reservation.Watched == watched
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
                               watched = reservation.Watched,
                               MovieImage = movie.ImageUrl
                           }).ToListAsync();
            return Ok(booking.OrderBy(m => m.PlayingDate));
        }



        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Deleted a reservation with id "+ id);

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
