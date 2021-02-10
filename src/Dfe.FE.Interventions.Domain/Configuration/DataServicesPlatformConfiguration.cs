namespace Dfe.FE.Interventions.Domain.Configuration
{
    public class DataServicesPlatformConfiguration
    {
        public string KafkaBrokers { get; set; }
        public string UkrlpTopicName { get; set; }
        public string UkrlpGroupId { get; set; }
        public string SubmitLearnerDataTopicName { get; set; }
        public string SubmitLearnerDataGroupId { get; set; }
    }
}