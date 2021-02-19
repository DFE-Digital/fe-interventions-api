using System.Net.Http;
using AutoMapper;
using Dfe.FE.Interventions.Api.Infrastructure.Middleware;
using Dfe.FE.Interventions.Application;
using Dfe.FE.Interventions.Data;
using Dfe.FE.Interventions.Data.FeProviders;
using Dfe.FE.Interventions.Data.Learners;
using Dfe.FE.Interventions.Data.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Configuration;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Locations;
using Dfe.FE.Interventions.Infrastructure.PostcodesIo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        
        public Startup(IConfiguration configuration)
        {
            _configuration = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .AddEnvironmentVariables()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Setup controllers
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("api-version"));
                
                options.DefaultApiVersion = ApiVersion.Parse("1.0");
                options.AssumeDefaultVersionWhenUnspecified = true;

                // TODO: Use standard format for api exceptions
            });

            // Setup config
            services.AddOptions();
            services.Configure<DataStoreConfiguration>(_configuration.GetSection("DataStore"));
            
            // Setup logging + telemetry
            services.AddApplicationInsightsTelemetry();
            
            // Add HTTP Client
            services.AddHttpClient();

            // Setup database
            services.AddDbContext<FeInterventionsDbContext>();
            services.AddScoped<IFeInterventionsDbContext, FeInterventionsDbContext>();
            services.AddScoped<IFeProviderRepository, FeProviderRepository>();
            services.AddScoped<ILearnerRepository, LearnerRepository>();
            services.AddScoped<ILearningDeliveryRepository, LearningDeliveryRepository>();
            
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

            // Setup application layer
            services.AddFeInterventionsManagers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseLoggingCorrelation();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}