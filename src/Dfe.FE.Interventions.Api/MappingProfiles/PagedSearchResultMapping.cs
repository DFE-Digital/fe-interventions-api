using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.LearningDeliveries;

namespace Dfe.FE.Interventions.Api.MappingProfiles
{
    public class PagedSearchResultMapping : Profile
    {
        public PagedSearchResultMapping()
        {
            CreateMap<PagedSearchResult<FeProviderSynopsis>, ApiPagedSearchResult<FeProviderSynopsis>>();
            CreateMap<PagedSearchResult<LearningDeliverySynopsis>, ApiPagedSearchResult<LearningDeliverySynopsis>>();
        }
    }
}