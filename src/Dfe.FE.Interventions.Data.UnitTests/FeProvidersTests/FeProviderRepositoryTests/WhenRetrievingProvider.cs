using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.FE.Interventions.Data.FeProviders;
using Dfe.FE.Interventions.Domain.FeProviders;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Data.UnitTests.FeProvidersTests.FeProviderRepositoryTests
{
    public class WhenRetrievingProvider
    {
        private List<FeProvider> _feProviders;
        private Mock<IFeInterventionsDbContext> _dbContext;
        private FeProviderRepository _repository;

        [SetUp]
        public void Arrange()
        {
            _feProviders = new List<FeProvider>();

            _dbContext = new Mock<IFeInterventionsDbContext>();
            _dbContext.Setup(db => db.FeProviders)
                .Returns(_feProviders.AsQueryable().BuildMockDbSet().Object);

            _repository = new FeProviderRepository(_dbContext.Object);
        }

        [Test, AutoData]
        public async Task AndTheProviderExistsThenItShouldReturnProvider(FeProvider provider)
        {
            _feProviders.Add(provider);

            var actual = await _repository.RetrieveProviderAsync(provider.Ukprn, CancellationToken.None);
            
            Assert.AreEqual(provider.Ukprn, actual.Ukprn);
            Assert.AreEqual(provider.LegalName, actual.LegalName);
            Assert.AreEqual(provider.Status, actual.Status);
            Assert.AreEqual(provider.PrimaryTradingName, actual.PrimaryTradingName);
            Assert.AreEqual(provider.CompanyRegistrationNumber, actual.CompanyRegistrationNumber);
            Assert.AreEqual(provider.LegalAddressLine1, actual.LegalAddressLine1);
            Assert.AreEqual(provider.LegalAddressLine2, actual.LegalAddressLine2);
            Assert.AreEqual(provider.LegalAddressLine3, actual.LegalAddressLine3);
            Assert.AreEqual(provider.LegalAddressLine4, actual.LegalAddressLine4);
            Assert.AreEqual(provider.LegalAddressTown, actual.LegalAddressTown);
            Assert.AreEqual(provider.LegalAddressCounty, actual.LegalAddressCounty);
            Assert.AreEqual(provider.LegalAddressPostcode, actual.LegalAddressPostcode);
        }

        [Test]
        public async Task AndTheProviderDoesNotExistThenItShouldReturnNull()
        {
            var actual = await _repository.RetrieveProviderAsync(12345678, CancellationToken.None);
            
            Assert.IsNull(actual);
        }
    }
}