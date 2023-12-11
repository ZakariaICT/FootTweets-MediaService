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

        // Remove the OnConfiguring method

    }
}
