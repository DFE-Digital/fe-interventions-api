using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Locations;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.FeProvidersTests.FeProviderManagerTests
{
    public class WhenRetrievingStatistics
    {
        private Mock<IFeProviderRepository> _feProviderRepositoryMock;
        private Mock<ILearnerRepository> _learnerRepositoryMock;
        private Mock<ILearningDeliveryRepository> _learningDeliveryRepositoryMock;
        private Mock<ILocationService> _locationServiceMock;
        private Mock<ILogger<FeProviderManager>> _loggerMock;
        private FeProviderManager _manager;

        [SetUp]
        public void Arrange()
        {
            _feProviderRepositoryMock = new Mock<IFeProviderRepository>();

            _learnerRepositoryMock = new Mock<ILearnerRepository>();
            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(123);

            _learningDeliveryRepositoryMock = new Mock<ILearningDeliveryRepository>();
            _learningDeliveryRepositoryMock.Setup(repo => repo.GetCountOfAimTypesDeliveredByProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(456);

            _locationServiceMock = new Mock<ILocationService>();

            _loggerMock = new Mock<ILogger<FeProviderManager>>();

            _manager = new FeProviderManager(
                _feProviderRepositoryMock.Object,
                _learnerRepositoryMock.Object,
                _learningDeliveryRepositoryMock.Object,
                _locationServiceMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task ThenItShouldReturnAnFeProviderStatisticsObject()
        {
            var ukprn = 12345678;

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, CancellationToken.None);

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfApprenticeshipLearnersFromLearnerRepoWithFundingModel36()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfApprenticeshipLearners);
            _learnerRepositoryMock.Verify(repo => repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(ukprn, new[] {36}, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfLearners16To19FromLearnerRepoWithFundingModel25Or82()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfLearners16To19);
            _learnerRepositoryMock.Verify(repo => repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(ukprn, new[] {25, 82}, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfAdultEducationLearnersFromLearnerRepoWithFundingModel35Or81()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfAdultEducationLearners);
            _learnerRepositoryMock.Verify(repo => repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(ukprn, new[] {35, 81}, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfOtherFundingLearnersFromLearnerRepoWithFundingModel10Or70()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfOtherFundingLearners);
            _learnerRepositoryMock.Verify(repo => repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(ukprn, new[] {10, 70}, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfNonFundedLearnersFromLearnerRepoWithFundingModel99()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfNonFundedLearners);
            _learnerRepositoryMock.Verify(repo => repo.GetCountOfContinuingLearnersAtProviderWithFundingModelsAsync(ukprn, new[] {99}, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfActiveLearnersFromLearnerRepo()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfContinuingLearnersAtProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfActiveLearners);
            _learnerRepositoryMock.Verify(repo => repo.GetCountOfContinuingLearnersAtProviderAsync(ukprn, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfLearnersOnABreakFromLearnerRepo()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learnerRepositoryMock.Setup(repo =>
                    repo.GetCountOfLearnersOnABreakAtProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfLearnersOnABreak);
            _learnerRepositoryMock.Verify(repo => repo.GetCountOfLearnersOnABreakAtProviderAsync(ukprn, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldPopulateNumberOfAimTypesFromLearningDeliveryRepo()
        {
            var ukprn = 12345678;
            var expected = 45;
            var cancellationToken = new CancellationToken();

            _learningDeliveryRepositoryMock.Setup(repo =>
                    repo.GetCountOfAimTypesDeliveredByProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveStatisticsAsync(ukprn, cancellationToken);

            Assert.AreEqual(expected, actual.NumberOfAimTypes);
            _learningDeliveryRepositoryMock.Verify(repo => repo.GetCountOfAimTypesDeliveredByProviderAsync(ukprn, cancellationToken),
                Times.Once);
        }

        [TestCase(1234567)]
        [TestCase(123456789)]
        public void AndUkprnIsNot8DigitsThenItShouldThrowAnInvalidRequestException(int ukprn)
        {
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.RetrieveStatisticsAsync(ukprn, CancellationToken.None));
            Assert.AreEqual("UKPRN must be an 8 digit number", actual.Message);
        }
    }
}