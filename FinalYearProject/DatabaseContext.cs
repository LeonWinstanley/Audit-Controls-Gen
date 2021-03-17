using FinalYearProject.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalYearProject
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> Options)
            : base(Options)
        { }
        
        public DbSet<Control> Controls { get; set; }
        
        public DbSet<ControlEvaluationControls> ControlEvaluationControls { get; set; }
        
        public DbSet<ControlEvaluations> ControlEvaluations { get; set; }
        
        public DbSet<User> Users { get; set; }
        
    }
}