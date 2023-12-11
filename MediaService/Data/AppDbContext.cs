using MediaService.Model;
using Microsoft.EntityFrameworkCore;

namespace MediaService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Pictures> pictures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Remove this line
            // optionsBuilder.UseSqlServer(@"Server=mssqlstud.fhict.local;Database=dbi469980_userdb;User Id=dbi469980_userdb;Password=Xtt4d-8HNK; TrustServerCertificate=True;");
        }


    }
}
