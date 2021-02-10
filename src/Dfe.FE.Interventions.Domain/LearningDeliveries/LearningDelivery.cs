using System;

namespace Dfe.FE.Interventions.Domain.LearningDeliveries
{
    public class LearningDelivery
    {
        public Guid Id { get; set; }
        public Guid LearnerId { get; set; }
        public int? AimType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int? FundingModel { get; set; }
        public int? StandardCode { get; set; }
        public int? CompletionStatus { get; set; }
        public int? Outcome { get; set; }
        public string OutcomeGrade { get; set; }
        public int? WithdrawalReason { get; set; }
        public string DeliveryLocationPostcode { get; set; }
    }
}