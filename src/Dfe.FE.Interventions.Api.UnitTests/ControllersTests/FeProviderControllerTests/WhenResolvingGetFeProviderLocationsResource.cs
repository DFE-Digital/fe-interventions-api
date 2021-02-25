using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Api.Controllers;
using Dfe.FE.Interventions.Application;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Api.UnitTests.ControllersTests.FeProviderControllerTests
{
    public class WhenResolvingGetFeProviderLocationsResource
    {
        private Mock<IFeProviderManager> _feProviderManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<FeProviderController>> _loggerMock;
        private UrlHelperStub _urlHelperStub;
        private FeProviderController _controller;

        [SetUp]
        public void Arrange()
        {
            _feProviderManagerMock = new Mock<IFeProviderManager>();
            _feProviderManagerMock.Setup(manager => manager.RetrieveAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FeProvider());
            _feProviderManagerMock.Setup(manager => manager.RetrieveLocationStatisticsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FeProviderLocationStatistics[0]);

            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(mapper => mapper.Map<ApiFeProvider>(It.IsAny<FeProvider>()))
                .Returns(new ApiFeProvider());

            _loggerMock = new Mock<ILogger<FeProviderController>>();

            _urlHelperStub = new UrlHelperStub(typeof(FeProviderController).GetMethod("GetLocationsAsync"));

            _controller = new FeProviderController(
                _feProviderManagerMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
            _controller.Url = _urlHelperStub;
        }

        [Test]
        public async Task AndUkprnSpecifiedButIsNotANumberThenItShouldReturnBadRequest()
        {
            var result = await _controller.GetLocationsAsync("not-a-number", CancellationToken.None);

            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;

            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual("UKPRN must be an 8 digit number", problemDetails.Detail);
        }

        [Test]
        public async Task AndTheProviderIsNotFoundThenItShouldReturnNotFoundResult()
        {
            var ukprn = 12345678;

            _feProviderManagerMock.Setup(manager => manager.RetrieveAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeProvider) null);

            var result = await _controller.GetLocationsAsync(ukprn.ToString(), CancellationToken.None);

            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task ThenItShouldCheckProviderExists()
        {
            var ukprn = 12345678;
            var cancellationToken = new CancellationToken();

            await _controller.GetLocationsAsync(ukprn.ToString(), cancellationToken);

            _feProviderManagerMock.Verify(manager => manager.RetrieveAsync(ukprn, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task AndManagerThrowsInvalidRequestExceptionWhenGettingProviderThenItShouldReturnBadRequest()
        {
            var exception = new InvalidRequestException("unit test exception");
            _feProviderManagerMock.Setup(manager => manager.RetrieveAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var result = await _controller.GetLocationsAsync("1", CancellationToken.None);

            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;

            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(exception.Message, problemDetails.Detail);
        }

        [Test]
        public async Task ThenItShouldGetLocationStatisticFromManagerForProvider()
        {
            var ukprn = 12345678;
            var cancellationToken = new CancellationToken();

            await _controller.GetLocationsAsync(ukprn.ToString(), cancellationToken);

            _feProviderManagerMock.Verify(manager => manager.RetrieveLocationStatisticsAsync(ukprn, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnLocationStatistics()
        {
            var ukprn = 12345678;
            var cancellationToken = new CancellationToken();
            var expected = new FeProviderLocationStatistics[0];
            
            _feProviderManagerMock.Setup(manager => manager.RetrieveLocationStatisticsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var result = await _controller.GetLocationsAsync(ukprn.ToString(), cancellationToken);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as FeProviderLocationStatistics[];
            Assert.IsNotNull(result);
            Assert.IsNotNull(okObjectResult);
            Assert.AreSame(expected, actual);
        }

        [Test]
        public async Task AndManagerThrowsInvalidRequestExceptionWhenGettingStatisticsThenItShouldReturnBadRequest()
        {
            var exception = new InvalidRequestException("unit test exception");
            _feProviderManagerMock.Setup(manager => manager.RetrieveLocationStatisticsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            var result = await _controller.GetLocationsAsync("1", CancellationToken.None);

            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;

            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(exception.Message, problemDetails.Detail);
        }
    }
}