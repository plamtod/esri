using Demographic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demographic.Infrastructure.Configurations
{
    internal class PopulationConfiguration : IEntityTypeConfiguration<Population>
    {
        public void Configure(EntityTypeBuilder<Population> builder)
        {
            builder.ToTable("Populations", "dbo");

            builder
                .HasKey(p => p.Id);

            builder
                .Property(b => b.StateName)
                .HasMaxLength(100)
                .IsRequired();
            builder
                .Property(b => b.PopulationCount)
                .HasColumnName("Count")
                .IsRequired();

            // Define an index on the StateName field
            builder.HasIndex(e => e.StateName).HasDatabaseName("IX_StateName");

            builder
                .Property(b => b.PopulationCount)
                .IsRequired();
        }
    }
}
