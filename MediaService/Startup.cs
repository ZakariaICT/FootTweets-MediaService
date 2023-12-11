using MediaService.Data;
using MediaService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MediaService
{
    // Startup.cs
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Other service configurations...

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IMediaRepo, MediaRepo>();

            // Other service registrations...
        }




        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure middleware...
        }
    }

}
