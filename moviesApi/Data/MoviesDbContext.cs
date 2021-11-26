using Microsoft.EntityFrameworkCore;
using moviesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Data
{
    public class MoviesDbContext : DbContext
    {
        public MoviesDbContext(DbContextOptions<MoviesDbContext>options): base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Logs> SystemLogs { get; set; }

        public DbSet<Reservation> Reservations { get; set; }
    }
}
