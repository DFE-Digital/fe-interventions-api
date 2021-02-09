using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Application.Learners;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Microsoft.Extensions.DependencyInjection;

namespace Dfe.FE.Interventions.Application
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFeInterventionsManagers(this IServiceCollection services)
        {
            services.AddScoped<IFeProviderManager, FeProviderManager>();
            services.AddScoped<ILearnerManager, LearnerManager>();
            services.AddScoped<ILearningDeliveryManager, LearningDeliveryManager>();
        }
    }
}