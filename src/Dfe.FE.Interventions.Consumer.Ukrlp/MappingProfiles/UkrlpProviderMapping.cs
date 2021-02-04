using AutoMapper;
using Dfe.FE.Interventions.Consumer.Ukrlp.Ukrlp;
using Dfe.FE.Interventions.Domain.FeProviders;

namespace Dfe.FE.Interventions.Consumer.Ukrlp.MappingProfiles
{
    public class UkrlpProviderMapping : Profile
    {
        public UkrlpProviderMapping()
        {
            CreateMap<Provider, FeProvider>();
        }
    }
}