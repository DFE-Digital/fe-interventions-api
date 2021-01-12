namespace Dfe.FE.Interventions.Domain.FeProviders
{
    public class FeProvider : FeProviderSynopsis
    {
        public string PrimaryTradingName { get; set; }
        public string CompanyRegistrationNumber { get; set; }
        public string LegalAddressLine1 { get; set; }
        public string LegalAddressLine2 { get; set; }
        public string LegalAddressLine3 { get; set; }
        public string LegalAddressLine4 { get; set; }
        public string LegalAddressTown { get; set; }
        public string LegalAddressCounty { get; set; }
        public string LegalAddressPostcode { get; set; }
    }
}