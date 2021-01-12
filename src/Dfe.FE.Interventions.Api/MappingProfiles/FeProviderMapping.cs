using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Domain.FeProviders;

namespace Dfe.FE.Interventions.Api.MappingProfiles
{
    public class FeProviderMapping : Profile
    {
        public FeProviderMapping()
        {
            CreateMap<FeProvider, ApiFeProvider>();
        }
    }
}