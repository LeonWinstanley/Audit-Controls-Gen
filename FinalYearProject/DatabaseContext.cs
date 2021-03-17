using FinalYearProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }
        
        public DbSet<Controls> Controls { get; set; }
    }
}