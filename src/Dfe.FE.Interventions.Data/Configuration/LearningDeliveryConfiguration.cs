using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.FE.Interventions.Data.Configuration
{
    public class LearningDeliveryConfiguration : IEntityTypeConfiguration<LearningDelivery>
    {
        public void Configure(EntityTypeBuilder<LearningDelivery> builder)
        {
            builder
                .ToTable("LearningDelivery")
                .HasKey(x => x.Id);
        }
    }
}