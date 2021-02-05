using System;
using System.Linq;
using AutoMapper;
using Dfe.FE.Interventions.Consumer.Ukrlp.Ukrlp;
using Dfe.FE.Interventions.Domain.FeProviders;

namespace Dfe.FE.Interventions.Consumer.Ukrlp.MappingProfiles
{
    public class UkrlpProviderMapping : Profile
    {
        private const string CompaniesHouseVerificationAuthority = "Companies House";

        public UkrlpProviderMapping()
        {
            CreateMap<Provider, FeProvider>()
                .ForMember(dst => dst.Ukprn, cfg => cfg.MapFrom(src => src.UnitedKingdomProviderReferenceNumber))
                .ForMember(dst => dst.LegalName, cfg => cfg.MapFrom(src => src.ProviderName))
                .ForMember(dst => dst.Status, cfg => cfg.MapFrom(src => src.ProviderStatus))
                .ForMember(dst => dst.PrimaryTradingName, cfg => cfg.MapFrom(src => src.ProviderName))
                .ForMember(dst => dst.CompanyRegistrationNumber, cfg => cfg.MapFrom(src => CompaniesHouseNumber(src)))
                .ForMember(dst => dst.LegalAddressLine1, cfg => cfg.MapFrom(src => MappableOfficeAddress(src).Address1))
                .ForMember(dst => dst.LegalAddressLine2, cfg => cfg.MapFrom(src => MappableOfficeAddress(src).Address2))
                .ForMember(dst => dst.LegalAddressLine3, cfg => cfg.MapFrom(src => MappableOfficeAddress(src).Address3))
                .ForMember(dst => dst.LegalAddressLine4, cfg => cfg.MapFrom(src => MappableOfficeAddress(src).Address4))
                .ForMember(dst => dst.LegalAddressTown, cfg => cfg.MapFrom(src => MappableOfficeAddress(src).Town))
                .ForMember(dst => dst.LegalAddressCounty, cfg => cfg.MapFrom(src => MappableOfficeAddress(src).County))
                .ForMember(dst => dst.LegalAddressPostcode, cfg => cfg.MapFrom(src => MappableOfficeAddress(src).PostCode));
        }

        private AddressStructure MappableOfficeAddress(Provider provider)
        {
            var address = provider.ProviderContacts
                ?.FirstOrDefault(c => c.ContactType.Equals("L", StringComparison.InvariantCultureIgnoreCase))
                ?.ContactAddress;
            return address ?? new AddressStructure();
        }

        private string CompaniesHouseNumber(Provider provider)
        {
            return MappableVerificationDetails(provider, CompaniesHouseVerificationAuthority).VerificationID;
        }
        private VerificationDetails MappableVerificationDetails(Provider provider, string source)
        {
            var verification = provider.VerificationDetails
                ?.FirstOrDefault(vd => vd.VerificationAuthority.Equals(source, StringComparison.InvariantCultureIgnoreCase));
            return verification ?? new VerificationDetails();
        }
    }
}