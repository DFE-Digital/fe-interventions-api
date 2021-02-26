using System;

namespace Dfe.FE.Interventions.Domain.LearningDeliveries
{
    public class LearningDeliverySynopsis
    {
        public Guid Id { get; set; }
        public int? FundingModel { get; set; }
        public string DeliveryLocationPostcode { get; set; }
        public string DeliveryLocationRegion { get; set; }
        public int? ProgrammeType { get; set; }
    }
}