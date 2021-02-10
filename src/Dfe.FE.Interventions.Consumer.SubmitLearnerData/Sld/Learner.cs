using System;

namespace Dfe.FE.Interventions.Consumer.SubmitLearnerData.Sld
{
    public class Learner
    {
        public int Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public long Uln { get; set; }
        public string FamilyName { get; set; }
        public string GivenNames { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string NiNumber { get; set; }
        public LearningDelivery[] LearningDeliveries { get; set; }
    }
}