using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using moviesApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace moviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private MoviesDbContext _moviesDbContext;
        private readonly ILogger<LogsController> _logger;

        public LogsController(MoviesDbContext moviesDbContext, ILogger<LogsController> logger)
        {
            _logger = logger;
            _moviesDbContext = moviesDbContext;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetLogs")]     
        public async Task<IActionResult> GetLogs(string sort, int? pageNumber, int? pageSize)
        {
            string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation(userEmail + " Get All logs");

            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 5;
            var logs = await (from log in _moviesDbContext.SystemLogs
                                   select new
                                   {
                                       log.Id,
                                       log.Message,
                                       log.TimeStamp,
                                       Action = log.Properties,
                                   }).ToListAsync();
            switch (sort)
            {
                case "desc":
                    return Ok(logs
                        .Skip((currentPageNumber - 1) * currentPageSize)
                        .Take(currentPageSize)
                        .OrderByDescending(m => m.TimeStamp));

                case "asc":
                    return Ok(logs.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.TimeStamp));

                default:
                    return Ok(logs.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
            }

        }

        //[Authorize(Roles = "Admin")]
        //[HttpDelete("DeleteLogs")]
        //public async Task<IActionResult> DeleteLogs(int id)
        //{
        //    string userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    _logger.LogInformation(userEmail + " Deleted a log with id " + id);

        //    var log = await _moviesDbContext.SystemLogs.FindAsync(id);
        //    if (log == null)
        //    {
        //        return NotFound();
        //    }
        //    _moviesDbContext.SystemLogs.Remove(log);
        //    _moviesDbContext.SaveChanges();
        //    return Ok("Deleted succefully");
        //}
    }
}
