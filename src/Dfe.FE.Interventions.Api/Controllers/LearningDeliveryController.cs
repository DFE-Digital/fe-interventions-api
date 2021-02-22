using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Application;
using Dfe.FE.Interventions.Application.LearningDeliveries;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.LearningDeliveries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    public class LearningDeliveryController : ControllerBase
    {
        private readonly ILearningDeliveryManager _learningDeliveryManager;
        private readonly IMapper _mapper;
        private readonly ILogger<LearningDeliveryController> _logger;

        public LearningDeliveryController(
            ILearningDeliveryManager learningDeliveryManager,
            IMapper mapper,
            ILogger<LearningDeliveryController> logger)
        {
            _learningDeliveryManager = learningDeliveryManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet, Route("fe-providers/{ukprn}/learning-deliveries")]
        public async Task<IActionResult> ListByUkprnAsync(
            string ukprn,
            [FromQuery] string page,
            CancellationToken cancellationToken)
        {
            // Parse UKPRN
            var parsedUkprn = 0;
            if (!ukprn.TryParseAsInt(out parsedUkprn))
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = "UKPRN must be an 8 digit number",
                });
            }

            // Parse page
            var parsedPageNumber = 1;
            if (!string.IsNullOrEmpty(page) && !int.TryParse(page, out parsedPageNumber))
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = "Page must be a number greater than 0",
                });
            }

            // Execute search
            PagedSearchResult<LearningDeliverySynopsis> result;
            try
            {
                _logger.LogInformation("Searching for learning deliveries for provider {UKPRN}, PageNumber: {PageNumber}",
                    parsedUkprn, parsedPageNumber);
                
                result = await _learningDeliveryManager.ListForProviderAsync(parsedUkprn, parsedPageNumber, cancellationToken);
                
                _logger.LogInformation("Returning {NumberOfResults} learning deliveries for provider {UKPRN}",
                    result.Results?.Length ?? 0, parsedUkprn);
            }
            catch (InvalidRequestException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = ex.Message,
                });
            }

            // Map results to response
            var response = _mapper.Map<ApiPagedSearchResult<LearningDeliverySynopsis>>(result);
            response.Links = new ApiPagedSearchResultLinks
            {
                First = Url.ActionLink(null, null, new {page = 1, ukprn = parsedUkprn}),
                Last = Url.ActionLink(null, null, new {page = response.TotalNumberOfPages, ukprn = parsedUkprn}),
            };

            if (response.CurrentPage > 1)
            {
                response.Links.Prev = Url.ActionLink(null, null, new {page = response.CurrentPage - 1, ukprn = parsedUkprn});
            }

            if (response.CurrentPage < response.TotalNumberOfPages)
            {
                response.Links.Next = Url.ActionLink(null, null, new {page = response.CurrentPage + 1, ukprn = parsedUkprn});
            }

            return Ok(response);
        }
    }
}