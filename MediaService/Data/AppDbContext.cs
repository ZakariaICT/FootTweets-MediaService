using MediaService.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace MediaService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Pictures> pictures { get; set; } // Updated to PascalCase for better convention


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=postgres-service-media;Database=mydatabase;Username=myuser;Password=mypassword;");
        }


        // Remove the OnConfiguring method

    }
}
