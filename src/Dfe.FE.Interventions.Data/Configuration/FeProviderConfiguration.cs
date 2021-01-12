using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.FE.Interventions.Data.Configuration
{
    public class FeProviderConfiguration : IEntityTypeConfiguration<FeProvider>
    {
        public void Configure(EntityTypeBuilder<FeProvider> builder)
        {
            builder
                .ToTable("FeProvider")
                .HasKey(x=>x.Ukprn);
        }
    }
}