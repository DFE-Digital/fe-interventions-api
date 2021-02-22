using AutoMapper;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData.MappingProfiles
{
    public class SldLearningDeliveryMapping : Profile
    {
        public SldLearningDeliveryMapping()
        {
            // Properties of same name (i.e. AimType, Outcome, etc) will automatically map, so excluded
            CreateMap<Sld.LearningDelivery, Domain.LearningDeliveries.LearningDelivery>()
                .ForMember(src => src.StartDate, opts => opts.MapFrom(dst => dst.LearnStartDate))
                .ForMember(src => src.PlannedEndDate, opts => opts.MapFrom(dst => dst.LearnPlanEndDate))
                .ForMember(src => src.ActualEndDate, opts => opts.MapFrom(dst => dst.LearnActEndDate))
                .ForMember(src => src.FundingModel, opts => opts.MapFrom(dst => dst.FundModel))
                .ForMember(src => src.StandardCode, opts => opts.MapFrom(dst => dst.StdCode))
                .ForMember(src => src.CompletionStatus, opts => opts.MapFrom(dst => dst.CompStatus))
                .ForMember(src => src.OutcomeGrade, opts => opts.MapFrom(dst => dst.OutGrade))
                .ForMember(src => src.WithdrawalReason, opts => opts.MapFrom(dst => dst.WithdrawReason))
                .ForMember(src => src.DeliveryLocationPostcode, opts => opts.MapFrom(dst => dst.DelLocPostCode))
                .ForMember(src => src.ProgrammeType, opts => opts.MapFrom(dst => dst.ProgType));
        }
    }
}