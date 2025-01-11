using Demographic.Infrastructure.Servieces.Contracts;
using Demographic.Infrastructure.Servieces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace Demographic.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection RegisterInfrastructure(this IServiceCollection services, IConfiguration configuration) 
        { 
            services.AddDbContext<PopulationContext>(opt =>
                opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
            );

            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddScoped<IGeoService, GeoService>();
            services.AddScoped<IPopulationService, PopulationService>();
            

            return services;
        }
    }
}
