using Demographic.Infrastructure.Servieces.Contracts;
using Demographic.Infrastructure.Servieces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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

            //Congiguring Health Ckeck
            services.ConfigureHealthChecks(configuration);

            return services;
        }

        public static void ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddSqlServer(configuration["ConnectionStrings:DefaultConnection"], healthQuery: "select 1", name: "SQL Server", failureStatus: HealthStatus.Unhealthy, tags: new[] { "Feedback", "Database" });

            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                opt.AddHealthCheckEndpoint("feedback api", "/api/health"); //map health check api    

            })
                .AddInMemoryStorage();
        }


    }
}
