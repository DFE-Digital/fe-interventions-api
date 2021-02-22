using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Api.Controllers;
using Dfe.FE.Interventions.Application;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Api.UnitTests.ControllersTests.LearningDeliveryControllerTests
{
    public class WhenResolvingListLearningDeliveriesForProviderResource
    {
        private Mock<ILearningDeliveryManager> _learningDeliveryManagerMock;
        private Mock<IMapper> _mapperMock;
        private Mock<ILogger<LearningDeliveryController>> _loggerMock;
        private UrlHelperStub _urlHelperStub;
        private LearningDeliveryController _controller;

        [SetUp]
        public void Arrange()
        {
            _learningDeliveryManagerMock = new Mock<ILearningDeliveryManager>();
            _learningDeliveryManagerMock.Setup(manager => manager.ListForProviderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedSearchResult<LearningDeliverySynopsis>());

            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(It.IsAny<PagedSearchResult<LearningDeliverySynopsis>>()))
                .Returns((PagedSearchResult<LearningDeliverySynopsis> source) =>
                    new ApiPagedSearchResult<LearningDeliverySynopsis>
                    {
                        Results = source.Results,
                        CurrentPage = source.CurrentPage,
                        TotalNumberOfPages = source.TotalNumberOfPages,
                        TotalNumberOfRecords = source.TotalNumberOfRecords,
                        PageStartIndex = source.PageStartIndex,
                        PageFinishIndex = source.PageFinishIndex,
                    });

            _loggerMock = new Mock<ILogger<LearningDeliveryController>>();

            _urlHelperStub = new UrlHelperStub(typeof(LearningDeliveryController).GetMethod("ListByUkprnAsync"));

            _controller = new LearningDeliveryController(
                _learningDeliveryManagerMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
            _controller.Url = _urlHelperStub;
        }

        [Test]
        public async Task AndUkprnIsNotANumberThenItShouldReturnBadRequest()
        {
            var result = await _controller.ListByUkprnAsync("not-a-number", null, CancellationToken.None);

            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;

            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual("UKPRN must be an 8 digit number", problemDetails.Detail);
        }

        [Test]
        public async Task AndPageNumberIsNotANumberThenItShouldReturnBadRequest()
        {
            var result = await _controller.ListByUkprnAsync("12345678", "not-a-number", CancellationToken.None);

            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;

            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual("Page must be a number greater than 0", problemDetails.Detail);
        }

        [Test]
        public async Task ThenItShouldDefaultToPage1IfNotSpecified()
        {
            var cancellationToken = new CancellationToken();
            await _controller.ListByUkprnAsync("12345678", null, cancellationToken);

            _learningDeliveryManagerMock.Verify(manager => manager.ListForProviderAsync(It.IsAny<int>(), 1, cancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldCallManagerWithRequestedParameters(int ukprn, int pageNumber)
        {
            var cancellationToken = new CancellationToken();
            await _controller.ListByUkprnAsync(ukprn.ToString(), pageNumber.ToString(), cancellationToken);

            _learningDeliveryManagerMock.Verify(manager => manager.ListForProviderAsync(ukprn, pageNumber, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldMapResultsToApiFormat()
        {
            var source = new PagedSearchResult<LearningDeliverySynopsis>();
            _learningDeliveryManagerMock.Setup(manager => manager.ListForProviderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(source);

            await _controller.ListByUkprnAsync("12345678", "1", CancellationToken.None);

            _mapperMock.Verify(mapper => mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(source),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnMappedData()
        {
            var mapped = new ApiPagedSearchResult<LearningDeliverySynopsis>();
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(It.IsAny<PagedSearchResult<LearningDeliverySynopsis>>()))
                .Returns(mapped);

            var result = await _controller.ListByUkprnAsync("12345678", "1", CancellationToken.None);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<LearningDeliverySynopsis>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(okObjectResult);
            Assert.AreSame(mapped, actual);
        }

        [Test]
        public async Task AndRequestedPageIs1AndOnly1PageOfResultsThenItShouldIncludeLinkToFirstAndLastPage()
        {
            var mapped = new ApiPagedSearchResult<LearningDeliverySynopsis>
            {
                CurrentPage = 1,
                TotalNumberOfPages = 1,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(It.IsAny<PagedSearchResult<LearningDeliverySynopsis>>()))
                .Returns(mapped);

            var result = await _controller.ListByUkprnAsync("12345678", "1", CancellationToken.None);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<LearningDeliverySynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page=1", actual.Links.First);
            Assert.IsNull(actual.Links.Prev);
            Assert.IsNull(actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page=1", actual.Links.Last);
        }

        [Test]
        public async Task AndRequestedPageIsNotFirstPageOfResultsThenItShouldIncludeLinkToFirstAndPrevAndLastPage()
        {
            var currentPage = 5;
            var mapped = new ApiPagedSearchResult<LearningDeliverySynopsis>
            {
                CurrentPage = currentPage,
                TotalNumberOfPages = currentPage,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(It.IsAny<PagedSearchResult<LearningDeliverySynopsis>>()))
                .Returns(mapped);

            var result = await _controller.ListByUkprnAsync("12345678",currentPage.ToString(), CancellationToken.None);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<LearningDeliverySynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page=1", actual.Links.First);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page={currentPage - 1}", actual.Links.Prev);
            Assert.IsNull(actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page={currentPage}", actual.Links.Last);
        }

        [Test]
        public async Task AndRequestedPageIsNotFirstOrLastPageOfResultsThenItShouldIncludeLinkToFirstAndPrevAndNextAndLastPage()
        {
            var currentPage = 5;
            var mapped = new ApiPagedSearchResult<LearningDeliverySynopsis>
            {
                CurrentPage = currentPage,
                TotalNumberOfPages = currentPage + 2,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(It.IsAny<PagedSearchResult<LearningDeliverySynopsis>>()))
                .Returns(mapped);

            var result = await _controller.ListByUkprnAsync("12345678",currentPage.ToString(), CancellationToken.None);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<LearningDeliverySynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page=1", actual.Links.First);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page={currentPage - 1}", actual.Links.Prev);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page={currentPage + 1}", actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page={currentPage + 2}", actual.Links.Last);
        }

        [Test]
        public async Task AndRequestedPageIsLastPageOfResultsThenItShouldIncludeLinkToFirstAndPrevAndLastPage()
        {
            var mapped = new ApiPagedSearchResult<LearningDeliverySynopsis>
            {
                CurrentPage = 3,
                TotalNumberOfPages = 3,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(It.IsAny<PagedSearchResult<LearningDeliverySynopsis>>()))
                .Returns(mapped);

            var result = await _controller.ListByUkprnAsync("12345678","1", CancellationToken.None);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<LearningDeliverySynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page=1", actual.Links.First);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page=2", actual.Links.Prev);
            Assert.IsNull(actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}/fe-providers/12345678/learning-deliveries?page=3", actual.Links.Last);
        }
        
        [Test]
        public async Task AndManagerThrowsInvalidRequestExceptionThenItShouldReturnBadRequest()
        {
            var exception = new InvalidRequestException("unit test exception");
            _learningDeliveryManagerMock.Setup(manager => manager.ListForProviderAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            
            var result = await _controller.ListByUkprnAsync("12345678","1", CancellationToken.None);
        
            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;
        
            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(exception.Message, problemDetails.Detail);
        }
    }
}