using System.Net.Http;
using AutoMapper;
using Dfe.Edis.Kafka;
using Dfe.FE.Interventions.Application;
using Dfe.FE.Interventions.Data;
using Dfe.FE.Interventions.Data.FeProviders;
using Dfe.FE.Interventions.Domain.Configuration;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Locations;
using Dfe.FE.Interventions.Infrastructure.PostcodesIo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData
{
    public class Startup
    {
        private IConfigurationRoot _configuration;

        public Startup()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Setup config
            services.AddOptions();
            services.Configure<DataStoreConfiguration>(_configuration.GetSection("DataStore"));
            services.Configure<DataServicesPlatformConfiguration>(_configuration.GetSection("DataServicesPlatform"));
            
            // Add HTTP Client
            services.AddHttpClient();

            // Setup database
            services.AddDbContext<FeInterventionsDbContext>();
            services.AddScoped<IFeInterventionsDbContext, FeInterventionsDbContext>();
            services.AddScoped<IFeProviderRepository, FeProviderRepository>();
            
            // Add location service
            services.AddSingleton<ILocationService>(sp =>
            {
                var httpClientFactory = sp.GetService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient();

                var logger = sp.GetService<ILogger<PostcodesIoApiLocationService>>();

                return new PostcodesIoApiLocationService(httpClient, logger);
            });

            // Setup mapper
            services.AddAutoMapper(GetType().Assembly);
            
            // Setup Kafka
            services.AddSingleton(serviceProvider =>
            {
                var dataServicesPlatformConfig = serviceProvider.GetService<IOptions<DataServicesPlatformConfiguration>>();
                return new KafkaConsumerConfiguration
                {
                    BootstrapServers = dataServicesPlatformConfig.Value.KafkaBrokers,
                    GroupId = dataServicesPlatformConfig.Value.SubmitLearnerDataGroupId,
                };
            });
            services.AddKafkaConsumer();

            // Setup application layer
            services.AddFeInterventionsManagers();
            
            // Setup background services
            services.AddHostedService<Worker>();
        }
    }
}