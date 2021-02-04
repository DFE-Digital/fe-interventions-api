using System;
using System.Linq;
using AutoMapper;
using Dfe.FE.Interventions.Consumer.Ukrlp.MappingProfiles;
using Dfe.FE.Interventions.Consumer.Ukrlp.Ukrlp;
using Dfe.FE.Interventions.Domain.FeProviders;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Consumer.Ukrlp.UnitTests.MappingProfilesTests.UkrlpProviderMappingTests
{
    public class WhenMappingProviderToFeProvider
    {
        private IMapper _mapper;

        [SetUp]
        public void Arrange()
        {
            var cfg = new MapperConfiguration(configure => { configure.AddProfile<UkrlpProviderMapping>(); });
            _mapper = cfg.CreateMapper();
        }

        [Test]
        public void ThenItShouldMapUkprnFromUnitedKingdomProviderReferenceNumber()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.UnitedKingdomProviderReferenceNumber, actual.Ukprn);
        }

        [Test]
        public void ThenItShouldMapLegalNameFromProviderName()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderName, actual.LegalName);
        }

        [Test]
        public void ThenItShouldMapStatusFromProviderStatus()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderStatus, actual.Status);
        }

        [Test]
        public void ThenItShouldMapPrimaryTradingNameFromProviderName()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderName, actual.PrimaryTradingName);
        }

        [Test]
        public void AndHasCompaniesHouseVerificationThenItShouldMapCompanyRegistrationNumberFromCompaniesHouseVerificationId()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.VerificationDetails.Single().VerificationID, actual.CompanyRegistrationNumber);
        }

        [Test]
        public void AndDoesNotHaveCompaniesHouseVerificationThenItShouldMapCompanyRegistrationNumberToNull()
        {
            var provider = MakeProvider();
            provider.VerificationDetails.Single().VerificationAuthority = "test";

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.CompanyRegistrationNumber);
        }

        [Test]
        public void AndHsNoVerificationsThenItShouldMapCompanyRegistrationNumberToNull()
        {
            var provider = MakeProvider();
            provider.VerificationDetails = new VerificationDetails[0];

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.CompanyRegistrationNumber);
        }

        [Test]
        public void AndHasLContactThenItShouldMapLegalAddressLine1FromContactAddress1()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderContacts.First().ContactAddress.Address1, actual.LegalAddressLine1);
        }

        [Test]
        public void AndHasLContactThenItShouldMapLegalAddressLine2FromContactAddress2()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderContacts.First().ContactAddress.Address2, actual.LegalAddressLine2);
        }

        [Test]
        public void AndHasLContactThenItShouldMapLegalAddressLine3FromContactAddress3()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderContacts.First().ContactAddress.Address3, actual.LegalAddressLine3);
        }

        [Test]
        public void AndHasLContactThenItShouldMapLegalAddressLine4FromContactAddress4()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderContacts.First().ContactAddress.Address4, actual.LegalAddressLine4);
        }

        [Test]
        public void AndHasLContactThenItShouldMapLegalAddressTownFromContactTown()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderContacts.First().ContactAddress.Town, actual.LegalAddressTown);
        }

        [Test]
        public void AndHasLContactThenItShouldMapLegalAddressCountyFromContactCounty()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderContacts.First().ContactAddress.County, actual.LegalAddressCounty);
        }

        [Test]
        public void AndHasLContactThenItShouldMapLegalAddressPostcodeFromContactPostCode()
        {
            var provider = MakeProvider();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.AreEqual(provider.ProviderContacts.First().ContactAddress.PostCode, actual.LegalAddressPostcode);
        }

        [Test]
        public void AndDoesNotHaveLContactThenItShouldMapLegalAddressLine1ToNull()
        {
            var provider = MakeProvider();
            provider.ProviderContacts = provider.ProviderContacts.Skip(1).ToArray();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.LegalAddressLine1);
        }

        [Test]
        public void AndDoesNotHaveLContactThenItShouldMapLegalAddressLine2ToNull()
        {
            var provider = MakeProvider();
            provider.ProviderContacts = provider.ProviderContacts.Skip(1).ToArray();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.LegalAddressLine2);
        }

        [Test]
        public void AndDoesNotHaveLContactThenItShouldMapLegalAddressLine3ToNull()
        {
            var provider = MakeProvider();
            provider.ProviderContacts = provider.ProviderContacts.Skip(1).ToArray();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.LegalAddressLine3);
        }

        [Test]
        public void AndDoesNotHaveLContactThenItShouldMapLegalAddressLine4ToNull()
        {
            var provider = MakeProvider();
            provider.ProviderContacts = provider.ProviderContacts.Skip(1).ToArray();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.LegalAddressLine4);
        }

        [Test]
        public void AndDoesNotHaveLContactThenItShouldMapLegalAddressTownToNull()
        {
            var provider = MakeProvider();
            provider.ProviderContacts = provider.ProviderContacts.Skip(1).ToArray();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.LegalAddressTown);
        }

        [Test]
        public void AndDoesNotHaveLContactThenItShouldMapLegalAddressCountyToNull()
        {
            var provider = MakeProvider();
            provider.ProviderContacts = provider.ProviderContacts.Skip(1).ToArray();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.LegalAddressCounty);
        }

        [Test]
        public void AndDoesNotHaveLContactThenItShouldMapLegalAddressPostcodeToNull()
        {
            var provider = MakeProvider();
            provider.ProviderContacts = provider.ProviderContacts.Skip(1).ToArray();

            var actual = _mapper.Map<FeProvider>(provider);

            Assert.IsNull(actual.LegalAddressPostcode);
        }


        private Provider MakeProvider()
        {
            return new Provider
            {
                UnitedKingdomProviderReferenceNumber = 12345678,
                ProviderName = "PROVIDER ONE",
                AccessibleProviderName = "Provider 1",
                ProviderStatus = "Active",
                ProviderVerificationDate = DateTime.Today,
                ProviderContacts = new[]
                {
                    new ProviderContact
                    {
                        ContactType = "L",
                        ContactAddress = new AddressStructure
                        {
                            Address1 = "Unit A",
                            Address2 = "123 Fake Street",
                            Address3 = "Someplace",
                            Address4 = "Madeupsville",
                            Town = "Fictiontown",
                            County = "Norealshire",
                            PostCode = "AB12 3CD",
                        },
                    },
                    new ProviderContact
                    {
                        ContactType = "P",
                        ContactAddress = new AddressStructure
                        {
                            Address1 = "sdfsdf",
                            Address2 = "dfgfdg",
                            Address3 = "efcc",
                            Address4 = "vfd dc",
                            Town = "sdsdfv",
                            County = "svs",
                            PostCode = "erewd",
                        },
                    },
                },
                VerificationDetails = new[]
                {
                    new VerificationDetails
                    {
                        VerificationAuthority = "Companies House",
                        VerificationID = "96325874",
                    }
                },
            };
        }
    }
}