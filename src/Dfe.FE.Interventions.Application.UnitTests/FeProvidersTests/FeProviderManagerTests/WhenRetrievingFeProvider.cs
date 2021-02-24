using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Dfe.FE.Interventions.Domain.Learners;
using Dfe.FE.Interventions.Domain.Locations;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Application.UnitTests.FeProvidersTests.FeProviderManagerTests
{
    public class WhenRetrievingFeProvider
    {
        private Mock<IFeProviderRepository> _feProviderRepositoryMock;
        private Mock<ILearnerRepository> _learnerRepositoryMock;
        private Mock<ILocationService> _locationServiceMock;
        private Mock<ILogger<FeProviderManager>> _loggerMock;
        private FeProviderManager _manager;

        [SetUp]
        public void Arrange()
        {
            _feProviderRepositoryMock = new Mock<IFeProviderRepository>();

            _learnerRepositoryMock = new Mock<ILearnerRepository>();

            _locationServiceMock = new Mock<ILocationService>();

            _loggerMock = new Mock<ILogger<FeProviderManager>>();

            _manager = new FeProviderManager(
                _feProviderRepositoryMock.Object,
                _learnerRepositoryMock.Object,
                _locationServiceMock.Object,
                _loggerMock.Object);
        }

        [TestCase(1234567)]
        [TestCase(123456789)]
        public void AndUkprnIsNot8DigitsThenItShouldThrowAnInvalidRequestException(int ukprn)
        {
            var actual = Assert.ThrowsAsync<InvalidRequestException>(async () =>
                await _manager.RetrieveAsync(ukprn,CancellationToken.None));
            Assert.AreEqual("UKPRN must be an 8 digit number", actual.Message);
        }

        [Test]
        public async Task ThenItShouldCallRepositoryWithUkprnAndCancellationToken()
        {
            var ukprn = 12345678;
            var cancellationToken = new CancellationToken();
            
            await _manager.RetrieveAsync(ukprn, cancellationToken);
            
            _feProviderRepositoryMock.Verify(repo=>repo.RetrieveProviderAsync(ukprn, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task AndProviderFoundThenItShouldReturnProvider()
        {
            var expected = new FeProvider();
            _feProviderRepositoryMock.Setup(repo => repo.RetrieveProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var actual = await _manager.RetrieveAsync(12345678, CancellationToken.None);
            
            Assert.AreSame(expected, actual);
        }

        [Test]
        public async Task AndProviderNotFoundThenItShouldReturnNull()
        {
            _feProviderRepositoryMock.Setup(repo => repo.RetrieveProviderAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeProvider)null);

            var actual = await _manager.RetrieveAsync(12345678, CancellationToken.None);
            
            Assert.IsNull(actual);
        }
    }
}