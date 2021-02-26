using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Locations;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.LearningDeliveriesTests.LearningDeliveryManagerTests
{
    public class WhenListingForProvider
    {
        private Mock<ILearningDeliveryRepository> _learningDeliveryRepositoryMock;
        private Mock<ILearnerRepository> _learnerRepositoryMock;
        private Mock<ILocationService> _locationServiceMock;
        private Mock<ILogger<LearningDeliveryManager>> _loggerMock;
        private LearningDeliveryManager _manager;

        [SetUp]
        public void Arrange()
        {
            _learningDeliveryRepositoryMock = new Mock<ILearningDeliveryRepository>();
            _learningDeliveryRepositoryMock
                .Setup(repo => repo.ListForProviderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedSearchResult<LearningDeliverySynopsis>
                {
                    TotalNumberOfPages = int.MaxValue,
                });

            _learnerRepositoryMock = new Mock<ILearnerRepository>();

            _locationServiceMock = new Mock<ILocationService>();

            _loggerMock = new Mock<ILogger<LearningDeliveryManager>>();

            _manager = new LearningDeliveryManager(
                _learningDeliveryRepositoryMock.Object,
                _learnerRepositoryMock.Object,
                _locationServiceMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task ThenItShouldGetPageOfResultsFromRepository()
        {
            var ukprn = 12345678;
            var pageNumber = 1;
            var cancellationToken = new CancellationToken();

            await _manager.ListForProviderAsync(ukprn, pageNumber, cancellationToken);

            _learningDeliveryRepositoryMock.Verify(repo => repo.ListForProviderAsync(ukprn, pageNumber, PaginationConstants.PageSize, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnResultFromRepository()
        {
            var ukprn = 12345678;
            var pageNumber = 1;
            var cancellationToken = new CancellationToken();
            var expected = new PagedSearchResult<LearningDeliverySynopsis> {TotalNumberOfPages = int.MaxValue};
            _learningDeliveryRepositoryMock
                .Setup(repo => repo.ListForProviderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.ListForProviderAsync(ukprn, pageNumber, cancellationToken);

            Assert.AreSame(expected, actual);
        }

        [TestCase(1234567)]
        [TestCase(123456789)]
        public void AndUkprnIsNot8DigitsThenItShouldThrowAnInvalidRequestException(int ukprn)
        {
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.ListForProviderAsync(ukprn, 1, CancellationToken.None));
            Assert.AreEqual("UKPRN must be an 8 digit number", actual.Message);
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void AndPageNumberNotPositiveNumberThenItShouldThrowAnInvalidRequestException(int pageNumber)
        {
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.ListForProviderAsync(12345678, pageNumber, CancellationToken.None));
            Assert.AreEqual("Page must be a number greater than 0", actual.Message);
        }
        
        [Test]
        public void AndPageNumberExceedsTotalNumberOfPagesThenItShouldThrowAnInvalidRequestException()
        {
            _learningDeliveryRepositoryMock
                .Setup(repo => repo.ListForProviderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedSearchResult<LearningDeliverySynopsis>
                {
                    TotalNumberOfPages = 2,
                });

            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.ListForProviderAsync(12345678, 3, CancellationToken.None));
            Assert.AreEqual("Page number exceeds available pages. Requested page 3, but only 2 pages available", actual.Message);
        }
    }
}