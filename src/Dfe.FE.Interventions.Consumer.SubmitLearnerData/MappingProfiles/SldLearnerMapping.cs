using AutoMapper;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData.MappingProfiles
{
    public class SldLearnerMapping : Profile
    {
        public SldLearnerMapping()
        {
            // Properties of same name (i.e. Ukprn, LearnRefNumber, etc) will automatically map, so excluded
            CreateMap<Sld.Learner, Domain.Learners.Learner>()
                .ForMember(src => src.FirstNames, opts => opts.MapFrom(dst => dst.GivenNames))
                .ForMember(src => src.LastName, opts => opts.MapFrom(dst => dst.FamilyName))
                .ForMember(src => src.NationalInsuranceNumber, opts => opts.MapFrom(dst => dst.NiNumber));
        }
    }
}