using Dfe.FE.Interventions.Domain.Learners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dfe.FE.Interventions.Data.Configuration
{
    public class LearnerConfiguration : IEntityTypeConfiguration<Learner>
    {
        public void Configure(EntityTypeBuilder<Learner> builder)
        {
            builder
                .ToTable("Learner")
                .HasKey(x => x.Id);
        }
    }
}