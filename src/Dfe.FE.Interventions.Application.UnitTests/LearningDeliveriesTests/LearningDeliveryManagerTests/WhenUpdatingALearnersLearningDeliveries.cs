using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Locations;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.LearningDeliveriesTests.LearningDeliveryManagerTests
{
    public class WhenUpdatingALearnersLearningDeliveries
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

            _learnerRepositoryMock = new Mock<ILearnerRepository>();
            _learnerRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Learner());

            _locationServiceMock = new Mock<ILocationService>();
            _locationServiceMock.Setup(svc => svc.GetByPostcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Location
                {
                    Region = "Somewhere",
                });

            _loggerMock = new Mock<ILogger<LearningDeliveryManager>>();

            _manager = new LearningDeliveryManager(
                _learningDeliveryRepositoryMock.Object,
                _learnerRepositoryMock.Object,
                _locationServiceMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public void AndNotAllLearningDeliveriesHaveALearnerIdThenItShouldThrowException()
        {
            var learnerId = Guid.NewGuid();
            var learningDeliveries = new[]
            {
                new LearningDelivery {LearnerId = learnerId},
                new LearningDelivery(),
                new LearningDelivery {LearnerId = Guid.Empty},
                new LearningDelivery {LearnerId = learnerId},
            };

            Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, CancellationToken.None));
        }

        [Test]
        public void AndNotAllLearningDeliveriesHaveTheSameLearnerIdThenItShouldThrowException()
        {
            var learnerId = Guid.NewGuid();
            var learningDeliveries = new[]
            {
                new LearningDelivery {LearnerId = learnerId},
                new LearningDelivery {LearnerId = Guid.NewGuid()},
                new LearningDelivery {LearnerId = learnerId},
            };

            Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, CancellationToken.None));
        }

        [Test]
        public void AndLearnerIdNotFoundThenItShouldThrowException()
        {
            var learnerId = Guid.NewGuid();
            var learningDeliveries = new[]
            {
                new LearningDelivery {LearnerId = learnerId},
                new LearningDelivery {LearnerId = learnerId},
                new LearningDelivery {LearnerId = learnerId},
            };
            _learnerRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Learner) null);
            var cancellationToken = new CancellationToken();

            Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, cancellationToken));
            _learnerRepositoryMock.Verify(repo => repo.GetAsync(learnerId, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldReplaceAllLearningDeliveriesForLearner()
        {
            var learnerId = Guid.NewGuid();
            var learningDeliveries = new[]
            {
                new LearningDelivery {LearnerId = learnerId},
                new LearningDelivery {LearnerId = learnerId},
                new LearningDelivery {LearnerId = learnerId},
            };
            var cancellationToken = new CancellationToken();

            await _manager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, cancellationToken);

            _learningDeliveryRepositoryMock.Verify(repo => repo.ReplaceAllLearningDeliveriesForLearnerAsync(learnerId, learningDeliveries, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task AndDeliveryLocationPostcodeIsAvailableThenItShouldLookupRegion()
        {
            // Arrange
            var learnerId = Guid.NewGuid();
            var postcode1 = "AA1 1AA";
            var postcode2 = "BB2 2BB";
            var learningDeliveries = new[]
            {
                new LearningDelivery {LearnerId = learnerId, DeliveryLocationPostcode = postcode1},
                new LearningDelivery {LearnerId = learnerId, DeliveryLocationPostcode = postcode2},
            };
            var region1 = Guid.NewGuid().ToString();
            var region2 = Guid.NewGuid().ToString();
            _locationServiceMock.Setup(svc => svc.GetByPostcodeAsync(postcode1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Location
                {
                    Region = region1,
                });
            _locationServiceMock.Setup(svc => svc.GetByPostcodeAsync(postcode2, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Location
                {
                    Region = region2,
                });
            var cancellationToken = new CancellationToken();

            // Act
            await _manager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, cancellationToken);

            // Assert
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(postcode1, cancellationToken),
                Times.Once);
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(postcode2, cancellationToken),
                Times.Once);
            _learningDeliveryRepositoryMock.Verify(repo => repo.ReplaceAllLearningDeliveriesForLearnerAsync(
                    learnerId,
                    It.Is<LearningDelivery[]>(lds => lds[0].DeliveryLocationRegion == region1 && lds[1].DeliveryLocationRegion == region2),
                    cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task AndDeliveryLocationPostcodeIsNotAvailableThenItShouldNotLookupRegion()
        {
            // Arrange
            var learnerId = Guid.NewGuid();
            var postcode1 = "AA1 1AA";
            var learningDeliveries = new[]
            {
                new LearningDelivery {LearnerId = learnerId, DeliveryLocationPostcode = postcode1},
                new LearningDelivery {LearnerId = learnerId, DeliveryLocationPostcode = null},
            };
            var region1 = Guid.NewGuid().ToString();
            _locationServiceMock.Setup(svc => svc.GetByPostcodeAsync(postcode1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Location
                {
                    Region = region1,
                });
            var cancellationToken = new CancellationToken();

            // Act
            await _manager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, cancellationToken);

            // Assert
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(1));
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(postcode1, cancellationToken),
                Times.Once);
            _learningDeliveryRepositoryMock.Verify(repo => repo.ReplaceAllLearningDeliveriesForLearnerAsync(
                    learnerId,
                    It.Is<LearningDelivery[]>(lds => lds[0].DeliveryLocationRegion == region1 && lds[1].DeliveryLocationRegion == null),
                    cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task AndDeliveryLocationPostcodeIsAvailableButIfNotFoundByLocationServiceThenItShouldNotModifyRegion()
        {
            // Arrange
            var learnerId = Guid.NewGuid();
            var postcode1 = "AA1 1AA";
            var postcode2 = "BB2 2BB";
            var region1 = Guid.NewGuid().ToString();
            var region2 = Guid.NewGuid().ToString();
            var learningDeliveries = new[]
            {
                new LearningDelivery {LearnerId = learnerId, DeliveryLocationPostcode = postcode1, DeliveryLocationRegion = region1},
                new LearningDelivery {LearnerId = learnerId, DeliveryLocationPostcode = postcode2, DeliveryLocationRegion = region2},
            };
            _locationServiceMock.Setup(svc => svc.GetByPostcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Location)null);
            var cancellationToken = new CancellationToken();

            // Act
            await _manager.UpdateLearnersLearningDeliveriesAsync(learningDeliveries, cancellationToken);

            // Assert
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(postcode1, cancellationToken),
                Times.Once);
            _locationServiceMock.Verify(svc => svc.GetByPostcodeAsync(postcode2, cancellationToken),
                Times.Once);
            _learningDeliveryRepositoryMock.Verify(repo => repo.ReplaceAllLearningDeliveriesForLearnerAsync(
                    learnerId,
                    It.Is<LearningDelivery[]>(lds => lds[0].DeliveryLocationRegion == region1 && lds[1].DeliveryLocationRegion == region2),
                    cancellationToken),
                Times.Once);
        }
    }
}