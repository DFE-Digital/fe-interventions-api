using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Api.Controllers;
using Dfe.FE.Interventions.Application;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Dfe.FE.Interventions.Api.UnitTests.ControllersTests.FeProviderControllerTests
{
    public class WhenResolvingListFeProvidersResource
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
            _feProviderManagerMock.Setup(manager => manager.SearchAsync(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PagedSearchResult<FeProviderSynopsis>());

            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(It.IsAny<PagedSearchResult<FeProviderSynopsis>>()))
                .Returns((PagedSearchResult<FeProviderSynopsis> source) =>
                    new ApiPagedSearchResult<FeProviderSynopsis>
                    {
                        Results = source.Results,
                        CurrentPage = source.CurrentPage,
                        TotalNumberOfPages = source.TotalNumberOfPages,
                        TotalNumberOfRecords = source.TotalNumberOfRecords,
                        PageStartIndex = source.PageStartIndex,
                        PageFinishIndex = source.PageFinishIndex,
                    });

            _loggerMock = new Mock<ILogger<FeProviderController>>();

            _urlHelperStub = new UrlHelperStub();

            _controller = new FeProviderController(
                _feProviderManagerMock.Object,
                _mapperMock.Object,
                _loggerMock.Object);
            _controller.Url = _urlHelperStub;
        }

        [TestCase("not-a-number")]
        [TestCase("1234567")]
        [TestCase("123456789")]
        public async Task AndUkprnSpecifiedButIfNot8DigitNumberThenItShouldReturnBadRequest(string ukprn)
        {
            var result = await _controller.GetAsync(ukprn, null, null, CancellationToken.None);

            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;

            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual("UKPRN must be an 8 digit number", problemDetails.Detail);
        }

        [TestCase("not-a-number")]
        [TestCase("0")]
        [TestCase("-1")]
        public async Task AndPageNumberIsNotAPositiveNumberThenItShouldReturnBadRequest(string pageNumber)
        {
            var result = await _controller.GetAsync(null, null, pageNumber, CancellationToken.None);

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
            await _controller.GetAsync(null, null, null, cancellationToken);

            _feProviderManagerMock.Verify(manager => manager.SearchAsync(It.IsAny<int?>(), It.IsAny<string>(), 1, cancellationToken),
                Times.Once);
        }

        [TestCase(12345678, null, 1)]
        [TestCase(null, "Provider One", 2)]
        [TestCase(null, null, 3)]
        public async Task ThenItShouldCallManagerWithRequestedParameters(int? ukprn, string name, int pageNumber)
        {
            var cancellationToken = new CancellationToken();
            await _controller.GetAsync(ukprn?.ToString(), name, pageNumber.ToString(), cancellationToken);

            _feProviderManagerMock.Verify(manager => manager.SearchAsync(ukprn, name, pageNumber, cancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldMapResultsToApiFormat()
        {
            var source = new PagedSearchResult<FeProviderSynopsis>();
            _feProviderManagerMock.Setup(manager => manager.SearchAsync(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(source);

            await _controller.GetAsync(null, null, null, CancellationToken.None);

            _mapperMock.Verify(mapper => mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(source),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldReturnMappedData()
        {
            var mapped = new ApiPagedSearchResult<FeProviderSynopsis>();
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(It.IsAny<PagedSearchResult<FeProviderSynopsis>>()))
                .Returns(mapped);

            var result = await _controller.GetAsync(null, null, "1", CancellationToken.None);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<FeProviderSynopsis>;
            Assert.IsNotNull(result);
            Assert.IsNotNull(okObjectResult);
            Assert.AreSame(mapped, actual);
        }

        [TestCase(12345678, null)]
        [TestCase(null, "Provider One")]
        [TestCase(null, null)]
        public async Task AndRequestedPageIs1AndOnly1PageOfResultsThenItShouldIncludeLinkToFirstAndLastPage(int? ukprn, string name)
        {
            var mapped = new ApiPagedSearchResult<FeProviderSynopsis>
            {
                CurrentPage = 1,
                TotalNumberOfPages = 1,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(It.IsAny<PagedSearchResult<FeProviderSynopsis>>()))
                .Returns(mapped);

            var result = await _controller.GetAsync(ukprn?.ToString(), name, "1", CancellationToken.None);

            var expectedCriteriaUrl = EncodeCriteriaForLinkUrl(ukprn, name);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<FeProviderSynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page=1{expectedCriteriaUrl}", actual.Links.First);
            Assert.IsNull(actual.Links.Prev);
            Assert.IsNull(actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page=1{expectedCriteriaUrl}", actual.Links.Last);
        }

        [TestCase(12345678, null, 2)]
        [TestCase(null, "Provider One", 2)]
        [TestCase(null, null, 2)]
        [TestCase(12345678, null, 5)]
        [TestCase(null, "Provider One", 5)]
        [TestCase(null, null, 5)]
        public async Task AndRequestedPageIsNotFirstPageOfResultsThenItShouldIncludeLinkToFirstAndPrevAndLastPage(int? ukprn, string name, int currentPage)
        {
            var mapped = new ApiPagedSearchResult<FeProviderSynopsis>
            {
                CurrentPage = currentPage,
                TotalNumberOfPages = currentPage,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(It.IsAny<PagedSearchResult<FeProviderSynopsis>>()))
                .Returns(mapped);

            var result = await _controller.GetAsync(ukprn?.ToString(), name, currentPage.ToString(), CancellationToken.None);

            var expectedCriteriaUrl = EncodeCriteriaForLinkUrl(ukprn, name);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<FeProviderSynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page=1{expectedCriteriaUrl}", actual.Links.First);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page={currentPage - 1}{expectedCriteriaUrl}", actual.Links.Prev);
            Assert.IsNull(actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page={currentPage}{expectedCriteriaUrl}", actual.Links.Last);
        }

        [TestCase(12345678, null, 2)]
        [TestCase(null, "Provider One", 2)]
        [TestCase(null, null, 2)]
        [TestCase(12345678, null, 5)]
        [TestCase(null, "Provider One", 5)]
        [TestCase(null, null, 5)]
        public async Task AndRequestedPageIsNotFirstOrLastPageOfResultsThenItShouldIncludeLinkToFirstAndPrevAndNextAndLastPage(int? ukprn, string name, int currentPage)
        {
            var mapped = new ApiPagedSearchResult<FeProviderSynopsis>
            {
                CurrentPage = currentPage,
                TotalNumberOfPages = currentPage + 2,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(It.IsAny<PagedSearchResult<FeProviderSynopsis>>()))
                .Returns(mapped);

            var result = await _controller.GetAsync(ukprn?.ToString(), name, currentPage.ToString(), CancellationToken.None);

            var expectedCriteriaUrl = EncodeCriteriaForLinkUrl(ukprn, name);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<FeProviderSynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page=1{expectedCriteriaUrl}", actual.Links.First);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page={currentPage - 1}{expectedCriteriaUrl}", actual.Links.Prev);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page={currentPage + 1}{expectedCriteriaUrl}", actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page={currentPage + 2}{expectedCriteriaUrl}", actual.Links.Last);
        }

        [TestCase(12345678, null)]
        [TestCase(null, "Provider One")]
        [TestCase(null, null)]
        public async Task AndRequestedPageIsLastPageOfResultsThenItShouldIncludeLinkToFirstAndPrevAndLastPage(int? ukprn, string name)
        {
            var mapped = new ApiPagedSearchResult<FeProviderSynopsis>
            {
                CurrentPage = 3,
                TotalNumberOfPages = 3,
            };
            _mapperMock.Setup(mapper => mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(It.IsAny<PagedSearchResult<FeProviderSynopsis>>()))
                .Returns(mapped);

            var result = await _controller.GetAsync(ukprn?.ToString(), name, "1", CancellationToken.None);

            var expectedCriteriaUrl = EncodeCriteriaForLinkUrl(ukprn, name);

            var okObjectResult = result as OkObjectResult;
            var actual = okObjectResult?.Value as ApiPagedSearchResult<FeProviderSynopsis>;
            Assert.NotNull(actual?.Links);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page=1{expectedCriteriaUrl}", actual.Links.First);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page=2{expectedCriteriaUrl}", actual.Links.Prev);
            Assert.IsNull(actual.Links.Next);
            Assert.AreEqual($"{_urlHelperStub.BaseUrl}?page=3{expectedCriteriaUrl}", actual.Links.Last);
        }
        
        [Test]
        public async Task AndManagerThrowsInvalidRequestExceptionThenItShouldReturnBadRequest()
        {
            var exception = new InvalidRequestException("unit test exception");
            _feProviderManagerMock.Setup(manager => manager.SearchAsync(It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
            
            var result = await _controller.GetAsync(null, null, "1", CancellationToken.None);

            var badRequestResult = result as BadRequestObjectResult;
            var problemDetails = badRequestResult?.Value as ProblemDetails;

            Assert.IsNotNull(result);
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(problemDetails);
            Assert.AreEqual(exception.Message, problemDetails.Detail);
        }


        private string EncodeCriteriaForLinkUrl(int? ukprn, string name)
        {
            var criteria = new[]
            {
                ukprn.HasValue ? $"ukprn={ukprn}" : null,
                !string.IsNullOrEmpty(name) ? $"name={name}" : null,
            }.Where(x => x != null);
            if (!criteria.Any())
            {
                return string.Empty;
            }

            var expectedUrlCriteria = criteria.Aggregate((x, y) => $"{x}&{y}");
            if (!string.IsNullOrEmpty(expectedUrlCriteria))
            {
                expectedUrlCriteria = "&" + expectedUrlCriteria;
            }

            return expectedUrlCriteria;
        }
    }
}