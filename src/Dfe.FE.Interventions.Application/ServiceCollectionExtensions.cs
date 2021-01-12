using Dfe.FE.Interventions.Application.FeProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.FE.Interventions.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFeInterventionsManagers(this IServiceCollection services)
        {
            services.AddScoped<IFeProviderManager, FeProviderManager>();
        }
    }
}