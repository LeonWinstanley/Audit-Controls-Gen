using FinalYearProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> Options)
            : base(Options)
        { }
        public DbSet<Control> Controls { get; set; }
        public DbSet<ControlEvaluationControls> ControlEvaluationControls { get; set; }
        public DbSet<ControlEvaluations> ControlEvaluations { get; set; }
        
    }
}