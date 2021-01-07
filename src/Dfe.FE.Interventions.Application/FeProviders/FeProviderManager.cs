using System.Threading;
using System.Threading.Tasks;
using Dfe.FE.Interventions.Domain;
using Dfe.FE.Interventions.Domain.FeProviders;
using Microsoft.Extensions.Logging;

namespace Dfe.FE.Interventions.Application.FeProviders
{
    public interface IFeProviderManager
    {
        Task<PagedSearchResult<FeProviderSynopsis>> SearchAsync(int? ukprn, string legalName, int pageNumber, CancellationToken cancellationToken);
        Task<FeProvider> RetrieveAsync(int ukprn, CancellationToken cancellationToken);
    }

    public class FeProviderManager : IFeProviderManager
    {
        private readonly IFeProviderRepository _feProviderRepository;
        private readonly ILogger<FeProviderManager> _logger;

        public FeProviderManager(
            IFeProviderRepository feProviderRepository,
            ILogger<FeProviderManager> logger)
        {
            _feProviderRepository = feProviderRepository;
            _logger = logger;
        }

        public async Task<PagedSearchResult<FeProviderSynopsis>> SearchAsync(int? ukprn, string legalName, int pageNumber, CancellationToken cancellationToken)
        {
            if (ukprn.HasValue && (ukprn < 10000000 || ukprn > 99999999))
            {
                throw new InvalidRequestException("UKPRN must be an 8 digit number");
            }

            if (pageNumber < 1)
            {
                throw new InvalidRequestException("Page must be a number greater than 0");
            }

            var result = await _feProviderRepository.SearchFeProvidersAsync(ukprn, legalName, pageNumber, PaginationConstants.PageSize, cancellationToken);
            if (pageNumber > result.TotalNumberOfPages)
            {
                throw new InvalidRequestException($"Page number exceeds available pages. " +
                                                  $"Requested page {pageNumber}, but only {result.TotalNumberOfPages} pages available");
            }
            
            return result;
        }

        public async Task<FeProvider> RetrieveAsync(int ukprn, CancellationToken cancellationToken)
        {
            if (ukprn < 10000000 || ukprn > 99999999)
            {
                throw new InvalidRequestException("UKPRN must be an 8 digit number");
            }

            var provider = await _feProviderRepository.RetrieveProviderAsync(ukprn, cancellationToken);
            return provider;
        }
    }
}