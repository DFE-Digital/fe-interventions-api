using System;
using Dfe.FE.Interventions.Data.Configuration;
using Dfe.FE.Interventions.Domain.Configuration;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace Dfe.FE.Interventions.Data
{
    public interface IFeInterventionsDbContext
    {
        DbSet<FeProvider> FeProviders { get; }
    }
    
    public class FeInterventionsDbContext : DbContext, IFeInterventionsDbContext
    {
        private DataStoreConfiguration _config;

        public FeInterventionsDbContext(
            IOptions<DataStoreConfiguration> config,
            DbContextOptions<FeInterventionsDbContext> options)
            : base(options)
        {
            _config = config.Value;
        }
        
        public DbSet<FeProvider> FeProviders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FeProviderConfiguration());
        }
    }
}