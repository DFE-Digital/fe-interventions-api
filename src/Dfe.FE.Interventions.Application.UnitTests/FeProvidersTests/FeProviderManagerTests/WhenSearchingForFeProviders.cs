using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.FeProvidersTests.FeProviderManagerTests
{
    public class WhenSearchingForFeProviders
    {
        private Mock<IFeProviderRepository> _feProviderRepositoryMock;
        private Mock<ILogger<FeProviderManager>> _loggerMock;
        private FeProviderManager _manager;

        [SetUp]
        public void Arrange()
        {
            _feProviderRepositoryMock = new Mock<IFeProviderRepository>();
            _feProviderRepositoryMock
                .Setup(repo => repo.SearchFeProvidersAsync(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedSearchResult<FeProviderSynopsis>
                {
                    TotalNumberOfPages = int.MaxValue,
                });

            _loggerMock = new Mock<ILogger<FeProviderManager>>();

            _manager = new FeProviderManager(
                _feProviderRepositoryMock.Object,
                _loggerMock.Object);
        }

        [TestCase(12345678, "provider one", 1)]
        [TestCase(null, "provider one", 1)]
        [TestCase(12345678, null, 1)]
        [TestCase(null, null, 1)]
        [TestCase(null, null, 99)]
        public async Task ThenItShouldCallRepositoryWithGivenParameters(int? ukprn, string legalName, int pageNumber)
        {
            var cancellationToken = new CancellationToken();
            await _manager.SearchAsync(ukprn, legalName, pageNumber, cancellationToken);

            _feProviderRepositoryMock.Verify(repo => repo.SearchFeProvidersAsync(ukprn, legalName, pageNumber, PaginationConstants.PageSize, cancellationToken),
                Times.Once);
        }

        [TestCase(12345678, "provider one", 1)]
        [TestCase(null, "provider one", 1)]
        [TestCase(12345678, null, 1)]
        [TestCase(null, null, 1)]
        [TestCase(null, null, 99)]
        public async Task ThenItShouldReturnResultsFromRepository(int? ukprn, string legalName, int pageNumber)
        {
            var expected = new PagedSearchResult<FeProviderSynopsis>
            {
                TotalNumberOfPages = int.MaxValue,
            };
            _feProviderRepositoryMock
                .Setup(repo => repo.SearchFeProvidersAsync(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.SearchAsync(ukprn, legalName, pageNumber, CancellationToken.None);
            Assert.AreSame(expected, actual);
        }

        [TestCase(1234567)]
        [TestCase(123456789)]
        public void AndUkprnIsNot8DigitsThenItShouldThrowAnInvalidRequestException(int ukprn)
        {
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.SearchAsync(ukprn, null, 1, CancellationToken.None));
            Assert.AreEqual("UKPRN must be an 8 digit number", actual.Message);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void AndPageNumberNotPositiveNumberThenItShouldThrowAnInvalidRequestException(int pageNumber)
        {
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.SearchAsync(null, null, pageNumber, CancellationToken.None));
            Assert.AreEqual("Page must be a number greater than 0", actual.Message);
        }
        
        [Test]
        public void AndPageNumberExceedsTotalNumberOfPagesThenItShouldThrowAnInvalidRequestException()
        {
            _feProviderRepositoryMock
                .Setup(repo => repo.SearchFeProvidersAsync(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedSearchResult<FeProviderSynopsis>
                {
                    TotalNumberOfPages = 2,
                });

            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.SearchAsync(null, null, 3, CancellationToken.None));
            Assert.AreEqual("Page number exceeds available pages. Requested page 3, but only 2 pages available", actual.Message);
        }
    }
}