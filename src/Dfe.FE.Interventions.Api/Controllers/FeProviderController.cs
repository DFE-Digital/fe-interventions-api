using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dfe.FE.Interventions.Api.ApiModels;
using Dfe.FE.Interventions.Application;
using Dfe.FE.Interventions.Application.FeProviders;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("fe-providers")]
    public class FeProviderController : ControllerBase
    {
        private readonly IFeProviderManager _feProviderManager;
        private readonly IMapper _mapper;
        private readonly ILogger<FeProviderController> _logger;

        public FeProviderController(
            IFeProviderManager feProviderManager,
            IMapper mapper,
            ILogger<FeProviderController> logger)
        {
            _feProviderManager = feProviderManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListAsync(
            [FromQuery] string ukprn,
            [FromQuery] string name,
            [FromQuery] string page,
            CancellationToken cancellationToken)
        {
            // Parse UKPRN
            int? parsedUkprn = null;
            if (!string.IsNullOrEmpty(ukprn) && !ukprn.TryParseAsNullableInt(out parsedUkprn))
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
            PagedSearchResult<FeProviderSynopsis> result;
            try
            {
                _logger.LogInformation("Searching for providers. Ukprn: {UKPRN}, Name: {Name}, PageNumber: {PageNumber}",
                    parsedUkprn, name, parsedPageNumber);
                
                result = await _feProviderManager.SearchAsync(parsedUkprn, name, parsedPageNumber, cancellationToken);
                
                _logger.LogInformation("Returning {NumberOfResults} providers",
                    result.Results?.Length ?? 0);
            }
            catch (InvalidRequestException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = ex.Message,
                });
            }

            // Map results to response
            var response = _mapper.Map<ApiPagedSearchResult<FeProviderSynopsis>>(result);
            response.Links = new ApiPagedSearchResultLinks
            {
                First = Url.ActionLink(null, null, new {page = 1, ukprn = parsedUkprn, name}),
                Last = Url.ActionLink(null, null, new {page = response.TotalNumberOfPages, ukprn = parsedUkprn, name}),
            };

            if (response.CurrentPage > 1)
            {
                response.Links.Prev = Url.ActionLink(null, null, new {page = response.CurrentPage - 1, ukprn = parsedUkprn, name});
            }

            if (response.CurrentPage < response.TotalNumberOfPages)
            {
                response.Links.Next = Url.ActionLink(null, null, new {page = response.CurrentPage + 1, ukprn = parsedUkprn, name});
            }

            // Return
            return Ok(response);
        }

        [HttpGet, Route("{ukprn}")]
        public async Task<IActionResult> GetAsync(
            string ukprn,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(ukprn, out var parsedUkprn))
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = "UKPRN must be an 8 digit number",
                });
            }

            FeProvider result;
            try
            {
                result = await _feProviderManager.RetrieveAsync(parsedUkprn, cancellationToken);
            }
            catch (InvalidRequestException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = ex.Message,
                });
            }

            if (result == null)
            {
                return NotFound();
            }

            var response = _mapper.Map<ApiFeProvider>(result);
            response.Links = new ApiFeProviderLinks
            {
                Self = Url.ActionLink(null, null, new {ukprn}),
            };

            return Ok(response);
        }

        [HttpGet, Route("{ukprn}/statistics")]
        public async Task<IActionResult> GetStatisticsAsync(
            string ukprn,
            CancellationToken cancellationToken)
        {
            if (!int.TryParse(ukprn, out var parsedUkprn))
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = "UKPRN must be an 8 digit number",
                });
            }
            
            try
            {
                var provider = await _feProviderManager.RetrieveAsync(parsedUkprn, cancellationToken);
                if (provider == null)
                {
                    return NotFound();
                }
            }
            catch (InvalidRequestException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = ex.Message,
                });
            }

            try
            {
                var statistics = await _feProviderManager.RetrieveStatisticsAsync(parsedUkprn, cancellationToken);

                return Ok(statistics);
            }
            catch (InvalidRequestException ex)
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = ex.Message,
                });
            }
        }
    }
}