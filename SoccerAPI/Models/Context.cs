using Microsoft.EntityFrameworkCore;

namespace SoccerAPI.Models
{
    public class Context : DbContext
    {
        public Context() { }

        public Context(DbContextOptions<Context> options) 
            : base(options) 
        {
        }

        public virtual DbSet<Club> Clubs { get; set; }
        public virtual DbSet<Coach> Coaches { get; set; }
        public virtual DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=.;Database=Soccer;Trusted_Connection=True;TrustServerCertificate=True;");
        }

      

    }
}
