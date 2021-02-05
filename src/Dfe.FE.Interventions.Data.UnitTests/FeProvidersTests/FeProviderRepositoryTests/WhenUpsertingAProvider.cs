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
    public class WhenUpsertingAProvider
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
        public async Task AndAnExistingProviderDoesNotExistWithUkprnThenItShouldCreateProvider(FeProvider provider)
        {
            var mockDbSet = _feProviders.AsQueryable().BuildMockDbSet();
            _dbContext.Setup(db => db.FeProviders)
                .Returns(mockDbSet.Object);
            var cancellationToken = new CancellationToken();

            var created = await _repository.UpsertProviderAsync(provider, cancellationToken);

            Assert.IsTrue(created);
            mockDbSet.Verify(dbSet => dbSet.Add(provider), Times.Once);
            _dbContext.Verify(context => context.CommitAsync(cancellationToken), Times.Once);
        }

        [Test, AutoData]
        public async Task AndAnExistingProviderDoesExistWithUkprnThenItShouldUpdateProvider(FeProvider updatedProvider, FeProvider existingProvider)
        {
            existingProvider.Ukprn = updatedProvider.Ukprn;
            _feProviders.Add(existingProvider);
            var mockDbSet = _feProviders.AsQueryable().BuildMockDbSet();
            _dbContext.Setup(db => db.FeProviders)
                .Returns(mockDbSet.Object);
            var cancellationToken = new CancellationToken();

            var created = await _repository.UpsertProviderAsync(updatedProvider, cancellationToken);

            Assert.IsFalse(created);
            Assert.AreEqual(updatedProvider.LegalName, existingProvider.LegalName);
            Assert.AreEqual(updatedProvider.Status, existingProvider.Status);
            Assert.AreEqual(updatedProvider.PrimaryTradingName, existingProvider.PrimaryTradingName);
            Assert.AreEqual(updatedProvider.CompanyRegistrationNumber, existingProvider.CompanyRegistrationNumber);
            Assert.AreEqual(updatedProvider.LegalAddressLine1, existingProvider.LegalAddressLine1);
            Assert.AreEqual(updatedProvider.LegalAddressLine2, existingProvider.LegalAddressLine2);
            Assert.AreEqual(updatedProvider.LegalAddressLine3, existingProvider.LegalAddressLine3);
            Assert.AreEqual(updatedProvider.LegalAddressLine4, existingProvider.LegalAddressLine4);
            Assert.AreEqual(updatedProvider.LegalAddressTown, existingProvider.LegalAddressTown);
            Assert.AreEqual(updatedProvider.LegalAddressCounty, existingProvider.LegalAddressCounty);
            Assert.AreEqual(updatedProvider.LegalAddressPostcode, existingProvider.LegalAddressPostcode);
            _dbContext.Verify(context => context.CommitAsync(cancellationToken), Times.Once);
        }
    }
}