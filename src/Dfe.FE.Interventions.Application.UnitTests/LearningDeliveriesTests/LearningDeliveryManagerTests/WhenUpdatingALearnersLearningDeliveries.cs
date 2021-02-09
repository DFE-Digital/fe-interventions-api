using System;
using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.LearningDeliveriesTests.LearningDeliveryManagerTests
{
    public class WhenUpdatingALearnersLearningDeliveries
    {
        private Mock<ILearningDeliveryRepository> _learningDeliveryRepositoryMock;
        private Mock<ILearnerRepository> _learnerRepositoryMock;
        private Mock<ILogger<LearningDeliveryManager>> _loggerMock;
        private LearningDeliveryManager _manager;

        [SetUp]
        public void Arrange()
        {
            _learningDeliveryRepositoryMock = new Mock<ILearningDeliveryRepository>();

            _learnerRepositoryMock = new Mock<ILearnerRepository>();
            _learnerRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Learner());

            _loggerMock = new Mock<ILogger<LearningDeliveryManager>>();

            _manager = new LearningDeliveryManager(
                _learningDeliveryRepositoryMock.Object,
                _learnerRepositoryMock.Object,
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
            
            _learningDeliveryRepositoryMock.Verify(repo=>repo.ReplaceAllLearningDeliveriesForLearnerAsync(learnerId, learningDeliveries, cancellationToken),
                Times.Once);
        }
    }
}