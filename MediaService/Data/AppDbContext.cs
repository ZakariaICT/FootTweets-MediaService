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
            optionsBuilder.UseNpgsql("Host=mediaservice-postgres;Database=mydatabase;Username=myuser;Password=mypassword;");
        }


        // Remove the OnConfiguring method

    }
}
