using Congratulate.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Congratulate.Infrastructure.Data
{
    public class BirthdayContext : DbContext
    {
        public BirthdayContext(DbContextOptions<BirthdayContext> options) : base(options) { }

        public DbSet<Person> Persons { get; set; }
    }
} 