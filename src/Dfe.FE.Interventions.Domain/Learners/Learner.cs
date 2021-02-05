using System;

namespace Dfe.FE.Interventions.Domain.Learners
{
    public class Learner
    {
        public Guid Id { get; set; }
        public int Ukprn { get; set; }
        public string LearnRefNumber { get; set; }
        public int Uln { get; set; }
        public string FirstNames { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string NationalInsuranceNumber { get; set; }
    }
}