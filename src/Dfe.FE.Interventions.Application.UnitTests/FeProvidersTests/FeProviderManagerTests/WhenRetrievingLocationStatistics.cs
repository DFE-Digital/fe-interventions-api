using System.Collections.Generic;
using System.Linq;
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
    public class WhenRetrievingLocationStatistics
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
            _learnerRepositoryMock.Setup(repo => repo.GetCountOfContinuingLearnersByProviderLocationAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, int>
                {
                    {"AA1 1AA", 123},
                });
            _learnerRepositoryMock.Setup(repo => repo.GetCountOfLearnersOnABreakByProviderLocationAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, int>
                {
                    {"AA1 1AA", 456},
                });

            _learningDeliveryRepositoryMock = new Mock<ILearningDeliveryRepository>();
            _learningDeliveryRepositoryMock.Setup(repo => repo.GetCountOfAimTypesDeliveredByProviderLocationAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, int>
                {
                    {"AA1 1AA", 456},
                });

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
        public async Task ThenItShouldReturnAnArrayOfFeProviderLocationStatistics()
        {
            var ukprn = 12345678;

            var actual = await _manager.RetrieveLocationStatisticsAsync(ukprn, CancellationToken.None);

            Assert.IsNotNull(actual);
        }

        [Test]
        public async Task ThenItShouldReturnNumberOfActiveLearnersFromLearnerRepo()
        {
            var ukprn = 12345678;

            _learnerRepositoryMock.Setup(repo => repo.GetCountOfContinuingLearnersByProviderLocationAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, int>
                {
                    {"AA1 1AA", 123},
                    {"BB2 2BB", 456},
                });

            var actual = await _manager.RetrieveLocationStatisticsAsync(ukprn, CancellationToken.None);

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.IsNotNull(actual.SingleOrDefault(x => x.DeliveryLocationPostcode == "AA1 1AA"));
            Assert.AreEqual(123, actual.Single(x => x.DeliveryLocationPostcode == "AA1 1AA").NumberOfActiveLearners);
            Assert.IsNotNull(actual.SingleOrDefault(x => x.DeliveryLocationPostcode == "BB2 2BB"));
            Assert.AreEqual(456, actual.Single(x => x.DeliveryLocationPostcode == "BB2 2BB").NumberOfActiveLearners);
        }

        [Test]
        public async Task ThenItShouldReturnNumberOfLearnersOnABreakFromLearnerRepo()
        {
            var ukprn = 12345678;

            _learnerRepositoryMock.Setup(repo => repo.GetCountOfLearnersOnABreakByProviderLocationAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, int>
                {
                    {"AA1 1AA", 123},
                    {"BB2 2BB", 456},
                });

            var actual = await _manager.RetrieveLocationStatisticsAsync(ukprn, CancellationToken.None);

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.IsNotNull(actual.SingleOrDefault(x => x.DeliveryLocationPostcode == "AA1 1AA"));
            Assert.AreEqual(123, actual.Single(x => x.DeliveryLocationPostcode == "AA1 1AA").NumberOfLearnersOnABreak);
            Assert.IsNotNull(actual.SingleOrDefault(x => x.DeliveryLocationPostcode == "BB2 2BB"));
            Assert.AreEqual(456, actual.Single(x => x.DeliveryLocationPostcode == "BB2 2BB").NumberOfLearnersOnABreak);
        }

        [Test]
        public async Task ThenItShouldReturnNumberOfAimTypesFromLearningDeliveryRepo()
        {
            var ukprn = 12345678;

            _learningDeliveryRepositoryMock.Setup(repo => repo.GetCountOfAimTypesDeliveredByProviderLocationAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Dictionary<string, int>
                {
                    {"AA1 1AA", 123},
                    {"BB2 2BB", 456},
                });

            var actual = await _manager.RetrieveLocationStatisticsAsync(ukprn, CancellationToken.None);

            Assert.IsNotNull(actual);
            Assert.AreEqual(2, actual.Length);
            Assert.IsNotNull(actual.SingleOrDefault(x => x.DeliveryLocationPostcode == "AA1 1AA"));
            Assert.AreEqual(123, actual.Single(x => x.DeliveryLocationPostcode == "AA1 1AA").NumberOfAimTypes);
            Assert.IsNotNull(actual.SingleOrDefault(x => x.DeliveryLocationPostcode == "BB2 2BB"));
            Assert.AreEqual(456, actual.Single(x => x.DeliveryLocationPostcode == "BB2 2BB").NumberOfAimTypes);
        }

        [TestCase(1234567)]
        [TestCase(123456789)]
        public void AndUkprnIsNot8DigitsThenItShouldThrowAnInvalidRequestException(int ukprn)
        {
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.RetrieveLocationStatisticsAsync(ukprn, CancellationToken.None));
            Assert.AreEqual("UKPRN must be an 8 digit number", actual.Message);
        }
    }
}