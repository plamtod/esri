using Demographic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demographic.Infrastructure
{
    public sealed class PopulationContext(DbContextOptions<PopulationContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PopulationContext).Assembly);
            base.OnModelCreating(modelBuilder); 
        }

        public DbSet<Population> Populations { get; set; }
    }
}
