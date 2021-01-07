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
        public async Task<IActionResult> GetAsync(
            [FromQuery] string ukprn,
            [FromQuery] string name,
            [FromQuery] string page,
            CancellationToken cancellationToken)
        {
            // Parse UKPRN
            int? parsedUkprn = null;
            if (!string.IsNullOrEmpty(ukprn) &&
                (!TryParseNullableInt(ukprn, out parsedUkprn) || ukprn.Length != 8))
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = "UKPRN must be an 8 digit number",
                });
            }

            // Parse page
            var parsedPageNumber = 1;
            if (!string.IsNullOrEmpty(page)
                && (!int.TryParse(page, out parsedPageNumber) || parsedPageNumber < 1))
            {
                return BadRequest(new ProblemDetails
                {
                    Detail = "Page must be a number greater than 0",
                });
            }

            // Search store
            PagedSearchResult<FeProviderSynopsis> result;
            try
            {
                result = await _feProviderManager.SearchAsync(parsedUkprn, name, parsedPageNumber, cancellationToken);
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
            response.Links = new ApiPagedSearchResultLinks();

            response.Links.First = Url.ActionLink(null, null, new {page = 1, ukprn = parsedUkprn, name = name});
            if (response.CurrentPage > 1)
            {
                response.Links.Prev = Url.ActionLink(null, null, new {page = response.CurrentPage - 1, ukprn = parsedUkprn, name = name});
            }
            if (response.CurrentPage < response.TotalNumberOfPages)
            {
                response.Links.Next = Url.ActionLink(null, null, new {page = response.CurrentPage + 1, ukprn = parsedUkprn, name = name});
            }
            response.Links.Last = Url.ActionLink(null, null, new {page = response.TotalNumberOfPages, ukprn = parsedUkprn, name = name});

            // Return
            return Ok(response);
        }

        private static bool TryParseNullableInt(string value, out int? parsed)
        {
            if (!int.TryParse(value, out var temp))
            {
                parsed = null;
                return false;
            }

            parsed = temp;
            return true;
        }
    }
}