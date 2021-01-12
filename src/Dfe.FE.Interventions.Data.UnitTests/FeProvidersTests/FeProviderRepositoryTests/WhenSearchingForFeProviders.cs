using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Data.FeProviders;
using Dfe.FE.Interventions.Domain.FeProviders;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Data.UnitTests.FeProvidersTests.FeProviderRepositoryTests
{
    public class WhenSearchingForFeProviders
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

        [Test]
        public async Task ThenItShouldReturnMatchingProvidersByUkprn()
        {
            _feProviders.Add(new FeProvider {Ukprn = 10000001, LegalName = "Provider One", Status = "Status A"});
            _feProviders.Add(new FeProvider {Ukprn = 10000002, LegalName = "Provider Two", Status = "Status B"});

            var actual = await _repository.SearchFeProvidersAsync(_feProviders[0].Ukprn, null, 1, 12, CancellationToken.None);

            Assert.IsNotNull(actual);
            Assert.IsNotNull(actual.Results);
            Assert.AreEqual(1, actual.Results.Length);
            Assert.AreEqual(_feProviders[0].Ukprn, actual.Results[0].Ukprn);
            Assert.AreEqual(_feProviders[0].LegalName, actual.Results[0].LegalName);
            Assert.AreEqual(_feProviders[0].Status, actual.Results[0].Status);
        }

        [Test]
        public async Task ThenItShouldReturnPaginationInformation()
        {
            for (var i = 0; i < 20; i++)
            {
                _feProviders.Add(new FeProvider {Ukprn = 10000001, LegalName = $"Provider {i}", Status = "Active"});
            }

            var actual = await _repository.SearchFeProvidersAsync(10000001, null, 1, 12, CancellationToken.None);

            Assert.IsNotNull(actual);
            Assert.AreEqual(1, actual.CurrentPage);
            Assert.AreEqual(2, actual.TotalNumberOfPages);
            Assert.AreEqual(20, actual.TotalNumberOfRecords);
            Assert.AreEqual(1, actual.PageStartIndex);
            Assert.AreEqual(12, actual.PageFinishIndex);
        }

        [TestCase(25, 1, 12, 1, 12)]
        [TestCase(25, 2, 12, 13, 24)]
        [TestCase(25, 3, 12, 25, 25)]
        public async Task ThenItShouldReturnCorrectPageOfInformation(int numberOfRecords, int pageNumber, int pageSize, int expectedStart, int expectedEnd)
        {
            for (var i = 0; i < numberOfRecords; i++)
            {
                _feProviders.Add(new FeProvider {Ukprn = 10000001, LegalName = $"Provider {i}", Status = "Active"});
            }

            var actual = await _repository.SearchFeProvidersAsync(10000001, null, pageNumber, pageSize, CancellationToken.None);

            var expectedResults = _feProviders.Skip(expectedStart - 1).Take(expectedEnd - expectedStart + 1).ToArray();
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedResults.Length, actual.Results.Length);
            for (var i = 0; i < expectedResults.Length; i++)
            {
                Assert.IsNotNull(actual.Results.SingleOrDefault(x => x.LegalName == expectedResults[i].LegalName),
                    $"Expected results to contain {expectedResults[i].LegalName} but was missing");
            }

            Assert.AreEqual(pageNumber, actual.CurrentPage);
            Assert.AreEqual((int) Math.Ceiling((float) numberOfRecords / pageSize), actual.TotalNumberOfPages);
            Assert.AreEqual(numberOfRecords, actual.TotalNumberOfRecords);
            Assert.AreEqual(expectedStart, actual.PageStartIndex);
            Assert.AreEqual(expectedEnd, actual.PageFinishIndex);
        }
    }
}