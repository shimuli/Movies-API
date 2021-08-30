﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using moviesApi.Data;
using moviesApi.Dto;
using moviesApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private MoviesDbContext _moviesDbContext;
        string _baseUrl;

        private readonly IMapper _mapper;
        string userimageUrl = null;

        public MoviesController(MoviesDbContext moviesDbContext, IHttpContextAccessor context, IMapper mapper)
        {
            _moviesDbContext = moviesDbContext;
            // for url
            var request = context.HttpContext.Request;
            _baseUrl = $"{request.Scheme}://{request.Host}";
            _mapper = mapper;
        }

        // POST api/<MoviesController>
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<IActionResult> addMovie([FromForm] CreateMoviesDto movieDto)
        {
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");
            if (movieDto.Image != null)
            {
                var filestream = new FileStream(filePath, FileMode.Create);
                movieDto.Image.CopyTo(filestream);
                movieDto.ImageUrl = _baseUrl + filePath.Remove(0, 7);
            }

          
            var movies = _mapper.Map<Movie>(movieDto);           
            await _moviesDbContext.Movies.AddAsync(movies);
            await _moviesDbContext.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, movieDto);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> updateMovie(int id, [FromForm] CreateMoviesDto updateMovieDto)
        {
            var movieupdate = await _moviesDbContext.Movies.FindAsync(id);
            if (movieupdate == null)
            {
                return NotFound();
            }
            else
            {
                var movies = _mapper.Map<Movie>(updateMovieDto);
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", guid + ".jpg");
                if (updateMovieDto.Image != null)
                {
                    var filestream = new FileStream(filePath, FileMode.Create);
                    updateMovieDto.Image.CopyTo(filestream);
                    movieupdate.ImageUrl = _baseUrl + filePath.Remove(0, 9);
                }

                movieupdate.Name = updateMovieDto.Name;
                movieupdate.Duration = updateMovieDto.Duration;
                movieupdate.Description = updateMovieDto.Description;
                movieupdate.PlayingDate = updateMovieDto.PlayingDate;
                movieupdate.PlayingTIme = updateMovieDto.PlayingTIme;
                movieupdate.Language = updateMovieDto.Language;
                movieupdate.Rating = updateMovieDto.Rating;
                movieupdate.Genre = updateMovieDto.Genre;
                movieupdate.TrailerUrl = updateMovieDto.TrailerUrl;
                movieupdate.TicketPrice = updateMovieDto.TicketPrice;
                await _moviesDbContext.SaveChangesAsync();
                return StatusCode(StatusCodes.Status201Created, updateMovieDto);
            }
        }

         [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _moviesDbContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            _moviesDbContext.Movies.Remove(movie);
            _moviesDbContext.SaveChanges();
            return Ok("Deleted succefully");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AllMovies(string sort, int? pageNumber, int? pageSize)
        {
            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 5;
            var allmovies = await (from movies in _moviesDbContext.Movies
            select new
            {
                Id = movies.Id,
                Name = movies.Name,
                Duration = movies.Duration,
                Language = movies.Language,
                Rating = movies.Rating,
                Genre = movies.Genre,
                Imageurl = movies.ImageUrl
            }).ToListAsync();
            switch (sort)
            {
                case "desc":
                    return Ok(allmovies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));

                case "asc":
                    return Ok(allmovies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));

                default:
                    return Ok(allmovies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
            }
           
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> getMovieDetails(int id)
        {
            var movie = await _moviesDbContext.Movies.FindAsync(id);
            if(movie == null)
            {
                return NotFound(); // MoviesDto
            }

            var movieDto = _mapper.Map<MoviesDto>(movie);
            return Ok(movieDto);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> searchMovie(string query)
        {
            var movies = await (from movie in _moviesDbContext.Movies
                               where movie.Name.StartsWith(query)
                               select new
                               {
                                   Id = movie.Id,
                                   Name = movie.Name,
                                   Duration = movie.Duration,
                                   Language = movie.Language,
                                   Rating = movie.Rating,
                                   Genre = movie.Genre,
                                   Imageurl = movie.ImageUrl

                               }).ToListAsync();
            return Ok(movies);

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Todaymovies()
        {
            var movies = await (from movie in _moviesDbContext.Movies
                                where movie.PlayingDate == DateTime.Today
                                select new
                               {
                                    Id = movie.Id,
                                    Name = movie.Name,
                                    Duration = movie.Duration,
                                    Language = movie.Language,
                                    Rating = movie.Rating,
                                    Genre = movie.Genre,
                                    Imageurl = movie.ImageUrl
                                }).Take(4).ToListAsync();
            return Ok(movies);
        }

    }
}
