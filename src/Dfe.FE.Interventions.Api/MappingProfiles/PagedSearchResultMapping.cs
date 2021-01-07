using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;

namespace Dfe.FE.Interventions.Api.MappingProfiles
{
    public class PagedSearchResultMapping : Profile
    {
        public PagedSearchResultMapping()
        {
            CreateMap<PagedSearchResult<FeProviderSynopsis>, ApiPagedSearchResult<FeProviderSynopsis>>();
        }
    }
}