using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Application.Learners;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.FeProvidersTests.LearnerManagerTests
{
    public class WhenUpsertingALearner
    {
        private Mock<ILearnerRepository> _learnerRepositoryMock;
        private Mock<IFeProviderRepository> _providerRepositoryMock;
        private Mock<ILogger<LearnerManager>> _loggerMock;
        private LearnerManager _manager;

        [SetUp]
        public void Arrange()
        {
            _learnerRepositoryMock = new Mock<ILearnerRepository>();

            _providerRepositoryMock = new Mock<IFeProviderRepository>();
            _providerRepositoryMock.Setup(repo => repo.RetrieveProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FeProvider());

            _loggerMock = new Mock<ILogger<LearnerManager>>();

            _manager = new LearnerManager(
                _learnerRepositoryMock.Object,
                _providerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task ThenItShouldUpsertLearnerInRepository()
        {
            var learner = new Learner {Ukprn = 12345678, LearnRefNumber = "df1ds32f1"};
            var cancellationToken = new CancellationToken();

            await _manager.UpsertLearner(learner, cancellationToken);

            _learnerRepositoryMock.Verify(repo => repo.UpsertLearnerAsync(learner, cancellationToken),
                Times.Once);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task ThenItShouldThrowInvalidRequestExceptionIfNoLearnRefNumber(string learnRefNumber)
        {
            var learner = new Learner
            {
                Ukprn = 12345678,
                LearnRefNumber = learnRefNumber,
            };

            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.UpsertLearner(learner, CancellationToken.None));
            Assert.AreEqual("Must provide LearnRefNumber", actual.Message);
        }

        [Test]
        public async Task ThenItShouldThrowInvalidRequestExceptionIfNoProviderWithUkprnFound()
        {
            var learner = new Learner {Ukprn = 12345678, LearnRefNumber = "df1ds32f1"};
            _providerRepositoryMock.Setup(repo => repo.RetrieveProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeProvider) null);

            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.UpsertLearner(learner, CancellationToken.None));
            Assert.AreEqual("Cannot find provider with UKPRN 12345678", actual.Message);
        }
    }
}