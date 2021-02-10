using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Data.Configuration;
using Dfe.FE.Interventions.Domain.Configuration;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Dfe.FE.Interventions.Data
{
    public interface IFeInterventionsDbContext
    {
        DbSet<FeProvider> FeProviders { get; }
        DbSet<Learner> Learners { get; }
        DbSet<LearningDelivery> LearningDeliveries { get; }

        Task<int> CommitAsync(CancellationToken cancellationToken);
        Task<int> ExecuteSqlAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken);
    }
    
    public class FeInterventionsDbContext : DbContext, IFeInterventionsDbContext
    {
        private readonly DataStoreConfiguration _config;

        public FeInterventionsDbContext(
            IOptions<DataStoreConfiguration> config,
            DbContextOptions<FeInterventionsDbContext> options)
            : base(options)
        {
            _config = config.Value;
        }
        
        public DbSet<FeProvider> FeProviders { get; set; }
        public DbSet<Learner> Learners { get; set; }
        public DbSet<LearningDelivery> LearningDeliveries { get; set; }

        public async Task<int> ExecuteSqlAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken)
        {
            return await Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }

        public async Task<int> CommitAsync(CancellationToken cancellationToken)
        {
            return await SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var useManagedServiceIdentity = !_config.ConnectionString.Contains("User ID=", StringComparison.InvariantCultureIgnoreCase) &&
                                            !_config.ConnectionString.Contains("Trusted_Connection=true", StringComparison.InvariantCultureIgnoreCase);
            if (useManagedServiceIdentity)
            {
                var connection = new SqlConnection(_config.ConnectionString);
                connection.AccessToken = (new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider())
                    .GetAccessTokenAsync("https://database.windows.net/").Result;
                optionsBuilder.UseSqlServer(connection);
            }
            else
            {
                optionsBuilder.UseSqlServer(_config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FeProviderConfiguration());
            modelBuilder.ApplyConfiguration(new LearnerConfiguration());
            modelBuilder.ApplyConfiguration(new LearningDeliveryConfiguration());
        }
    }
}